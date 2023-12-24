using ECommerce.Models;
using ECommerce.Models.Cart;
using ECommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ECommerce.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;

        public CartController(IProductRepository rep)
        {
            _productRepository = rep;
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            List<OrderItem> orderItems = HttpContext.Session.Get<List<OrderItem>>(".OrderItems")?
                .Where(oi => oi.Quantity > 0)
                .ToList() ?? new List<OrderItem>();
            List<int>  productIds = orderItems.Select(oi => oi.ProductId).ToList();

            var filteredProducts = await _productRepository.FilteredProductsByIds(productIds);

            List<CartDTO> cart = filteredProducts
                .Join(
                 orderItems,
                 product => product.ProductId,
                 orderItem => orderItem.ProductId,
                 (product, orderItem) => new CartDTO
                 {
                     ProductId = product.ProductId,
                     ProductName = product.Name,
                     ShortDescription = product.ShortDescription,
                     Quantity = orderItem.Quantity,
                     Price = product.Price
                 })
                .ToList();
            
           
            return View(cart);
        }
        
    }
}
