using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class ProductDetail
    {
        [Required]
        public int ProductDetailId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please, write title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please, write description")]
        public string Description { get; set; }

    }
}
