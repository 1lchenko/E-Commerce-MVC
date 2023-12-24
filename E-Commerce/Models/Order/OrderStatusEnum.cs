namespace ECommerce.Models
{
    public class OrderStatusEnum
    {
        public enum OrderStatus
        {
            Cancelled = 0,
            Accepted = 1,        
            Processing = 2,     
            Assembling = 3,      
            Delivering = 4,     
            Delivered = 5      
        }

    }
}
