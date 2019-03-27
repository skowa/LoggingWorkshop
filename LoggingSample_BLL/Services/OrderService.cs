using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_DAL.Context;

namespace LoggingSample_BLL.Services
{
    public class OrderService : IDisposable
    {
        private readonly AppDbContext _context = new AppDbContext();

        public Task<IEnumerable<OrderModel>> GetCustomerOrders(int customerId)
        {
            return _context.Orders.Where(item => item.CustomerId == customerId).ToListAsync().ContinueWith(
                task =>
                {
                    var orders = task.Result;
                    
                    return orders.Count != 0 ? orders.Select(order => order.Map()) : Enumerable.Empty<OrderModel>();
                });
        }

        public Task<OrderModel> GetOrder(int customerId, int orderId)
        {
            return _context.Orders.SingleOrDefaultAsync(item => item.Id == orderId && item.CustomerId == customerId)
                .ContinueWith(task =>
                {
                    var order = task.Result;

                    return order?.Map();
                });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}