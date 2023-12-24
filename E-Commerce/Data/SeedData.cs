using Microsoft.AspNetCore.Identity;
using ECommerce.Models.IdentityModels;

namespace ECommerce.Data
{
    public class SeedUsersData
    {
        

        public static async Task SeedDefaultUsers(IServiceProvider serviceProvider)
        {
            var _userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            var _roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            var _context = serviceProvider.GetService<EShopDbContext>();

            //Adding some roles to bd
            await _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            var admin = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
            };

            var userInDb = await _userManager.FindByEmailAsync(admin.Email);
            if (userInDb is null)
            {
               var res =  await _userManager.CreateAsync(admin,"Admin@123");
                await _userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
            }

             
             

             

            
            

           await _context.SaveChangesAsync();


        }


    }
}
