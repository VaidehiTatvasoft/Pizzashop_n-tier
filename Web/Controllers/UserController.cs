using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using Entity.ViewModel;
using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Web.Attributes;
using Entity.Shared;

namespace pizzashop.Controllers
{
    public class UserController : Controller
    {
        private readonly PizzaShopContext _context;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IAccountService _accountService;

        public UserController(PizzaShopContext context, IUserService userService, IEmailService emailService, IAccountService accountService)
        {
            _context = context;
            _userService = userService;
            _emailService = emailService;
            _accountService = accountService;
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

        [CustomAuthorize(1, RolePermissionEnum.Permission.Users_CanEdit)]
        [Route("/adduser")]
        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            var countries = await _userService.GetAllCountriesAsync();
            var roles = await _userService.GetAllRolesAsync();
            ViewBag.Countries = countries.Select(country => new SelectListItem
            {
                Value = country.Id.ToString(),
                Text = country.Name
            });
            ViewBag.Roles = roles.Select(role => new SelectListItem
            {
                Value = role.Id.ToString(),
                Text = role.Name
            });

            return View();
        }

        [CustomAuthorize(1, RolePermissionEnum.Permission.Users_CanEdit)]
        [Route("/adduser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserAsync(AddUserModel model, IFormFile ProfileImage)
        {
            if (ModelState.IsValid)
            {
                var existingUserByEmail = await _userService.GetUserByEmail(model.Email);
                if (existingUserByEmail != null)
                {
                    TempData["ErrorMessage"] = "Email already exists.";
                    ModelState.AddModelError("", "Email already exists.");
                    await LoadDropdowns();
                    return View(model);
                }

                var existingUserByUsername = await _userService.GetUserByUsername(model.Username);
                if (existingUserByUsername != null)
                {
                    TempData["ErrorMessage"] = "Username already exists.";
                    ModelState.AddModelError("", "Username already exists.");
                    await LoadDropdowns();
                    return View(model);
                }

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    if (!IsImageFile(ProfileImage))
                    {
                        TempData["ErrorMessage"] = "Only image files are allowed.";
                        ModelState.AddModelError("ProfileImage", "Only image files are allowed.");
                        await LoadDropdowns();
                        return View(model);
                    }
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    model.ProfileImage = fileName;
                }
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
                var result = await _userService.AddUserAsync(model, User);
                if (result)
                {
                    string subject = "Account Login Details";
                    string body = $@"<div style='font-family: sans-serif;'>
            <div style='background-color: #0066A7; padding: 10px; display: flex; justify-content: center; align-items: center; gap: 2rem;'>
                <img src='http://localhost:5222/images/pizzashop_logo.png' alt='' style='width:60px; background-color:white; border-radius:50%;'>
                <h1 style='color: white;'>PIZZASHOP</h1>
            </div>
            <p>
                Welcome to Pizza Shop, <br><br>
                Please find the details below for login to your account. <br>
                <div style='border: 1px solid black; padding: 0.5rem; font-weight: bold;'>
                    <h3>Login Details:</h3>
                    Username: {model.Email} <br>
                    Password: {model.PasswordHash}
                </div><br>
                If you encounter any issues or have any questions, please do not hesitate to contact our support team. <br><br>
            </p>
        </div>";

                    await _emailService.SendEmailAsync(model.Email, subject, body);

                    TempData["SuccessMessage"] = "User added successfully.";
                    return RedirectToAction("UserList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error adding user.";
                }
            }
            await LoadDropdowns();
            return View(model);
        }
        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanEdit)]
        [CustomAuthorize(1, RolePermissionEnum.Permission.Users_CanEdit)]
        [Route("/user/edituser")]
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
        [Route("/user/edituser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserViewModel model, IFormFile? ProfileImage)
        {
            if (ModelState.IsValid)
            {
                var existingUserByEmail = await _userService.GetUserByEmail(model.Email);
                if (existingUserByEmail != null && existingUserByEmail.Id != model.Id)
                {
                    TempData["ErrorMessage"] = "Email already exists.";
                    ModelState.AddModelError("", "Email already exists.");
                    await LoadDropdowns(model);
                    return View(model);
                }

                var existingUserByUsername = await _userService.GetUserByUsername(model.Username);
                if (existingUserByUsername != null && existingUserByUsername.Id != model.Id)
                {
                    TempData["ErrorMessage"] = "Username already exists.";
                    ModelState.AddModelError("", "Username already exists.");
                    await LoadDropdowns(model);
                    return View(model);
                }

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    if (!IsImageFile(ProfileImage))
                    {
                        TempData["ErrorMessage"] = "Only image files are allowed.";
                        ModelState.AddModelError("ProfileImage", "Only image files are allowed.");
                        await LoadDropdowns(model);
                        return View(model);
                    }

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
            await LoadDropdowns(model);
            return View(model);
        }

        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanDelete)]
        [CustomAuthorize(1, RolePermissionEnum.Permission.Users_CanDelete)]
        [Route("/user/deleteuser")]
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

        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanView)]
        [CustomAuthorize(1, RolePermissionEnum.Permission.Users_CanView)]
        [Route("/user/list")]
        public IActionResult UserList(string searchString, int pageIndex = 1, int pageSize = 5, string sortOrder = "", bool isAjax = false)
        {
            var users = _userService.GetUsersList(searchString, sortOrder, pageIndex, pageSize, out int count);

            ViewData["UsernameSortParam"] = sortOrder == "username_asc" ? "username_desc" : "username_asc";
            ViewData["RoleSortParam"] = sortOrder == "role_asc" ? "role_desc" : "role_asc";

            ViewBag.count = count;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalPage = (int)Math.Ceiling(count / (double)pageSize);
            ViewBag.searchString = searchString;
            ViewBag.sortOrder = sortOrder;

            if (users == null || !users.Any())
            {
                ViewBag.ErrorMessage = "User list is empty.";
            }

            ViewBag.UserList = users;

            if (isAjax)
            {
                return PartialView("UserList", users);
            }

            return View(users);
        }

        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanView)]
        [CustomAuthorize(1, RolePermissionEnum.Permission.Users_CanView)]

        [Route("/user/profile")]
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

        // [CustomAuthorize(1, RolePermissionEnum.Permission.CanEdit)]
        [CustomAuthorize(1, RolePermissionEnum.Permission.Users_CanEdit)]

        [HttpPost]
        public async Task<IActionResult> Profile(UserViewModel model, IFormFile? ProfileImage)
        {
            if (ModelState.IsValid)
            {
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    if (!IsImageFile(ProfileImage))
                    {
                        TempData["ErrorMessage"] = "Only image files are allowed.";
                        ModelState.AddModelError("ProfileImage", "Only image files are allowed.");
                        await LoadDropdowns(model);
                        return View(model);
                    }

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
                var result = await _userService.UpdateProfileAsync(model);
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
        private bool IsImageFile(IFormFile file)
        {
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && permittedExtensions.Contains(extension);
        }
        [Route("/user/changepassword")]
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
                    _accountService.Logout(HttpContext);
                    return RedirectToAction("Login", "Accounts");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error changing password.Check if new password is same as current password";
                }
            }
            return View(model);
        }

        private async Task LoadDropdowns(UserViewModel model = null)
        {
            var countries = await _userService.GetAllCountriesAsync();
            var roles = await _userService.GetAllRolesAsync();

            ViewBag.Countries = new SelectList(countries, "Id", "Name");
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            if (model != null)
            {
                ViewBag.States = new SelectList(await _userService.GetStatesByCountryIdAsync(model.CountryId ?? 0), "Id", "Name", model.StateId ?? 0);
                ViewBag.Cities = new SelectList(await _userService.GetCitiesByStateIdAsync(model.StateId ?? 0), "Id", "Name", model.CityId ?? 0);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetStates(int countryId)
        {
            var states = await _userService.GetStatesByCountryIdAsync(countryId);
            return Json(states);
        }

        [HttpGet]
        public async Task<IActionResult> GetCities(int stateId)
        {
            var cities = await _userService.GetCitiesByStateIdAsync(stateId);
            return Json(cities);
        }
    }
}