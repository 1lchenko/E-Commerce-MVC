using System.Text;
using Microsoft.AspNetCore.Http;


namespace EShop_Testing;

public class HomeControllerTests
{
     //Index:
     
     [Fact]
     public async Task Index_ViewBagSearchTextIsNotNull_WhenSearchTextIsNotNull()
     {
          //Arrange
          var testSearchText = "Products";
          var testProductPageViewModel = new ProductsPageViewModel
          {
               Products = new List<ProductShowDTO>
               {
                    new ProductShowDTO
                    {
                         ProductId = 1,
                         Price = 10000,
                         Name = "Products",
                         ShortDescription = "The latest IPhone model with a sleek design",
                         CategoryId = 1,
                         Images = null,
                         CategoryName = "Laptops",


                    },

               },
               PaginationInfo = new ProductRepository.PageViewModel(1, It.IsAny<int>(), 10),
          };
          var mockDetailRepository = new Mock<IProductDetailRepository>();
          var mockProductRepository = new Mock<IProductRepository>();
          mockProductRepository
               .Setup(rep => rep.GetProductsByIndexOfAsync(testSearchText, It.IsAny<int>()))
               .ReturnsAsync(testProductPageViewModel);
          var controller = new HomeController(mockProductRepository.Object,mockDetailRepository.Object);
           
          
          //Act
          var result = await controller.Index(null, testSearchText);
          
          //Assert
          var viewResult = Assert.IsType<ViewResult>(result);
          var model = Assert.IsType<ProductsPageViewModel>(viewResult.ViewData.Model);
          Assert.Equal(testProductPageViewModel,model);
          Assert.Equal(testSearchText, controller.ViewBag.searchText);
          Assert.Null(controller.ViewBag.CategoryId);
          mockProductRepository.Verify(rep => rep.GetProductsByIndexOfAsync(testSearchText, It.IsAny<int>()));
          
          

     }
     
     [Fact]
     public async Task Index_ViewBagCategoryIdIsNotNull_WhenCategoryIdIsNotNull()
     {
          //Arrange
          var testCategoryId = 1;
          var testProductPageViewModel = new ProductsPageViewModel
          {
               Products = new List<ProductShowDTO>
               {
                    new ProductShowDTO
                    {
                         ProductId = 1,
                         Price = 10000,
                         Name = "Products",
                         ShortDescription = "The latest IPhone model with a sleek design",
                         CategoryId = 1,
                         Images = null,
                         CategoryName = "Laptops",


                    },

               },
               PaginationInfo = new ProductRepository.PageViewModel(1, It.IsAny<int>(), 10),
          };
          var mockDetailRepository = new Mock<IProductDetailRepository>();
          var mockProductRepository = new Mock<IProductRepository>();
          mockProductRepository
               .Setup(rep => rep.GetProductsByCategoryAsync(testCategoryId, It.IsAny<int>()))
               .ReturnsAsync(testProductPageViewModel);
          var controller = new HomeController(mockProductRepository.Object,mockDetailRepository.Object);
           
          
          //Act
          var result = await controller.Index(testCategoryId, null);
          
          //Assert
          var viewResult = Assert.IsType<ViewResult>(result);
          var model = Assert.IsType<ProductsPageViewModel>(viewResult.ViewData.Model);
          Assert.Equal(testProductPageViewModel,model);
          Assert.Equal(testCategoryId, controller.ViewBag.CategoryId);
          Assert.Null(controller.ViewBag.searchText);
          mockProductRepository.Verify(rep => rep.GetProductsByCategoryAsync(testCategoryId, It.IsAny<int>()));
          

     }
     
