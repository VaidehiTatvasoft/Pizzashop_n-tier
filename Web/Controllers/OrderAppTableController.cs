using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class OrderAppTableController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
