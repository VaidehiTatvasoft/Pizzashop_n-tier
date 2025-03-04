using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation
{
    public class ModifierRepository : IModifierRepository
    {
        private readonly  PizzaShopContext _context;

        public ModifierRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Modifier>> GetModifiersByGroupAsync(int groupId)
        {
            return await _context.Modifiers.Where(m => m.ModifierGroupId == groupId).ToListAsync();
        }

        public async Task<IEnumerable<Modifier>> GetAllModifiersAsync()
        {
            return await _context.Modifiers.ToListAsync();
        }

        public async Task AddModifierAsync(Modifier modifier)
        {
            _context.Modifiers.Add(modifier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateModifierAsync(Modifier modifier)
        {
            _context.Modifiers.Update(modifier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteModifierAsync(int modifierId)
        {
            var modifier = await _context.Modifiers.FindAsync(modifierId);
            if (modifier != null)
            {
                _context.Modifiers.Remove(modifier);
                await _context.SaveChangesAsync();
            }
        }
    }
}