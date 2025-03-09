using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Service.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;


namespace Pizzashop.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ITokenService tokenService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Request.Cookies.TryGetValue("AuthToken", out string token))
            {
                var userClaims = _tokenService.ValidateToken(token);

                if (userClaims != null)
                {
                    var roleId = userClaims.FindFirst(ClaimTypes.Role)?.Value;

                    if (roleId == "1")
                    {
                        return RedirectToAction("AdminDashboard", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _accountService.AuthenticateUser(model.Email, model.PasswordHash);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid email or password.";
                return View(model);
            }
            var tokenString = _tokenService.GenerateToken(user, TimeSpan.FromHours(24));
            _accountService.SetCookies(HttpContext, tokenString, model.RememberMe);
            TempData["Message"] = "Login successful!";

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

            string resetLink = Url.Action("ResetPassword", "Accounts", new { token = _tokenService.GenerateResetPasswordToken(user, TimeSpan.FromHours(1)) }, Request.Scheme);
            string body = $"<div><b>PIZZASHOP</b></div><div><p>Please click <a href='{resetLink}'><b>here</b></a> to reset your account Password.<br>If you encounter any issues or have any question, please do not hesitate to contact our support team.<br><em>Important Note:</em> For security reasons, the link will expire in 1 hour. If you did not request a password reset, please ignore this email or contact our support team immediately.</p></div>";

            await _accountService.SendForgotPasswordEmail(user.Email, body);

            TempData["Message"] = "Password reset link has been sent to your email.";
            return RedirectToAction("Index");
        }
        public IActionResult ResetPassword(string token)
        {
            var userClaims = _tokenService.ValidateResetPasswordToken(token);
            if (userClaims == null)
            {
                TempData["ErrorMessage"] = "Invalid or expired token.";
                return RedirectToAction("Index");
            }

            var email = userClaims.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid token.";
                return RedirectToAction("Index");
            }

            var model = new ResetPasswordModel { Email = email };
            return View(model);
        }

[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model, string token)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userClaims = _tokenService.ValidateResetPasswordToken(token);
            if (userClaims == null)
            {
                TempData["ErrorMessage"] = "Invalid or expired token.";
                return RedirectToAction("Index");
            }

            var email = userClaims.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email) || email != model.Email)
            {
                TempData["ErrorMessage"] = "Invalid token.";
                return RedirectToAction("Index");
            }

            await _accountService.ResetPassword(model.Email, model.NewPassword);
            _tokenService.MarkTokenAsUsed(token);

            TempData["Message"] = "Password has been reset. You can now login.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Logout()
        {
            _accountService.Logout(HttpContext);
            TempData["Message"] = "You have been logged out.";
            return RedirectToAction("Index", "Accounts");
        }
    }
}
