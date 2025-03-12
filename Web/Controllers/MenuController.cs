using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using Entity.ViewModel;

namespace Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IItemService _itemService;
        private readonly IModifierService _modifierService;
        private readonly IModifierGroupService _modifierGroupService;

        public MenuController(
            ICategoryService categoryService, 
            IItemService itemService,
            IModifierService modifierService,
            IModifierGroupService modifierGroupService)
        {
            _categoryService = categoryService;
            _itemService = itemService;
            _modifierService = modifierService;
            _modifierGroupService = modifierGroupService;
        }
[Route ("/menu")]

        public async Task<IActionResult> Index()
        {
            return View();
        }
[Route ("/menu/items")]

        public async Task<IActionResult> Items(int? categoryId)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categoryId == null)
            {
                categoryId = categories.First().Id;
            }

            ViewBag.Items = await _itemService.GetItemsByCategoryAsync(categoryId.Value);
            return View(categories);
        }
[Route ("/menu/modifiers")]
        public async Task<IActionResult> Modifiers(int? groupId)
        {
            var modifierGroups = await _modifierGroupService.GetAllModifierGroupsAsync();
            if (groupId == null)
            {
                groupId = modifierGroups.First().Id;
            }

            ViewBag.Items = await _modifierService.GetModifiersByGroupAsync(groupId.Value);
            return View(modifierGroups);
        }
    }
}