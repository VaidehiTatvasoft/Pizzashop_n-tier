using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Entity.Data;

namespace Pizzashop.Service.Interfaces
{
    public interface IAccountService
    {
        Task<(User?, bool isActive)> AuthenticateUser(string email, string password);
        Task<User?> GetUserByEmail(string email);
        Task ResetPassword(string email, string newPassword);
        void Logout(HttpContext context);
        void SetCookies(HttpContext context, string token, bool rememberMe);
        Task SendForgotPasswordEmail(string email, string resetLink);

    }
}