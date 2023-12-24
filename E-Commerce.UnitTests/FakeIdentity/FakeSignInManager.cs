using Microsoft.AspNetCore.Identity;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EShop_Testing.FakeIdentity;

public class FakeSignInManager : SignInManager<IdentityUser>
{
    public bool PasswordSignInAsyncIsSuccess { get; set; } = true;
    public FakeSignInManager()
        : base(new FakeUserManager(),
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<IdentityUser>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<IdentityUser>>().Object)
    { }

    public override Task<SignInResult> PasswordSignInAsync(IdentityUser user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        if (PasswordSignInAsyncIsSuccess)
        {
            return Task.FromResult(SignInResult.Success);
        }
        else
        {
             
            return Task.FromResult(SignInResult.Failed);
        }
       
    }
    
    public override Task SignOutAsync()
    {
        return Task.CompletedTask;
    }

    

}