using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;

namespace Repository.Implementation;

public class TableRepository: ITableRepository
{
    private readonly PizzaShopContext _context;

    public TableRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public List<TableViewModel> GetAllTablesAsync()
    {
        var tables = _context.Tables.Where(t => t.IsDeleted == false)
        .Select(t => new TableViewModel
        {
            Id = t.Id,
            SectionId = t.SectionId,
            Name = t.Name,
            Capacity = t.Capacity,
            IsAvailable = t.IsAvailable,
            IsDeleted = t.IsDeleted,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = t.CreatedBy,
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = t.ModifiedBy
        })
        .OrderBy(t => t.Name).ToList();
        return tables;
    }
}
