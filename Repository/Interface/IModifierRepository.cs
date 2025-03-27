using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface IModifierRepository
{
    Task<List<Modifier>> GetAllModifiersAsync();
    Task<List<Modifier>> GetModifiersByGroupAsync(int modifierGroupId);
    Task<Modifier> GetModifierByIdAsync(int modifierId);
    Task<bool> AddModifierAsync(Modifier modifier);
    Task<bool> UpdateModifierAsync(Modifier modifier);
    Task<bool> DeleteModifierAsync(int modifierId);
}
