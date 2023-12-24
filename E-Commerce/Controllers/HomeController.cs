using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ECommerce.Models;
using ECommerce.Models.Home;
using ECommerce.Models.Home.ProductDTO;
using ECommerce.Repositories;

namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductDetailRepository _detailRepository;
        public HomeController(IProductRepository productRepository, IProductDetailRepository detailRepository)
        {
            _productRepository = productRepository;
            _detailRepository = detailRepository;
        }
        
        public async Task<IActionResult> Index(int? categoryId, string? searchText,int page = 1)
        {

            ProductsPageViewModel productsPageViewModel;
            if (searchText != null)
            {
                productsPageViewModel = await _productRepository.GetProductsByIndexOfAsync(searchText, page);
                ViewBag.searchText = searchText;
                ViewBag.CategoryId = null;
            }
            else if (categoryId != null) 
            {
                productsPageViewModel = await _productRepository.GetProductsByCategoryAsync(categoryId, page);
                ViewBag.searchText = null;
                ViewBag.CategoryId = categoryId;
            }
            else
            {
                productsPageViewModel = await _productRepository.GetAllProductAsync(page);
                ViewBag.searchText = null;
                ViewBag.CategoryId = null;
            }
           
            return View(productsPageViewModel);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SaveToSession([FromBody] ProductSessionDTO product)
        {
            List<OrderItem> orderItems = HttpContext.Session.Get<List<OrderItem>>(".OrderItems") ?? new List<OrderItem>();

            OrderItem? orderItem = orderItems.FirstOrDefault(x => x.ProductId == product.ProductId);

            if (orderItem == null)
            {
                orderItem = new OrderItem
                {
                    ProductId = product.ProductId,
                    Quantity = 0, 
                    AmountForOne = product.Amount,
                    ProductName = product.ProductName
                };
                orderItems.Add(orderItem);
            }

            orderItem.Quantity++;

            HttpContext.Session.Set(".OrderItems", orderItems);

            return Ok();
        }


        [HttpDelete]
        [Authorize] 
        public IActionResult RemoveFromSession([FromBody] ProductSessionDTO product)
        {
            List<OrderItem>? orderItems = HttpContext.Session.Get<List<OrderItem>>(".OrderItems");
            
            if (orderItems == null)
            {
                return NotFound("OrderItems not found in session");
            }
            
            OrderItem? orderItem = orderItems.FirstOrDefault(x => x.ProductId == product.ProductId);
            
            if (orderItem == null)
            {
                return NotFound("Product with given ID not found in session");
            }

            orderItem.Quantity -= 1;
            
            if (orderItem.Quantity <= 0)
            {
                orderItems.Remove(orderItem);
            }

            HttpContext.Session.Set(".OrderItems", orderItems);
            return Ok();
        }


        public async Task<IActionResult> ReadDescriptions(int productId)
        {
            var product = await _productRepository.GetProductShowDtoByIdAsync(productId);

            if (product == null)
            {
                return NotFound("Product not found for read description");
            }

            var listDetailsDescription = await _detailRepository.GetProductsDetailsAsync(productId);

            ViewBag.Product = product;
            return View(listDetailsDescription);
        }
      
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }

     


}
