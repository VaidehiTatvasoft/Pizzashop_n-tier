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

        public async Task<List<Modifier>> GetAllModifiersAsync()
        {
            return await _context.Modifiers.Where(m => m.IsDeleted != true).ToListAsync();
        }

        public async Task<List<Modifier>> GetModifiersByGroupAsync(int modifierGroupId)
        {
            return await _context.Modifiers.Where(m => m.ModifierGroupId == modifierGroupId && m.IsDeleted != true).ToListAsync();
        }

        public async Task<Modifier> GetModifierByIdAsync(int modifierId)
        {
            return await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == modifierId && m.IsDeleted != true);
        }

        public async Task<bool> AddModifierAsync(Modifier modifier)
        {
            _context.Modifiers.Add(modifier);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateModifierAsync(Modifier modifier)
        {
            _context.Modifiers.Update(modifier);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteModifierAsync(int modifierId)
        {
            var modifier = await _context.Modifiers.FindAsync(modifierId);
            if (modifier != null)
            {
                modifier.IsDeleted = true;
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}