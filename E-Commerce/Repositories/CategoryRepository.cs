using AutoMapper;
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly EShopDbContext _context;
    private readonly IMapper _mapper;
    
    public CategoryRepository(EShopDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        var allCategories = await _context.Categories.ToListAsync();
        return allCategories;
    }

    public async Task AddCategoryAsync(Category newCategory)
    {
        await _context.Categories.AddAsync(newCategory);
        await _context.SaveChangesAsync();
    }
    

    public async Task<Category?> FindCategoryById(int id)
    {
        Category? category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
        
        if (category == null)
        {
            return false;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCategoryAsync(Category updateCategory)
    {
        Category? category = await _context.Categories
            .FirstOrDefaultAsync(x => x.CategoryId == updateCategory.CategoryId);

        if (category == null)
        {
            return false;
        }

        _mapper.Map(updateCategory, category);
        await _context.SaveChangesAsync();
        return true;
    }
}