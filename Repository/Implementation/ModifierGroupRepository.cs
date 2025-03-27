
using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation
{
    public class ModifierGroupRepository : IModifierGroupRepository
    {
        private readonly PizzaShopContext _context;

        public ModifierGroupRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<List<ModifierGroup>> GetAllModifierGroupsAsync()
        {
            return await _context.ModifierGroups.Where(mg => mg.IsDeleted != true).ToListAsync();
        }

        public async Task<ModifierGroup> GetModifierGroupByIdAsync(int modifierGroupId)
        {
            return await _context.ModifierGroups.FirstOrDefaultAsync(mg => mg.Id == modifierGroupId && mg.IsDeleted != true);
        }

        public async Task<ModifierGroup> GetModifierGroupByNameAsync(string name)
        {
            return await _context.ModifierGroups.FirstOrDefaultAsync(mg => mg.Name == name && mg.IsDeleted != true);
        }

        public async Task<bool> AddModifierGroupAsync(ModifierGroup modifierGroup)
        {
            _context.ModifierGroups.Add(modifierGroup);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateModifierGroupAsync(ModifierGroup modifierGroup)
        {
            _context.ModifierGroups.Update(modifierGroup);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteModifierGroupAsync(int modifierGroupId)
        {
            var modifierGroup = await _context.ModifierGroups.FindAsync(modifierGroupId);
            if (modifierGroup != null)
            {
                modifierGroup.IsDeleted = true;
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}