using System.Security.Claims;
using Entity.ViewModel;

namespace Service.Interface;

public interface IModifierService
{
    Task<IEnumerable<ModifierViewModel>> GetAllModifiersAsync();
    Task<IEnumerable<ModifierViewModel>> GetModifiersByGroupAsync(int modifierGroupId);
    Task<ModifierViewModel> GetModifierByIdAsync(int modifierId);
    Task<bool> AddNewModifierAsync(ModifierViewModel modifier, ClaimsPrincipal userClaims);
    Task<bool> UpdateModifierAsync(ModifierViewModel modifier, ClaimsPrincipal userClaims);
    Task<bool> DeleteModifierAsync(int modifierId);
}
