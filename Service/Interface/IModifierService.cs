using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;

namespace Service.Interface
{
    public interface IModifierService
    {
Task<bool> AddNewModifier(string category, ModifierViewModel model, ClaimsPrincipal userClaims);

        Task<bool> AddModifiersToGroup(List<int> modifierIds, int groupId);

        Task<bool> DeleteModifierById(int id);

        Task<bool> UpdateCategoryBy(ModifierGroup modifierGroup);

        Task<List<ModifierGroup>> GetAllModifiers();

        Task<List<Modifier>> GetItemsByModifiers(int modifierId);

        Task<ModifierViewModel> GetModifierDetailById(int id);

        Task<bool> EditModifier(ModifierViewModel model, int id);
         Task<List<ModifierGroup>> GetAllModifierGroups(); 
        Task<List<Modifier>> GetModifiersByGroupIdAsync(int groupId);

        // Task<(List<Modifier> Items, int TotalCount)> GetPaginatedModifierItems(int? modifierId, int offset, int pageSize, string searchString);

        Task<bool> AddNewModifierItem(MenuModifierViewModel model);

        Task<List<Modifier>> GetAllModifiers(string searchString);
}
}