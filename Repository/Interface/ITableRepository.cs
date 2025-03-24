using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface ITableRepository
{
     List<TableViewModel> GetAllTablesAsync();
    public bool DeleteTable(int id, int userId);

    public bool AddTable(Table model);

    public bool UpdateTable(Table model);

    public bool IsTableExist(string name, int sectionId, int tableId);
    public Task<TableViewModel> GetTableById(int id);
    public List<Table> GetTablesBySectionId(int sectionId, int pageSize, int pageIndex, string? searchString);
    public int GetTableCountBySectionId(int sId, string? searchString);
}
