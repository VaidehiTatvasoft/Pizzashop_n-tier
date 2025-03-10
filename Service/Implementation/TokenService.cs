using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pizzashop.Service.Interfaces;

namespace Pizzashop.Service.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ConcurrentDictionary<string, DateTime> _usedTokens = new ConcurrentDictionary<string, DateTime>();
        private readonly ILogger<TokenService> _logger;


        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;

        }

        public string GenerateAuthToken(User user, TimeSpan validFor)
        {
            return GenerateToken(user, validFor, "auth");
        }

        public string GenerateResetPasswordToken(User user, TimeSpan validFor)
        {
            return GenerateToken(user, validFor, "reset");
        }

        private string GenerateToken(User user, TimeSpan validFor, string tokenType)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "your_subject"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                new Claim("TokenType", tokenType)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.Add(validFor),
                signingCredentials: signIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateAuthToken(string token)
        {
            return ValidateToken(token, "auth");
        }

        public ClaimsPrincipal ValidateResetPasswordToken(string token)
        {
            return ValidateToken(token, "reset");
        }

        private ClaimsPrincipal ValidateToken(string token, string expectedTokenType)
        {
            if (string.IsNullOrEmpty(token) || _usedTokens.ContainsKey(token))
            {
                _logger.LogWarning("Token is null or has been used: {Token}", token);
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    _logger.LogWarning("Token has expired: {Token}", token);
                    return null;
                }

                var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "TokenType")?.Value;
                if (tokenType != expectedTokenType)
                {
                    _logger.LogWarning("Token type mismatch: {Token}", token);
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed: {Token}", token);
                return null;
            }
        }

        public void MarkTokenAsUsed(string token)
        {
            _usedTokens[token] = DateTime.UtcNow;
        }
    }
}
