using Entity.Data;

namespace Repository.Interface
{
    public interface IMenuRepository
    {       
        Task<IEnumerable<Modifier>> GetAllModifiersAsync();
    }
}
