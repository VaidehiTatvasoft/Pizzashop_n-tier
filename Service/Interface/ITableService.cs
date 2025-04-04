using System.Security.Claims;
using Entity.ViewModel;
using Entity.Data;


namespace Service.Interface;

public interface ITableService
{
        public List<TableViewModel> GetAllTables();
    public bool DeleteTable(int id, ClaimsPrincipal userClaims);
    public bool MultiDeleteTable(int[] tableIds, ClaimsPrincipal userClaims);
    public bool AddTable(TableViewModel model, ClaimsPrincipal userClaims , out string message);
    public bool UpdateTable(TableViewModel model, ClaimsPrincipal userClaims,out string message);
    public Task<Table> GetTableById(int id);
    public int GetTableCountBySectionId(int sId, string? searchInput);
    public TableSectionViewModel GetTablesBySectionId(int sectionId, int pageSize, int pageIndex, string? searchInput);
}
