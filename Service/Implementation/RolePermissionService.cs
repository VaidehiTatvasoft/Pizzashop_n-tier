using Entity.Data;
using Entity.ViewModel;
using Repository.Interfaces;
using Service.Interface;

namespace Pizzashop.Service.Implementation
{
public class RolePermissionService : IRolePermissionService
{
    private readonly IRoleRepository _roleService;
    private readonly IRolePermissionRepository _rolePermission;

    public RolePermissionService(IRoleRepository role, IRolePermissionRepository rolePermission)
    {
        _roleService = role;
        _rolePermission = rolePermission;
    }

    public async Task<IEnumerable<Role>> GetAllRoles()
    {
        return await _roleService.GetAllRolesAsync();
    }

    public List<RolePermissionViewModel>? GetRolePermissionByRoleId(int roleId)
    {
        var rolePermission = _rolePermission.GetRolePermissionByRoleId(roleId);
        if (rolePermission != null && rolePermission.Count > 0)
            return rolePermission;
        return null;
    }

    public async Task<bool> UpdateRolePermission(List<RolePermissionViewModel> model, string email)
    {
        bool isRolePermission = await _rolePermission.UpdateRolePermissionAsync(model, email);
        return isRolePermission;
    }
}
}