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
                var userClaims = _tokenService.ValidateAuthToken(token);

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
                return View("Index");
            }
            var tokenString = _tokenService.GenerateAuthToken(user, TimeSpan.FromHours(24));
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
        [Route("/forgotpassword")]
        public IActionResult ForgotPassword(string? email)
        {
            var model = new ForgotPassword { Email = email ?? "" };
            return View(model);
        }
        [Route("/forgotpassword")]
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

            string resetLink = Url.Action("ResetPassword", "Accounts", new { token = _tokenService.GenerateResetPasswordToken(user, TimeSpan.FromHours(2)) }, Request.Scheme);
            string body = $@"<div style='font-family: sans-serif; width: 70%'>
                <div style='background-color: #0066A7; padding: 10px; display: flex; justify-content: center; align-items: center; gap: 2rem; margin:0;'>
                    
                    <h1 style='color: white;'>PIZZASHOP</h1>
                </div >
                <div style= 'background-color: #F2F2F2; margin:0;'>
                    <p style= 'margin:0; padding:10px;'>
                
                    Pizza Shop, <br><br>
                    Please click  <a href='{resetLink}'><b>here</b></a> for reset your account password.<br><br>If you encounter any issues or have any questions, please do not hesitate to contact our support team. <br><br><span style='color: #E5CAAB;'>Important Note:</span> For Security reasons, the link will expire in 24 hours. If you did not request a password reset, please ignore this email or contact our support team immediately.</p><br><br></div></div>";

            await _accountService.SendForgotPasswordEmail(user.Email, body);

            TempData["Message"] = "Password reset link has been sent to your email.";
            return RedirectToAction("Index");
        }
        [Route("/resetpassword")]
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

            ViewBag.Token = token;
            var model = new ResetPasswordModel { Email = email };
            return View(model);
        }
        [Route("/resetpassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model, string token)
        {
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

            if (!ModelState.IsValid)
            {
                return View(model);
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
