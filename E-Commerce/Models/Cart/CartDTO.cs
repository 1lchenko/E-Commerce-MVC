

namespace ECommerce.Models.Cart
{
    public class CartDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ShortDescription { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; } 
        public int TotalPrice => Price * Quantity;
    }
}
