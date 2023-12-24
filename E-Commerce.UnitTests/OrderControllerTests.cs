using System.Security.Claims;
using EShop_Testing.FakeIdentity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace EShop_Testing;

public class OrderControllerTests
{
    //PlaceOrder:

    [Fact]
    public async Task PlaceOrder_ReturnNotFound_WhenOrderItemsIsNull()
    {
        //Arrange
        
        var mockOrderRepository = new Mock<IOrderRepository>();
        var userManager = new FakeUserManager();
        
        //GetUserId():
        var user = new IdentityUser { Id = "123" };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) }));
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(principal);
        
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
        var sessionMock = new Mock<ISession>();
        
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.Session).Returns(sessionMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContextMock.Object
        };
        //Act
        var result = await controller.PlaceOrder();
        
        //Assert
        var viewNotFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Order not may will be without items", viewNotFound.Value);
       
    }
    
    [Fact]
    public async Task PlaceOrderPost_ReturnView_WhenModelStateInvalid()
    {
        //Arrange
        var testOrderAdd = new OrderAddDTO();
        var mockOrderRepository = new Mock<IOrderRepository>();
        var userManager = new FakeUserManager();
        userManager.ShouldReturnUserById = true;
        
        //GetUserId():
        var user = new IdentityUser { Id = "123" };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) }));
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(principal);
        
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
        var sessionMock = new Mock<ISession>();
        
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.Session).Returns(sessionMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContextMock.Object
        };
        controller.ModelState.AddModelError("Adress", "Required");
        //Act
        var result = await controller.PlaceOrder(testOrderAdd);
        
        //Assert
        Assert.IsType<ViewResult>(result);
        
        
    }
    
    //Thanks:
    
    // [Fact]
    // public async Task Thanks_ReturnsBadRequest_WhenHeaderValuesCountIsZero()
    // {
    //     //Arrange
    //     var testOrderId = 0; // should not exist
    //     StringValues testHeaderValues = new StringValues();
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var userManager = new FakeUserManager();
    //    
    //     var requestMock = new Mock<HttpRequest>();
    //
    //
    //     requestMock
    //         .Setup(r => r.Headers.TryGetValue("Referer", out testHeaderValues))
    //         .Returns(false)
    //         .Callback<string, string>((key, testHeaderValues) => testHeaderValues = "RefererHeaderValue");
    //
    //         
    //     
    //     var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
    //     var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
    //     
    //     //Act
    //     var result = controller.Thanks(testOrderId);
    //     
    //     //Assert
    //     var viewBadRequest = Assert.IsType<BadRequestObjectResult>(result);
    //     Assert.Equal("This action can only be accessed via redirection from another action.", viewBadRequest.Value);
    // }
    //
    //EditOrder:

    [Fact]
    public async Task EditOrder_ReturnsNotFound_WhenOrderIsNull()
    {
        //Arrange
        var testOrderId = 0; // Order with this orderId not exist
        var mockOrderRepository = new Mock<IOrderRepository>();
        mockOrderRepository
            .Setup(rep => rep.GetOrderByIdAsync(testOrderId, true))
            .ReturnsAsync((Order)null);
        var userManager = new FakeUserManager();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
       
        
       
        //Act
        var result = await controller.EditOrder(testOrderId);
        
        //Assert
        var viewNotFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Order not found for editing", viewNotFound.Value);
        mockOrderRepository.Verify(r => r.GetOrderByIdAsync(testOrderId, true));
    }
    
    [Fact]
    public async Task EditOrder_ReturnsViewResult_WhenOrderIsNotNull()
    {
        //Arrange
        var testOrderId = 1; // Order with this orderId exist
        var testOrder = new Order
        {
            OrderId = 1,
            Adress = "Adress",
            Comment = null,
            OrderDate = DateTime.Now,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    AmountForOne = 1,
                    OrderId = 1,
                    OrderItemId = 1,
                    ProductId = 1,
                    ProductName = "Laptop",
                    Quantity = 1,
                },
            },
        };
        var mockOrderRepository = new Mock<IOrderRepository>();
        mockOrderRepository
            .Setup(rep => rep.GetOrderByIdAsync(testOrderId, true))
            .ReturnsAsync(testOrder);
        var userManager = new FakeUserManager();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
        
        //Act
        var result = await controller.EditOrder(testOrderId);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(testOrder, viewResult.ViewData.Model);
        mockOrderRepository.Verify(r => r.GetOrderByIdAsync(testOrderId, true));
    }
    
    [Fact]
    public async Task EditOrderPost_ReturnsEditOrderView_WhenModelStateIsInvalid()
    {
        //Arrange
        var testOrder = new Order
        {
            OrderId = 1,
            Adress = null,
            Comment = null,
            OrderDate = DateTime.Now,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    AmountForOne = 1,
                    OrderId = 1,
                    OrderItemId = 1,
                    ProductId = 1,
                    ProductName = "Laptop",
                    Quantity = 1,
                },
            },
        };
        var mockOrderRepository = new Mock<IOrderRepository>();
        var userManager = new FakeUserManager();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
        controller.ModelState.AddModelError("Adress", "Required");
        //Act
        var result = await controller.EditOrder(testOrder);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(testOrder, viewResult.ViewData.Model);
        Assert.Equal("EditOrder", viewResult.ViewName);

    }
    
    [Fact]
    public async Task EditOrderPost_ReturnsEditOrderView_WhenValidationErrorIsNotNull()
    {
        //Arrange
        var testOrder = new Order
        {
            OrderId = 1,
            Adress = "Adress",
            Comment = null,
            OrderDate = DateTime.Now,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    AmountForOne = 1,
                    OrderId = 1,
                    OrderItemId = 1,
                    ProductId = 1,
                    ProductName = "Laptop",
                    Quantity = 0,
                },
            },
        };
        var mockOrderRepository = new Mock<IOrderRepository>();
        var userManager = new FakeUserManager();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
        
        //Act
        var result = await controller.EditOrder(testOrder);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(testOrder, viewResult.ViewData.Model);
        Assert.Equal("EditOrder", viewResult.ViewName);
        Assert.NotNull(controller.ModelState["OrderItemsIsEmpty"]);

    }
    
    [Fact]
    public async Task EditOrderPost_ReturnsNotFoundResult_WhenEditOrderAsyncIsNotSucceed()
    {
        //Arrange
        var testOrder = new Order
        {
            OrderId = 0, // not exist
            Adress = "Adress",
            Comment = null,
            OrderDate = DateTime.Now,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    AmountForOne = 1,
                    OrderId = 1,
                    OrderItemId = 1,
                    ProductId = 1,
                    ProductName = "Laptop",
                    Quantity = 1,
                },
            },
        };
        var mockOrderRepository = new Mock<IOrderRepository>();
        mockOrderRepository
            .Setup(rep => rep.EditOrderAsync(testOrder))
            .ReturnsAsync(false);
        var userManager = new FakeUserManager();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
        
        //Act
        var result = await controller.EditOrder(testOrder);
        
        //Assert
        var viewNotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Order not exist for deleting", viewNotFoundResult.Value);
        mockOrderRepository.Verify(r => r.EditOrderAsync(testOrder));

    }
    
    [Fact]
    public async Task EditOrderPost_ReturnsRedirectToAction_WhenEditOrderAsyncIsSucceed()
    {
        //Arrange
        var testOrder = new Order
        {
            OrderId = 1, // exist
            Adress = "Adress",
            Comment = null,
            OrderDate = DateTime.Now,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    AmountForOne = 1,
                    OrderId = 1,
                    OrderItemId = 1,
                    ProductId = 1,
                    ProductName = "Laptop",
                    Quantity = 1,
                },
            },
        };
        var mockOrderRepository = new Mock<IOrderRepository>();
        mockOrderRepository
            .Setup(rep => rep.EditOrderAsync(testOrder))
            .ReturnsAsync(true);
        var userManager = new FakeUserManager();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
        
        //Act
        var result = await controller.EditOrder(testOrder);
        
        //Assert
        var viewRedirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AccountController.Control), viewRedirectResult.ActionName);
        Assert.Equal("Account", viewRedirectResult.ControllerName);
        Assert.NotNull(viewRedirectResult.RouteValues);
        mockOrderRepository.Verify(r => r.EditOrderAsync(testOrder));
        

    }
    
    //DeleteOrder:

    [Fact]
    public async Task DeleteOrder_ReturnsNotFoundResult_When_OrderIsNull()
    {
        
        //Arrange
        var testOrderId = 0; //  not exist Order with this Id
        var mockOrderRepository = new Mock<IOrderRepository>();
        mockOrderRepository
            .Setup(rep => rep.DeleteOrderAsync(testOrderId))
            .ReturnsAsync(false);
        var userManager = new FakeUserManager();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
        
        //Act
        var result = await controller.DeleteOrder(testOrderId);
        
        //Assert
        var viewNotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Order not found to delete", viewNotFoundResult.Value);
      
    }
    
    [Fact]
    public async Task DeleteOrder_ReturnsRedirectToActionResult_When_OrderIsNotNull()
    {
        
        //Arrange
        var testOrderId = 1;  
        var mockOrderRepository = new Mock<IOrderRepository>();
        mockOrderRepository
            .Setup(rep => rep.DeleteOrderAsync(testOrderId))
            .ReturnsAsync(true);
        var userManager = new FakeUserManager();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var controller = new OrderController(mockOrderRepository.Object,userManager, mockHttpContextAccessor.Object);
        
        //Act
        var result = await controller.DeleteOrder(testOrderId);
        
        //Assert
        var viewRedirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AccountController.Control), viewRedirectResult.ActionName);
        Assert.Equal("Account", viewRedirectResult.ControllerName);
      
    }
}