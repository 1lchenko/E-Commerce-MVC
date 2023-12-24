using System.Security.Claims;
using ECommerce.Models.IdentityModels;
using ECommerce.Models.IdentityModels.ResetPassword;
using ECommerce.Services;
using EShop_Testing.FakeIdentity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;


namespace EShop_Testing;

public class AccountControllerTests
{
    private FakeUserManager _mockUserManager;
    private FakeSignInManager _mockSignInManager;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private LoginDTO _loginDTO;
    private Mock<IOrderRepository> _mockRepoOrder;
    private Mock<ILogger<AccountController>> _mockLogger;

    public AccountControllerTests()
    {
        _mockUserManager = new FakeUserManager();
        _mockRepoOrder = new Mock<IOrderRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockSignInManager = new FakeSignInManager();
        _loginDTO = new LoginDTO { Email = "test@test.com", Password = "Test1234", Remember = false };
        _mockLogger = new Mock<ILogger<AccountController>>();
    }
    
    //Login:
    
    [Fact]
    public async Task LoginPost_ValidModelStateAndAllOperationSucceed_ReturnsRedirect()
    {
        //Arrange
        _mockUserManager.ShouldReturnUserByEmail = true;
        _mockSignInManager.PasswordSignInAsyncIsSuccess = true;
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = new AccountController
            (_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);

        //Act
        var result = await controller.Login(_loginDTO);

        //Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/", redirectResult.Url);
    }
    
    [Fact]
    public async Task Login_InvalidModelState_ReturnsView()
    {
        //Arrange
        _mockUserManager.ShouldReturnUserByEmail = true;
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        controller.ModelState.AddModelError("Email", "Required");
        
        //Act
        var result = await controller.Login(_loginDTO);

        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(_loginDTO, viewResult.Model);
        Assert.NotNull(controller.ModelState["Email"]);
    }
    
    [Fact]
    public async Task Login_PasswordSignInResultIsFalse_ReturnsView()
    {
        //Arrange
        _mockSignInManager.PasswordSignInAsyncIsSuccess = false;
        _mockUserManager.ShouldReturnUserByEmail = true;
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);

