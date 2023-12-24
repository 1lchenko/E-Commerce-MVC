



namespace TestProject1;

public class BasicTests 
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public BasicTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Account/Logout")]
    [InlineData("/Account/Login")]
    [InlineData("/Account/Registration")]
    [InlineData("/Account/ForgotPassword")]
    public async Task Post_DeleteProduct_ReturnsOkResult(string url)
    {
        var response = await _httpClient.GetAsync("/");
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", 
            response.Content.Headers.ContentType.ToString());
    }
    
}