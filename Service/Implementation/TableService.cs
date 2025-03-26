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
    public TableSectionViewModel GetTablesBySectionId(int sectionId, int pageSize, int pageIndex, string? searchString)
    {
        var tables = _tableRepository.GetTablesBySectionId(sectionId, pageSize, pageIndex, searchString);
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
        var totalTable = _tableRepository.GetTableCountBySectionId(sectionId, searchString);
        var model = new TableSectionViewModel
        {
            Tables = filteredTables,
            PageSize = pageSize,
            PageIndex = pageIndex,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(totalTable / (double)pageSize),
            TotalItems = totalTable
        };
        return model;
    }

    public int GetTableCountBySectionId(int sId, string? searchString)
    {
        return _tableRepository.GetTableCountBySectionId(sId, searchString!);
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
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return false;
        }
        var userId = int.Parse(userIdClaim.Value);
        try
        {
            var occupied = false;

            foreach (var table in tableIds)
            {
                var isDelete = _tableRepository.DeleteTable(table, userId);
                if (isDelete == false)
                {
                    occupied = true;
                    break;
                }
            }
            if (occupied == true)
                return false;
            return true;
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool AddTable(TableViewModel model, ClaimsPrincipal userClaims)
    {
        Table isTable = _tableRepository.IsTableExist(model.Name, model.SectionId, model.Id);
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
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

            return _tableRepository.AddTable(newTable);
        }
        return false;
    }

    public async Task<TableViewModel> GetTableById(int id)
    {
        return await _tableRepository.GetTableById(id);
    }

    public bool UpdateTable(TableViewModel model, ClaimsPrincipal userClaims)
    {
        Table table = _tableRepository.IsTableExist(model.Name, model.SectionId, model.Id);
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return false;
        }
        var userId = int.Parse(userIdClaim.Value);
        if (table != null)
        {

            table.Name = model.Name;
            table.SectionId = model.SectionId;
            table.Capacity = model.Capacity;
            table.IsAvailable = model.IsAvailable;
            table.CreatedBy = userId;
            table.CreatedAt = DateTime.UtcNow;


            return _tableRepository.UpdateTable(table);
        }
        return false;
    }
    //   public bool UpdateTable(TableViewModel model, int userId)
    //     {
    //         Table table = _tableRepository.IsTableExist(model.Name, model.SectionId, model.Id);

    //         if (table != null)
    //         {
    //             table.Name = model.Name;
    //             table.SectionId = model.SectionId;
    //             table.Capacity = model.Capacity;
    //             table.IsAvailable = model.IsAvailable;
    //             table.CreatedBy = userId;
    //             table.CreatedAt = DateTime.UtcNow;

    //             return _tableRepository.UpdateTable(table);
    //         }
    //         return false;
    //     }
}
