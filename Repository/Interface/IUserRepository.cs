using Entity.Data;
using Entity.ViewModel;
using System.Threading.Tasks;

namespace Repository.Interfaces;

    public interface IUserRepository
    {
        Task<User?> AuthenticateUser(string email, string password);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByUsername(string username);
        Task<string> GetRoleNameById(int roleId);
        Task<User?> GetUserById(int userId);
        Task ResetPassword(string email, string newPassword);
        Task<bool> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        IEnumerable<User> GetUserList(string searchString, string sortOrder, int pageIndex, int pageSize, out int count);
        Task<City> GetCityByIdAsync(int cityId);
        Task<List<City>> GetCitiesByStateIdAsync(int stateId);
        Task<Country> GetCountryByIdAsync(int countryId);
        Task<List<Country>> GetAllCountriesAsync();
        Task<State> GetStateByIdAsync(int stateId);
        Task<List<State>> GetStatesByCountryIdAsync(int countryId);
    }