     [Fact]
     public async Task Index_ViewBagCategoryIdIsNullAndViewBagSearchTextIsNull_WhenCategoryIdIsNullAndCategoryIdIsNull()
     {
          //Arrange
          var testProductPageViewModel = new ProductsPageViewModel
          {
               Products = new List<ProductShowDTO>
               {
                    new ProductShowDTO
                    {
                         ProductId = 1,
                         Price = 10000,
                         Name = "Products",
                         ShortDescription = "The latest IPhone model with a sleek design",
                         CategoryId = 1,
                         Images = null,
                         CategoryName = "Laptops",


                    },

               },
               PaginationInfo = new ProductRepository.PageViewModel(1, It.IsAny<int>(), 10),
          };
          var mockDetailRepository = new Mock<IProductDetailRepository>();
          var mockProductRepository = new Mock<IProductRepository>();
          mockProductRepository
               .Setup(rep => rep.GetAllProductAsync(It.IsAny<int>()))
               .ReturnsAsync(testProductPageViewModel);
          var controller = new HomeController(mockProductRepository.Object,mockDetailRepository.Object);
           
          
          //Act
          var result = await controller.Index(null, null);
          
          //Assert
          var viewResult = Assert.IsType<ViewResult>(result);
          var model = Assert.IsType<ProductsPageViewModel>(viewResult.ViewData.Model);
          Assert.Equal(testProductPageViewModel,model);
          Assert.Null(controller.ViewBag.CategoryId);
          Assert.Null(controller.ViewBag.searchText);
          mockProductRepository.Verify(rep => rep.GetAllProductAsync(It.IsAny<int>()));
          

     }
     
     //SaveToSession:
     
     [Fact]
     public void SaveToSession_Should_Save_Product_To_Session()
     {
          // Arrange
          var mockDetailRepository = new Mock<IProductDetailRepository>();
          var mockProductRepository = new Mock<IProductRepository>();
          var controller = new HomeController(mockProductRepository.Object,mockDetailRepository.Object);
          
          var sessionMock = new Mock<ISession>();
          var httpContextMock = new Mock<HttpContext>();
          httpContextMock.Setup(c => c.Session).Returns(sessionMock.Object);
          controller.ControllerContext = new ControllerContext
          {
               HttpContext = httpContextMock.Object
          };

          var product = new ProductSessionDTO
          {
               ProductId = 1,
               ProductName = "Test Product",
               Amount = 10
          };

          // Act
          var result = controller.SaveToSession(product);

          // Assert
          sessionMock.Verify(s => s.Set(".OrderItems", It.IsAny<byte[]>()), Times.Once);
          Assert.IsType<OkResult>(result);
     }
     
     //RemoveFromSession:
     //
     // [Fact]
     // public async Task RemoveFromSession_ReturnsNotFound_WhenOrderItemsIsNull()
     // {
     //      // Arrange
     //      var list = new List<OrderItem>
     //      {
     //           new OrderItem
     //           {
     //                OrderId = 1,
     //           }
     //      };
     //      
     //      
     //      var mockDetailRepository = new Mock<IProductDetailRepository>();
     //      var mockProductRepository = new Mock<IProductRepository>();
     //      var controller = new HomeController(mockProductRepository.Object,mockDetailRepository.Object);
     //
     //      var mockHttpContext = new Mock<HttpContext>();
     //      var mockHttpSession = new MockHttpSession();
     //      mockHttpSession[".OrderItems"] = list;
     //      mockHttpContext.Setup(x => x.Session).Returns(mockHttpSession);
     //      controller.ControllerContext.HttpContext = mockHttpContext.Object;
     //       
     //      
     //      
     //
     //      var product = new ProductSessionDTO
     //      {
     //           ProductId = 1,
     //           ProductName = "Test Product",
     //           Amount = 10
     //      };
     //
     //      // Act
     //      var result = controller.RemoveFromSession(product);
     //
     //      // Assert
     //      var viewNotFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
     //      Assert.Equal("OrderItems not found in session",viewNotFoundObjectResult.Value);
     //      
     //      
     // }
     
     //RemoveFromSession:

