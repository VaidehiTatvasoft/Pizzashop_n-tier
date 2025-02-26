using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Service.Interfaces;
using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace Pizzashop.Web.Controllers
{
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IConfiguration _configuration;

        public AccountsController(ILoginService loginService, IConfiguration configuration)
        {
            _loginService = loginService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _loginService.AuthenticateUser(model.Email, model.PasswordHash);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"] ?? "DefaultSubject"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var keyString = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
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
    }
}