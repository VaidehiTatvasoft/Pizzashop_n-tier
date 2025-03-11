using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using Entity.ViewModel;
using Entity.Data;
using Microsoft.EntityFrameworkCore;

namespace pizzashop.Controllers
{
    public class UserController : Controller
    {
        private readonly PizzaShopContext _context;
        private readonly IUserService _userService;

        public UserController(PizzaShopContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProfileImage()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return NotFound("User not found");
            }

            var profileImagePath = await _userService.GetUserProfileImageAsync(email);
            return Json(new { profileImgPath = profileImagePath ?? "Default_pfp.svg.png" });
        }
        public IActionResult AddUser()
        {
            ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name");
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserAsync(AddUserModel model, IFormFile ProfileImage)
        {
            if (ModelState.IsValid)
            {
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    model.ProfileImage = fileName;
                }

                var result = await _userService.AddUserAsync(model, User);
                if (result)
                {
                    TempData["SuccessMessage"] = "User added successfully.";
                    return RedirectToAction("UserList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error adding user.";
                }
            }
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name");
            ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name");
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
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            await LoadDropdowns(user);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserViewModel model, IFormFile? ProfileImage)
        {
            if (ModelState.IsValid)
            {
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    model.ProfileImage = fileName;
                }
                else
                {
                    var existingUser = await _userService.GetUserViewModelByIdAsync(model.Id);
                    if (existingUser != null)
                    {
                        model.ProfileImage = existingUser.ProfileImage;
                    }
                }

                var result = await _userService.EditUserAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "User updated successfully.";
                    return RedirectToAction("UserList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error updating user.";
                }
            }
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", model.RoleId);
            ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name", model.CountryId);
            ViewBag.States = new SelectList(_context.States.Where(s => s.CountryId == model.CountryId).ToList(), "Id", "Name", model.StateId);
            ViewBag.Cities = new SelectList(_context.Cities.Where(c => c.StateId == model.StateId).ToList(), "Id", "Name", model.CityId);
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

public IActionResult UserList(string searchString, int pageIndex = 1, int pageSize = 5, string sortOrder = "")
{
    var users = _userService.GetUsersList(searchString, sortOrder, pageIndex, pageSize, out int count);

    ViewData["UsernameSortParam"] = sortOrder == "username_asc" ? "username_desc" : "username_asc";
    ViewData["RoleSortParam"] = sortOrder == "role_asc" ? "role_desc" : "role_asc";

    ViewBag.count = count;
    ViewBag.pageIndex = pageIndex;
    ViewBag.pageSize = pageSize;
    ViewBag.totalPage = (int)Math.Ceiling(count / (double)pageSize);
    ViewBag.searchString = searchString;

    if (users == null || !users.Any())
    {
        ViewBag.ErrorMessage = "User list is empty.";
        return View();
    }

    ViewBag.UserList = users;
    return View();
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
        public async Task<IActionResult> Profile(UserViewModel model, IFormFile? ProfileImage)
        {
            if (ModelState.IsValid)
            {
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }
                    model.ProfileImage = fileName;
                }

                var result = await _userService.UpdateProfileAsync(model, User);
                if (result)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction("AdminDashboard", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error updating profile.";
                }
            }
            await LoadDropdowns(model);
            return View(model);
        }

        private async Task LoadDropdowns(UserViewModel model)
        {
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "Id", "Name", model.CountryId);
            ViewBag.States = new SelectList(await _context.States.Where(s => s.CountryId == model.CountryId).ToListAsync(), "Id", "Name", model.StateId);
            ViewBag.Cities = new SelectList(await _context.Cities.Where(c => c.StateId == model.StateId).ToListAsync(), "Id", "Name", model.CityId);
        }

        [HttpGet]
        public JsonResult GetStates(int countryId)
        {
            var states = _context.States
                .Where(s => s.CountryId == countryId)
                .Select(s => new
                {
                    id = s.Id,
                    name = s.Name
                }).ToList();
            return Json(states);
        }

        [HttpGet]
        public JsonResult GetCities(int stateId)
        {
            var cities = _context.Cities
                .Where(c => c.StateId == stateId)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name
                }).ToList();
            return Json(cities);
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ChangePasswordAsync(model, User);
                if (result)
                {
                    TempData["SuccessMessage"] = "Password changed successfully.";
                    return RedirectToAction("AdminDashboard", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error changing password.";
                }
            }
            return View(model);
        }
    }
}
