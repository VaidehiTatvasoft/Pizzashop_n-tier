using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Repository.Implementation;

public class OrderRepository: IOrderRepository
    {
        private readonly PizzaShopContext _context;

        public OrderRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Invoices)
                    .ThenInclude(i => i.Payments)
                .ToListAsync();
        }

        
}
