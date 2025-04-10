using Entity.Data;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class TableRepository : ITableRepository
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

    public int GetTableCountBySectionId(int sId, string? searchInput)
    {
        var tableQuery = _context.Tables.Where(i => i.SectionId == sId && i.IsDeleted == false);
        if (!string.IsNullOrEmpty(searchInput))
        {
            searchInput = searchInput.Trim().ToLower();

            tableQuery = tableQuery.Where(n =>
                n.Name!.ToLower().Contains(searchInput)
            );
        }
        int count = tableQuery.ToList()!.Count();
        return count;
    }

    public List<Table> GetTablesBySectionId(int sectionId, int pageSize, int pageIndex, string? searchInput)
    {
        var tablesQuery = _context.Tables.Where(c => c.SectionId == sectionId && c.IsDeleted == false);

        if (!string.IsNullOrEmpty(searchInput))
        {
            searchInput = searchInput.Trim().ToLower();
            tablesQuery = tablesQuery.Where(i => i.Name.ToLower().Contains(searchInput));
        }

        var tableList = tablesQuery.OrderBy(u => u.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return tableList;
    }
    public bool DeleteTable(int id, int userId)
    {
        var table = _context.Tables.FirstOrDefault(i => i.Id == id);
        if (table!.IsAvailable == false)
        {
            return false;
        }
        table!.IsDeleted = true;
        table.ModifiedBy = userId;
        _context.SaveChanges();

        return true;
    }
    public bool BatchDeleteTables(int[] tableIds, int userId)
    {
        if (tableIds == null || tableIds.Length == 0)
            return false;

        var tables = _context.Tables.Where(t => tableIds.Contains(t.Id)).ToList();

        bool occupied = false;

        foreach (var table in tables)
        {
            if (table.IsAvailable == false)
            {
                occupied = true;
                break;
            }

            table.IsDeleted = true;
            table.ModifiedBy = userId;
        }
        if (occupied)
            return false;

        _context.SaveChanges();
        return true;
    }
    public bool AddTable(Table model)
    {
        try
        {
            _context.Tables.Add(model);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
            return false;
        }
    }

    public bool UpdateTable(Table model)
    {
        try
        {
            var existingEntity = _context.Tables.Local.FirstOrDefault(t => t.Id == model.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }
            _context.Tables.Update(model);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
            return false;
        }
    }

    public async Task<Table> GetTableById(int id)
    {
        var table = await _context.Tables
            .AsNoTracking()
            .Where(t => t.Id == id && t.IsDeleted == false)
            .Select(t => new Table
            {
                Id = t.Id,
                Name = t.Name,
                SectionId = t.SectionId,
                Capacity = t.Capacity,
                IsAvailable = t.IsAvailable
            })
            .FirstOrDefaultAsync();

        return table!;
    }

    public Table IsTableExist(string name, int sectionId, int? tableId = null)
    {
        name = name.Trim().ToLower();
        var table = _context.Tables.FirstOrDefault(t => t.Name.ToLower() == name && t.SectionId == sectionId && (t.IsDeleted == false || t.IsDeleted == null));

        if (tableId.HasValue)
        {
            var existingTable = _context.Tables.FirstOrDefault(t => t.Id == tableId && (t.IsDeleted == false || t.IsDeleted == null));
            if (table != null && table.Id != tableId)
            {
                return table;
            }
            return existingTable;
        }
        return table;
    }
}
