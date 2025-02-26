using System.Threading.Tasks;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Entity.Data;

namespace Pizzashop.Service.Implementation
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;

        public LoginService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateUser(string email, string passwordHash)
        {
            return await _userRepository.AuthenticateUser(email, passwordHash);
        }
    }
}