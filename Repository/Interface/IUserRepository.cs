using Entity.Data;
using Entity.ViewModel;
using System.Threading.Tasks;

namespace Pizzashop.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> AuthenticateUser(string email, string password);
        Task<User?> GetUserByEmail(string email);
        Task<string> GetRoleNameById(int roleId);
        Task<User?> GetUserById(int userId);
        Task ResetPassword(string email, string newPassword);
        Task<bool> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<string> GetUserProfileImageAsync(string email);
        Task<bool> DeleteUserAsync(int id);
        IEnumerable<User> GetUserList(string searchString, string sortOrder, int pageIndex, int pageSize, out int count);
    }
}