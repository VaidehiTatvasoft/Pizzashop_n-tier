using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PizzaShopContext _context;

        public CategoryRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<List<MenuCategory>> GetAllCategoriesAsync()
        {
            return await _context.MenuCategories.Where(mc => mc.IsDeleted != true).ToListAsync();
        }

        public async Task<MenuCategory> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.MenuCategories.FirstOrDefaultAsync(mc => mc.Id == categoryId && mc.IsDeleted != true);
        }

        public async Task<MenuCategory> GetCategoryByNameAsync(string categoryName)
        {
            return await _context.MenuCategories.FirstOrDefaultAsync(mc => mc.Name == categoryName && mc.IsDeleted != true);
        }

        public async Task<bool> AddCategoryAsync(MenuCategory category)
        {
            _context.MenuCategories.Add(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCategoryAsync(MenuCategory category)
        {
            _context.MenuCategories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.MenuCategories.FindAsync(categoryId);
            if (category != null)
            {
                category.IsDeleted = true;
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}