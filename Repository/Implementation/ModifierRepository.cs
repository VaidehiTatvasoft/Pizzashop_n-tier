using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation
{
    public class ModifierRepository : IModifierRepository
    {
        private readonly PizzaShopContext _context;

        public ModifierRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<bool> AddModifierAsync(ModifierGroup modifierGroup)
        {
            _context.Set<ModifierGroup>().Add(modifierGroup);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateModifierBy(ModifierGroup modifierGroup)
        {
            _context.Set<ModifierGroup>().Update(modifierGroup);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<List<ModifierGroup>> GetAllModifiers()
        {
            return await _context.ModifierGroups.Where(mg => mg.IsDeleted != true).ToListAsync();
        }

        public async Task<ModifierGroup> GetModifierByName(string category)
        {
            return await _context.ModifierGroups.FirstOrDefaultAsync(mg => mg.Name == category);
        }

        public async Task<List<Modifier>> GetItemsByModifier(int modifierId)
        {
            return await _context.Modifiers
                .Where(m => m.ModifierGroupId == modifierId)
                .Include(m => m.Unit)
                .ToListAsync();
        }

        public async Task<ModifierGroup> GetModifierByIdAsync(int id)
        {
            return await _context.ModifierGroups.FirstOrDefaultAsync(mg => mg.Id == id);
        }

        public async Task<IEnumerable<ModifierGroup>> GetModifierGroupsByIds(int[] modifierGroupIds)
        {
            return await _context.ModifierGroups
                .Include(mg => mg.Modifiers)
                .Where(mg => modifierGroupIds.Contains(mg.Id))
                .ToListAsync();
        }
    }
}