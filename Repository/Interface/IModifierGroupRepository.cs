using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface IModifierGroupRepository
{
    Task<List<ModifierGroup>> GetAllModifierGroupsAsync();
    Task<ModifierGroup> GetModifierGroupByIdAsync(int modifierGroupId);
    Task<ModifierGroup> GetModifierGroupByNameAsync(string name);
    Task<bool> AddModifierGroupAsync(ModifierGroup modifierGroup);
    Task<bool> UpdateModifierGroupAsync(ModifierGroup modifierGroup);
    Task<bool> DeleteModifierGroupAsync(int modifierGroupId);
}
