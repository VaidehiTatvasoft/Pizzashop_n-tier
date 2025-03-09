using System.Security.Claims;
using Entity.Data;

namespace Pizzashop.Service.Interfaces
{
    public interface ITokenService
{
        string GenerateAuthToken(User user, TimeSpan validFor);
        string GenerateResetPasswordToken(User user, TimeSpan validFor);
        ClaimsPrincipal ValidateAuthToken(string token);
        ClaimsPrincipal ValidateResetPasswordToken(string token);
        void MarkTokenAsUsed(string token);
}
    }
