using ECommerce.Models;
using ECommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
     

        public OrderController(
            IOrderRepository repository,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _orderRepository = repository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
           
        }


        public async Task<IActionResult> PlaceOrder()
        {
            var userId = GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            ViewBag.IsConfirm = user!.EmailConfirmed;
            var orderItems = HttpContext.Session.Get<List<OrderItem>>(".OrderItems");
            
            if (orderItems == null)
            {
                return NotFound("Order not may will be without items");
            }

            ViewBag.TotalPrice = orderItems.Sum(x => x.Quantity * x.AmountForOne);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(OrderAddDTO order)
        {
            var userId = GetUserId();
            var orderItems = HttpContext.Session.Get<List<OrderItem>>(".OrderItems");
            if (ModelState.IsValid)
            {

                if (orderItems == null || orderItems.Count == 0)
                {
                    return NotFound("Order not may will be without items");
                }

                int orderId = await _orderRepository.AddOrderAsync(order, orderItems,userId);
                
                HttpContext.Session.Remove(".OrderItems");

                return RedirectToAction(nameof(Thanks), new { orderId = orderId });

            }
            
            var user = await _userManager.FindByIdAsync(userId);
            ViewBag.IsConfirm = user!.EmailConfirmed;
            return View(order);
        }

        
        public IActionResult Thanks(int orderId)
        {
            Request.Headers.TryGetValue("Referer", out var headerValues);
            
            if (headerValues.Count == 0)
            {
                return BadRequest("This action can only be accessed via redirection from another action.");
            }
            
            return View(orderId);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditOrder(int productDetailId)
        {

            var order = await _orderRepository.GetOrderByIdAsync(productDetailId, true);
            
            if (order == null)
            {
                return NotFound("Order not found for editing");
            }
            
            ViewBag.StatusOrder = Enum.GetValues(typeof(OrderStatusEnum.OrderStatus));
            return View(order);
        }

      
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(Order updatedOrder)
        {
            if (ModelState.IsValid)
            {
                var validationError = ValidateOrderItems(updatedOrder.OrderItems);
                if (validationError != null)
                {
                    ModelState.AddModelError("OrderItemsIsEmpty", validationError);
                }
                else
                {
                    var result = await _orderRepository.EditOrderAsync(updatedOrder);
                    if (result)
                    {
                        return RedirectToAction(nameof(AccountController.Control), "Account", new { orderNumber = updatedOrder.OrderId });
                    }

                    return NotFound("Order not exist for deleting");
                }
            }
            ViewBag.StatusOrder = Enum.GetValues(typeof(OrderStatusEnum.OrderStatus));
            return View("EditOrder", updatedOrder);
        }
        
        

      
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var result = await _orderRepository.DeleteOrderAsync(orderId);

            if (!result)
            {
                return NotFound("Order not found to delete");
            }

            return RedirectToAction(nameof(AccountController.Control),"Account");
        }

       
        

        private static string? ValidateOrderItems(IEnumerable<OrderItem>? orderItems)
        {
            var validItems = orderItems?.Where(x => x.Quantity > 0).ToList();
            if (validItems == null || validItems.Count == 0)
            {
                return "You can not make order without items";
            }

            return null;
        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }

    }
}
