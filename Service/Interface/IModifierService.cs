using Entity.Data;

namespace Service.Interface
{
    public interface IModifierService
    {
        Task<IEnumerable<Modifier>> GetAllModifiersAsync();
    }
}