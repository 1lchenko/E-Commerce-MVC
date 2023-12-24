using ECommerce.Models;

namespace ECommerce.Repositories;

public interface ICategoryRepository
{
    public Task<List<Category>> GetAllCategoriesAsync();
    
    public Task AddCategoryAsync(Category newCategory);

    public Task<Category?> FindCategoryById(int id);

    public Task<bool> DeleteCategoryAsync(int id);
    
    public Task<bool> UpdateCategoryAsync(Category category);
}