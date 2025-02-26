using System.Threading.Tasks;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Entity.Data;
using MailKit;
using System;

namespace Pizzashop.Service.Implementation
{
    public class AccountService : IAccountService
{
    private readonly IMailService _emailService;
    private readonly AppContext _context;

    public AccountService(IMailService emailService, AppContext context)
    {
        _emailService = emailService;
        _context = context;
    }

    public Task<User?> AuthenticateUser(string email, string password)
    {
        // Implementation here
    }

    public async Task SendForgotPasswordEmail(string email, string resetLink)
    {
        // Email sending logic here
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
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