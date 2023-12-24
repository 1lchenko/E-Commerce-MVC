using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EShop_Testing.FakeIdentity;

public class FakeUserManager : UserManager<IdentityUser>
{
    public bool ShouldReturnUserByEmail { get; set; } = true;
    public bool ShouldReturnUserById { get; set; } = true;
    public bool ResetPasswordIsSucceed { get; set; } = true;
    public bool CreateAsyncIsSucceed { get; set; } = true; 
    
    public bool AddToRoleAsyncIsSucceed { get; set; } = true;

    public bool ConfirmEmailAsyncIsSucceed { get; set; } = true;
    
    public FakeUserManager()
        : base(new Mock<IUserStore<IdentityUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<IdentityUser>>().Object,
            new IUserValidator<IdentityUser>[0],
            new IPasswordValidator<IdentityUser>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<IdentityUser>>>().Object)
    { }
    
    public override Task<IdentityUser> FindByEmailAsync(string email)
    {
        if (ShouldReturnUserByEmail)
        {
            var user = new IdentityUser { Email = email };
            return Task.FromResult(user);
        }
        else
        {
            return Task.FromResult<IdentityUser>(null);
        }
    }
    public override Task<string> GeneratePasswordResetTokenAsync(IdentityUser user)
    {
        return Task.FromResult("3881893728202"); // some random code token
    }

    public override Task<IdentityUser?> FindByIdAsync(string userId)
    {
        if (ShouldReturnUserById)
        {
             
            IdentityUser? user = new IdentityUser { Id = userId, Email = "test@test.com"};
            return Task.FromResult<IdentityUser?>(user);
        }
        else
        {
            return Task.FromResult<IdentityUser>(null);
        }
    }

    public override Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string password)
    {
        if (ResetPasswordIsSucceed)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
            
        }
        else
        {
            var error = new IdentityError();
            error.Description = "ResetPasswordAsyncError";
            return Task.FromResult<IdentityResult>(IdentityResult.Failed(error));
        }
    }

    public override Task<IdentityResult> CreateAsync(IdentityUser user, string password)
    {
        if (CreateAsyncIsSucceed)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
            
        }
        else
        {
            var error = new IdentityError();
            error.Description = "CreateAsyncError";
            return Task.FromResult<IdentityResult>(IdentityResult.Failed(error));
        }
    }

    public override Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
    {
        return Task.FromResult<string>("testKey123");
    }
    
    public override Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role)
    {
        if (AddToRoleAsyncIsSucceed)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
            
        }
        else
        {
            var error = new IdentityError();
            error.Description = "AddToRoleAsync";
            return Task.FromResult<IdentityResult>(IdentityResult.Failed(error));
        }
    } 
    public override Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token)
    {
        if (ConfirmEmailAsyncIsSucceed)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
            
        }
        else
        {
            var error = new IdentityError();
            error.Code = "ConfirmEmailAsyncError";
            error.Description = "ConfirmEmailAsyncError";
            return Task.FromResult<IdentityResult>(IdentityResult.Failed(error));
        }
    }
    
}