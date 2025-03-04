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
            return await _context.ModifierGroups.ToListAsync();
        }
    }
}