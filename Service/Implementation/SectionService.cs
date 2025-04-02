using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;
public class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepository;

    public SectionService(ISectionRepository sectionRepository)
    {
        _sectionRepository = sectionRepository;
    }

    public List<SectionViewModel> GetAllSections()
    {
        var sections = _sectionRepository.GetAllSectionsAsync();
        return sections;
    }

    public async Task<SectionViewModel?> GetSectionByIdAsync(int id)
    {
        var section = _sectionRepository.GetSectionById(id);
        if (section == null)
            return null;

        return new SectionViewModel
        {
            Id = section.Id,
            Name = section.Name,
            Description = section.Description,
        };
    }

public async Task<(bool success, string errorMessage)> AddSectionAsync(SectionViewModel model, ClaimsPrincipal userClaims)
{
    var userIdClaim = userClaims.FindFirst("UserId");
    if (userIdClaim == null)
    {
        return (false, "User not authorized.");
    }
    var userId = int.Parse(userIdClaim.Value);

    bool isNameUnique = await _sectionRepository.IsSectionNameUniqueAsync(model.Name);
    if (!isNameUnique)
    {
        return (false, "Section name must be unique.");
    }

    var section = new Section
    {
        Name = model.Name,
        Description = model.Description,
        CreatedBy = userId,
        CreatedAt = DateTime.UtcNow,
        ModifiedBy = userId,
        ModifiedAt = DateTime.UtcNow,
    };

    bool isAdded = await _sectionRepository.AddSectionAsync(section);
    return isAdded ? (true, string.Empty) : (false, "Failed to add section.");
}

    
public async Task<(bool success, string errorMessage)> UpdateSectionAsync(SectionViewModel model, ClaimsPrincipal userClaims)
{
    var userIdClaim = userClaims.FindFirst("UserId");
    if (userIdClaim == null)
    {
        return (false, "User not authorized.");
    }
    var userId = int.Parse(userIdClaim.Value);

    bool isNameUnique = await _sectionRepository.IsSectionNameUniqueAsync(model.Name, model.Id);
    if (!isNameUnique)
    {
        return (false, "Section name must be unique.");
    }

    var existingSection = _sectionRepository.GetSectionById(model.Id);
    if (existingSection == null)
    {
        return (false, "Section not found.");
    }

    existingSection.Name = model.Name;
    existingSection.Description = model.Description;
    existingSection.ModifiedBy = userId;
    existingSection.ModifiedAt = DateTime.UtcNow;

    bool isUpdated = await _sectionRepository.UpdateSectionAsync(existingSection);
    return isUpdated ? (true, string.Empty) : (false, "Failed to update section.");
}
 public async Task<string> DeleteSectionAsync(int id, bool softDelete, ClaimsPrincipal userClaims)
    {
         var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return "UserId Not found";
        }
        var userId = int.Parse(userIdClaim.Value);
        var isSectionDeleted = await _sectionRepository.DeleteSectionAsync(id, softDelete, userId);
        if (isSectionDeleted == "table is occupied")
        {
            return "table is occupied";
        }
        else if (isSectionDeleted == "success")
        {
            return "success";
        }
        else
        {
            return "section not found";
        }
    }
}
