using Entity.Data;
using Entity.ViewModel;

namespace Service.Interface;

public interface IRolePermissionService
{
    public Task<IEnumerable<Role>> GetAllRoles();
    public List<RolePermissionViewModel>? GetRolePermissionByRoleId(int roleId);
    Task<bool> UpdateRolePermission(List<RolePermissionViewModel> model, string email);
}
