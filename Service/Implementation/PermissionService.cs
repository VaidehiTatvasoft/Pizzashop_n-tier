using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Data;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Web.Repositories.Interfaces;
using Web.Services.Interfaces;

namespace Web.Services
{
    public class PermissionService : IPermissionService
    {
            private readonly PizzaShopContext _context;

        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(PizzaShopContext context,IPermissionRepository permissionRepository)
        {
            _context = context;
            _permissionRepository = permissionRepository;
        }

        public async Task<IEnumerable<RoleViewModel>> GetRolesAsync()
        {
            return await _permissionRepository.GetAllRolesAsync();
        }

       public async Task<IEnumerable<RolePermission>> GetPermissionsByRoleIdAsync(int roleId)
        {
            return await _context.RolePermissions
                                .Include(rp => rp.Permission)
                                .Where(rp => rp.RoleId == roleId)
                                .ToListAsync();
        }


        public async Task UpdatePermissionAsync(Permission permission)
        {
            await _permissionRepository.UpdatePermissionAsync(permission);
        }
    }
}