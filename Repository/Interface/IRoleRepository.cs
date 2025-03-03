using Entity.Data;


namespace Repository.Interface;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllRolesAsync();
}
