using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Implementation;
using Pizzashop.Repository.Implementation;
using pizzashop.Services.Interfaces;
using Service.Interface;
using Repository.Interfaces;
using Repository.Interface;
using Repository.Implementation;
using Service.Implementation;
using Entity.Data;
using Microsoft.EntityFrameworkCore;

namespace Web
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PizzaShopContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("pizza_shopConnection")));

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IModifierService, ModifierService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<ITaxAndFeeService, TaxAndFeeService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IModifierRepository, ModifierRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<ITaxAndFeeRepository, TaxAndFeeRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}