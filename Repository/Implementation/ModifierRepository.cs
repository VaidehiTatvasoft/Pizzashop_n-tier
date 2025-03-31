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
         public IQueryable<Modifier> GetItemsByModifierQuery(int? modifierId, string searchString)
        {
            var menuItems = _context.Modifiers
                .Where(mi => mi.ModifierGroupId == modifierId && mi.IsDeleted != true).OrderBy(mi => mi.Name)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {

                var lowerSearchString = searchString.ToLower();
                menuItems = menuItems.Where(mi => mi.Name.ToLower().Contains(lowerSearchString) ||
                                                  mi.Rate.ToString().Contains(lowerSearchString) ||
                                                  mi.Quantity.ToString().Contains(lowerSearchString));
            }

            return menuItems;
        }
        public async Task<Modifier> GetModifierItemByName(string name)
        {
            return await _context.Modifiers.FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<bool> AddModifierItemAsync(Modifier modifier)
        {
            _context.Set<Modifier>().Add(modifier);
            return await _context.SaveChangesAsync() > 0;
        }
        
        public async Task<List<Modifier>> GetAllModifiers(string searchString)
        {
            var modifiers = await _context.Modifiers
                .Where(m => string.IsNullOrEmpty(searchString) || m.Name.ToLower().Contains(searchString.ToLower()))
                .ToListAsync();

            return modifiers;
        }

        public async Task<List<Modifier>> GetModifiersByIds(List<int> modifierIds)
        {
            return await _context.Modifiers.Where(m => modifierIds.Contains(m.Id)).ToListAsync();
        }

        public async Task<bool> UpdateModifiers(List<Modifier> modifiers)
        {
            _context.Modifiers.UpdateRange(modifiers);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}