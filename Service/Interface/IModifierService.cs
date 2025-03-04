using Entity.Data;

namespace Service.Interface
{
    public interface IModifierService
    {
       Task<IEnumerable<Modifier>> GetModifiersByGroupAsync(int groupId);
        Task<IEnumerable<Modifier>> GetAllModifiersAsync();
        Task AddModifierAsync(Modifier modifier);
        Task UpdateModifierAsync(Modifier modifier);
        Task DeleteModifierAsync(int modifierId);
    }
}