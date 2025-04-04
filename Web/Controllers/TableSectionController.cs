using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Service.Interface;
using Web.Attributes;
using Entity.Shared;

namespace Web.Controllers;

public class TableSectionController : Controller
{
    private readonly ISectionService _sectionService;
    private readonly ITableService _tableService;


    public TableSectionController(ITableService tableService, ISectionService sectionService)
    {
        _tableService = tableService;
        _sectionService = sectionService;
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanView)]
    [HttpGet]
    public IActionResult TableSection(int? id, int pageSize = 5, int pageIndex = 1, string searchInput = "")
    {
        var sections = _sectionService.GetAllSections();
        if (!sections.Any())
        {
            return NotFound("No Tables found.");
        }
        var validSectionId = id ?? sections.First().Id;

        var tables = _tableService.GetTablesBySectionId(validSectionId, pageSize, pageIndex, searchInput);
        tables.Sections = sections;
        return View(tables);
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanView)]
    public IActionResult GetAllTables()
    {
        var tables = _tableService.GetAllTables();
        return PartialView("_TableList", tables);
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanView)]
    [HttpGet]
    public async Task<IActionResult> GetTablesBySectionId(int sectionId, int pageSize, int pageIndex, string searchInput = "")
    {
        var sections = _sectionService.GetAllSections();

        TableSectionViewModel model = _tableService.GetTablesBySectionId(sectionId, pageSize == 0 ? 5 : pageSize, pageIndex == 0 ? 1 : pageIndex, searchInput);
        model.Sections = sections;
        return PartialView("_TableList", model);
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanView)]
    [HttpGet]
    public async Task<JsonResult> GetAllSections()
    {
        var sections = Json(_sectionService.GetAllSections());
        return sections;
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanView)]
    [HttpGet]
    public IActionResult GetAllSectionsForFilter()
    {
        var sections = _sectionService.GetAllSections();
        return PartialView("_SectionList", sections);
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanEdit)]
    [HttpGet]
    public IActionResult AddNewTable()
    {
        return PartialView("_AddEditTable", new TableViewModel());
    }
    [HttpPost]
    public async Task<IActionResult> AddNewTable(TableViewModel model)
    {
        if (ModelState.IsValid)
        {
            string message;
            var tableAdded = _tableService.AddTable(model, User, out message);

            if (tableAdded)
            {
                TempData["SuccessMessage"] = message;
                return Json(new { success = true, message });
            }
            else
            {
                TempData["ErrorMessage"] = message;
                return Json(new { success = false, message });
            }
        }
        else
        {
            var errorMessage = string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            return Json(new { success = false, message = errorMessage });
        }
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanDelete)]
    [HttpPost]
    public async Task<IActionResult>? DeleteTable(int id)
    {
        try
        {
            var isDeleted = _tableService.DeleteTable(id, User);
            if (!isDeleted)
                return Json(new { isSuccess = false, message = "Table is Occupied so you can not delete it." });
            return Json(new { isSuccess = true, message = "Table Deleted Successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error While Delete Table. Please Try again!" });
        }
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanDelete)]
    [HttpPost]
    public async Task<IActionResult>? MultiDeleteTable(int[] itemIds)
    {
        try
        {
            var isDeleted = _tableService.MultiDeleteTable(itemIds, User);
            if (!isDeleted)
                return Json(new { isSuccess = false, message = "You can not delete the occupied table." });
            return Json(new { isSuccess = true, message = "Tables Deleted Successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error While Delete Table. Please Try again!" });
        }
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanEdit)]
    [HttpGet]
    public async Task<IActionResult> EditTable(int id)
    {
        var table = await _tableService.GetTableById(id);
        if (table == null)
        {
            return NotFound();
        }

        var tableViewModel = new TableViewModel
        {
            Id = table.Id,
            Name = table.Name,
            SectionId = table.SectionId,
            Capacity = table.Capacity,
            IsAvailable = table.IsAvailable
        };

        return PartialView("_AddEditTable", tableViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditTable(TableViewModel model)
    {
        if (ModelState.IsValid)
        {
            string message;
            var tableUpdated = _tableService.UpdateTable(model, User, out message);

            if (tableUpdated)
            {
                TempData["SuccessMessage"] = message;
                return Json(new { success = true, message });
            }
            else
            {
                TempData["ErrorMessage"] = message;
                return Json(new { success = false, message });
            }
        }
        else
        {
            var errorMessage = string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            return Json(new { success = false, message = errorMessage });
        }
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanEdit)]
    [HttpGet]
    public async Task<IActionResult> AddEditSection(int? id)
    {
        if (id == null || id == 0)
            return PartialView("_AddEditSection", new SectionViewModel());

        var tableSection = await _sectionService.GetSectionByIdAsync(id.Value);
        return PartialView("_AddEditSection", tableSection);
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanEdit)]
    [HttpPost]
    public async Task<IActionResult> AddEditSection(SectionViewModel sectionViewModel)
    {
        if (!ModelState.IsValid)
            return PartialView("_AddEditSection", sectionViewModel);

        try
        {
            (bool success, string errorMessage) result;
            if (sectionViewModel.Id == 0)
            {
                result = await _sectionService.AddSectionAsync(sectionViewModel, User);
            }
            else
            {
                result = await _sectionService.UpdateSectionAsync(sectionViewModel, User);
            }

            if (result.success)
            {
                TempData["SuccessMessage"] = "Section saved successfully.";
                return Json(new { success = true, message = "Section saved successfully." });
            }
            else
            {
                TempData["ErrorMessage"] = result.errorMessage;
                return Json(new { success = false, message = result.errorMessage });
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return Json(new { success = false, message = ex.Message });
        }
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TablesAndSections_CanDelete)]
    [HttpPost]
    public async Task<IActionResult> DeleteSection(int id, bool softDelete = true)
    {

        var isDeleted = await _sectionService.DeleteSectionAsync(id, softDelete, User);
        if (isDeleted == "table is occupied")
        {
            TempData["ErrorMessage"] = "Table of this section is occupied you cannot delete it";
            return Json(new { isSuccess = false, message = "You can not delete this section because any table of this section is occupied" });
        }
        else if (isDeleted == "success")
        {
            return Json(new { isSuccess = true, message = "Section deleted successfully" });
        }
        else
        {
            return Json(new { isSuccess = false, message = "section not found" });
        }
    }

}