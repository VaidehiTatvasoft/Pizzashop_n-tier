using System.Threading.Tasks;
using Entity.Data;
using Microsoft.AspNetCore.Http;

namespace Pizzashop.Service.Interfaces
{
    public interface IAccountService
{
    Task<User?> AuthenticateUser(string email, string password);
    Task SendForgotPasswordEmail(string email, string resetLink);
    Task<User?> GetUserByEmail(string email);
    Task ResetPassword(string email, string newPassword);
    void Logout(HttpContext context); 

}
}