using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PizzaShopContext _context;

        public CategoryRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync()
        {
            return await _context.MenuCategories.Where(c => c.IsDeleted == false).ToListAsync();
        }

        public async Task<MenuCategory> GetCategoryByIdAsync(int id)
        {
            return await _context.MenuCategories.FindAsync(id);
        }

        public async Task AddCategoryAsync(MenuCategory category)
        {
            await _context.MenuCategories.AddAsync(category);
            Console.WriteLine("Category added: " + category.Name);
            await _context.SaveChangesAsync();
            Console.WriteLine("Changes saved to the database.");
        }

        public async Task UpdateCategoryAsync(MenuCategory category)
        {
            _context.MenuCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteCategoryAsync(int id)
        {
            var category = await _context.MenuCategories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
                _context.MenuCategories.Update(category);
                await _context.SaveChangesAsync();

                var menuItems = await _context.MenuItems.Where(mi => mi.CategoryId == id).ToListAsync();
                foreach (var item in menuItems)
                {
                    item.IsDeleted = true;
                    _context.MenuItems.Update(item);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}