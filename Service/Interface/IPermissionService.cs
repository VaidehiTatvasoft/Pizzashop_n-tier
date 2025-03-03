using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Data;
using Entity.ViewModel;

namespace Web.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<RoleViewModel>> GetRolesAsync();
        Task<IEnumerable<RolePermission>> GetPermissionsByRoleIdAsync(int roleId);
        Task UpdatePermissionAsync(Permission permission);
    }
}