using Entity.Data;

namespace Repository.Interface;

public interface IModifierRepository
{
Task<bool> AddModifierAsync(ModifierGroup modifierGroup);

        Task<bool> UpdateModifierBy(ModifierGroup modifierGroup);

        Task<List<ModifierGroup>> GetAllModifiers();

        Task<ModifierGroup> GetModifierByName(string category);

        Task<List<Modifier>> GetItemsByModifier(int modifierId);

        Task<ModifierGroup> GetModifierByIdAsync(int id);

        Task<IEnumerable<ModifierGroup>> GetModifierGroupsByIds(int[] modifierGroupIds);

        IQueryable<Modifier> GetItemsByModifierQuery(int? modifierId, string searchString);

        Task<Modifier> GetModifierItemByName(string name);

        Task<bool> AddModifierItemAsync(Modifier modifier);

        Task<List<Modifier>> GetAllModifiers(string searchString);

        Task<List<Modifier>> GetModifiersByIds(List<int> modifierIds);

        Task<bool> UpdateModifiers(List<Modifier> modifiers);
}
