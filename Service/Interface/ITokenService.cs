using System.Collections.Generic;
using System.Security.Claims;
using Entity.Data;

namespace Pizzashop.Service.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        ClaimsPrincipal ValidateToken(string token);
    }
}