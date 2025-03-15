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

       public async Task<IEnumerable<MenuItem>> GetAllItems()
        {
            return await _context.MenuItems.ToListAsync();
        }

        public async Task<MenuItem> GetItemById(int id)
        {
            return await _context.MenuItems.FindAsync(id);
        }

        public async Task AddItem(MenuItem item)
        {
            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItem(MenuItem item)
        {
            _context.MenuItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItem(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return;

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}