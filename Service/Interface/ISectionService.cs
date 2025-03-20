using System.Security.Claims;
using Entity.ViewModel;

namespace Service.Interface;

public interface ISectionService
{

    Task<IEnumerable<SectionViewModel>> GetAllSectionsAsync();
    Task<SectionViewModel?> GetSectionByIdAsync(int id);
    Task<bool> AddSectionAsync(SectionViewModel model, ClaimsPrincipal userClaims);
    Task<bool> UpdateSectionAsync(SectionViewModel model, ClaimsPrincipal userClaims);
    Task DeleteSectionAsync(int id, bool softDelete,ClaimsPrincipal userClaims);
}
