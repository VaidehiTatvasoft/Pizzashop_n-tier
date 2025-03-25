using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Entity.Shared; 
using Entity.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
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

                        var permissions = new List<RolePermissionEnum.Permission>();
                        foreach(var rp in rolePermissions)
                        {
                            if (rp.CanView)
                                permissions.Add((RolePermissionEnum.Permission)rp.PermissionId);
                            if (rp.CanEdit)
                                permissions.Add((RolePermissionEnum.Permission)rp.PermissionId + 100); 
                            if (rp.CanDelete)
                                permissions.Add((RolePermissionEnum.Permission)rp.PermissionId + 200); 
                        }

                        context.Items["Permissions"] = permissions;
                    }
                }
            }

            await _next(context);
        }
    }
}