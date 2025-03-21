using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

public class TaxController : Controller
{
    private readonly ITaxAndFeeService _taxAndFeeService;
        public TaxController(ITaxAndFeeService taxAndFeeService)
        {
            _taxAndFeeService = taxAndFeeService;
        }
        [Route("/tax/taxlist")]
        [HttpGet]
        public async Task<IActionResult> TaxList()
        {
            var taxes = await _taxAndFeeService.GetAllTaxes();
            return View(taxes);
        }

        [Route("/tax/taxform")]
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
        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> AddTax(TaxandFeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tax = await _taxAndFeeService.AddTax(model);
                if (tax)
                {
                    return Json(new { success = true, message = "Tax created successfully." });
                }
                else
                {
                    Response.StatusCode = 400;
                    return PartialView("_TaxFormPartial", model);
                }
            }
            return PartialView("_TaxFormPartial", model);
        }
        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(TaxandFeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _taxAndFeeService.UpdateTax(model);
                if (result)
                {
                    return Json(new { success = true, message = "Tax updated successfully." });
                }
                else
                {
                    Response.StatusCode = 400;
                    return PartialView("_TaxFormPartial", model);
                }
            }
            Response.StatusCode = 400;
            return PartialView("_TaxFormPartial", model);
        }
        [Route("delete/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _taxAndFeeService.DeleteTax(id);
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

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, bool isActive, bool isDefault)
        {
            var tax = await _taxAndFeeService.GetTaxById(id);

            if (tax == null)
            {
                return Json(new { success = false, message = "Tax not found." });
            }

            tax.IsActive = isActive;
            tax.IsDefault = isDefault;

            var result = await _taxAndFeeService.UpdateTax(tax);

            if (result)
            {
                return Json(new { success = true, message = "Status updated successfully." });
            }

            return Json(new { success = false, message = "Failed to update status." });
        }
}
