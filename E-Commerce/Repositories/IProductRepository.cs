using ECommerce.Models;
using ECommerce.Models.Home;
using ECommerce.Models.Home.ProductDTO;

namespace ECommerce.Repositories
{
    public interface IProductRepository
    {
        Task<ProductsPageViewModel> GetAllProductAsync(int page );
        Task<Product?> GetProductAsync(int id);
        Task<List<Product>> FilteredProductsByIds(List<int> productIds);
        Task<ProductsPageViewModel> GetProductsByCategoryAsync(int? categoryId, int page );
        Task<ProductEditDTO?> GetProductEditDtoByIdAsync(int id);
        Task<ProductShowDTO?> GetProductShowDtoByIdAsync(int id);
        Task<ProductsPageViewModel> GetProductsByIndexOfAsync(string searchText, int page);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> EditProductAsync(ProductEditDTO productEditDto);
        Task CreateProductAsync(ProductAddDTO productAddDto);
    }
}

