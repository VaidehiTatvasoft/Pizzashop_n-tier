using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;
[Authorize(Roles = "2")]

public class WaitingTokenController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
