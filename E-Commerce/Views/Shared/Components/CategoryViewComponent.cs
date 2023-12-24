using ECommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Views.Shared.Components
{

    public class CategoryViewComponent : ViewComponent
    {

        private readonly EShopDbContext _context;

        public CategoryViewComponent(EShopDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }
    }
}
