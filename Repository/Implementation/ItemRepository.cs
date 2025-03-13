using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Data;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation
{
    public class ItemRepository : IItemRepository
    {
        private readonly PizzaShopContext _context;

        public ItemRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(int categoryId)
        {
            return await _context.MenuItems
                .Where(mi => mi.CategoryId == categoryId && !mi.IsDeleted.GetValueOrDefault(false))
                .ToListAsync();
        }

        public async Task<MenuItem> GetItemDetailsByIdAsync(int id)
        {
            return await _context.MenuItems
                .FirstOrDefaultAsync(mi => mi.Id == id && !mi.IsDeleted.GetValueOrDefault(false));
        }

        public async Task<bool> AddItemAsync(MenuItem item)
        {
            _context.MenuItems.Add(item);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateItemAsync(MenuItem item)
        {
            _context.MenuItems.Update(item);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteItemByIdAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return false;

            item.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}