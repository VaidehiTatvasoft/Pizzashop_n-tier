using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Pizzashop.Service.Interfaces;

namespace Pizzashop.Service.Implementation
{
    public class MailService : IMailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
 var fromAddress = new MailAddress("test.dotnet@etatvasoft.com", "PizzaShop");
            var toAddress = new MailAddress(to);
            const string fromPassword = "P}N^{z-]7Ilp";
            
            var smtp = new SmtpClient
            {
                Host = "mail.etatvasoft.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }        }
    }
}