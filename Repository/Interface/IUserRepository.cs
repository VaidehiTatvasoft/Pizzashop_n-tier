using Entity.Data;
using System.Threading.Tasks;

namespace Pizzashop.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> AuthenticateUser(string email, string passwordHash);
        Task<User?> GetUserByEmail(string email);
        Task ResetPassword(string email, string newPassword);
    }
}