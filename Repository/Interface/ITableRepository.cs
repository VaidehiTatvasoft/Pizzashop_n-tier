using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface ITableRepository
{
    List<TableViewModel> GetAllTablesAsync();
    public bool DeleteTable(int id, int userId);
    public bool AddTable(Table model);
    public bool UpdateTable(Table model);
    bool BatchDeleteTables(int[] tableIds, int userId);
    public Table IsTableExist(string name, int sectionId, int? tableId);
    public Task<Table> GetTableById(int id);
    public List<Table> GetTablesBySectionId(int sectionId, int pageSize, int pageIndex, string? searchInput);
    public int GetTableCountBySectionId(int sId, string? searchInput);
}
