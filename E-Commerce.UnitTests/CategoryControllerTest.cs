

namespace EShop_Testing;

public class CategoryControllerTest
{
    // Category:
    [Fact]
    public async Task AddCategoryPost_ReturnsAddCategoryView_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockRepoCategory = new Mock<ICategoryRepository>();

        var controller = new CategoryController
            (mockRepoCategory.Object);
        controller.ModelState.AddModelError("CategoryName", "Required");
        
        //Act
        var result = await controller.AddCategory(new Category());
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("AddCategory", viewResult.ViewName);
    }

    [Fact]
    public async Task AddCategoryPost_ReturnsARedirectAndAddCategory_WhenModelStateIsValid()
    {
        // Arrange
        
        var mockRepoCategory = new Mock<ICategoryRepository>();
        mockRepoCategory
            .Setup(rep => rep.AddCategoryAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        
        var controller = new CategoryController
            (mockRepoCategory.Object);
        var testCategory = new Category()
        {
            CategoryId = 1,
            CategoryName = "Washing machines"
        };
        
        //Act
        var result = await controller.AddCategory(testCategory);
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(redirectToActionResult.ControllerName);
        Assert.Equal("GetAllCategories", redirectToActionResult.ActionName);
        mockRepoCategory.Verify(r => r.AddCategoryAsync(testCategory));
    }
    
    [Fact]
    public async Task DeleteCategoryPost_ReturnsARedirectAndDeleteCategory_WhenResultDeleteCategoryIsTrue()
    {
        // Arrange
        int testCategoryId = 1;
        var mockRepoCategory = new Mock<ICategoryRepository>();
        mockRepoCategory
            .Setup(rep => rep.DeleteCategoryAsync(testCategoryId))
            .ReturnsAsync(true);
        
        var controller = new CategoryController
            (mockRepoCategory.Object);
       
        
        //Act
        var result = await controller.DeleteCategory(testCategoryId);
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(redirectToActionResult.ControllerName);
        Assert.Equal("GetAllCategories", redirectToActionResult.ActionName);
        mockRepoCategory.Verify(r => r.DeleteCategoryAsync(testCategoryId));
    }

    [Fact]
    public async Task DeleteCategoryPost_NotFoundResult_WhenResultDeleteCategoryIsFalse()
    {
        // Arrange
        int testCategoryId = 0;
        
        var mockRepoCategory = new Mock<ICategoryRepository>();
        mockRepoCategory
            .Setup(rep => rep.DeleteCategoryAsync(testCategoryId))
            .ReturnsAsync(false);
        
        var controller = new CategoryController
            (mockRepoCategory.Object);
       
        
        //Act
        var result = await controller.DeleteCategory(testCategoryId);
        
        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Item not found ib db for delete operation", notFoundResult.Value);
        mockRepoCategory.Verify(r => r.DeleteCategoryAsync(testCategoryId));
    }
    
    [Fact]
    public async Task EditCategoryGet_ReturnsViewResultWithCategory()
    {
        // Arrange
        int testCategoryId = 1;
        var testCategory = new Category()
        {
            CategoryId = 1,
            CategoryName = "Washing machines"
        };
        var mockRepoCategory = new Mock<ICategoryRepository>();
        mockRepoCategory
            .Setup(rep => rep.FindCategoryById(testCategoryId))
            .ReturnsAsync(testCategory);
        
        var controller = new CategoryController
            (mockRepoCategory.Object);
        
        // Act
        var result = await controller.EditCategory(testCategoryId);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Category>(
            viewResult.ViewData.Model);
        Assert.Equal("EditCategory", viewResult.ViewName);
        Assert.Equal(testCategory, model);
        mockRepoCategory.Verify(x => x.FindCategoryById(testCategoryId));
    }
    
    [Fact]
    public async Task EditCategoryGet_ReturnsNotFoundResultWhenCategoryNotFound()
    {
        // Arrange
        int testCategoryId = 0;
        var mockRepoCategory = new Mock<ICategoryRepository>();
        mockRepoCategory
            .Setup(rep => rep.FindCategoryById(testCategoryId))
            .ReturnsAsync((Category)null);
        
        var controller = new CategoryController
            (mockRepoCategory.Object);
        
        // Act
        var result = await controller.EditCategory(testCategoryId);
        
        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Item not found ib db for edit operation", notFoundResult.Value);
        mockRepoCategory.Verify(x => x.FindCategoryById(testCategoryId));
    }

    [Fact]
    public async Task EditCategoryPost_ReturnsEditCategoryView_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockRepoCategory = new Mock<ICategoryRepository>();
        
        var controller = new CategoryController
            (mockRepoCategory.Object);
        controller.ModelState.AddModelError("CategoryName", "Required");
        Category testCategory = new Category();
        
        //Act
        var result = await controller.EditCategory(testCategory);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Category>(
            viewResult.ViewData.Model);
        Assert.Equal("EditCategory", viewResult.ViewName);
        Assert.Equal(testCategory, model);
    }
    
    [Fact]
    public async Task EditCategoryPost_NotFoundResult_WhenResultUpdateCategoryIsFalse()
    {
        // Arrange
        var testCategory = new Category()
        {
            CategoryId = 0,  // not exist in db
            CategoryName = "Telephones"
        };
        
        var mockRepoCategory = new Mock<ICategoryRepository>();
        mockRepoCategory
            .Setup(rep => rep.UpdateCategoryAsync(testCategory))
            .ReturnsAsync(false);

        var controller = new CategoryController
            (mockRepoCategory.Object);
        
        
        //Act
        var result = await controller.EditCategory(testCategory);
        
        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Item not found ib db for edit operation", notFoundResult.Value);
        mockRepoCategory.Verify(r => r.UpdateCategoryAsync(testCategory));
    }
    
    [Fact]
    public async Task EditCategoryPost_ReturnsARedirectAndEditCategory_WhenResultUpdateCategoryIsTrue()
    {
        // Arrange
        var testCategory = new Category()
        {
            CategoryId = 1,   
            CategoryName = "Telephones"
        };
        
        var mockRepoCategory = new Mock<ICategoryRepository>();
        mockRepoCategory
            .Setup(rep => rep.UpdateCategoryAsync(testCategory))
            .ReturnsAsync(true);
        
        var controller = new CategoryController
            (mockRepoCategory.Object);
        
        
        //Act
        var result = await controller.EditCategory(testCategory);
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(redirectToActionResult.ControllerName);
        Assert.Equal("GetAllCategories", redirectToActionResult.ActionName);
        mockRepoCategory.Verify(r => r.UpdateCategoryAsync(testCategory));
    }
}