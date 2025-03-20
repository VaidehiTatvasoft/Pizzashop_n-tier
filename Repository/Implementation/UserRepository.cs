using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Repository.Interfaces;
using Entity.Data;
using Entity.ViewModel;

namespace Pizzashop.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly PizzaShopContext _context;

        public UserRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<string> GetRoleNameById(int roleId)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            return role?.Name;
        }
        public async Task ResetPassword(string email, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, salt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<bool> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return false;
            }

            user.IsDeleted = true;
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public IEnumerable<User> GetUserList(string searchString, string sortOrder, int pageIndex, int pageSize, out int count)
        {
            var userQuery = _context.Users.Where(u => u.IsDeleted == false);

            if (!string.IsNullOrEmpty(searchString))
            {
                userQuery = userQuery.Where(u => u.FirstName.ToLower().Contains(searchString.ToLower()) || u.LastName.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "username_asc":
                    userQuery = userQuery.OrderBy(u => u.FirstName);
                    break;

                case "username_desc":
                    userQuery = userQuery.OrderByDescending(u => u.FirstName);
                    break;

                case "role_asc":
                    userQuery = userQuery.OrderBy(u => u.Role.Name);
                    break;

                case "role_desc":
                    userQuery = userQuery.OrderByDescending(u => u.Role.Name);
                    break;

                default:
                    userQuery = userQuery.OrderBy(u => u.FirstName);
                    break;
            }

            count = userQuery.Count();

            return userQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _context.Countries.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<List<State>> GetStatesByCountryIdAsync(int countryId)
        {
            return await _context.States.Where(s => s.CountryId == countryId).OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<List<City>> GetCitiesByStateIdAsync(int stateId)
        {
            return await _context.Cities.Where(c => c.StateId == stateId).OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.OrderBy(r => r.Name).ToListAsync();
        }
    }
}