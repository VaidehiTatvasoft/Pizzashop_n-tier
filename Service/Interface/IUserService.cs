using System.Security.Claims;
using Entity.ViewModel;
using System.Threading.Tasks;
using Entity.Data;

namespace pizzashop.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddUserAsync(UserViewModel model, ClaimsPrincipal userClaims);
        Task<UserViewModel> GetUserViewModelByIdAsync(int id);
        Task<bool> EditUserAsync(UserViewModel model);
        Task<bool> DeleteUserAsync(int id);
        IEnumerable<User> GetUsersList(string searchString, string sortOrder, int pageIndex, int pageSize, out int count);
        Task<UserViewModel> GetUserProfileAsync(ClaimsPrincipal userClaims);
        Task<bool> UpdateProfileAsync(UserViewModel model, ClaimsPrincipal userClaims);
        Task<bool> ChangePasswordAsync(ChangePasswordModel model, ClaimsPrincipal userClaims);
    }
}