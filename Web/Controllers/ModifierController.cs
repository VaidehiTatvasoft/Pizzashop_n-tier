using Entity.Shared;
using Entity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Linq;
using System.Threading.Tasks;
using Web.Attributes;

namespace Web.Controllers
{
    public class ModifierController : Controller
    {
        private readonly IModifierService _modifierService;

        public ModifierController(IModifierService modifierService)
        {
            _modifierService = modifierService;
        }
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanView)]
        public async Task<IActionResult> ModifiersList(int? modifierId)
        {
            var modifiers = await _modifierService.GetAllModifiers();

            if (!modifierId.HasValue && modifiers.Any())
            {
                modifierId = modifiers.First().Id;
            }

            ViewBag.SelectedCategory = modifierId.Value;
            ViewBag.ModifierItems = await _modifierService.GetItemsByModifiers(modifierId.Value);

            return View("~/Views/Menu/ModifiersList.cshtml", modifiers);
        }

        [HttpPost]
        public async Task<IActionResult> AddModifier(ModifierViewModel model)
        {
            if (ModelState.IsValid)
            {
                var modifier = await _modifierService.AddNewModifier(model.Name, model, User);

                if (modifier)
                {
                    TempData["SuccessMessage"] = "Modifier created successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action(nameof(ModifiersList)) });
                }
                else
                {
                    TempData["ErrorMessage"] = "A modifier with this name already exists.";
                }
            }

            var modifiers = await _modifierService.GetAllModifiers();
            ViewBag.ModifierItems = await _modifierService.GetItemsByModifiers(model.Id);
            return View("~/Views/Menu/ModifiersList.cshtml", modifiers);
        }
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanEdit)]
        [HttpGet]
        public async Task<IActionResult> EditModifier(int id)
        {
            var modifier = await _modifierService.GetModifierDetailById(id);
            if (modifier == null)
            {
                return NotFound();
            }

            return PartialView("_EditModifierPartial", modifier);
        }

        [HttpPost]
        public async Task<IActionResult> EditModifier(ModifierViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _modifierService.EditModifier(model, model.Id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Modifier updated successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action(nameof(ModifiersList)) });
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update modifier.";
                }
            }

            return Json(new { success = false, message = "Invalid data." });
        }

        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModifier(int id)
        {
            var result = await _modifierService.DeleteModifierById(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Modifier deleted successfully.";
                return Json(new { success = true, redirectUrl = Url.Action(nameof(ModifiersList)) });
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting modifier or modifier not found.";
                return Json(new { success = false });
            }
        }
    }
}