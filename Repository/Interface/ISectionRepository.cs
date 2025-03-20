using Entity.Data;

namespace Repository.Interface;

public interface ISectionRepository
{
    Task<IEnumerable<Section>> GetAllSectionsAsync();
    Task<Section?> GetSectionByIdAsync(int id);
    Task<bool> AddSectionAsync(Section section);
    Task<bool> UpdateSectionAsync(Section section);
    Task DeleteSectionAsync(int id, bool softDelete);
}
