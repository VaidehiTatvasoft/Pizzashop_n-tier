using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interfaces;

public interface IRolePermissionRepository
{
    public List<Permission>? GetAllPermissions();
    public List<RolePermissionViewModel>? GetRolePermissionByRoleId(int roleId);

    Task<bool> UpdateRolePermissionAsync(List<RolePermissionViewModel> model, string email);
}

