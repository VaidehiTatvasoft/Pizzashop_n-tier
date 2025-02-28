using Entity.Data;
using Entity.ViewModel;
using System.Threading.Tasks;

namespace Pizzashop.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> AuthenticateUser(string email, string passwordHash);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int userId);
        Task ResetPassword(string email, string newPassword);
        Task<bool> AddUserAsync(User user);
        Task<UserViewModel> GetUserViewModelByIdAsync(int id);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<UserListInfo> GetUserListAsync(string search, int page, int pageSize, string sortColumn, string sortOrder);
        Task<UserViewModel> GetUserProfileByEmailAsync(string email);
    }
}