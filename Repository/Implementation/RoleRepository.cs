using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Web.Repositories.Interfaces;

namespace Web.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly PizzaShopContext _context;

        public RoleRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission).ToListAsync();
        }
    }
}