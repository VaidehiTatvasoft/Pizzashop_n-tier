using Entity.Data;
using Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository.Implementation
{
    public class ModifierGroupRepository : IModifierGroupRepository
    {
        private readonly PizzaShopContext _context;

        public ModifierGroupRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModifierGroup>> GetAllModifierGroupsAsync()
        {
            return await _context.ModifierGroups.Where(mg => mg.IsDeleted == false).ToListAsync();
        }

        public async Task<ModifierGroup> GetModifierGroupByIdAsync(int id)
        {
            return await _context.ModifierGroups.FindAsync(id);
        }

        public async Task AddModifierGroupAsync(ModifierGroup modifierGroup)
        {
            await _context.ModifierGroups.AddAsync(modifierGroup);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateModifierGroupAsync(ModifierGroup modifierGroup)
        {
            _context.ModifierGroups.Update(modifierGroup);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteModifierGroupAsync(int id)
        {
            var modifierGroup = await _context.ModifierGroups.FindAsync(id);
            if (modifierGroup != null)
            {
                modifierGroup.IsDeleted = true;
                _context.ModifierGroups.Update(modifierGroup);
                await _context.SaveChangesAsync();
                var relatedModifiers = await _context.Modifiers.Where(m => m.ModifierGroupId == id).ToListAsync();
                foreach (var modifier in relatedModifiers)
                {
                    modifier.IsDeleted = true;
                    _context.Modifiers.Update(modifier);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}