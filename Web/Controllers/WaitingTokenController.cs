using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class WaitingTokenController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
