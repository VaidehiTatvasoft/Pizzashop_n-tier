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

    public async Task<IEnumerable<SectionViewModel>> GetAllSectionsAsync()
    {
        var sections = await _sectionRepository.GetAllSectionsAsync();
        return sections.Select(s => new SectionViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
        }).ToList();
    }

    public async Task<SectionViewModel?> GetSectionByIdAsync(int id)
    {
        var section = await _sectionRepository.GetSectionByIdAsync(id);
        if (section == null)
            return null;

        return new SectionViewModel
        {
            Id = section.Id,
            Name = section.Name,
            Description = section.Description,
        };
    }

    public async Task<bool> AddSectionAsync(SectionViewModel model, ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return false;
        }
        var userId = int.Parse(userIdClaim.Value);
        var section = new Section
        {
            Name = model.Name,
            Description = model.Description,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            ModifiedBy = userId,
            ModifiedAt = DateTime.UtcNow,
        };
        return await _sectionRepository.AddSectionAsync(section);
    }

    public async Task<bool> UpdateSectionAsync(SectionViewModel model, ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return false;
        }
        var userId = int.Parse(userIdClaim.Value);
        var existingSection = await _sectionRepository.GetSectionByIdAsync(model.Id);
        if (existingSection == null)
        {
            return false;
        }

        existingSection.Name = model.Name;
        existingSection.Description = model.Description;
        existingSection.ModifiedBy = userId;
        existingSection.ModifiedAt = DateTime.UtcNow;

        return await _sectionRepository.UpdateSectionAsync(existingSection);
    }

    public async Task DeleteSectionAsync(int id, bool softDelete, ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return;
        }
        var userId = int.Parse(userIdClaim.Value);
        var section = await _sectionRepository.GetSectionByIdAsync(id);
        if (section != null)
        {
            section.ModifiedBy = userId;
            section.ModifiedAt = DateTime.UtcNow;
        }
        await _sectionRepository.DeleteSectionAsync(id, softDelete);
    }
}
