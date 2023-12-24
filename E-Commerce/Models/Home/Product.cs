using System.ComponentModel.DataAnnotations;
using ECommerce.Models.Home;

namespace ECommerce.Models
{
    public class Product
    {
        [Required]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Please, give item its Name")]
        public string Name { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public int Price { get; set; }
        
        [Required(ErrorMessage = "Please, declare short description")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters long.")]
        public string ShortDescription { get; set; }
        public List<Image>? Images { get; set; }
        public Category? Category { get; set; }
        public List<ProductDetail>? ProductDetails { get; set; }



    }
}
