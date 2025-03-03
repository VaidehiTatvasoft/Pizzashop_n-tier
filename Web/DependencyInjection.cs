using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Implementation;
using Pizzashop.Repository.Implementation;
using Pizzashop.Repository.Interfaces;
using pizzashop.Services.Interfaces;
using Web.Repositories.Interfaces;
using Web.Repositories;
using Web.Services.Interfaces;
using Web.Services;
using Service.Interface;
using Repository.Interface;

namespace Web
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>(); 
            services.AddScoped<IUserService, UserService>(); 
            services.AddScoped<IPermissionService, PermissionService>(); 
            services.AddScoped<IRoleService, RoleService>(); 
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}
