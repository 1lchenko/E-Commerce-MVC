using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.IdentityModels.ResetPassword
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
