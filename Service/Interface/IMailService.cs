using System.Threading.Tasks;

namespace Pizzashop.Service.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}