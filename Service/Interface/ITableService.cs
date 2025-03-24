using System.Security.Claims;
using Entity.ViewModel;

namespace Service.Interface;

public interface ITableService
{
        public List<TableViewModel> GetAllTables();
    public bool DeleteTable(int id, ClaimsPrincipal userClaims);
    public bool MultiDeleteTable(int[] tableIds, ClaimsPrincipal userClaims);
    public bool AdddTable(TableViewModel model, ClaimsPrincipal userClaims);
    public bool UpdateTable(TableViewModel model, ClaimsPrincipal userClaims);
    public Task<TableViewModel> GetTableById(int id);
    public int GetTableCountBySectionId(int sId, string? searchString);
    public TableSectionViewModel GetTablesBySectionId(int sectionId, int pageSize, int pageIndex, string? searchString);

}
