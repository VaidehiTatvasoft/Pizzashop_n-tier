using Entity.Data;

namespace Repository.Interface;

public interface IModifierRepository
{
    Task<IEnumerable<Modifier>> GetModifiersByGroupAsync(int groupId);
    Task<IEnumerable<Modifier>> GetAllModifiersAsync();
    Task AddModifierAsync(Modifier modifier);
    Task UpdateModifierAsync(Modifier modifier);
    Task DeleteModifierAsync(int modifierId);
}
