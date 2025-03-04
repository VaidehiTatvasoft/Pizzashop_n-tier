using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Implementation;
using Pizzashop.Repository.Implementation;
using Pizzashop.Repository.Interfaces;
using pizzashop.Services.Interfaces;
using Service.Interface;
using Repository.Interfaces;
using Repository.Interface;
using Repository.Implementation;
using Service.Implementation;

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
            services.AddScoped<IItemService, ItemService>(); 
            services.AddScoped<ICategoryService, CategoryService>(); 
            services.AddScoped<IModifierService, ModifierService>(); 
            services.AddScoped<IRolePermissionService, RolePermissionService>(); 
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}
