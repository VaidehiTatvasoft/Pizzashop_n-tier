using System.Threading.Tasks;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Entity.Data;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IMailService _emailService;
        private readonly PizzaShopContext _context;

        public AccountService(IMailService emailService, PizzaShopContext context)
        {
            _emailService = emailService;
            _context = context;
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email && a.PasswordHash == password);
            if (account != null)
            {
                return new User
                {
                    Id = account.Id,
                    Email = account.Email,
                    PasswordHash = account.PasswordHash,
                };
            }
            return null;
        }

        public async Task SendForgotPasswordEmail(string email, string resetLink)
        {
            await _emailService.SendEmailAsync(email, "Password Reset", $"Click here to reset your password: {resetLink}");
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            if (account != null)
            {
                return new User
                {
                    Id = account.Id,
                    Email = account.Email,
                    PasswordHash = account.PasswordHash,
                };
            }
            return null;
        }

        public async Task ResetPassword(string email, string newPassword)
        {
            var user = await GetUserByEmail(email);
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();
            }
        }
    }
}