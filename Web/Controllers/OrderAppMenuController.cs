using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class OrderAppMenuController: Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
