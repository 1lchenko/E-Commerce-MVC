using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Home.ProductDTO
{
    public class ProductAddDTO
    {
        [Required(ErrorMessage = "Please, give item its Name")]
        public string Name { get; set; }
        
        [Required]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Write positive Price!")]
        [Range(1, int.MaxValue, ErrorMessage = "Write positive Price!")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Please, declare short description")]
        [StringLength(150, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 150 characters long.")]
        public string ShortDescription { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
    }
}
