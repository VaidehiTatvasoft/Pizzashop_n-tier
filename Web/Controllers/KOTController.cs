using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

public class KOTController : Controller
{
    private readonly IMenuService _menuService;

    public KOTController(IMenuService menuService)
    {
        _menuService = menuService;
    }
    [Authorize(Roles = "2,3")]
    public async Task<IActionResult> KOT()
    {
        var categories = await _menuService.GetAllMenuCategoriesAsync();
        return View(categories);
    }
}
