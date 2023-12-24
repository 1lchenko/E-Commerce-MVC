using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class OrderItem
    {
        [Required]
        public int OrderItemId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(0,99, ErrorMessage = "min is 0, max is 99")]
        [Required]
        public int Quantity { get; set; }

        [Required]
        public int AmountForOne { get; set; }

    }
}
