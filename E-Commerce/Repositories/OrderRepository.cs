using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using AutoMapper;
using ECommerce.Exceptions;
using ECommerce.Data;
using ECommerce.Models;

namespace ECommerce.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EShopDbContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(EShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> AddOrderAsync(OrderAddDTO orderAddDto, List<OrderItem> orderItems, string userId)
        {
            
            var order = _mapper.Map<OrderAddDTO,Order>(orderAddDto);
            order.UserId = userId;
            order.OrderItems = orderItems;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order.OrderId;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
            {
                return false;
            }
            
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        

        public async Task<bool> EditOrderAsync(Order editOrder)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == editOrder.OrderId);

            if (order == null)
            {
                return false;
            }

            var zeroOrderItems = editOrder.OrderItems!.Where(x => x.Quantity == 0).ToList();
            editOrder.OrderItems = editOrder.OrderItems!.Where(x => x.Quantity > 0).ToList();
            
            if (zeroOrderItems.Count > 0)
            { 
                _context.OrderItems.RemoveRange(zeroOrderItems);
            }

            _mapper.Map(editOrder, order);
            await _context.SaveChangesAsync();
            return true;
            
        }

        public async Task<List<Order>> GetAllOrderByUserAsync(string userId, bool loadAll)
        {

            var query = _context.Orders
                .Include(x => x.OrderItems)
                .Where(x => x.UserId == userId);

            List<Order> orders;

            if (!loadAll)
            {
                orders =  await query.Take(10).ToListAsync();
            }
            else
            {
                orders =  await query.ToListAsync();
            }
            
            return orders;

        }

        
 

      /*  public async Task<List<Order>> GetAllOrdersAsync(string userId, bool loadAll)
        {
            List<Order> orders;
            
            if (loadAll)
            {
                orders = await _context.Orders.ToListAsync();
            }
           
            orders = await _context.Orders.Take(10).ToListAsync(); 
            
            return orders;
        } */

        public async Task<Order?> GetOrderByIdAsync(int? id, bool includeOrderItems)
        {
            Order? order;
           
            if (includeOrderItems)
            {
                order = await _context.Orders
                    .Include(x=>x.OrderItems)
                    .FirstOrDefaultAsync(x => x.OrderId == id);
            }
            else
            {
                order = await _context.Orders
                    .FirstOrDefaultAsync(x => x.OrderId == id);
            }

            return order;
        }

        
        
    }
}