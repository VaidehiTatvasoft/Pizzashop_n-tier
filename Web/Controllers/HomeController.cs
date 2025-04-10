﻿using System.Diagnostics;
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
    [Authorize(Roles = "1,2")]
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
    [Route("/unauthorized")]
    public IActionResult Unauthorized()
    {
        return RedirectToAction("Login", "Accounts");
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
