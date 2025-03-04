using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Pizzashop.Repository.Implementation
{
public class RoleRepository : IRoleRepository
{
    private readonly PizzaShopContext _context;

    public RoleRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<Role> GetRoleByIdAsync(int roleId)
    {
        return await _context.Roles.FirstOrDefaultAsync(a => a.Id == roleId);
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

}
}

