using ECommerce.Exceptions;


namespace EShop_Testing;

public class ProductControllerTest
{

    //Product:
    
    [Fact]
    public async Task GetAllProducts_WithoutSearchText_ReturnsViewWithAllProducts()
    {
        // Arrange
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(repo => repo.GetAllProductAsync(1))
            .ReturnsAsync(GetTestPageViewModel());
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        //Act
        var result = await controller.GetAllProducts(null, page: 1);
        
        // Asset
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ProductsPageViewModel>(
            viewResult.ViewData.Model);
        
        Assert.Equal(15, model.Products.Count());
        Assert.Null(controller.ViewBag.searchText);
        mockRepoProd.Verify(r => r.GetAllProductAsync(1));
        
    }
     
    [Fact]
    public async Task GetAllProducts_WithSearchText_ReturnsViewWithFilteredProducts()
    {
        // Arrange
        string testSearchText = "Laptop";
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(repo => repo.GetProductsByIndexOfAsync(testSearchText,1))
            .ReturnsAsync(GetTestPageViewModelBySearch());
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        //Act
        var result = await controller.GetAllProducts(testSearchText);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ProductsPageViewModel>(
            viewResult.ViewData.Model);
        Assert.Equal(3, model.Products.Count());
        Assert.NotNull(controller.ViewBag.searchText);
        mockRepoProd.Verify(r => r.GetProductsByIndexOfAsync(testSearchText,1));
    }

