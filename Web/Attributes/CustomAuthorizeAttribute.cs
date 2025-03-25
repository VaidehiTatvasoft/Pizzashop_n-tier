using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Entity.Shared; 
using System.Linq;

namespace Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int _requiredRoleId;
        private readonly RolePermissionEnum.Permission _requiredPermission;

        public CustomAuthorizeAttribute(int roleId, RolePermissionEnum.Permission permission)
        {
            _requiredRoleId = roleId;
            _requiredPermission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleClaim = user.FindFirst(System.Security.Claims.ClaimTypes.Role);
            if (roleClaim == null || !int.TryParse(roleClaim.Value, out int userRoleId) || userRoleId != _requiredRoleId)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userPermissions = context.HttpContext.Items["Permissions"] as List<RolePermissionEnum.Permission>;

            if (userPermissions == null || !userPermissions.Contains(_requiredPermission))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
