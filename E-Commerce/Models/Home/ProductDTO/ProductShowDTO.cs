

namespace ECommerce.Models.Home.ProductDTO
{
    public class ProductShowDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Price { get; set; }
        public string? ShortDescription { get; set; }
        public List<string>? Images { get; set; }

    }
}
