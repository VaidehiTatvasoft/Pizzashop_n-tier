using System.Threading.Tasks;
using Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Entity.Data;
using Microsoft.AspNetCore.Http;

namespace Pizzashop.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AccountService(IEmailService emailService, IUserRepository userRepository, ITokenService tokenService)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var user = await _userRepository.AuthenticateUser(email, password);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        public async Task SendForgotPasswordEmail(string email, string resetLink)
        {
            await _emailService.SendEmailAsync(email, "Password Reset",  resetLink);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public async Task ResetPassword(string email, string newPassword)
        {
            await _userRepository.ResetPassword(email, newPassword);
        }

        public void Logout(HttpContext context)
        {
            context.Session.Clear();
            context.Response.Cookies.Delete(".AspNetCore.Identity.Application");
            context.Response.Cookies.Delete("AuthToken");
        }

        public void SetCookies(HttpContext context, string token, bool rememberMe)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(1)
            };

            context.Response.Cookies.Append("AuthToken", token, cookieOptions);
        }

public async Task<User?> GetUserFromToken(HttpContext context)
   {
       var token = context.Request.Cookies["AuthToken"];
       if (token == null)
       {
           return null;
       }

       var principal = _tokenService.ValidateAuthToken(token);
       if (principal == null)
       {
           return null;
       }

       var userId = int.Parse(principal.FindFirst("UserId").Value);
       return await _userRepository.GetUserById(userId);
   }
    }
}