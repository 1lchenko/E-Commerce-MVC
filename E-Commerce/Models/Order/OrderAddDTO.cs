using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models;

public class OrderAddDTO
{
    [Required]
    public DateTime OrderDate { get; set; } = DateTime.Now;
        
    [Required]
    public string Status { get; set; } = OrderStatusEnum.OrderStatus.Accepted.ToString();

    [Required]
    [Range(1,Int32.MaxValue,ErrorMessage = "TotalPrice must be positive")]
    public int TotalPrice { get; set; }

    [Required(ErrorMessage = "Please, write your adress")]
    public string Adress { get; set; }

    [RegularExpression(@"\+380\d{9}", ErrorMessage = "Write telephone number in the format: +380XXXXXXXXX")]
    [Required(ErrorMessage = "Please, write your phone number")]
    public string PhoneNumber { get; set; }
    
    
    public string? Comment { get; set; }
}