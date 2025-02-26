using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Pizzashop.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pizzashop.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;


        public AccountsController(IAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }
        // GET: Login Page
        public IActionResult Index()
        {
            var email = Request.Cookies["UserEmail"];
            var password = Request.Cookies["UserPassword"];
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            var user = await _accountService.AuthenticateUser(model.Email, model.PasswordHash);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "your_subject"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signIn
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            if (model.RememberMe)
            {
                Response.Cookies.Append("UserEmail", user.Email, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                });
                Response.Cookies.Append("UserPassword", model.PasswordHash, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                });
            }

            if (user.RoleId == 1)
            {
                return RedirectToAction("AdminDashboard", "Home");
            }
            else
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }
        [HttpGet]
        public IActionResult ForgotPassword(string? email)
        {
            var model = new ForgotPassword { Email = email ?? "" };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _accountService.GetUserByEmail(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email not found.");
                return View(model);
            }

            string resetLink = $"http://localhost:5274/Accounts/ResetPassword?email={user.Email}";

            await _accountService.SendForgotPasswordEmail(user.Email, resetLink);

            TempData["Message"] = "Password reset link has been sent to your email.";
            return RedirectToAction("Index");
        }

        // GET: Reset Password Page
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index");
            }

            var model = new ResetPasswordModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _accountService.ResetPassword(model.Email, model.NewPassword);

            TempData["Message"] = "Password has been reset. You can now login.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Logout()
        {
            _accountService.Logout(HttpContext);
            return RedirectToAction("Index", "Accounts");
        }
    }
}