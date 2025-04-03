using System.Security.Claims;
using System.Text.Json;
using Entity.Data;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Service.Interfaces;
using Service.Interface;

namespace Web.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    private readonly IMenuModifierService _menuModifierService;

    public MenuController(IMenuService menuService, IMenuModifierService menuModifierService)
    {
        _menuService = menuService;
        _menuModifierService = menuModifierService;
    }

    // Helper method to fetch userId from claims
    private int? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            Console.WriteLine("UserId claim is missing or invalid.");
            return null;
        }

        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            Console.WriteLine($"Invalid UserIdClaim Value: {userIdClaim.Value}");
            return null;
        }

        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> Menu(int pageSize = 5, int pageIndex = 1, string searchString = "")
    {
        var categories = await _menuService.GetAllMenuCategoriesAsync();
        if (!categories.Any())
        {
            return NotFound("No categories found.");
        }

        var validCategoryId = categories.First().Id;
        var itemTabDetails = await _menuService.GetItemTabDetails(validCategoryId, pageSize, pageIndex, searchString);

        var modifierGroups = await _menuModifierService.GetAllMenuModifierGroupAsync();
        var validModifierGroupId = modifierGroups.First().Id;
        var modifierTabDetails = await _menuModifierService.GetModifierTabDetails(validModifierGroupId, pageSize, pageIndex, searchString);

        var model = new MenuViewModel
        {
            ItemTab = itemTabDetails,
            ModifierTab = modifierTabDetails
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(MenuCategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var category = await _menuService.AddNewCategory(model.Name, model);

            if (category)
            {
                TempData["SuccessMessage"] = "Category created successfully.";
                return Json(new { success = true, message = "Category added successfully." });
            }
            else
            {
                TempData["ErrorMessage"] = "A category with this name already exists.";
                return Json(new { success = false, message = "A category with this name already exists." });
            }
        }

        return Json(new { success = false, message = "Invalid data." });
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await _menuService.GetCategoryDetailById(id);
        if (category == null)
        {
            return NotFound();
        }
        return PartialView("_EditCategory", category);
    }

    [HttpPost]
    public async Task<IActionResult> EditCategory(MenuCategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _menuService.EditCategory(model, model.Id);

            if (result)
            {
                return Json(new { success = true, message = "Category updated successfully.", redirectUrl = Url.Action("Menu") });
            }
            else
            {
                return Json(new { success = false, message = "A category with this name already exists.", redirectUrl = Url.Action("Menu") });
            }
        }

        return Json(new { success = false, message = "Invalid data." });
    }

    [HttpGet]
    public async Task<IActionResult> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string searchString = "")
    {
        List<MenuItemViewModel> items = await _menuService.GetItemsByCategory(categoryId, pageSize == 0 ? 5 : pageSize, pageIndex == 0 ? 1 : pageIndex, searchString);
        var totalItemCount = _menuService.GetItemsCountByCId(categoryId, searchString);

        var model = new ItemTabViewModel
        {
            itemList = items,
            PageSize = pageSize,
            PageIndex = pageIndex,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(totalItemCount / (double)pageSize),
            TotalItems = totalItemCount
        };
        return PartialView("_ItemListPartial", model);
    }

    [HttpPost]
    public IActionResult DeleteCategory(int id)
    {
        var result = _menuService.SoftDeleteCategory(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Category deleted successfully.";
            return Json(new { success = true, message = "Category deleted successfully." });
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to delete category.";
            return Json(new { success = false, message = "Failed to delete category." });
        }
    }

    [HttpGet]
    public JsonResult GetAllUnits()
    {
        var units = Json(_menuService.GetAllUnits());
        return units;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategory()
    {
        var categories = await _menuService.GetAllMenuCategoriesAsync();
        return PartialView("_CategoryList", categories.ToList());
    }

    [HttpGet]
    public async Task<IActionResult> AddItem()
    {
        List<MenuModifierGroupViewModel> ModifierGroups = await _menuModifierService.GetAllMenuModifierGroupAsync();
        List<Unit> units = _menuService.GetAllUnits();
        List<MenuCategoryViewModel> categories = await _menuService.GetAllMenuCategoriesAsync();
        var model = new MenuItemViewModel
        {
            Units = units,
            Categories = categories,
            ModifierGroups = ModifierGroups
        };
        return PartialView("_AddItem", model);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(MenuItemViewModel model, string ItemModifiers)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
        {
            return Json(new { isSuccess = false, message = "User ID claim is missing or invalid." });
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { isValid = false, message = "Validation failed", errors });
        }

        try
        {
            bool isItemExist = _menuService.IsItemExist(model.Name, model.CategoryId);
            if (isItemExist)
            {
                return Json(new { isExist = true, message = "This item already exists in the selected category." });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking item existence: {ex.Message}");
            return Json(new { isSuccess = false, message = "Error while checking item existence." });
        }

        List<ItemModifierViewModel> itemModifiers = new List<ItemModifierViewModel>();
        if (!string.IsNullOrEmpty(ItemModifiers))
        {
            try
            {
                itemModifiers = JsonSerializer.Deserialize<List<ItemModifierViewModel>>(ItemModifiers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing ItemModifiers: {ex.Message}");
                return Json(new { isSuccess = false, message = "Invalid modifiers format.", error = ex.Message });
            }
        }
        model.ItemModifiersList = itemModifiers;

        try
        {
            var response = await _menuService.AddNewItem(model, userId.Value);
            return Json(new { isSuccess = true, message = "Item added successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding item: {ex.Message}, StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return Json(new { isSuccess = false, message = "Error while adding new item", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditMenuItem(int itemId)
    {
        var menuItem = await _menuService.GetMenuItemById(itemId);
        return PartialView("_EditItem", menuItem);
    }

    [HttpPost]
    public async Task<IActionResult?> EditMenuItem([FromForm] MenuItemViewModel model, string ItemModifiers)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
        {
            return Json(new { isSuccess = false, message = "User ID claim is missing or invalid." });
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { isValid = false, message = "Validation failed", errors });
        }

        List<ItemModifierViewModel> itemModifiers = new List<ItemModifierViewModel>();
        if (!string.IsNullOrEmpty(ItemModifiers))
        {
            try
            {
                itemModifiers = JsonSerializer.Deserialize<List<ItemModifierViewModel>>(ItemModifiers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing ItemModifiers: {ex.Message}");
                return Json(new { isSuccess = false, message = "Invalid modifiers format.", error = ex.Message });
            }
        }
        model.ItemModifiersList = itemModifiers;

        try
        {
            _menuService.EditItem(model, userId.Value);
            return Json(new { isSuccess = true, message = "Item updated successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing item: {ex.Message}, StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return Json(new { isSuccess = false, message = "Error while editing item", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult>? DeleteMenuItem(int id)
    {
        try
        {
            _menuService.DeleteMenuItem(id);
            return Json(new { isSuccess = true, message = "Item deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting item: {ex.Message}");
            return Json(new { isSuccess = false, message = "Error while deleting item" });
        }
    }

    [HttpPost]
    public async Task<IActionResult>? MultiDeleteMenuItem(int[] itemIds)
    {
        try
        {
            _menuService.MultiDeleteMenuItem(itemIds);
            return Json(new { isSuccess = true, message = "Items deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting items: {ex.Message}");
            return Json(new { isSuccess = false, message = "Error while deleting items" });
        }
    }

    // Modifier Section

    [HttpGet]
    public async Task<IActionResult> AddModifier()
    {
        var modifierGroup = await _menuModifierService.GetAllMenuModifierGroupAsync();
        var units = _menuService.GetAllUnits();
        var modal = new AddEditModifierViewModel
        {
            Units = units,
            ModifierGroups = modifierGroup
        };
        return PartialView("_AddModifier", modal);
    }

    [HttpGet]
    public async Task<IActionResult> GetModifiersByModifierGroup(int modifierGroupId, int pageSize, int pageIndex, string searchString = "")
    {
        List<MenuModifierViewModel> filteredModifiers = await _menuModifierService.GetModifiersByModifierGroup(modifierGroupId, pageSize == 0 ? 5 : pageSize, pageIndex == 0 ? 1 : pageIndex, searchString);
        int totalModifierCount = _menuModifierService.GetModifiersCountByCId(modifierGroupId, searchString);
        var model = new ModifierTabViewModel
        {
            modifier = filteredModifiers,
            PageSize = pageSize,
            PageIndex = pageIndex,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(totalModifierCount / (double)pageSize),
            TotalItems = totalModifierCount
        };
        return PartialView("_Modifier", model);
    }

    [HttpGet]
    public async Task<JsonResult> GetAllModifierGroups()
    {
        var modifiers = await _menuModifierService.GetAllMenuModifierGroupAsync();
        return Json(modifiers);
    }

    [HttpGet]
    public async Task<IActionResult> GetModifiersByGroup(int id, string name)
    {
        List<MenuModifierViewModel> Modifiers = _menuModifierService.GetModifiersByGroupId(id);
        var itemModifiers = new ItemModifierViewModel
        {
            ModifierList = Modifiers,
            ModifierGroupId = id,
            Name = name,
        };
        return PartialView("_ItemModifiers", itemModifiers);
    }

    [HttpPost]
    public async Task<IActionResult> AddModifier(AddEditModifierViewModel model)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
        {
            return Json(new { isSuccess = false, message = "User ID claim is missing or invalid." });
        }

        try
        {
            bool isAdded = _menuModifierService.AddModifier(model, userId.Value);

            if (isAdded)
            {
                return Json(new { isSuccess = true, message = "New modifier added successfully" });
            }
            else
            {
                return Json(new { isSuccess = false, message = "Modifier already exists" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding modifier: {ex.Message}");
            return Json(new { isSuccess = false, message = "Error while adding new modifier" });
        }
    }

    [HttpGet]
    public IActionResult EditModifier(int id)
    {
        var editModifier = _menuModifierService.GetModifierByid(id);
        return PartialView("_EditModifier", editModifier);
    }

    [HttpPost]
    public async Task<IActionResult> EditModifier(AddEditModifierViewModel model)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
        {
            return Json(new { isSuccess = false, message = "User ID claim is missing or invalid." });
        }

        try
        {
            bool isEdited = _menuModifierService.EditModifier(model, userId.Value);
            if (isEdited)
            {
                return Json(new { isSuccess = true, message = "Modifier updated successfully" });
            }
            else
            {
                return Json(new { isSuccess = false, message = "Modifier already exists" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing modifier: {ex.Message}");
            return Json(new { isSuccess = false, message = "Error while editing modifier" });
        }
    }

    [HttpPost]
    public IActionResult DeleteModifier(int id)
    {
        try
        {
            _menuModifierService.DeleteModifier(id);
            return Json(new { isSuccess = true, message = "Modifier deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting modifier: {ex.Message}");
            return Json(new { isSuccess = false, message = "Error while deleting modifier" });
        }
    }

    [HttpPost]
    public IActionResult DeleteMultipleModifier(int[] modifierIds)
    {
        try
        {
            _menuModifierService.DeleteMultipleModifiers(modifierIds);
            return Json(new { isSuccess = true, message = "Modifiers deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting modifiers: {ex.Message}");
            return Json(new { isSuccess = false, message = "Error while deleting modifiers" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllModifiers(int pageSize, int pageIndex, string? searchString)
    {
        var modifiers = await _menuModifierService.GetAllModifiers(pageSize, pageIndex, searchString);
        return PartialView("_AddExistingModifier", modifiers);
    }
[HttpPost]
public async Task<IActionResult> AddModifierGroup([FromBody] MenuModifierGroupViewModel model)
{
    if (string.IsNullOrEmpty(model.Name))
    {
        return Json(new { isSuccess = false, message = "Modifier group name is required." });
    }

    try
    {
        // Add the modifier group
        var modifierGroupId = await _menuModifierService.AddModifierGroupAsync(model);

        // Associate selected modifiers with the new group
        if (model.SelectedModifierIds != null && model.SelectedModifierIds.Any())
        {
            await _menuModifierService.AddModifiersToGroupAsync(modifierGroupId, model.SelectedModifierIds);
        }

        return Json(new { isSuccess = true, message = "Modifier group added successfully.", groupId = modifierGroupId });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error adding modifier group: {ex.Message}");
        return Json(new { isSuccess = false, message = "An error occurred while adding the modifier group." });
    }
}
}