     [Fact]
     public async Task RemoveFromSession_ReturnsViewResult_WhenProductIsNotNull()
     {
          //Arrange
          var testProductId = 1;
          var testProductShow = new ProductShowDTO
          {
               ProductId = 1,
               Price = 10000,
               Name = "Products",
               ShortDescription = "The latest IPhone model with a sleek design",
               CategoryId = 1,
               Images = null,
               CategoryName = "Laptops",


          };
          var testProductDetails = new List<ProductDetail>
          {
               new ProductDetail
               {
                    ProductDetailId = 1,
                    ProductId = 1,
                    Title = "Title",
                    Description = "Description"
               }
          };
          var mockDetailRepository = new Mock<IProductDetailRepository>();
          mockDetailRepository
               .Setup(rep => rep.GetProductsDetailsAsync(testProductId))
               .ReturnsAsync(testProductDetails);
          var mockProductRepository = new Mock<IProductRepository>();
          mockProductRepository
               .Setup(rep => rep.GetProductShowDtoByIdAsync(testProductId))
               .ReturnsAsync(testProductShow);
          var controller = new HomeController(mockProductRepository.Object,mockDetailRepository.Object);

          //Act
          var result = await controller.ReadDescriptions(testProductId);
          
          //Assert
          var viewResult = Assert.IsType<ViewResult>(result);
          var model = Assert.IsType<List<ProductDetail>>(viewResult.ViewData.Model);
          Assert.Equal(testProductDetails,model);
          mockProductRepository.Verify(rep => rep.GetProductShowDtoByIdAsync(testProductId));
          mockDetailRepository.Verify(rep => rep.GetProductsDetailsAsync(testProductId));

     }
     
     [Fact]
     public async Task RemoveFromSession_ReturnsNotFoundResult_WhenProductIsNull()
     {
          //Arrange
          var testProductId = 1;
          var mockDetailRepository = new Mock<IProductDetailRepository>();
          var mockProductRepository = new Mock<IProductRepository>();
          mockProductRepository
               .Setup(rep => rep.GetProductShowDtoByIdAsync(testProductId))
               .ReturnsAsync((ProductShowDTO)null);
          var controller = new HomeController(mockProductRepository.Object,mockDetailRepository.Object);

          //Act
          var result = await controller.ReadDescriptions(testProductId);
          
          //Assert
          var viewNotFound = Assert.IsType<NotFoundObjectResult>(result);
          Assert.Equal("Product not found for read description",viewNotFound.Value);
          mockProductRepository.Verify(rep => rep.GetProductShowDtoByIdAsync(testProductId));

     }
     
     
}

public class MockHttpSession : ISession
{
     Dictionary<string, object> sessionStorage = new Dictionary<string, object>();

     public object this[string name]
     {
          get { return sessionStorage[name]; }
          set { sessionStorage[name] = value; }
     }

     string ISession.Id
     {
          get
          {
               throw new NotImplementedException();
          }
     }

     bool ISession.IsAvailable
     {
          get
          {
               throw new NotImplementedException();
          }
     }

     IEnumerable<string> ISession.Keys
     {
          get { return sessionStorage.Keys; }
     }

     void ISession.Clear()
     {
          sessionStorage.Clear();
     }

    

     void ISession.Remove(string key)
     {
          sessionStorage.Remove(key);
     }

     void ISession.Set(string key, byte[] value)
     {
          sessionStorage[key] = value;
     }

     public async Task LoadAsync(CancellationToken cancellationToken = new CancellationToken())
     {
          throw new NotImplementedException();
     }

     public async Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
     {
          throw new NotImplementedException();
     }

     bool ISession.TryGetValue(string key, out byte[] value)
     {
          if (sessionStorage[key] != null)
          {
               value = Encoding.ASCII.GetBytes(sessionStorage[key].ToString());
               return true;
          }
          else
          {
               value = null;
               return false;
          }
     }        
}
 