    [Fact]
    public async Task AddProductPost_ReturnsAddProductView_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockRepoProd = new Mock<IProductRepository>();
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        controller.ModelState.AddModelError("ProductName", "Required");
        var productAddDTO = new ProductAddDTO();
        //Act
        var result = await controller.AddProduct(productAddDTO);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("ProductAddEdit/AddProduct", viewResult.ViewName);
        Assert.Equal(productAddDTO, viewResult.ViewData.Model);
    }
    
    [Fact]
    public async Task AddProductPost_ValidModelState_CreateProductAsyncThrowsException_ReturnsAddProductView()
    {
        // Arrange
        var testAddProduct = new ProductAddDTO
        {
            Name = "Laptop Medium",
            Price = 10000,
            ShortDescription = "The latest IPhone model with a sleek design",
            CategoryId = 1,
            ImageFiles = null
        };

        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd.Setup(rep => rep.CreateProductAsync(testAddProduct))
            .Throws(new AddImageException("Failed add image to product", new Exception()));
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();

        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
       
        //Act
        var result = await controller.AddProduct(testAddProduct);
     
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ProductAddDTO>(
            viewResult.ViewData.Model);
        Assert.Equal(testAddProduct,model);
        Assert.Equal("ProductAddEdit/AddProduct", viewResult.ViewName);
        Assert.NotNull(controller.ModelState["imageError"].Errors);
        mockRepoProd.Verify(r => r.CreateProductAsync(testAddProduct));
        
    }

    [Fact]
    public async Task AddProductPost_ReturnsARedirectAndAddsProduct_WhenModelStateIsValid()
    {
        // Arrange
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(rep => rep.CreateProductAsync(It.IsAny<ProductAddDTO>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        var addProduct = new ProductAddDTO
        {
            Name = "Laptop Medium",
            Price = 10000,
            ShortDescription = "The latest IPhone model with a sleek design",
            CategoryId = 1,
            ImageFiles = null
        };
        
        //Act
        var result = await controller.AddProduct(addProduct);
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(redirectToActionResult.ControllerName);
        Assert.Equal("GetAllProducts", redirectToActionResult.ActionName);
        mockRepoProd.Verify(r => r.CreateProductAsync(addProduct));
    }

    [Fact]
    public async Task EditProductGet_ReturnsNotFoundResultWhenProductEditDTONotFound()
    {
        // Arrange
        int testProductId = 1;
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(rep => rep.GetProductEditDtoByIdAsync(testProductId))
            .ReturnsAsync((ProductEditDTO)null);
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        // Act
        var result = await controller.EditProduct(testProductId);
        
        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product not found for edit operation", notFoundResult.Value);
        mockRepoProd.Verify(r => r.GetProductEditDtoByIdAsync(testProductId));

    }

    [Fact]
    public async Task EditProductGet_ReturnsViewResultWithProductEditDTO()
    {
        // Arrange
        int testProductId = 1;
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(rep => rep.GetProductEditDtoByIdAsync(testProductId))
            .ReturnsAsync(new ProductEditDTO
            {
                ProductId = 1,
                Price = 10000,
                Name = "Laptop Medium",
                ShortDescription = "The latest IPhone model with a sleek design",
                CategoryId = 1,
                ImageFiles = null
            });
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        // Act
        var result = await controller.EditProduct(testProductId);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ProductEditDTO>(
            viewResult.ViewData.Model);
        Assert.Equal("ProductAddEdit/EditProduct", viewResult.ViewName);
        Assert.Equal(testProductId, model.ProductId);
        mockRepoProd.Verify(x => x.GetProductEditDtoByIdAsync(testProductId));
    }

    [Fact]
    public async Task EditProductPost_ReturnsEditProductView_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockRepoProd = new Mock<IProductRepository>();
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        controller.ModelState.AddModelError("ProductName", "Required");
        ProductEditDTO editDTO = new ProductEditDTO();
        
        //Act
        var result = await controller.EditProduct(editDTO);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ProductEditDTO>(
            viewResult.ViewData.Model);
        Assert.Equal("ProductAddEdit/EditProduct", viewResult.ViewName);
        Assert.Equal(editDTO, model);
    }

    [Fact]
    public async Task EditProductPost_NotFoundResult_WhenResultEditProductIsFalse()
    {
        // Arrange
        var testProductEdit = new ProductEditDTO
        {
            ProductId = 0, // is not exist in db
            Price = 10000,
            Name = "Laptop Medium",
            ShortDescription = "The latest IPhone model with a sleek design",
            CategoryId = 1,
            ImageFiles = null,
        };
        
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(rep => rep.EditProductAsync(testProductEdit))
            .ReturnsAsync(false);
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        
        //Act
        var result = await controller.EditProduct(testProductEdit);
        
        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product not found for edit operation", notFoundResult.Value);
        mockRepoProd.Verify(r => r.EditProductAsync(testProductEdit));
    }

    [Fact]
    public async Task EditProductPost_ReturnsARedirectAndEditProduct_WhenResultEditProductIsTrue()
    {
        // Arrange
        var testProductEdit = new ProductEditDTO
        {
            ProductId = 1, 
            Price = 10000,
            Name = "Laptop Medium",
            ShortDescription = "The latest IPhone model with a sleek design",
            CategoryId = 1,
            ImageFiles = null
        };
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(rep => rep.EditProductAsync(testProductEdit))
            .ReturnsAsync(true);
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        
        //Act
        var result = await controller.EditProduct(testProductEdit);
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(redirectToActionResult.ControllerName);
        Assert.Equal("GetAllProducts", redirectToActionResult.ActionName);
        mockRepoProd.Verify(r => r.EditProductAsync(testProductEdit));
    }

    [Fact]
    public async Task DeleteProductPost_NotFoundResult_WhenResultDeleteProductIsFalse()
    {
        // Arrange
        int testProductId = 0;
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(rep => rep.DeleteProductAsync(testProductId))
            .ReturnsAsync(false);
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
       
        
        //Act
        var result = await controller.DeleteProduct(testProductId, null);
        
        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product not found for deleting operation", notFoundResult.Value);
        mockRepoProd.Verify(r => r.DeleteProductAsync(testProductId));
    }
    
    [Fact]
    public async Task DeleteProductPost_ReturnsARedirectAndDeleteProduct_WhenResultDeleteProductIsTrue()
    {
        // Arrange
        int testProductId = 1;
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(rep => rep.DeleteProductAsync(testProductId))
            .ReturnsAsync(true);
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
       
        
        //Act
        var result = await controller.DeleteProduct(testProductId, searchText: "Marnitola");
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(redirectToActionResult.ControllerName);
        Assert.Equal("GetAllProducts", redirectToActionResult.ActionName);
        Assert.Equal("Marnitola", redirectToActionResult.RouteValues["searchText"]);
        mockRepoProd.Verify(r => r.DeleteProductAsync(testProductId));
    }
    
    
    //DetailDescription:
    
    [Fact]
    public async Task AddDetailsDescriptionGet_NotFoundResult_WhenProductIsNull()
    {
        // Arrange
        int testProductId = 0;
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(rep => rep.GetProductAsync(testProductId))
            .ReturnsAsync((Product)null);
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        //Act
        var result = await controller.AddDetailsDescription(testProductId);
        
        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product is not found for add Details Description to it", notFoundResult.Value);
        mockRepoProd.Verify(r => r.GetProductAsync(testProductId));
    }
    
    [Fact]
    public async Task AddDetailsDescriptionGet_ReturnsViewAddDetailsDescription_WhenProductIsNotNull()
    {
        // Arrange
        int testProductId = 1;
        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd
            .Setup(rep => rep.GetProductAsync(testProductId))
            .ReturnsAsync(new Product
            {
                ProductId = 1,
                Price = 10000,
                Name = "Laptop Medium",
                ShortDescription = "The latest IPhone model with a sleek design",
                CategoryId = 1,
                ProductDetails = null,
                
            });
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        //Act
        var result = await controller.AddDetailsDescription(testProductId);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("AddEditDetailsDescription/AddDetailsDescription", viewResult.ViewName);
        mockRepoProd.Verify(r => r.GetProductAsync(testProductId));
    }
    
    [Fact]
    public async Task AddDetailsDescriptionPost_ReturnsRedirectToAction_WhenModelStateIsValid()
    {
        // Arrange
        var testProductDetails = new List<ProductDetail>
        {
            new ProductDetail
            {
                ProductDetailId = 1,
                ProductId = 1,
                Title = "Title",
                Description = "Description",
            }
        };
        var mockRepoProd = new Mock<IProductRepository>();
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        mockRepoDetail.Setup(repo => repo.AddProductsDetailsAsync(testProductDetails))
            .Returns(Task.CompletedTask)
            .Verifiable();
        

        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);

        // Act
        var result = await controller.AddDetailsDescription(testProductDetails);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ProductController.GetAllProducts), redirectToActionResult.ActionName);
    }
    
    [Fact]
    public async Task AddDetailsDescriptionPost_ReturnsNotFoundResult_WhenModelStateIsInvalid()
    {
        // Arrange
        var testProductDetails = new List<ProductDetail>
        {
            new ProductDetail
            {
                ProductDetailId = 1,
                ProductId = 1,
                Description = "Description",
            }
        };

        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd.Setup(rep => rep.GetProductAsync(testProductDetails[0].ProductId))
            .ReturnsAsync((Product)null);
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();

        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await controller.AddDetailsDescription(testProductDetails);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product not found for Add Details Description", notFoundResult.Value);
    }
    
    [Fact]
    public async Task AddDetailsDescriptionPost_ReturnsAddDetailsDescriptionView_WhenModelStateIsInvalid()
    {
        // Arrange
        var testProductDetails = new List<ProductDetail>
        {
            new ProductDetail
            {
                ProductDetailId = 1,
                ProductId = 1,
                Description = "Description",
            }
        };
        var testProduct = new Product
        {
            ProductId = 1,
            Name = "IPhone",
            ShortDescription = "15",
            CategoryId = 1,
            ProductDetails = null,
            Price = 100,
            Images = null,
            
            
        };

        var mockRepoProd = new Mock<IProductRepository>();
        mockRepoProd.Setup(rep => rep.GetProductAsync(testProductDetails[0].ProductId))
            .ReturnsAsync(testProduct);
        
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();

        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await controller.AddDetailsDescription(testProductDetails);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("AddEditDetailsDescription/AddDetailsDescription", viewResult.ViewName);
        Assert.Equal(testProductDetails, viewResult.ViewData.Model);
    }

    [Fact]
    public async Task AddDetailsDescriptionPost_ValidModelState_AddProductsDetailsAsyncThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var testproductDetailsList = new List<ProductDetail>
        {
            new ProductDetail
            {
                ProductDetailId = 1,
                ProductId = 1,
                Description = "Description",
            }
        };

        var mockRepoProd = new Mock<IProductRepository>();
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        mockRepoDetail.Setup(rep => rep.AddProductsDetailsAsync(testproductDetailsList))
            .Throws(new AddProductsDetailsException("Unable to add Products Details", new Exception()));
        
        var mockRepoCateg = new Mock<ICategoryRepository>();

        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
       
        //Act
        var result = await controller.AddDetailsDescription(testproductDetailsList);
     
        //Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Unable to add Products Details", badRequestResult.Value);
        
    }
    
    [Fact]
    public async Task EditProductDetailPost_ReturnsEditProductDetailView_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockRepoProd = new Mock<IProductRepository>();
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        controller.ModelState.AddModelError("Title", "Required");
        ProductDetail testProductDetail = new ProductDetail();
        
        //Act
        var result = await controller.EditProductDetail(testProductDetail);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("AddEditDetailsDescription/EditProductDetail", viewResult.ViewName);
        
    }
    
    [Fact]
    public async Task EditProductDetailPost_NotFoundResult_WhenResultEditProductsDetailIsFalse()
    {
        // Arrange
        var testProductDetail = new ProductDetail()
        {
             ProductDetailId = 0, // not exist
             ProductId = 1,
             Title = " Title ",
             Description = " Description ",
        };
        
        var mockRepoProd = new Mock<IProductRepository>();
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        mockRepoDetail
            .Setup(rep => rep.EditProductsDetailAsync(testProductDetail))
            .ReturnsAsync(false);
        var mockRepoCateg = new Mock<ICategoryRepository>();
         

        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        
        //Act
        var result = await controller.EditProductDetail(testProductDetail);
        
        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product Detail isn`t exist for update operation", notFoundResult.Value);
        mockRepoDetail.Verify(r => r.EditProductsDetailAsync(testProductDetail));
    }
    
    [Fact]
    public async Task EditProductDetailPost_ReturnsEditProductDetailView_WhenResultEditProductsDetailIsTrue()
    {
        // Arrange
        var testProductDetail = new ProductDetail()
        {
            ProductDetailId = 1,  
            ProductId = 1,
            Title = " Title ",
            Description = " Description ",
        };
        
        var mockRepoProd = new Mock<IProductRepository>();
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        mockRepoDetail
            .Setup(rep => rep.EditProductsDetailAsync(testProductDetail))
            .ReturnsAsync(true);
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
        
        
        //Act
        var result = await controller.EditProductDetail(testProductDetail);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("AddEditDetailsDescription/EditProductDetail", viewResult.ViewName);
        mockRepoDetail.Verify(r => r.EditProductsDetailAsync(testProductDetail));
    }
    
    [Fact]
    public async Task DeleteProductDetail_ReturnsOKResult_WhenResultDeleteProductsDetailIsTrue()
    {
        // Arrange
        int testproductDetailId = 1;
        var mockRepoProd = new Mock<IProductRepository>();
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        mockRepoDetail
            .Setup(rep => rep.DeleteProductsDetailAsync(testproductDetailId))
            .ReturnsAsync(true);
        
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
       
        
        //Act
        var result = await controller.DeleteProductDetail(testproductDetailId);
        
        //Assert
        Assert.IsType<OkResult>(result);
        mockRepoDetail.Verify(r => r.DeleteProductsDetailAsync(testproductDetailId));
    }
    
    [Fact]
    public async Task DeleteProductDetail_ReturnsNotFoundResult_WhenResultDeleteProductsDetailIsFalse()
    {
        // Arrange
        int testproductDetailId = 0;
        var mockRepoProd = new Mock<IProductRepository>();
        var mockRepoDetail = new Mock<IProductDetailRepository>();
        mockRepoDetail
            .Setup(rep => rep.DeleteProductsDetailAsync(testproductDetailId))
            .ReturnsAsync(false);
        
        var mockRepoCateg = new Mock<ICategoryRepository>();
        
        
        var controller = new ProductController
            (mockRepoProd.Object,mockRepoCateg.Object,mockRepoDetail.Object);
       
        
        //Act
        var result = await controller.DeleteProductDetail(testproductDetailId);
        
        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("This Product Detail not exist for deleting", notFoundResult.Value);
        mockRepoDetail.Verify(r => r.DeleteProductsDetailAsync(testproductDetailId));
    }
    
    private ProductsPageViewModel GetTestPageViewModel()
    {
        var viewModel = new ProductsPageViewModel
        {
            Products = new List<ProductShowDTO>
            {
                new ProductShowDTO
                {
                    ProductId = 1,
                    Price = 10000,
                    Name = "IPhone X",
                    ShortDescription = "The latest IPhone model with a sleek design",
                    CategoryId = 1,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 2,
                    Price = 5000,
                    Name = "Laptop Pro",
                    ShortDescription = "High-performance laptop for professionals",
                    CategoryId = 2,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 3,
                    Price = 750,
                    Name = "Tablet Mini",
                    ShortDescription = "Compact tablet for on-the-go use",
                    CategoryId = 3,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 4,
                    Price = 2500,
                    Name = "Smartwatch",
                    ShortDescription = "Stay connected with this stylish smartwatch",
                    CategoryId = 4,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 5,
                    Price = 300,
                    Name = "Wireless Earbuds",
                    ShortDescription = "High-quality earbuds for music enthusiasts",
                    CategoryId = 5,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 6,
                    Price = 1500,
                    Name = "Camera Kit",
                    ShortDescription = "Capture stunning photos with this camera bundle",
                    CategoryId = 6,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 7,
                    Price = 120,
                    Name = "Wireless Mouse",
                    ShortDescription = "Ergonomic wireless mouse for comfortable use",
                    CategoryId = 7,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 8,
                    Price = 200,
                    Name = "External Hard Drive",
                    ShortDescription = "Expand your storage with this external hard drive",
                    CategoryId = 8,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 9,
                    Price = 750,
                    Name = "Gaming Console",
                    ShortDescription = "Experience gaming like never before",
                    CategoryId = 9,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 10,
                    Price = 50,
                    Name = "Headphones",
                    ShortDescription = "High-quality headphones for immersive audio",
                    CategoryId = 10,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 11,
                    Price = 1200,
                    Name = "4K Ultra HD TV",
                    ShortDescription = "Experience stunning visuals with this 4K TV",
                    CategoryId = 11,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 12,
                    Price = 300,
                    Name = "Coffee Maker",
                    ShortDescription = "Brew your favorite coffee at home with this coffee maker",
                    CategoryId = 12,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 13,
                    Price = 180,
                    Name = "Smart Thermostat",
                    ShortDescription = "Control your home's temperature with a smart thermostat",
                    CategoryId = 13,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 14,
                    Price = 500,
                    Name = "Drone",
                    ShortDescription = "Capture aerial photos and videos with this drone",
                    CategoryId = 14,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 15,
                    Price = 80,
                    Name = "Fitness Tracker",
                    ShortDescription = "Monitor your health and fitness with a smart tracker",
                    CategoryId = 15,
                    Images = null
                },
               
            },
            PaginationInfo =  new ProductRepository.PageViewModel(15,1,15),
            
        };
        return viewModel;
    }
    private ProductsPageViewModel GetTestPageViewModelBySearch()
    {
        var viewModel = new ProductsPageViewModel
        {
            Products = new List<ProductShowDTO>
            {
                new ProductShowDTO
                {
                    ProductId = 1,
                    Price = 10000,
                    Name = "Laptop Medium",
                    ShortDescription = "The latest IPhone model with a sleek design",
                    CategoryId = 1,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 2,
                    Price = 5000,
                    Name = "Laptop Pro",
                    ShortDescription = "High-performance laptop for professionals",
                    CategoryId = 2,
                    Images = null
                },
                new ProductShowDTO
                {
                    ProductId = 3,
                    Price = 750,
                    Name = "Laptop Mini",
                    ShortDescription = "Compact tablet for on-the-go use",
                    CategoryId = 3,
                    Images = null
                },
                
               
            },
            PaginationInfo =  new ProductRepository.PageViewModel(3,1,15),
            
        };
        return viewModel;
    }
}