using Entity.Data;

namespace Repository.Interfaces;

public interface IRoleRepository
{
    Task<Role> GetRoleByIdAsync(int roleId);
    Task<IEnumerable<Role>> GetAllRolesAsync();
}