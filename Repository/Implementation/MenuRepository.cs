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

        public async Task<bool> AddCategoryAsync(MenuCategory menuCategory)
        {
            _context.Set<MenuCategory>().Add(menuCategory);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCategoryBy(MenuCategory menuCategory)
        {
            _context.Set<MenuCategory>().Update(menuCategory);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<List<MenuCategory>> GetAllCategories()
        {
            return await _context.MenuCategories.Where(mc => mc.IsDeleted != true).ToListAsync();
        }

        public async Task<MenuCategory> GetCategoryByName(string category)
        {
            return await _context.MenuCategories.FirstOrDefaultAsync(mc => mc.Name == category);
        }

        public async Task<List<MenuItem>> GetItemsByCategory(int categoryId)
        {
            return await _context.MenuItems.Where(mi => mi.CategoryId == categoryId && mi.IsDeleted != true).ToListAsync();
        }

        public async Task<MenuCategory> GetCategoryByIdAsync(int id)
        {
            return await _context.MenuCategories.FirstOrDefaultAsync(mc => mc.Id == id);
        }

        public async Task<bool> AddItemAsync(MenuItem menuItem)
        {
            _context.Set<MenuItem>().Add(menuItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddItemModifierAsync(MappingMenuItemsWithModifier itemModifier)
        {
            _context.Set<MappingMenuItemsWithModifier>().Add(itemModifier);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<MenuItem> GetItemDetailsById(int? id)
        {
            return await _context.MenuItems.Where(mi => mi.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<MappingMenuItemsWithModifier>> GetModifierGroupsByItemId(int id)
        {
            return await _context.MappingMenuItemsWithModifiers
                .Where(mmim => mmim.MenuItemId == id)
                .Include(mmim => mmim.ModifierGroup)
                    .ThenInclude(mg => mg.Modifiers)
                .ToListAsync();
        }
        public async Task<List<ModifierGroup>> GetModifierGroupsById(int groupId)
        {
            return await _context.ModifierGroups.Where(mg => mg.Id == groupId).Include(mg => mg.Modifiers).ToListAsync();
        }

        public async Task<bool> UpdateItem(MenuItem item)
        {
            _context.Set<MenuItem>().Update(item);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ModifierGroup> GetModifierGroupByIdAsync(int modifierGroupId)
        {
            return await _context.ModifierGroups
                .FirstOrDefaultAsync(mg => mg.Id == modifierGroupId);
        }
        public async Task<MappingMenuItemsWithModifier?> GetItemModifierMappingAsync(int menuItemId, int modifierGroupId)
        {
            return await _context.MappingMenuItemsWithModifiers
                .FirstOrDefaultAsync(m => m.MenuItemId == menuItemId && m.ModifierGroupId == modifierGroupId);
        }


        public async Task RemoveItemModifierAsync(MappingMenuItemsWithModifier mapping)
        {
            _context.MappingMenuItemsWithModifiers.Remove(mapping);
            await _context.SaveChangesAsync();
        }
    }
}
