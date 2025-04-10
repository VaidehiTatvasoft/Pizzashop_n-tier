using System.Security.Claims;
 using Entity.ViewModel;
 
 namespace Service.Interface;
 
 public interface ISectionService
 {
 
     public List<SectionViewModel> GetAllSections();
     Task<SectionViewModel?> GetSectionByIdAsync(int id);
     Task<(bool success, string errorMessage)> AddSectionAsync(SectionViewModel model, ClaimsPrincipal userClaims);
     Task<(bool success, string errorMessage)> UpdateSectionAsync(SectionViewModel model, ClaimsPrincipal userClaims);
     Task<string> DeleteSectionAsync(int id, bool softDelete, ClaimsPrincipal userClaims);
 }