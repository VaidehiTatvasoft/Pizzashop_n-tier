using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class TableService: ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableService)
    {
        _tableRepository = tableService;
    }

    public List<TableViewModel> GetAllTables()
    {
        var sections = _tableRepository.GetAllTablesAsync();
        return sections;
    }
}
