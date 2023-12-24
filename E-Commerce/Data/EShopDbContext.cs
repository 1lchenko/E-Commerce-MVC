using ECommerce.Models;
using ECommerce.Models.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data
{
    public class EShopDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public EShopDbContext(DbContextOptions<EShopDbContext> options) : base(options)
        { }

        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        
        public DbSet<Image> Images { get; set; }

       


    }
}
     
 
