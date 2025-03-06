using Entity.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IModifierGroupRepository
    {
        Task<IEnumerable<ModifierGroup>> GetAllModifierGroupsAsync();
         Task<ModifierGroup> GetModifierGroupByIdAsync(int id);
        Task AddModifierGroupAsync(ModifierGroup modifierGroup);
        Task UpdateModifierGroupAsync(ModifierGroup modifierGroup);
        Task SoftDeleteModifierGroupAsync(int id);
    }
}
