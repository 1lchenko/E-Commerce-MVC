using System.ComponentModel.DataAnnotations;


namespace ECommerce.Models
{
    public class Category
    {
        [Required]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please define the name of the category")]
        public string CategoryName { get; set; }







    }
}
