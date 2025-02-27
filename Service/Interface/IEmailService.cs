using System.Threading.Tasks;

namespace Pizzashop.Service.Interfaces
{
    public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
}