using Entity.Data;
using Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Repository.Implementation
{
    public class MenuRepository : IMenuRepository
    {
        private readonly PizzaShopContext _context;

         public MenuRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync()
        {
            return await _context.MenuCategories.Where(c => c.IsDeleted == false).ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAllItemsAsync()
        {
            return await _context.MenuItems.Where(i => i.IsDeleted == false).ToListAsync();
        }

        public async Task<IEnumerable<Modifier>> GetAllModifiersAsync()
        {
            return await _context.Modifiers.Where(m => m.IsDeleted == false).ToListAsync();
        }
    }
}
