 
 

using Microsoft.AspNetCore.Identity;

namespace TestProject1.Helpers;

public static class SeedData
{
    public static void InitializeDbForTests(EShopDbContext db)
    {
        db.Categories.AddRange();
        db.SaveChanges();
    }

    public static async Task CompleteLoginForTests(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        var testUser = new IdentityUser { UserName = "TestUserName", Email = "TestEmail@gmail.com" };
        var result = await userManager.CreateAsync(testUser, "TestPass@123");

        if (result.Succeeded)
        {
            await signInManager.PasswordSignInAsync(testUser, "TestPass@123", isPersistent: false,
                lockoutOnFailure: false);
        }
        else
        {
            throw new Exception();
        }
    }

    public static void ReinitializeDbForTests(EShopDbContext db)
    {
        db.Categories.RemoveRange();
        InitializeDbForTests(db);
    }

    public static List<Category> GetSeedingProducts()
    {
        return new List<Category>()
        {
            new Category
            {
                CategoryName = "TestName1"
            },
            new Category
            {
                CategoryName = "TestName2"
            },
            new Category
            {
                CategoryName = "TestName3"
            },
        };
    }
}