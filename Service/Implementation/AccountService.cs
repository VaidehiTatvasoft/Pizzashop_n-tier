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
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AccountService(IEmailService emailService, IUserRepository userRepository, IConfiguration configuration)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _configuration = configuration;
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
            await _emailService.SendEmailAsync(email, "Password Reset", $"Click here to reset your password: {resetLink}");
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key")));
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

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key"));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);

                // Fetch the user from the database using the userId
                return await _userRepository.GetUserById(userId);
            }
            catch
            {
                return null;
            }
        }
    }
}