using Entity.ViewModel;

namespace Repository.Interface;

public interface ITableRepository
{
     List<TableViewModel> GetAllTablesAsync();
}
