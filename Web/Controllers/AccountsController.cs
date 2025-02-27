using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Service.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Pizzashop.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

       public IActionResult Index()
        {
            
            return View();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _accountService.AuthenticateUser(model.Email, model.PasswordHash);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            var tokenString = _accountService.GenerateToken(user);
            _accountService.SetCookies(HttpContext, tokenString, model.RememberMe);

            if (user.RoleId == 1)
            {
                return RedirectToAction("AdminDashboard", "Home");
            }
            else
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }

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