using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Middleware
{
    public class FirstLoginMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<FirstLoginMiddleware> _logger;

        public FirstLoginMiddleware(RequestDelegate next, ILogger<FirstLoginMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();
            _logger.LogInformation($"Processing request for path: {path}");

            if (context.User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User is authenticated.");
                var isFirstLogin = context.User.Claims.FirstOrDefault(c => c.Type == "isFirstLogin")?.Value;
                _logger.LogInformation($"isFirstLogin claim value: {isFirstLogin}");

                if (bool.TryParse(isFirstLogin, out var isFirstLoginValue) && isFirstLoginValue)
                {
                    if (!path.Contains("/forgotpassword") && !path.Contains("/resetpassword") && !path.Contains("/user/changepassword") && !path.Contains("/logout"))
                    {
                        context.Response.Redirect("/user/changepassword");
                        return;
                    }
                }
            }
            else
            {
                _logger.LogInformation("User is not authenticated.");
            }

            await _next(context);
        }
    }
}