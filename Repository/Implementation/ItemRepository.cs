
using Entity.Data;
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

        public async Task<List<MenuItem>> GetAllItemsAsync()
        {
            return await _context.MenuItems.Where(mi => mi.IsDeleted != true).ToListAsync();
        }

        public async Task<List<MenuItem>> GetItemsByCategoryAsync(int categoryId)
        {
            return await _context.MenuItems.Where(mi => mi.CategoryId == categoryId && mi.IsDeleted != true).ToListAsync();
        }

        public async Task<MenuItem> GetItemByIdAsync(int itemId)
        {
            return await _context.MenuItems.FirstOrDefaultAsync(mi => mi.Id == itemId && mi.IsDeleted != true);
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

        public async Task<bool> DeleteItemAsync(int itemId)
        {
            var item = await _context.MenuItems.FindAsync(itemId);
            if (item != null)
            {
                item.IsDeleted = true;
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}