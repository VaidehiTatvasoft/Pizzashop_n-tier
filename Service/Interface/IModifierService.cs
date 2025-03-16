using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;

namespace Service.Interface
{
    public interface IModifierService
    {
       Task<List<ModifierGroup>> GetAllModifiers();

    Task<List<Modifier>> GetItemsByModifiers(int modifierId);

    Task<bool> AddNewModifier(string category, ModifierViewModel model,ClaimsPrincipal userClaims);

    Task<bool> DeleteModifierById(int id);

    Task<ModifierViewModel> GetModifierDetailById(int id);

    Task<bool> EditModifier(ModifierViewModel model, int id);
}
}