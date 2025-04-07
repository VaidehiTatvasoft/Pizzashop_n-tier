using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Entity.Shared;
using System.Linq;

namespace Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly RolePermissionEnum.Permission _requiredPermission;
        private ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public CustomAuthorizeAttribute( RolePermissionEnum.Permission permission)
        {
            _requiredPermission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _tempDataDictionaryFactory = context.HttpContext.RequestServices.GetService(typeof(ITempDataDictionaryFactory)) as ITempDataDictionaryFactory;
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Accounts", null);
                return;
            }

            var userPermissions = context.HttpContext.Items["Permissions"] as HashSet<RolePermissionEnum.Permission>;
            if (userPermissions == null || !userPermissions.Contains(_requiredPermission))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Result = new JsonResult(new { message = "You are not authorized to access this page." });
                }
                else
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                }
                return;
            }
        }
    }
}