        //Act
        var result = await controller.Login(_loginDTO);

        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(_loginDTO, viewResult.Model);
        Assert.NotNull(controller.ModelState["Fail Sign In"]);
    }
    
    [Fact]
    public async Task Login_UserIsNull_ReturnsView()
    {
        //Arrange
        _mockUserManager.ShouldReturnUserByEmail = false;
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);

        //Act
        var result = await controller.Login(_loginDTO);

        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(_loginDTO, viewResult.Model);
        Assert.NotNull(controller.ModelState["Fail Authentication"]);
    }
    
    //ForgotPassword:

    [Fact]
    public async Task ForgotPasswordPost_ReturnsForgotPasswordView_WhenModelStateInvalid()
    {
        //Arrange
        _mockUserManager.ShouldReturnUserByEmail = true;
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testForgotPasswordViewModel = new ForgotPasswordViewModel();
        controller.ModelState.AddModelError("Email", "Required");
        
        //Act
        var result = await controller.ForgotPassword(testForgotPasswordViewModel);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(testForgotPasswordViewModel, viewResult.Model);
        Assert.Equal("ResetPassword/ForgotPassword", viewResult.ViewName);
        Assert.NotNull(controller.ModelState["Email"]);
    }
    
    [Fact]
    public async Task ForgotPasswordPost_ReturnsForgotPasswordConfirmationView_WhenUserIsNull()
    {
        //Arrange
        _mockUserManager.ShouldReturnUserByEmail = false;
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testForgotPasswordViewModel = new ForgotPasswordViewModel
        {
            Email = "testEmail@test.com",
        };
       
        
        //Act
        var result = await controller.ForgotPassword(testForgotPasswordViewModel);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("ResetPassword/ForgotPasswordConfirmation", viewResult.ViewName);
       
    }
    
    [Fact]
    public async Task ForgotPasswordPost_ReturnsForgotPasswordView_WhenSendEmailIsFalse()
    {
        //Arrange
        
        var testForgotPasswordViewModel = new ForgotPasswordViewModel
        {
            Email = "testEmail@test.com",
        };
        _mockUserManager.ShouldReturnUserByEmail = true;
        var sendEmailMock = new Mock<ISendEmailService>();
        sendEmailMock
            .Setup(
                s => s.EmailSend(testForgotPasswordViewModel.Email,
                    "Reset Your Password",
                    "Please reset your password by clicking <a href=\"" + It.IsAny<string?>() + "\">here</a>", 
                    true))
            .Returns(false);
        
        var controller
            = new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var urlHelper = new Mock<IUrlHelper>();
        controller.Url = urlHelper.Object;

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        controller.ControllerContext.HttpContext = httpContext;
        
        //Act
        var result = await controller.ForgotPassword(testForgotPasswordViewModel);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("ResetPassword/ForgotPassword", viewResult.ViewName);
        Assert.Equal(testForgotPasswordViewModel, viewResult.Model);
       
    }
    
    [Fact]
    public async Task ForgotPasswordPost_ReturnsForgotPasswordConfirmationView_WhenSendEmailIsTrue()
    {
        //Arrange
        
        var testForgotPasswordViewModel = new ForgotPasswordViewModel
        {
            Email = "testEmail@test.com",
        };
        var mockUserManager = new FakeUserManager();
        mockUserManager.ShouldReturnUserByEmail = true;
        var sendEmailMock = new Mock<ISendEmailService>();
        sendEmailMock
            .Setup(
                s => s.EmailSend(testForgotPasswordViewModel.Email,
                    "Reset Your Password",
                    "Please reset your password by clicking <a href=\"" + It.IsAny<string?>() + "\">here</a>", 
                    true))
            .Returns(true);
        
        var controller
            = new AccountController(mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var urlHelper = new Mock<IUrlHelper>();
        controller.Url = urlHelper.Object;

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        controller.ControllerContext.HttpContext = httpContext;
        
        //Act
        var result = await controller.ForgotPassword(testForgotPasswordViewModel);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("ResetPassword/ForgotPasswordConfirmation", viewResult.ViewName);

    }
    
    //ResetPassword:
    [Fact]
    public async Task ResetPasswordPost_ReturnsNotFound_WhenModelStateValidAndUserIsNull()
    {
        //Arrange
        var testResetPasswordViewModel = new ResetPasswordViewModel
        {
            UserId = "1",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123",
            Code = "dwdc62HJFDJJ",
            
        };
        _mockUserManager.ShouldReturnUserById = false;
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        

        //Act
        var result = await controller.ResetPassword(testResetPasswordViewModel);
        
        //Assert
        var viewNotFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User Not Found", viewNotFound.Value);
    }
    
    [Fact]
    public async Task ResetPasswordPost_ReturnsResetPasswordView_WhenModelStateValidAndUserIsNotNullAndResultNotSucceed()
    {
        //Arrange
        
        _mockUserManager.ShouldReturnUserById = true;
        _mockUserManager.ResetPasswordIsSucceed = false;
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller =
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testResetPasswordViewModel = new ResetPasswordViewModel
        {
            UserId = "1",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123",
            Code = "dwdc62HJFDJJ",
            
        };

        //Act
        var result = await controller.ResetPassword(testResetPasswordViewModel);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("ResetPassword/ResetPassword", viewResult.ViewName);
        Assert.Equal(testResetPasswordViewModel, viewResult.ViewData.Model);
    }
    
    [Fact]
    public async Task ResetPasswordPost_ReturnsRedirect_WhenModelStateValidAndUserIsNotNullAndResultIsSucceed()
    {
        //Arrange
        
        _mockUserManager.ShouldReturnUserById = true;
        _mockUserManager.ResetPasswordIsSucceed = true;
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testResetPasswordViewModel = new ResetPasswordViewModel
        {
            UserId = "1",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123",
            Code = "dwdc62HJFDJJ",
            
        };

        //Act
        var result = await controller.ResetPassword(testResetPasswordViewModel);
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(redirectToActionResult.ControllerName);
        Assert.Equal(nameof(Settings), redirectToActionResult.ActionName);
        
        
    }
    
    [Fact]
    public async Task ResetPasswordPost_ReturnsResetPasswordView_WhenModelStateInvalid()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testResetPasswordViewModel = new ResetPasswordViewModel
        {
            UserId = "1",
            ConfirmPassword = "Admin@123",
            Code = "dwdc62HJFDJJ",
            
        };
        controller.ModelState.AddModelError("Password", "Required");

        //Act
        var result = await controller.ResetPassword(testResetPasswordViewModel);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("ResetPassword/ResetPassword", viewResult.ViewName);
        Assert.Equal(testResetPasswordViewModel, viewResult.ViewData.Model);
        
        
        
    }
    
    //Registration:
    [Fact]
    public async Task RegistrationPost_ReturnView_WhenModelStateIsInvalid()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testRegistrationDto = new RegistrationDTO();
        
        controller.ModelState.AddModelError("Password", "Required");

        //Act
        var result = await controller.Registration(testRegistrationDto);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(testRegistrationDto, viewResult.ViewData.Model);

    }
    
    [Fact]
    public async Task RegistrationPost_ReturnView_WhenModelStateIsValidAndIdentityResultIsNotSucceed()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        _mockUserManager.CreateAsyncIsSucceed = false;
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testRegistrationDTO = new RegistrationDTO
        {
            UserName = "Dima",
            Email = "test@test.com",
            Password = "123",
            ConfirmPassword = "123"
        };
        
         
        //Act
        var result = await controller.Registration(testRegistrationDTO);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(testRegistrationDTO, viewResult.ViewData.Model);
        Assert.NotNull(controller.ModelState["IdentityErrors"]);

    }
    
    [Fact]
    public async Task RegistrationPost_ReturnView_WhenModelStateIsValidAndIdentityResultIsSucceedAndSignInResultIsFalse()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        _mockUserManager.CreateAsyncIsSucceed = true;
        _mockSignInManager.PasswordSignInAsyncIsSuccess = false;
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testRegistrationDto = new RegistrationDTO
        {
            UserName = "Dima",
            Email = "test@test.com",
            Password = "123",
            ConfirmPassword = "123"
        };
        
         
        //Act
        var result = await controller.Registration(testRegistrationDto);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(testRegistrationDto, viewResult.ViewData.Model);
        Assert.NotNull(controller.ModelState["IdentityErrors"]);

    }
    
    [Fact]
    public async Task RegistrationPost_ReturnRedirectResult_WhenModelStateIsValidAndIdentityResultIsSucceedAndSignInResultIsTrue()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        _mockUserManager.CreateAsyncIsSucceed = true;
        _mockSignInManager.PasswordSignInAsyncIsSuccess = true;
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testRegistrationDto = new RegistrationDTO
        {
            UserName = "Dima",
            Email = "test@test.com",
            Password = "123",
            ConfirmPassword = "123"
        };
        
         
        //Act
        var result = await controller.Registration(testRegistrationDto);
        
        //Assert
        var viewRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", viewRedirect.ActionName);
        Assert.Equal("Home", viewRedirect.ControllerName);
         

    }
    
    //ConfirmEmail:
    [Fact]
    public async Task ConfirmEmail_ReturnRedirectResult_WhenIsSendEmailTrue()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        sendEmailMock
            .Setup(
                s => s.EmailSend(It.IsAny<string>(),
                    "Confirm Your Email",
                    "Please confirm your email by clicking <a href=\"" + It.IsAny<string?>() + "\">here</a>", 
                    true))
            .Returns(true);
        _mockUserManager.ShouldReturnUserById = true;
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var urlHelper = new Mock<IUrlHelper>();
        //GetUserId():
        var user = new IdentityUser { Id = "123" };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) }));
        controller.Url = urlHelper.Object;
        _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(principal);
        
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        controller.ControllerContext.HttpContext = httpContext;
         
        
         
        //Act
        var result = await controller.ConfirmEmail();
        
        //Assert
        var viewRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Settings", viewRedirect.ActionName);
        Assert.Null(viewRedirect.ControllerName);

    }
    
    [Fact]
    public async Task ConfirmEmail_ReturnRedirectResult_WhenIsSendEmailFalse()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        sendEmailMock
            .Setup(
                s => s.EmailSend(It.IsAny<string>(),
                    "Confirm Your Email",
                    "Please confirm your email by clicking <a href=\"" + It.IsAny<string?>() + "\">here</a>", 
                    true))
            .Returns(false);
        _mockUserManager.ShouldReturnUserById = true;
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var urlHelper = new Mock<IUrlHelper>();
        //GetUserId():
        var user = new IdentityUser { Id = "123" };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) }));
        controller.Url = urlHelper.Object;
        _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(principal);
        
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        controller.ControllerContext.HttpContext = httpContext;
         
        
         
        //Act
        var result = await controller.ConfirmEmail();
        
        //Assert
        var viewRedirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(Settings), viewRedirectToActionResult.ActionName);
        Assert.Null(viewRedirectToActionResult.ControllerName);
        
    }
    
    //ApplyConfirmEmail:
    [Fact]
    public async Task ApplyConfirmEmail_ReturnsNotFoundResult_WhenUserIsNull()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        _mockUserManager.ShouldReturnUserByEmail = false;
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testToken = "testToken123";
        var testEmail = "test@test.com";
            
        //Act
        var result = await controller.ApplyConfirmEmail(testToken,testEmail);
        
        //Assert
        var viewNotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found for Apply Confirm Email", viewNotFoundResult.Value);

    }
    
    [Fact]
    public async Task ApplyConfirmEmail_ReturnsRedirectResult_WhenUserIsNotNullAndIdentityResultSuccess()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        _mockUserManager.ShouldReturnUserByEmail = true;
        _mockUserManager.ConfirmEmailAsyncIsSucceed = true;
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testToken = "testToken123";
        var testEmail = "test@test.com";
            
        //Act
        var result = await controller.ApplyConfirmEmail(testToken,testEmail);
        
        //Assert
        var viewRedirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(Settings), viewRedirectToActionResult.ActionName);

    }
    
    [Fact]
    public async Task ApplyConfirmEmail_ReturnsBadRequest_WhenUserIsNotNullAndIdentityResultNotSuccess()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        _mockUserManager.ShouldReturnUserByEmail = true;
        _mockUserManager.ConfirmEmailAsyncIsSucceed = false;
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        var testToken = "testToken123";
        var testEmail = "test@test.com";
            
        //Act
        var result = await controller.ApplyConfirmEmail(testToken,testEmail);
        
        //Assert
        var viewBadRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Apply Confirm Email Operation Is Failed", viewBadRequestObjectResult.Value);

    }
    
    //Control:
    [Fact]
    public async Task Control_ReturnsViewResult_WhenOrderNumberIsNull()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        int? testOrderNumber = null;
            
        //Act
        var result = await controller.Control(testOrderNumber);
        
        //Assert
        Assert.IsType<ViewResult>(result);

    }
    [Fact]
    public async Task Control_ReturnsViewResult_WhenOrderIsNull()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        int? testOrderNumber = 1;
        _mockRepoOrder
            .Setup(rep => rep.GetOrderByIdAsync(testOrderNumber, true))
            .ReturnsAsync((Order)null);
            
        //Act
        var result = await controller.Control(testOrderNumber);
        
        //Assert
        Assert.IsType<ViewResult>(result);

    }
    [Fact]
    public async Task Control_ReturnsViewResultWithOrderModel_WhenOrderNotNullAndOrderNumberNotNull()
    {
        //Arrange
        var sendEmailMock = new Mock<ISendEmailService>();
        var controller = 
            new AccountController(_mockUserManager, _mockSignInManager, _mockHttpContextAccessor.Object,_mockRepoOrder.Object,sendEmailMock.Object,_mockLogger.Object);
        int? testOrderNumber = 1;
        var testOrder = new Order
        {
            OrderId = 1,
            OrderDate = DateTime.Now,
            UserId = "testUserId",
            Adress = "Kiev",
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderId = 1,
                    AmountForOne = 5,
                    OrderItemId = 1,
                    ProductId = 1,
                    ProductName = "Telephone",
                    Quantity = 2,
                }
            }
        };
        _mockRepoOrder
            .Setup(rep => rep.GetOrderByIdAsync(testOrderNumber, true))
            .ReturnsAsync(testOrder);
            
        //Act
        var result = await controller.Control(testOrderNumber);
        
        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(testOrder, viewResult.ViewData.Model);

    }
    
}




 
 