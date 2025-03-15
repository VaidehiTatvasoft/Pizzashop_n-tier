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

        public async Task<IEnumerable<MenuItem>> GetItemsByCategory(int categoryId)
        {
            return await _context.MenuItems.Where(mi => mi.CategoryId == categoryId).ToListAsync();
        }

        public async Task<MenuCategory> GetCategoryByIdAsync(int id)
        {
            return await _context.MenuCategories.FirstOrDefaultAsync(mc => mc.Id == id);
        }

        public async Task<MenuItem> GetItemDetailsById(int? id)
        {
            return await _context.MenuItems.Where(mi => mi.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> AddItemModifierAsync(MappingMenuItemsWithModifier itemModifier)
        {
            _context.Set<MappingMenuItemsWithModifier>().Add(itemModifier);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<MappingMenuItemsWithModifier>> GetModifierGroupsByItemId(int id)
        {
            return await _context.MappingMenuItemsWithModifiers
                .Where(mmim => mmim.MenuItemId == id)
                .Include(mmim => mmim.ModifierGroup)
                    .ThenInclude(mg => mg.Modifiers)
                .ToListAsync();
        }

        public async Task<ModifierGroup> GetModifierGroupByIdAsync(int modifierGroupId)
        {
            return await _context.ModifierGroups
                .FirstOrDefaultAsync(mg => mg.Id == modifierGroupId);
        }

        public async Task<bool> UpdateItem(MenuItem item)
        {
            _context.Set<MenuItem>().Update(item);
            return await _context.SaveChangesAsync() > 0;
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

        public async Task<bool> DeleteItemByIdAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return false;

            item.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateCategoryBy(MenuCategory category)
        {
            _context.Set<MenuCategory>().Update(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddItemAsync(MenuItem menuItem)
        {
            _context.Set<MenuItem>().Add(menuItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(int categoryId)
        {
            return await _context.MenuItems.Where(mi => mi.CategoryId == categoryId).ToListAsync();
        }

        public async Task<bool> UpdateItemAsync(MenuItem item)
        {
            _context.Set<MenuItem>().Update(item);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}