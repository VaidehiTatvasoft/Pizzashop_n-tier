using Entity.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Entity.ViewModel;
using Repository.Interfaces;

namespace Pizzashop.Repository.Implementation
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly PizzaShopContext _context;
        private readonly IRoleRepository _role;

        public RolePermissionRepository(PizzaShopContext context, IRoleRepository role)
        {
            _context = context;
            _role = role;
        }

        public List<Permission>? GetAllPermissions()
        {
            List<Permission> permissions = _context.Permissions.ToList();
            if (permissions.Count > 0)
                return permissions;
            return null;
        }

        public List<RolePermissionViewModel>? GetRolePermissionByRoleId(int roleId)
        {
            List<RolePermission> rolePermisssion = _context.RolePermissions.Where(rp => rp.RoleId == roleId).OrderBy(rp => rp.PermissionId).ToList();

            if (rolePermisssion.Count > 0)
            {
                var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
                var rolePermissionList = new List<RolePermissionViewModel>();

                foreach (var rp in rolePermisssion)
                {
                    rolePermissionList.Add(new RolePermissionViewModel
                    {
                        Id = rp.Id,
                        RoleId = rp.RoleId,
                        RoleName = role?.Name,
                        Permissionid = rp.PermissionId,
                        PermissionName = _context.Permissions.FirstOrDefault(p => p.Id == rp.PermissionId)?.Name,
                        CanEdit = rp.CanEdit,
                        CanView = rp.CanView,
                        CanDelete = rp.CanDelete
                    });
                }

                return rolePermissionList;
            }

            return null;
        }

        public async Task<bool> UpdateRolePermissionAsync(List<RolePermissionViewModel> model, string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var role = await _role.GetRoleByIdAsync(user.RoleId);

            foreach (var rp in model)
            {
                var rolePermission = await _context.RolePermissions.FirstOrDefaultAsync(x => x.Id == rp.Id);
                if (rolePermission != null)
                {
                    rolePermission.CanEdit = rp.CanEdit;
                    rolePermission.CanView = rp.CanView;
                    rolePermission.CanDelete = rp.CanDelete;
                    rolePermission.ModifiedAt = DateTime.UtcNow;
                    rolePermission.ModifiedBy = user.Id;
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

