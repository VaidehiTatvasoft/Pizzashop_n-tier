using Entity.Shared;
using Entity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Web.Attributes;

namespace Web.Controllers;
[Route("tax")]
[Authorize]
public class TaxController : Controller
{
    private readonly ITaxAndFeeService _taxAndFeeService;

    public TaxController(ITaxAndFeeService taxAndFeeService)
    {
        _taxAndFeeService = taxAndFeeService;
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TaxesAndFees_CanView)]
    [Route("taxlist")]
    [HttpGet]
    public async Task<IActionResult> TaxList()
    {
        var taxes = await _taxAndFeeService.GetAllTaxes();
        return View(taxes);
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TaxesAndFees_CanEdit)]
    [Route("taxform")]
    [HttpGet]
    public async Task<IActionResult> TaxForm(int? id)
    {
        if (id.HasValue)
        {
            var tax = await _taxAndFeeService.GetTaxById(id.Value);
            return PartialView("_TaxFormPartial", tax);
        }
        return PartialView("_TaxFormPartial", new TaxandFeeViewModel());
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TaxesAndFees_CanEdit)]
    [Route("addtax")]
    [HttpPost]
    public async Task<IActionResult> AddTax(TaxandFeeViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _taxAndFeeService.AddTax(model, User);
            if (result)
            {
                return Json(new { success = true, message = "Tax created successfully.", taxId = model.Id });
            }
            else
            {
                TempData["ErrorMessage"] = "Tax with this name already exists.";
                ModelState.AddModelError("", "Tax with this name already exists.");
                return PartialView("_TaxFormPartial", model);
            }
        }
        return PartialView("_TaxFormPartial", model);
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TaxesAndFees_CanEdit)]
    [Route("edit")]
    [HttpPost]
    public async Task<IActionResult> Edit(TaxandFeeViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _taxAndFeeService.UpdateTax(model, User);
            if (result)
            {
                return Json(new { success = true, message = "Tax updated successfully." });
            }
            else
            {
                TempData["ErrorMessage"] = "Tax with this name already exists.";
                ModelState.AddModelError("", "Tax with this name already exists."); return PartialView("_TaxFormPartial", model);
            }
        }
        return PartialView("_TaxFormPartial", model);
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TaxesAndFees_CanDelete)]
    [Route("delete/{id}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _taxAndFeeService.DeleteTax(id, User);
        if (result)
        {
            TempData["SuccessMessage"] = "Tax deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to delete tax.";
        }
        return RedirectToAction(nameof(TaxList));
    }

    [Route("search")]
    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        var taxes = await _taxAndFeeService.GetAllTaxes();
        if (!string.IsNullOrEmpty(query))
        {
            query = query.ToLower();
            taxes = taxes.Where(t => t.Name.ToLower().Contains(query)).ToList();
        }
        return PartialView("_TaxTablePartial", taxes);
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.TaxesAndFees_CanEdit)]
    [Route("updateTaxStatus")]
    [HttpPost]
    public async Task<IActionResult> UpdateTaxStatus(int id, bool isActive, bool isDefault)
    {
        var result = await _taxAndFeeService.UpdateTaxStatus(id, isActive, isDefault, User);

        if (result)
        {
            return Json(new { success = true, message = "Status updated successfully." });
        }

        return Json(new { success = false, message = "Failed to update status." });
    }
}

