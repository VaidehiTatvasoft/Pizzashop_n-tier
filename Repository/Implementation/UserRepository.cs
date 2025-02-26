using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Repository.Interfaces;
using Entity.Data;

namespace Pizzashop.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly PizzaShopContext _context;

        public UserRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateUser(string email, string passwordHash)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash);
        }
    }
}