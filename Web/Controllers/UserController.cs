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

        [CustomAuthorize(RolePermissionEnum.Permission.Users_CanEdit)]
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

        [CustomAuthorize(RolePermissionEnum.Permission.Users_CanEdit)]
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
                    await LoadDropdowns(model);
                    return View(model);
                }

                var existingUserByUsername = await _userService.GetUserByUsername(model.Username);
                if (existingUserByUsername != null)
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
                var plainTextPassword = model.PasswordHash;
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
                Password: {plainTextPassword}
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
            await LoadDropdowns(model);
            return View(model);
        }
        [CustomAuthorize(RolePermissionEnum.Permission.Users_CanEdit)]
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
        [CustomAuthorize(RolePermissionEnum.Permission.Users_CanEdit)]
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

        [CustomAuthorize(RolePermissionEnum.Permission.Users_CanDelete)]
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

        [CustomAuthorize(RolePermissionEnum.Permission.Users_CanView)]
        [Route("/user/list")]
        public IActionResult UserList(string searchString, int pageIndex = 1, int pageSize = 5, string sortOrder = "", bool isAjax = false)
        {
            var userIdClaim = User.FindFirst("UserId");
            int userId = 0;

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
            {
                userId = int.Parse(userIdClaim.Value);
            }

            var users = _userService.GetUsersList(userId, searchString, sortOrder, pageIndex, pageSize, out int count);

            ViewData["UsernameSortParam"] = sortOrder == "username_asc" ? "username_desc" : "username_asc";
            ViewData["RoleSortParam"] = sortOrder == "role_asc" ? "role_desc" : "role_asc";

            ViewBag.count = count;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalPage = (int)Math.Ceiling(count / (double)pageSize);
            ViewBag.totalItems = count;
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
        [Authorize]
        [Route("/user/profile")]
        [HttpGet]
        public async Task<IActionResult> Profile(bool useOrderAppLayout = false)
        {
            var user = await _userService.GetUserProfileAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            if (user.RoleId != 2 && user.RoleId != 3)
            {
                useOrderAppLayout = false;
            }

            ViewBag.UseOrderAppLayout = useOrderAppLayout;
            await LoadDropdowns(user);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserViewModel model, IFormFile? ProfileImage, bool useOrderAppLayout)
        {
            if (ModelState.IsValid)
            {

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    if (!IsImageFile(ProfileImage))
                    {
                        TempData["ErrorMessage"] = "Only image files are allowed.";
                        ModelState.AddModelError("ProfileImage", "Only image files are allowed.");
                        ViewBag.UseOrderAppLayout = useOrderAppLayout;
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
                var existingUserByUsername = await _userService.GetUserByUsername(model.Username);
                if (existingUserByUsername != null && existingUserByUsername.Id != model.Id)
                {
                    TempData["ErrorMessage"] = "Username already exists.";
                    ModelState.AddModelError("", "Username already exists.");
                    ViewBag.UseOrderAppLayout = useOrderAppLayout;
                    await LoadDropdowns(model);
                    return View(model);
                }
                var result = await _userService.UpdateProfileAsync(model);
                if (result)
                {
                    // ViewBag.UseOrderAppLayout = useOrderAppLayout;
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    if (useOrderAppLayout)
                    {
                        return RedirectToAction("Index", "OrderAppTable");
                    }
                    else
                    {
                        return RedirectToAction("AdminDashboard", "Home");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Error updating profile.";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ErrorMessage"] = string.Join(", ", errors);
            }
            ViewBag.UseOrderAppLayout = useOrderAppLayout;
            await LoadDropdowns(model);
            return View(model);
        }
        private bool IsImageFile(IFormFile file)
        {
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".jfif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && permittedExtensions.Contains(extension);
        }
        [Authorize]
        [Route("/user/changepassword")]
        [HttpGet]
        public async Task<IActionResult> ChangePassword(bool useOrderAppLayout = false)
        {
            var user = await _userService.GetUserProfileAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            if (user.RoleId != 2 && user.RoleId != 3)
            {
                useOrderAppLayout = false;
            }
            ViewBag.UseOrderAppLayout = useOrderAppLayout;
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

        private async Task LoadDropdowns(AddUserModel model = null)
        {
            var countries = await _userService.GetAllCountriesAsync();
            var roles = await _userService.GetAllRolesAsync();

            ViewBag.Countries = new SelectList(countries, "Id", "Name", model?.CountryId);
            ViewBag.Roles = new SelectList(roles, "Id", "Name", model?.RoleId);
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