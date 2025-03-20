using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

public class TableSectionController : Controller
{
    private readonly ISectionService _sectionService;

    public TableSectionController(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }

    public async Task<IActionResult> Section()
    {
        var sections = await _sectionService.GetAllSectionsAsync();
        return View("Section", sections.OrderBy(s => s.Name)); 
    }

    [HttpGet]
    public async Task<IActionResult> AddEditSection(int? id)
    {
        if (id == null || id == 0)
            return PartialView("_AddEditSection", new SectionViewModel());

        var section = await _sectionService.GetSectionByIdAsync(id.Value);
        return PartialView("_AddEditSection", section);
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
            TempData["ErrorMessage"] = "Name already Exist";
            ModelState.AddModelError(string.Empty, ex.Message);
            return PartialView("_AddEditSection", sectionViewModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSection(int id, bool softDelete = true)
    {
        await _sectionService.DeleteSectionAsync(id, softDelete, User);
        var sections = await _sectionService.GetAllSectionsAsync();
        return View("Section", sections.OrderBy(s => s.Name)); 
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmDeleteSection(int id)
    {
        var section = await _sectionService.GetSectionByIdAsync(id);
        if (section == null)
            return NotFound();

        return PartialView("_DeleteSection", section);
    }
}
