using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Entity.Data;

namespace Pizzashop.Service.Interfaces
{
    public interface IAccountService
    {
        Task<User?> AuthenticateUser(string email, string password);
        Task<User?> GetUserByEmail(string email);
        Task ResetPassword(string email, string newPassword);
        void Logout(HttpContext context);
    }
}