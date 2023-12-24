using ECommerce.Models;

namespace ECommerce.Repositories;

public interface IProductDetailRepository
{
    Task AddProductsDetailsAsync(List<ProductDetail> productDetails);

    Task<List<ProductDetail>> GetProductsDetailsAsync(int productId);

    Task<bool> EditProductsDetailAsync(ProductDetail productDetail);
    
    Task<bool> DeleteProductsDetailAsync(int productDetailId);
}