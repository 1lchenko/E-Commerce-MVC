using ECommerce.Models;

namespace ECommerce.Repositories
{
    public interface IOrderRepository
    {
         Task<int> AddOrderAsync(OrderAddDTO order, List<OrderItem> orderItems, string userId);
         Task<bool> DeleteOrderAsync(int id);
         
         // Task<List<Order>> GetAllOrdersAsync(string userId, bool loadAll);
         
         Task<bool> EditOrderAsync(Order order);
         Task<Order?> GetOrderByIdAsync(int? id, bool includeOrderItems);
         Task<List<Order>> GetAllOrderByUserAsync(string userId, bool loadAll);
        






    }
}
