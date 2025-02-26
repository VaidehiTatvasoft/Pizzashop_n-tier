using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Implementation;

namespace Web
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMailService, MailService>(); 
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}
