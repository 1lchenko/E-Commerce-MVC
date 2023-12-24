using ECommerce.Models;
using ECommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository; 
    }
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    { 
        var categories = await _categoryRepository.GetAllCategoriesAsync();
        
        return View(categories);
    }
    [HttpGet]
    public IActionResult AddCategory()
    {
        return View("AddCategory");
    }
    
          
    [HttpPost]
    public async Task<IActionResult> AddCategory(Category newCategory)
    {
        if (ModelState.IsValid) 
        {
            await _categoryRepository.AddCategoryAsync(newCategory);
            return RedirectToAction(nameof(GetAllCategories));
        }
    
        return View("AddCategory",newCategory);
    }
    
            
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        var result = await _categoryRepository.DeleteCategoryAsync(categoryId);
                
        if (result == false)
        { return NotFound("Item not found ib db for delete operation");
        }
                
        return RedirectToAction(nameof(GetAllCategories));
    }
    
          
    [HttpGet]
    public async Task<IActionResult> EditCategory(int categoryId)
    {
    
        var category = await _categoryRepository.FindCategoryById(categoryId);
    
        if (category == null)
        { 
            return NotFound("Item not found ib db for edit operation");
        }
    
        return View("EditCategory",category);
    }
    
          
    [HttpPost]
    public async Task<IActionResult> EditCategory(Category editCategory)
    { 
        if (ModelState.IsValid) 
        { 
            var result =  await _categoryRepository.UpdateCategoryAsync(editCategory);
    
            if (result == false)
            {
                return NotFound("Item not found ib db for edit operation");
            }
            return RedirectToAction(nameof(GetAllCategories));
        }
    
        return View("EditCategory", editCategory);
                
    }

      
        
}