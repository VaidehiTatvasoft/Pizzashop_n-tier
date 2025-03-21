using System.Security.Claims;
using Entity.ViewModel;
using System.Threading.Tasks;
using Entity.Data;

namespace pizzashop.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<Country>> GetAllCountriesAsync();
        Task<List<State>> GetStatesByCountryIdAsync(int countryId);
        Task<List<City>> GetCitiesByStateIdAsync(int stateId);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByUsername(string username);
        Task<bool> AddUserAsync(AddUserModel model, ClaimsPrincipal userClaims);
        Task<UserViewModel> GetUserViewModelByIdAsync(int id);
        Task<bool> EditUserAsync(UserViewModel model);
        Task<bool> DeleteUserAsync(int id);
        Task<string> GetUserProfileImageAsync(string email);
        IEnumerable<User> GetUsersList(string searchString, string sortOrder, int pageIndex, int pageSize, out int count);
        Task<UserViewModel> GetUserProfileAsync(ClaimsPrincipal userClaims);
        Task<bool> UpdateProfileAsync(UserViewModel model, ClaimsPrincipal userClaims);
        Task<bool> ChangePasswordAsync(ChangePasswordModel model, ClaimsPrincipal userClaims);
    }
}