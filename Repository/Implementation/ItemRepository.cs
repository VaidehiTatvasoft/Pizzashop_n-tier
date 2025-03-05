using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation
{
    public class ItemRepository : IItemRepository
    {
        private readonly  PizzaShopContext _context;

        public ItemRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(int categoryId)
        {
            return await _context.MenuItems.Where(i => i.CategoryId == categoryId).ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAllItemsAsync()
        {
            return await _context.MenuItems.Where(i => i.IsDeleted == false).ToListAsync();
        }

        public async Task AddItemAsync(MenuItem item)
        {
            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(MenuItem item)
        {
            _context.MenuItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int itemId)
        {
            var item = await _context.MenuItems.FindAsync(itemId);
            if (item != null)
            {
                _context.MenuItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
