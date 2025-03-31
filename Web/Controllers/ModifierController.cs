using Entity.Shared;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Service.Interface;
using System.Linq;
using System.Threading.Tasks;
using Web.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Controllers
{
    public class ModifierController : Controller
    {
        private readonly IModifierService _modifierService;
        private readonly IUnitService _unitService;

        public ModifierController(IModifierService modifierService, IUnitService unitService)
        {
            _modifierService = modifierService;
            _unitService = unitService;
        }

        // [HttpGet("modifierslist")]
        // [CustomAuthorize(1, RolePermissionEnum.Permission.Menu_CanView)]
        // public async Task<IActionResult> ModifiersList(int? modifierId, int offset = 0, int pageSize = 5, string searchString = null)
        // {
        //     var modifiers = await _modifierService.GetAllModifiers();

        //     if (!modifierId.HasValue && modifiers.Any())
        //     {
        //         modifierId = modifiers.First().Id;
        //     }

        //     ViewBag.SelectedCategory = modifierId.Value;
        //     ViewBag.ModifierItems = await _modifierService.GetItemsByModifiers(modifierId.Value);

            // var result = await _modifierService.GetPaginatedModifierItems(modifierId, offset, pageSize, searchString);
            // var paginationViewModel = new PaginationViewModel
            // {
            //     CurrentOffset = offset,
            //     PageSize = pageSize,
            //     TotalCount = result.TotalCount,
            //     PageSizeList = new List<int> { 5, 10, 15 }
            // };

            // var viewModel = new ModifierItemListViewModel
            // {
            //     ModifierItems = result.Items,
            //     Pagination = paginationViewModel
            // };

            // if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            // {
            //     return PartialView("~/Views/Menu/_ModifierItemsPartial.cshtml", viewModel.ModifierItems);
            // }

            // ViewBag.ModifierItems = viewModel.ModifierItems;
            // ViewBag.Pagination = viewModel.Pagination;
            // return View("~/Views/Menu/ModifiersList.cshtml", modifiers);
        // }

        // [HttpGet("modifierspagination/{modifierId?}")]
        // [CustomAuthorize(1, RolePermissionEnum.Permission.Menu_CanView)]
        // public async Task<IActionResult> ModifiersPagination(int? modifierId, int offset = 0, int pageSize = 5, string searchString = null)
        // {
        //     var result = await _modifierService.GetPaginatedModifierItems(modifierId, offset, pageSize, searchString);
        //     var paginationViewModel = new PaginationViewModel
        //     {
        //         CurrentOffset = offset,
        //         PageSize = pageSize,
        //         TotalCount = result.TotalCount,
        //         PageSizeList = new List<int> { 5, 10, 15 }
        //     };

        //     return PartialView("_PaginationPartial", paginationViewModel);
        // }

        [HttpPost("addmodifier")]
        [CustomAuthorize(1, RolePermissionEnum.Permission.Menu_CanEdit)]
        public async Task<IActionResult> AddModifier(ModifierViewModel model, List<int> existingModifierIds)
        {
            if (ModelState.IsValid)
            {
                var category = await _modifierService.AddNewModifier(model.Name, model, User);

                if (category != null)
                {
                    // if (existingModifierIds != null && existingModifierIds.Any())
                    // {
                    //     var result = await _modifierService.AddModifiersToGroup(existingModifierIds, category.Id);
                    //     if (!result)
                    //     {
                    //         return Json(new { success = false, message = "Failed to add existing modifiers to the group." });
                    //     }
                    // }

                    return Json(new { success = true, message = "Modifier created successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "A modifier with this name already exists." });
                }
            }

            return Json(new { success = false, message = "Invalid data." });
        }

        [HttpPost("addmodifierstogroup")]
        [CustomAuthorize(1, RolePermissionEnum.Permission.Menu_CanEdit)]
        public async Task<IActionResult> AddModifiersToGroup([FromBody] AddModifiersToGroupRequest request)
        {
            if (request.ModifierIds == null || !request.ModifierIds.Any())
            {
                return Json(new { success = false, message = "No modifiers selected." });
            }

            var result = await _modifierService.AddModifiersToGroup(request.ModifierIds, request.GroupId);

            if (result)
            {
                return Json(new { success = true, message = "Modifiers added successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Failed to add modifiers." });
            }
        }

        [HttpGet("editmodifier/{id}")]
        [CustomAuthorize(1, RolePermissionEnum.Permission.Menu_CanEdit)]
        public async Task<IActionResult> EditModifier(int id)
        {
            var modifier = await _modifierService.GetModifierDetailById(id);
            if (modifier == null)
            {
                return NotFound();
            }

            return PartialView("~/Views/Menu/_EditModifierPartial.cshtml", modifier);
        }

        [HttpPost("editmodifier")]
        public async Task<IActionResult> EditModifier(ModifierViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _modifierService.EditModifier(model, model.Id);

                if (result)
                {
                    return Json(new { success = true, message = "Modifier updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update modifier." });
                }
            }

            return Json(new { success = false, message = "Invalid data." });
        }

        public IActionResult AddModifierGroup()
        {
            return PartialView("~/Views/Menu/_AddModifierPartial.cshtml");
        }

        [HttpPost("deletemodifier/{id}")]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanDelete)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModifier(int id)
        {
            var result = await _modifierService.DeleteModifierById(id);

            if (result)
            {
                return Json(new { success = true, message = "Modifier deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Error deleting modifier or modifier not found." });
            }
        }

        [HttpGet("getallmodifiergroups")]
        public async Task<IActionResult> GetAllModifierGroups()
        {
            var modifiers = await _modifierService.GetAllModifiers();
            return PartialView("~/Views/Menu/_ModifierGroupsListPartial.cshtml", modifiers);
        }

        [HttpGet("getallmodifiers")]
        public async Task<IActionResult> GetAllModifiers(string searchString, int offset = 0, int pageSize = 5)
        {
            var modifiers = await _modifierService.GetAllModifiers(searchString);

            var paginatedModifiers = modifiers
                .Skip(offset)
                .Take(pageSize)
                .ToList();

            ViewBag.Pagination = new PaginationViewModel
            {
                CurrentOffset = offset,
                PageSize = pageSize,
                TotalCount = modifiers.Count(),
                PageSizeList = new List<int> { 5, 10, 15 }
            };

            return PartialView("~/Views/Menu/_ModifierList.cshtml", paginatedModifiers);
        }

        [HttpGet("addmodifieritem")]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanEdit)]
        public async Task<IActionResult> AddModifierItem()
        {
            ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name");
            ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");
            return PartialView("~/Views/Menu/_AddModifierItemPartial.cshtml");
        }

        [HttpPost("addmodifieritem")]
        public async Task<IActionResult> AddModifierItem(MenuModifierViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name");
                ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");
                return PartialView("~/Views/Menu/_AddModifierItemPartial.cshtml", model);
            }

            var item = await _modifierService.AddNewModifierItem(model);

            if (item)
            {
                return Json(new { success = true, message = "Modifier item added successfully." });
            }
            else
            {
                return Json(new { success = false, message = "A modifier item with this name already exists." });
            }
        }

        // [HttpGet("/modifier/editmodifieritem/{id}")]
        // [CustomAuthorize(1, RolePermissionEnum.Permission.Menu_CanEdit)]
        // public async Task<IActionResult> EditModifierItem(int id)
        // {
        //     var item = await _modifierService.GetModifierItemById(id);
        //     if (item == null)
        //     {
        //         return NotFound();
        //     }

        //     ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name");
        //     ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");
        //     return PartialView("~/Views/Menu/_EditModifierItemPartial.cshtml", item);
        // }

        // [HttpPost("/modifier/editmodifieritem")]
        // public async Task<IActionResult> EditModifierItem(MenuModifierViewModel model)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return PartialView("~/Views/Menu/_EditModifierItemPartial.cshtml", model);
        //     }

        //     var result = await _modifierService.EditModifierItem(model);

        //     if (result)
        //     {
        //         TempData["SuccessMessage"] = "Modifier item updated successfully.";
        //         return Json(new { success = true, redirectUrl = Url.Action(nameof(ModifiersList)) });
        //     }
        //     else
        //     {
        //         TempData["ErrorMessage"] = "Failed to update modifier item.";
        //         return Json(new { success = false, message = "Failed to update modifier item." });
        //     }
        // }
    }

    public class AddModifiersToGroupRequest
    {
        public List<int> ModifierIds { get; set; }
        public int GroupId { get; set; }
    }
}