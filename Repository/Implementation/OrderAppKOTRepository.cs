using Entity.Data;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation
{
    public class OrderAppKOTRepository : IOrderAppKOTRepository
    {
        private readonly PizzaShopContext _context;

        public OrderAppKOTRepository(PizzaShopContext context)
        {
            _context = context;
        }

         public async Task<List<Order>> GetOrdersAsync(int? categoryId = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderedItems)
                    .ThenInclude(oi => oi.OrderedItemModifierMappings)
                        .ThenInclude(oim => oim.Modifier)
                .Include(o => o.TableOrderMappings)
                    .ThenInclude(tom => tom.Table)
                        .ThenInclude(t => t.Section)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(o => o.OrderedItems.Any(oi => oi.MenuItem.CategoryId == categoryId));
            }

            return await query.ToListAsync();
        }
    }
}