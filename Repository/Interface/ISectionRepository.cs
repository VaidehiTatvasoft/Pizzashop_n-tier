using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface ISectionRepository
{
    List<SectionViewModel> GetAllSectionsAsync();
    Section GetSectionById(int sectionId);
    Task<bool> IsSectionNameUniqueAsync(string name, int? sectionId = null);

    Task<bool> AddSectionAsync(Section section);
    Task<bool> UpdateSectionAsync(Section section);
    Task<string> DeleteSectionAsync(int id, bool softDelete, int userId);
}
