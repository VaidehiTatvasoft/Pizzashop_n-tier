using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Services.Interfaces;
using pizzashop.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pizzashop.Controllers
{
    [Authorize]  
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        public IActionResult AddUser()
        {
            ViewBag.Countries = new SelectList(_userService.GetCountries(), "Id", "Name");
            ViewBag.Roles = new SelectList(_roleService.GetRoles(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserAsync(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.AddUserAsync(model, User);
                if (result)
                {
                    return RedirectToAction("UserList");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userService.GetUserViewModelByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await LoadDropdowns(user);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.EditUserAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "User updated successfully.";
                    return RedirectToAction("UserList");
                }
            }
            await LoadDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction("UserList");
            }
            TempData["ErrorMessage"] = "Error deleting user.";
            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> UserList(string search, int page = 1, int pageSize = 5, string sortColumn = "Name", string sortOrder = "asc")
        {
            var users = await _userService.GetUserListAsync(search, page, pageSize, sortColumn, sortOrder);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userService.GetUserProfileAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            await LoadDropdowns(user);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateProfileAsync(model, User);
                if (result)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction("Profile");
                }
            }
            await LoadDropdowns(model);
            return View(model);
        }

        private async Task LoadDropdowns(UserViewModel model)
        {
            ViewBag.Countries = new SelectList(await _userService.GetCountriesAsync(), "Id", "Name", model.CountryId);
            ViewBag.States = new SelectList(await _userService.GetStatesByCountryIdAsync(model.CountryId), "Id", "Name", model.StateId);
            ViewBag.Cities = new SelectList(await _userService.GetCitiesByStateIdAsync(model.StateId), "Id", "Name", model.CityId);
        }

        [HttpGet]
        public JsonResult GetStates(int countryId)
        {
            var states = _userService.GetStatesByCountryId(countryId);
            return Json(states);
        }

        [HttpGet]
        public JsonResult GetCities(int stateId)
        {
            var cities = _userService.GetCitiesByStateId(stateId);
            return Json(cities);
        }

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ChangePasswordAsync(model, User);
                if (result)
                {
                    return RedirectToAction("AdminDashboard", "Home");
                }
            }
            return View(model);
        }
    }
}