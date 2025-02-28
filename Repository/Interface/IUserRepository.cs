using Entity.Data;
using Entity.ViewModel;
using System.Threading.Tasks;

namespace Pizzashop.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> AuthenticateUser(string email, string password);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int userId);
        Task ResetPassword(string email, string newPassword);
        Task<bool> AddUserAsync(UserViewModel model, int userId);
        Task<UserViewModel> GetUserViewModelByIdAsync(int id);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<UserListInfo> GetUserListAsync(string search, int page, int pageSize, string sortColumn, string sortOrder);
        Task<UserViewModel> GetUserProfileByEmailAsync(string email);
    }
}