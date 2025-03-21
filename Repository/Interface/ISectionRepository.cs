using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface ISectionRepository
{
 List<SectionViewModel> GetAllSectionsAsync();
     Task<Section?> GetSectionByIdAsync(int id);
    Task<bool> AddSectionAsync(Section section);
    Task<bool> UpdateSectionAsync(Section section);
    Task DeleteSectionAsync(int id, bool softDelete);
}
