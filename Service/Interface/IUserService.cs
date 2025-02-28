using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;

namespace pizzashop.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddUserAsync(UserViewModel model, ClaimsPrincipal userClaims);
        Task<UserViewModel> GetUserViewModelByIdAsync(int id);
        Task<bool> EditUserAsync(UserViewModel model);
        Task<bool> DeleteUserAsync(int id);
        Task<UserList> GetUserListAsync(string search, int page, int pageSize, string sortColumn, string sortOrder);
        Task<UserViewModel> GetUserProfileAsync(ClaimsPrincipal userClaims);
        Task<bool> UpdateProfileAsync(UserViewModel model, ClaimsPrincipal userClaims);
        Task<bool> ChangePasswordAsync(ChangePasswordModel model, ClaimsPrincipal userClaims);
    }
}
