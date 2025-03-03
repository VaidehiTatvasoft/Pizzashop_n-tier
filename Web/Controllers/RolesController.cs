using Entity.Data;
using System.Collections.Generic;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Services.Interfaces;
using Web.Repositories.Interfaces;
using Web.Services.Interfaces;
using Service.Interface;

namespace Web.Controllers;

public class RolesController : Controller
{
    private readonly PizzaShopContext _context;
    private readonly IRoleService _roleService;

    private readonly IPermissionService _permissionService;


    public RolesController(PizzaShopContext context, IPermissionService permissionService, IRoleService roleService)
    {
        _context = context;
        _permissionService = permissionService;
        _roleService = roleService;
    }


    public async Task<IActionResult> Roles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return View(roles);
    }

    public async Task<IActionResult> Permissions(int id)
    {
        var rolePermissions = await _permissionService.GetPermissionsByRoleIdAsync(id);
        return View(rolePermissions);
    }

}
