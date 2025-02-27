using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Implementation;
using Pizzashop.Repository.Implementation;
using Pizzashop.Repository.Interfaces;

namespace Web
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();  
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}
