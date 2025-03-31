using System.Diagnostics;
using Entity.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;
using Web.Models;

namespace Web.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    [Authorize]
    public IActionResult Dashboard()
    {
        return View();
    }
    
    [Route("/home/admindashboard")]
    public IActionResult AdminDashboard()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult NotFound()
    {
        return View("404");
    }
    public IActionResult Unauthorized()
    {
        return View("401");
    }
    public IActionResult Forbidden()
        {
            return View("403");
        }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
