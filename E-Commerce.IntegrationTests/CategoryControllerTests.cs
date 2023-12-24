
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TestProject1;

public class CategoryControllerTests 
    : IClassFixture<CustomWebApplicationFactory<Program>> 
    
{
    private readonly CustomWebApplicationFactory<Program> _factory;
   
    public CategoryControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

//     [Fact]
//     public async Task Get_AllCategories_ReturnsOkResult()
//     {
//         // Arrange
//         using (var scope = _factory.Services.CreateScope())
//         {
//             var scopedServices = scope.ServiceProvider;
//             var db = scopedServices.GetRequiredService<EShopDbContext>();
//             SeedData.InitializeDbForTests(db);
//         }
//
//         var client = _factory.WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureTestServices(services =>
//                 {
//                     services.AddAuthentication(defaultScheme: "TestScheme")
//                         .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
//                             "TestScheme", options => { });
//                 });
//             })
//             .CreateClient(new WebApplicationFactoryClientOptions
//             {
//                 AllowAutoRedirect = false
//             });
//         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");
//         
//         // Act
//         var response = await client.GetAsync("/Category/AddCategory");
//         
//         Assert.Equal(HttpStatusCode.OK,response.StatusCode);
//     }
//
//     [Fact]
//     public async Task Post_DeleteCategory_ReturnsRedirectToGetAllCategories()
//     {
//         
//     }
  }

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim (ClaimTypes.Name, "Test User") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}