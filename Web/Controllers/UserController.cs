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
        [Authorize(Roles = "1")]
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
        [Authorize(Roles = "1")]
        [Route("/adduser")]
        public async Task<IActionResult> AddUser()
        {
            var model = new AddUserModel
            {
                Countries = await _userService.GetAllCountriesAsync(),
                Roles = await _userService.GetAllRolesAsync()
            };
            return View(model);
        }

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
                    model.Countries = await _userService.GetAllCountriesAsync();
                    model.Roles = await _userService.GetAllRolesAsync();
                    return View(model);
                }

                var existingUserByUsername = await _userService.GetUserByUsername(model.Username);
                if (existingUserByUsername != null)
                {
                    TempData["ErrorMessage"] = "Username already exists.";
                    ModelState.AddModelError("", "Username already exists.");
                    model.Countries = await _userService.GetAllCountriesAsync();
                    model.Roles = await _userService.GetAllRolesAsync();
                    return View(model);
                }

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
            model.Countries = await _userService.GetAllCountriesAsync();
            model.Roles = await _userService.GetAllRolesAsync();
            return View(model);
        }

        [Authorize(Roles = "1")]
        [Route("/user/edituser")]
        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userService.GetUserViewModelByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Roles = await _userService.GetAllRolesAsync();
            user.Countries = await _userService.GetAllCountriesAsync();
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

                    return View(model);
                }

                var existingUserByUsername = await _userService.GetUserByUsername(model.Username);
                if (existingUserByUsername != null && existingUserByUsername.Id != model.Id)
                {
                    TempData["ErrorMessage"] = "Username already exists.";
                    ModelState.AddModelError("", "Username already exists.");
                    model.Roles = await _userService.GetAllRolesAsync();
                    model.Countries = await _userService.GetAllCountriesAsync();
                    return View(model);
                }

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
            model.Roles = await _userService.GetAllRolesAsync();
            model.Countries = await _userService.GetAllCountriesAsync();
            return View(model);
        }
        [Authorize(Roles = "1")]
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
        [Authorize(Roles = "1")]
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

        [Authorize(Roles = "1")]
        [Route("/user/profile")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userService.GetUserProfileAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            user.Countries = await _userService.GetAllCountriesAsync();
            return View(user);
        }
        [Route("/user/profile")]
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
                else
                {
                    var existingUser = await _userService.GetUserViewModelByIdAsync(model.Id);
                    if (existingUser != null)
                    {
                        model.ProfileImage = existingUser.ProfileImage;
                    }
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
            model.Countries = await _userService.GetAllCountriesAsync();
            return View(model);
        }

        [Authorize(Roles = "1")]
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
    }
}
