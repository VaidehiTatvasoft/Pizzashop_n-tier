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

        public MenuController(ICategoryService categoryService, IItemService itemService, IModifierService modifierService)
        {
            _categoryService = categoryService;
            _itemService = itemService;
            _modifierService = modifierService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var items = await _itemService.GetAllItemsAsync();
            var modifiers = await _modifierService.GetAllModifiersAsync();
            var model = new MenuViewModel
            {
                Categories = categories,
                Items = items,
                Modifiers = modifiers
            };
            return View(model);
        }
    }
}