using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Order
    {
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        [Required]
        public string Status { get; set; } = OrderStatusEnum.OrderStatus.Accepted.ToString();

        [NotMapped]
        public int TotalPrice
        {
            get
            {
                return OrderItems?.Sum(x => x.Quantity * x.AmountForOne) ?? 0;
            }
            
        }

        [Required(ErrorMessage = "Please, write your adress")]
        public string Adress { get; set; }

        [RegularExpression(@"\+380\d{9}", ErrorMessage = "Введите номер телефона в формате +380XXXXXXXXX")]
        [Required(ErrorMessage = "Please, write your phone number")]
        public string PhoneNumber { get; set; }
        public string? Comment { get; set; }
        public virtual List<OrderItem>? OrderItems { get; set; }
        
        public IdentityUser? User { get; set; }

    }
}
