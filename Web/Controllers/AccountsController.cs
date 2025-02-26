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

       public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: Login Page
        public IActionResult Index()
        {
            var email = Request.Cookies["UserEmail"];
            var password = Request.Cookies["UserPassword"];
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                "your_issuer",
                "your_audience",
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signIn
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Set cookies if rememberMe is true
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

            return Ok(new { Token = tokenString });
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
    }
}