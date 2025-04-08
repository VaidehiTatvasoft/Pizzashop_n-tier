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
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMenuModifierService, MenuModifierService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IOrderAppKOTService, OrderAppKOTService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<ITaxAndFeeService, TaxAndFeeService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IMappingMenuItemsWithModifierRepository, MappingMenuItemsWithModifierRepository>();
            services.AddScoped<IMenuCategoryRepository, MenuCategoryRepository>();
            services.AddScoped<IMenuItemsRepository, MenuItemRepository>();
            services.AddScoped<IMenuModifierGroupRepository, MenuModifierGroupRepository>();
            services.AddScoped<IMenuModifierRepository, MenuModifierRepository>();
            services.AddScoped<IOrderAppKOTRepository, OrderAppKOTRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<ITaxAndFeeRepository, TaxAndFeeRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}