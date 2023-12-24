using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.IdentityModels
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Declare your email address")]
        [Display(Name = "User Email")]
        public string Email { get; set; }
        public string? ReturnUrl { get; set; } = "/";

        [Required(ErrorMessage = "Declare your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool Remember { get; set; }
    }
}
 