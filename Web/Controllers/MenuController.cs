using Entity.Shared;
using Entity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Interface;
using Web.Attributes;


namespace Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IUnitService _unitService;
        private readonly IModifierService _modifierService;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMenuService menuService, IUnitService unitService, IModifierService modifierService, ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _unitService = unitService;
            _modifierService = modifierService;
            _logger = logger;
        }

        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanView)]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanView)]
        [HttpGet]
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

        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanEdit)]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanView)]
        [HttpGet]
        public IActionResult AddCategory()
        {
            return PartialView("_AddCategoryPartial", new MenuCategoryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(MenuCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AddCategoryPartial", model);
            }

            try
            {
                var category = await _menuService.AddNewCategory(model.Name, model, User);
                if (category)
                {
                    TempData["SuccessMessage"] = "Category created successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("MenuList") });
                }
                else
                {
                    TempData["ErrorMessage"] = "A category with this name already exists.";
                    return PartialView("_AddCategoryPartial", model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding category");
                TempData["ErrorMessage"] = "An error occurred while adding the category. Please try again.";
                return PartialView("_AddCategoryPartial", model);
            }
        }
        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanEdit)]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanEdit)]
        [HttpGet]
        public async Task<IActionResult> EditCategory(int Id)
        {
            var category = await _menuService.GetCategoryDetailById(Id);
            if (category == null)
            {
                return NotFound();
            }

            return PartialView("_EditCategoryPartial", category);
        }
        [HttpPost]
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
                    Console.WriteLine(ex.Message);
                    return Json(new { success = false, message = ex.Message });
                }
            }

            var errors = string.Join("; ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));

            return Json(new { success = false, message = "Invalid data.", errors = errors });
        }
        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanDelete)]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanDelete)]
        [HttpPost]
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

        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanEdit)]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanEdit)]
        [HttpGet]
        public async Task<IActionResult> AddItem()
        {
            try
            {
                ViewBag.Categories = new SelectList(await _menuService.GetAllCategories(), "Id", "Name");
                ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name");
                ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");
                return PartialView("_AddItemPartial", new MenuItemViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading add item form");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(MenuItemViewModel model, IFormFile? Image)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _menuService.GetAllCategories(), "Id", "Name");
                ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name");
                ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");
                return PartialView("_AddItemPartial", model);
            }

            try
            {
                if (Image != null && Image.Length > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var filePath = Path.Combine("wwwroot/uploads", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }
                    model.Image = fileName;
                }

                await _menuService.AddNewItem(model, User);
                return Json(new { success = true, redirectUrl = Url.Action("MenuList") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanEdit)]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanEdit)]
        [HttpGet]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(MenuItemViewModel menuItemViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _menuService.GetAllCategories(), "Id", "Name", menuItemViewModel.CategoryId);
                ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name", menuItemViewModel.UnitId);
                ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");
                return PartialView("_EditItemPartial", menuItemViewModel);
            }

            try
            {
                var result = await _menuService.EditItemAsync(menuItemViewModel, User);

                if (result)
                {
                    TempData["SuccessMessage"] = "Item updated successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("MenuList") });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update item.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing item");
                ModelState.AddModelError("", "Error editing item.");
            }

            ViewBag.Categories = new SelectList(await _menuService.GetAllCategories(), "Id", "Name", menuItemViewModel.CategoryId);
            ViewBag.Units = new SelectList(await _unitService.GetAllUnits(), "Id", "Name", menuItemViewModel.UnitId);
            ViewBag.ModifierGroups = new SelectList(await _modifierService.GetAllModifiers(), "Id", "Name");

            return PartialView("_EditItemPartial", menuItemViewModel);
        }

        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanDelete)]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var result = await _menuService.DeleteItemById(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Item deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting item or item not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item");
                TempData["ErrorMessage"] = "Error deleting item.";
            }

            return RedirectToAction("MenuList");
        }
        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanDelete)]
        [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultipleItems([FromBody] List<int> itemIds)
        {
            if (itemIds == null || !itemIds.Any())
            {
                return BadRequest("No items selected for deletion.");
            }

            try
            {
                foreach (var itemId in itemIds)
                {
                    await _menuService.DeleteItemById(itemId);
                }

                TempData["SuccessMessage"] = "Selected items deleted successfully.";
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple items");
                return StatusCode(500, new { success = false, message = "Error deleting items." });
            }
        }
        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanView)]
                [CustomAuthorize(1,RolePermissionEnum.Permission.Menu_CanView)]
        [HttpGet]
        public async Task<IActionResult> SelectedModifiers(int groupId)
        {
            try
            {
                var modifiers = await _modifierService.GetModifiersByGroupIdAsync(groupId);
                if (modifiers == null)
                {
                    return BadRequest("An error occurred while fetching modifiers. Please try again.");
                }
                return PartialView("_ModifierGroupPartial", modifiers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching modifiers");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}