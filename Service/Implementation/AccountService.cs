using System.Threading.Tasks;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Pizzashop.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IEmailService _emailService;
        private readonly PizzaShopContext _context;
        private readonly IConfiguration _configuration;

        public AccountService(IEmailService emailService, PizzaShopContext context, IConfiguration configuration)
        {
            _emailService = emailService;
            _context = context;
            _configuration = configuration;
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            if (account != null && BCrypt.Net.BCrypt.Verify(password, account.PasswordHash))
            {
                return new User
                {
                    Id = account.Id,
                    Email = account.Email,
                    PasswordHash = account.PasswordHash,
                    RoleId = account.RoleId
                };
            }
            return null;
        }

        public async Task SendForgotPasswordEmail(string email, string resetLink)
        {
            await _emailService.SendEmailAsync(email, "Password Reset", $"Click here to reset your password: {resetLink}");
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            if (account != null)
            {
                return new User
                {
                    Id = account.Id,
                    Email = account.Email,
                    PasswordHash = account.PasswordHash,
                    RoleId = account.RoleId 
                };
                
            }
            return null;
        }

        public async Task ResetPassword(string email, string newPassword)
        {
            var user = await GetUserByEmail(email);
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();
            }
        }

        public void Logout(HttpContext context)
        {
            context.Session.Clear();
            context.Response.Cookies.Delete(".AspNetCore.Identity.Application");
            context.Response.Cookies.Delete("AuthToken");
        }

        public string GenerateToken(User user)
        {
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void SetCookies(HttpContext context, User user, string token, bool rememberMe)
        {
            if (rememberMe)
            {
                context.Response.Cookies.Append("UserEmail", user.Email, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                });
                context.Response.Cookies.Append("UserPassword", user.PasswordHash, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                });
            }
            context.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true,
            });
        }
    }
}