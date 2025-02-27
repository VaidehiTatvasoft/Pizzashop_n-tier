using System.Security.Claims;
using Entity.Data;

namespace Pizzashop.Service.Interfaces
{
    public interface ITokenService
{
    string GenerateToken(User user, TimeSpan validFor);
    ClaimsPrincipal ValidateToken(string token);
}
    }
