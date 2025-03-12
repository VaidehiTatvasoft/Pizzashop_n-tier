using System.Security.Claims;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Services.Interfaces;
using Service.Interface;

namespace Web.Controllers;

public class RolePermissionController : Controller
{
    private readonly IUserService _user;
    private readonly IRolePermissionService _rolePermission;

    public RolePermissionController(IUserService user, IRolePermissionService rolePermission)
    {
        _user = user;
        _rolePermission = rolePermission;
    }
    [Route("/roles")]

    [HttpGet]
    public async Task<IActionResult> Role()
    {
        var roles = await _rolePermission.GetAllRoles();
        return View(roles);
    }
    [Route("/permission")]

    [HttpGet]
    public IActionResult Permission(int id)
    {
        var AuthToken = Request.Cookies["AuthToken"];
        if (String.IsNullOrEmpty(AuthToken))
        {
            return RedirectToAction("Index", "Accounts");
        }
        var model = _rolePermission.GetRolePermissionByRoleId(id);
        if (model != null)
            return View(model);
        return RedirectToAction("Permission");
    }
    [Route("/permission")]

    [HttpPost]
    public async Task<IActionResult> Permission(List<RolePermissionViewModel> model)
    {
        if (ModelState.IsValid)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var AuthToken = Request.Cookies["AuthToken"];

            if (String.IsNullOrEmpty(AuthToken))
            {
                return RedirectToAction("Index", "Accounts");
            }

            bool updateRolePermission = await _rolePermission.UpdateRolePermission(model, email);
            if (updateRolePermission)
            {
                TempData["SuccessUpdate"] = "Role And Permissions Updated Successfully";
                return View(model);
            }
            TempData["ErrorUpdate"] = "Something went wrong Please Try Again";
            return View(model);
        }
        else
        {
            return View(model);
        }
    }
}
