using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Implementation;
using Pizzashop.Repository.Implementation;
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
            services.AddScoped<IMenuService, MenuService>(); 
            services.AddScoped<IModifierService, ModifierService>(); 
            services.AddScoped<IRolePermissionService, RolePermissionService>(); 
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IModifierRepository, ModifierRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}
