using Entity.Data;

namespace Pizzashop.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User>? AuthenticateUser(string email, string passwordHash);
    }
}
