using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Service.Interface;
using System.Threading.Tasks;
using Entity.Data;

namespace Web.Controllers
{
    public class ModifierGroupController : Controller
    {
        private readonly IModifierGroupService _modifierGroupService;

        public ModifierGroupController(IModifierGroupService modifierGroupService)
        {
            _modifierGroupService = modifierGroupService;
        }

        public async Task<IActionResult> Index()
        {
            var modifierGroups = await _modifierGroupService.GetAllModifierGroupsAsync();
            return View(modifierGroups);
        }

        [HttpPost]
        public async Task<IActionResult> AddModifierGroup(ModifierGroup modifierGroup)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                return Json(new { success = false, message = "User is not logged in or UserId is missing." });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(new { success = false, message = "Invalid UserId format." });
            }

            modifierGroup.CreatedBy = userId;

            if (ModelState.IsValid)
            {
                await _modifierGroupService.AddModifierGroupAsync(modifierGroup);
                return Json(new { success = true });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "Invalid data", errors });
        }

        public async Task<IActionResult> EditModifierGroup(int id)
        {
            var modifierGroup = await _modifierGroupService.GetModifierGroupByIdAsync(id);
            if (modifierGroup == null)
            {
                return NotFound();
            }
            return View(modifierGroup);
        }

        [HttpPost]
        public async Task<IActionResult> EditModifierGroup(ModifierGroup modifierGroup)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                return Json(new { success = false, message = "User is not logged in or UserId is missing." });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(new { success = false, message = "Invalid UserId format." });
            }

            modifierGroup.ModifiedBy = userId;
            if (ModelState.IsValid)
            {
                await _modifierGroupService.UpdateModifierGroupAsync(modifierGroup);
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModifierGroup(int id)
        {
            await _modifierGroupService.SoftDeleteModifierGroupAsync(id);
            return RedirectToAction("Modifiers", "Menu");
        }
    }
}