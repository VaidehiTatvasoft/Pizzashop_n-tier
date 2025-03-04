using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Implementation;
using Pizzashop.Repository.Implementation;
using Pizzashop.Repository.Interfaces;
using pizzashop.Services.Interfaces;
using Service.Interface;
using Repository.Interfaces;

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
            services.AddScoped<IRolePermissionService, RolePermissionService>(); 
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}
