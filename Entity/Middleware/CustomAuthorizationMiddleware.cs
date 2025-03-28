using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Entity.Shared; 
using Entity.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Entity.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomAuthorizationMiddleware> _logger;

        public CustomAuthorizationMiddleware(RequestDelegate next, ILogger<CustomAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, PizzaShopContext dbContext)
        {
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var roleClaim = user.FindFirst(System.Security.Claims.ClaimTypes.Role);
                if (roleClaim != null)
                {
                    if (int.TryParse(roleClaim.Value, out int roleId))
                    {
                        var rolePermissions = await dbContext.RolePermissions
                            .Where(rp => rp.RoleId == roleId)
                            .Select(rp => new 
                            {
                                rp.PermissionId,
                                rp.CanView,
                                rp.CanEdit,
                                rp.CanDelete
                            })
                            .ToListAsync();

                        _logger.LogInformation($"Retrieved Role Permissions: {string.Join(", ", rolePermissions.Select(rp => $"Permission ID: {rp.PermissionId}, CanView: {rp.CanView}, CanEdit: {rp.CanEdit}, CanDelete: {rp.CanDelete}"))}");

                        var permissions = new HashSet<RolePermissionEnum.Permission>(); // Use HashSet to avoid duplicates
                        foreach(var rp in rolePermissions)
                        {
                            _logger.LogInformation($"Role ID: {roleId}, Permission ID: {rp.PermissionId}, CanView: {rp.CanView}, CanEdit: {rp.CanEdit}, CanDelete: {rp.CanDelete}");

                            if (rp.CanView)
                            {
                                                                // permissions.Add((RolePermissionEnum.Permission)rp.PermissionId);

                                switch (rp.PermissionId)
                                {
                                    case 1:
                                        permissions.Add(RolePermissionEnum.Permission.Users_CanView);
                                        break;
                                    case 2:
                                        permissions.Add(RolePermissionEnum.Permission.RolesAndPermissions_CanView);
                                        break;
                                    case 3:
                                        permissions.Add(RolePermissionEnum.Permission.Menu_CanView);
                                        break;
                                    case 4:
                                        permissions.Add(RolePermissionEnum.Permission.TablesAndSections_CanView);
                                        break;
                                    case 5:
                                        permissions.Add(RolePermissionEnum.Permission.TaxesAndFees_CanView);
                                        break;
                                    case 6:
                                        permissions.Add(RolePermissionEnum.Permission.Orders_CanView);
                                        break;
                                    case 7:
                                        permissions.Add(RolePermissionEnum.Permission.Customers_CanView);
                                        break;
                                    default:
                                        _logger.LogWarning($"Unknown PermissionId for CanView: {rp.PermissionId}");
                                        break;
                                }
                                _logger.LogInformation($"Added View Permission: {(RolePermissionEnum.Permission)rp.PermissionId}");
                            }
                            if (rp.CanEdit)
                            {
                                switch (rp.PermissionId)
                                {
                                    case 1:
                                        permissions.Add(RolePermissionEnum.Permission.Users_CanEdit);
                                        break;
                                    case 2:
                                        permissions.Add(RolePermissionEnum.Permission.RolesAndPermissions_CanEdit);
                                        break;
                                    case 3:
                                        permissions.Add(RolePermissionEnum.Permission.Menu_CanEdit);
                                        break;
                                    case 4:
                                        permissions.Add(RolePermissionEnum.Permission.TablesAndSections_CanEdit);
                                        break;
                                    case 5:
                                        permissions.Add(RolePermissionEnum.Permission.TaxesAndFees_CanEdit);
                                        break;
                                    case 6:
                                        permissions.Add(RolePermissionEnum.Permission.Orders_CanEdit);
                                        break;
                                    case 7:
                                        permissions.Add(RolePermissionEnum.Permission.Customers_CanEdit);
                                        break;
                                    default:
                                        _logger.LogWarning($"Unknown PermissionId for CanEdit: {rp.PermissionId}");
                                        break;
                                }
                                _logger.LogInformation($"Added Edit Permission: {(RolePermissionEnum.Permission)rp.PermissionId}");
                            }
                            if (rp.CanDelete)
                            {
                                switch (rp.PermissionId)
                                {
                                    case 1:
                                        permissions.Add(RolePermissionEnum.Permission.Users_CanDelete);
                                        break;
                                    case 2:
                                        permissions.Add(RolePermissionEnum.Permission.RolesAndPermissions_CanDelete);
                                        break;
                                    case 3:
                                        permissions.Add(RolePermissionEnum.Permission.Menu_CanDelete);
                                        break;
                                    case 4:
                                        permissions.Add(RolePermissionEnum.Permission.TablesAndSections_CanDelete);
                                        break;
                                    case 5:
                                        permissions.Add(RolePermissionEnum.Permission.TaxesAndFees_CanDelete);
                                        break;
                                    case 6:
                                        permissions.Add(RolePermissionEnum.Permission.Orders_CanDelete);
                                        break;
                                    case 7:
                                        permissions.Add(RolePermissionEnum.Permission.Customers_CanDelete);
                                        break;
                                    default:
                                        _logger.LogWarning($"Unknown PermissionId for CanDelete: {rp.PermissionId}");
                                        break;
                                }
                                _logger.LogInformation($"Added Delete Permission: {(RolePermissionEnum.Permission)rp.PermissionId}");
                            }
                        }

                        // Logging the role ID and permissions
                        _logger.LogInformation($"Final Permissions for Role ID: {roleId}: {string.Join(", ", permissions)}");

                        context.Items["Permissions"] = permissions;
                    }
                }
            }

            await _next(context);
        }
    }
}