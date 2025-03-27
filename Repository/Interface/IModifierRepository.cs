using Entity.Data;

namespace Repository.Interface;

public interface IModifierRepository
{
    Task<List<ModifierGroup>> GetAllModifiers();

    Task<List<Modifier>> GetItemsByModifier(int modifierId);

    Task<ModifierGroup> GetModifierByName(string category);

    Task<bool> AddModifierAsync(ModifierGroup modifierGroup);

    Task<bool> UpdateModifierBy(ModifierGroup modifierGroup);

    Task<ModifierGroup> GetModifierByIdAsync(int id);

    Task<IEnumerable<ModifierGroup>> GetModifierGroupsByIds(int[] modifierGroupIds);
}
