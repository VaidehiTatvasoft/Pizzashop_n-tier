using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

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
    [HttpGet]
    public IActionResult TableSection()
    {
        var sections = _sectionService.GetAllSections();
        var tables = _tableService.GetAllTables();

        var tableSection = new TableSectionViewModel
        {
            Tables = tables,
            Sections = sections
        };
        return View(tableSection);
    }



    [HttpGet]
    public async Task<IActionResult> AddEditSection(int? id)
    {
        if (id == null || id == 0)
            return PartialView("_AddEditSection", new SectionViewModel());

        var tableSection = await _sectionService.GetSectionByIdAsync(id.Value);
        return PartialView("_AddEditSection", tableSection);
    }

    [HttpPost]
    public async Task<IActionResult> AddEditSection(SectionViewModel sectionViewModel)
    {
        if (!ModelState.IsValid)
            return PartialView("_AddEditSection", sectionViewModel);

        try
        {
            if (sectionViewModel.Id == 0)
                await _sectionService.AddSectionAsync(sectionViewModel, User);
            else
                await _sectionService.UpdateSectionAsync(sectionViewModel, User);

            return Json(new { success = true, message = "Section saved successfully." });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSection(int id, bool softDelete = true)
    {
        await _sectionService.DeleteSectionAsync(id, softDelete, User);
        var tableSection = await _sectionService.GetSectionByIdAsync(id);
        return View("TableSection","TableSection");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmDeleteSection(int id)
    {
        var tableSection = await _sectionService.GetSectionByIdAsync(id);
        if (tableSection == null)
            return NotFound();

        return PartialView("_DeleteSection", tableSection);
    }
}