using Entity.Data;

namespace Service.Interface;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetAllRolesAsync();
}
