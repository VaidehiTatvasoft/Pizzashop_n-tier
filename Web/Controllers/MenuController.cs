using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

using Entity.ViewModel;

namespace Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IUnitService _unitService;
        private readonly IModifierService _modifierService;

        public MenuController(IMenuService menuService, IUnitService unitService, IModifierService modifierService)
        {
            _menuService = menuService;
            _unitService = unitService;
            _modifierService = modifierService;
        }

        [HttpGet("menulist/{categoryId?}")]
        public async Task<IActionResult> MenuList(int? categoryId)
        {
            var categories = await _menuService.GetAllCategories();

            if (!categoryId.HasValue && categories.Any())
            {
                categoryId = categories.First().Id;
            }

            ViewBag.SelectedCategory = categoryId.Value;
            ViewBag.MenuItems = await _menuService.GetItemsByCategory(categoryId.Value);

            return View(categories);
        }

        [HttpGet("addcategory")]
        public IActionResult AddCategory()
        {
            return PartialView("_AddCategoryPartial", new MenuCategoryViewModel());
        }

        [HttpPost("addcategory")]
        public async Task<IActionResult> AddCategory(MenuCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var category = await _menuService.AddNewCategory(model.Name, model);

                    if (category)
                    {
                        TempData["SuccessMessage"] = "Category created successfully.";
                        return Json(new { success = true, redirectUrl = Url.Action("MenuList") });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            var errors = string.Join("; ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));

            return Json(new { success = false, message = "Invalid data.", errors = errors });
        }

        [HttpGet("editcategory/{id}")]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _menuService.GetCategoryDetailById(id);
            if (category == null)
            {
                return NotFound();
            }

            return PartialView("_EditCategoryPartial", category);
        }

        [HttpPost("editcategory")]
        public async Task<IActionResult> EditCategory(MenuCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var updated = await _menuService.EditCategory(model, model.Id);

                    if (updated)
                    {
                        TempData["SuccessMessage"] = "Category updated successfully.";
                        return Json(new { success = true, redirectUrl = Url.Action("MenuList") });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            var errors = string.Join("; ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));

            return Json(new { success = false, message = "Invalid data.", errors = errors });
        }

        [HttpPost("deletecategory/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _menuService.DeleteCategoryById(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Category deleted successfully.";
                return RedirectToAction("MenuList");
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting category or category not found.";
                return RedirectToAction("MenuList");
            }
        }

        [HttpGet("additem")]
        public async Task<IActionResult> AddItem()
        {
            ViewBag.Categories = new SelectList(await _menuService.GetAllCategories(), "Id", "Name");
            ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name");
            ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");
            return PartialView("_AddItemPartial");
        }

        [HttpPost("additem")]
        public async Task<IActionResult> AddItem(MenuItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                ViewBag.Categories = new SelectList(await _menuService.GetAllCategories(), "Id", "Name");
                ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name");
                ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");
                return PartialView("_AddItemPartial", model);
            }

            await _menuService.AddNewItem(model);
            return RedirectToAction("MenuList");
        }

        [HttpGet("edititem/{id}")]
        public async Task<IActionResult> EditItem(int id)
        {
            var item = await _menuService.GetItemDetailsById(id);

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(await _menuService.GetAllCategories(), "Id", "Name", item.CategoryId);
            ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name", item.UnitId);
            ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");

            return PartialView("_EditItemPartial", item);
        }

        [HttpGet("selectedmodifiers/{groupId}")]
        public async Task<IActionResult> SelectedModifiers(int groupId)
        {
            var modifiers = await _menuService.GetMofiersById(groupId);
            return PartialView("_ModifierGroupsPartial", modifiers);
        }

        [HttpPost("edititem/{id}")]
        public async Task<IActionResult> EditItem(MenuItemViewModel menuItemViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _menuService.EditItemAsync(menuItemViewModel);

                if (result)
                {
                    TempData["SuccessMessage"] = "Item updated successfully.";
                    return RedirectToAction("MenuList");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update item.");
                }
            }

            ViewBag.Categories = new SelectList(await _menuService.GetAllCategories(), "Id", "Name", menuItemViewModel.CategoryId);
            ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name", menuItemViewModel.UnitId);
            ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");

            return PartialView("_EditItemPartial", menuItemViewModel);
        }

        [HttpPost("deleteitem/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var result = await _menuService.DeleteItemById(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Item deleted successfully.";
                return RedirectToAction("MenuList");
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting item or item not found.";
                return RedirectToAction("MenuList");
            }
        }
    }
}