using System.Threading.Tasks;
using Entity.Data;

namespace Pizzashop.Service.Interfaces
{
    public interface ILoginService
    {
        Task<User?> AuthenticateUser(string email, string passwordHash);
    }
}