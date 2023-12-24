using ECommerce.Models.Home.ProductDTO;
using ECommerce.Repositories;

namespace ECommerce.Models.Home
{
    public class ProductsPageViewModel
    {
        public List<ProductShowDTO> Products { get; set; }
        public ProductRepository.PageViewModel PaginationInfo { get; set; }
    }

}
