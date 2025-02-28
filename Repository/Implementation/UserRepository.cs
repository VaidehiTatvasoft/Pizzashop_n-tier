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

        public async Task<UserViewModel> GetUserViewModelByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    Phone = u.Phone,
                    CountryId = u.CountryId,
                    StateId = u.StateId,
                    CityId = u.CityId,
                    Address = u.Address,
                    Zipcode = u.Zipcode,
                    RoleId = u.RoleId,
                    Status = u.IsDeleted.HasValue && u.IsDeleted.Value ? "Inactive" : "Active",
                    Email = u.Email,
                    ProfileImage = u.ProfileImage,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
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

         public async Task<UserListInfo> GetUserListAsync(string search, int page, int pageSize, string sortColumn, string sortOrder)
    {
        var usersQuery = _context.Users.AsQueryable();

        var totalUsers = await usersQuery.CountAsync();
        var users = await usersQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

        return new UserListInfo
        {
            Users = users,
            TotalUsers = totalUsers,
            CurrentPage = page,
            TotalPages = totalPages
        };
    }

        public async Task<UserViewModel> GetUserProfileByEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .Select(u => new UserViewModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    Phone = u.Phone,
                    RoleId = u.RoleId,
                    RoleName = _context.Roles.Where(r => r.Id == u.RoleId).Select(r => r.Name).FirstOrDefault(),
                    CountryId = u.CountryId,
                    CountryName = _context.Countries.Where(c => c.Id == u.CountryId).Select(c => c.Name).FirstOrDefault(),
                    StateId = u.StateId,
                    StateName = _context.States.Where(s => s.Id == u.StateId).Select(s => s.Name).FirstOrDefault(),
                    CityId = u.CityId,
                    CityName = _context.Cities.Where(c => c.Id == u.CityId).Select(c => c.Name).FirstOrDefault(),
                    Zipcode = u.Zipcode,
                    Address = u.Address
                })
                .FirstOrDefaultAsync();
        }
        
    }
}