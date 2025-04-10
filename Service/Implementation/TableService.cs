using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class TableService : ITableService
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
    public TableSectionViewModel GetTablesBySectionId(int sectionId, int pageSize, int pageIndex, string? searchInput)
    {
        var tables = _tableRepository.GetTablesBySectionId(sectionId, pageSize, pageIndex, searchInput);
        var filteredTables = tables.Select(t => new TableViewModel
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
        }).ToList();
        var totalTable = _tableRepository.GetTableCountBySectionId(sectionId, searchInput);
        var model = new TableSectionViewModel
        {
            Tables = filteredTables,
            PageSize = pageSize,
            PageIndex = pageIndex,
            searchInput = searchInput,
            TotalPage = (int)Math.Ceiling(totalTable / (double)pageSize),
            TotalItems = totalTable
        };
        return model;
    }

    public int GetTableCountBySectionId(int sId, string? searchInput)
    {
        return _tableRepository.GetTableCountBySectionId(sId, searchInput!);
    }

    public bool DeleteTable(int id, ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return false;
        }
        var userId = int.Parse(userIdClaim.Value);
        var isDelete = _tableRepository.DeleteTable(id, userId);
        return isDelete;
    }

    public bool MultiDeleteTable(int[] tableIds, ClaimsPrincipal userClaims)
    {
        if (tableIds == null || tableIds.Length == 0)
            return false;
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
            return false;
        var userId = int.Parse(userIdClaim.Value);
        try
        {
            var result = _tableRepository.BatchDeleteTables(tableIds, userId);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred while deleting tables: {e.Message}");
            return false;
        }
    }

    public bool AddTable(TableViewModel model, ClaimsPrincipal userClaims, out string message)
    {
        Table isTable = _tableRepository.IsTableExist(model.Name, model.SectionId, model.Id);
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            message = "User not authorized.";
            return false;
        }
        var userId = int.Parse(userIdClaim.Value);
        if (isTable == null)
        {
            var newTable = new Table
            {
                Name = model.Name,
                SectionId = model.SectionId,
                Capacity = model.Capacity,
                IsAvailable = model.IsAvailable,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            bool isAdded = _tableRepository.AddTable(newTable);
            message = isAdded ? "Table added successfully." : "Failed to add table.";
            return isAdded;
        }
        message = "Table already exists.";
        return false;
    }

    public async Task<Table> GetTableById(int id)
    {
        return await _tableRepository.GetTableById(id);
    }

    public bool UpdateTable(TableViewModel model, ClaimsPrincipal userClaims, out string message)
    {
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            message = "User not authorized.";
            return false;
        }
        var userId = int.Parse(userIdClaim.Value);

        Table existingTable = _tableRepository.IsTableExist(model.Name, model.SectionId, model.Id);
        if (existingTable != null && existingTable.Id != model.Id)
        {
            message = "Table with the same name already exists.";
            return false;
        }

        var table = _tableRepository.GetTableById(model.Id).Result;
        if (table == null)
        {
            message = "Table not found.";
            return false;
        }

        table.Name = model.Name;
        table.SectionId = model.SectionId;
        table.Capacity = model.Capacity;
        table.IsAvailable = model.IsAvailable;
        table.ModifiedBy = userId;
        table.ModifiedAt = DateTime.UtcNow;
        table.IsDeleted = false;

        bool isUpdated = _tableRepository.UpdateTable(table);
        message = isUpdated ? "Table updated successfully." : "Failed to update table.";
        return isUpdated;
    }
}
