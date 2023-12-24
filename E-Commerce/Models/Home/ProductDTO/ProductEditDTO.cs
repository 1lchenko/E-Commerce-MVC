
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Home.ProductDTO
{
    public class ProductEditDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please, give item its Name")]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Range(1,int.MaxValue, ErrorMessage = "Write positive price")]
        [Required(ErrorMessage = "Please, determine price of product")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Please, declare short description")]
        [StringLength(150, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters long.")]
        public string ShortDescription { get; set; }

        public bool DeleteAllImages { get; set; }
        public List<string>? Images { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
    }
}
