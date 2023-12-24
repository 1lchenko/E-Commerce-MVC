using AutoMapper;
using ECommerce.Data;
using ECommerce.Exceptions;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories;

public class ProductDetailRepository : IProductDetailRepository
{
    private readonly EShopDbContext _context;
    private readonly IMapper _mapper;

    public ProductDetailRepository(EShopDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task AddProductsDetailsAsync(List<ProductDetail> productDetails)
    {
        try
        {
            await _context.ProductDetails.AddRangeAsync(productDetails);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new AddProductsDetailsException("Unable to add Products Details", e);
        }
        
    }

    public async Task<List<ProductDetail>> GetProductsDetailsAsync(int productId)
    {
        var productsDescription = await _context.ProductDetails
            .Where(x => x.ProductId == productId)
            .ToListAsync();
        
        return productsDescription;
    }

    public async Task<bool> EditProductsDetailAsync(ProductDetail detail)
    {
        var productDetail = await _context.ProductDetails
            .FirstOrDefaultAsync(x => x.ProductDetailId == detail.ProductDetailId);

        if (productDetail == null)
        {
            return false;
        }

        _mapper.Map(detail, productDetail);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProductsDetailAsync(int productDetailId)
    {
        var productDetail = await _context.ProductDetails
            .FirstOrDefaultAsync(x => x.ProductDetailId == productDetailId);

        if (productDetail == null)
        {
            return false;
        }

        _context.ProductDetails.Remove(productDetail);
        await _context.SaveChangesAsync();
        return true;
    }
}