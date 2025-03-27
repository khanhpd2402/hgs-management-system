using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HGSMAPI.Configurations;

namespace HGSMAPI.Middlewares
{
    public class AuthorizationUser : Attribute, IAuthorizationFilter
    {

        private readonly string[] _roles;
        private readonly IConfiguration _configuration;

        public AuthorizationUser(params string[] roles)
        {
            _roles = roles;

            // Build the configuration from appsettings.json
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = configBuilder.Build();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.ToString().StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Extract the access token from the Authorization header
            var accessToken = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();

            try
            {
                ClaimsPrincipal claimsPrincipal = JWTConfig.ValidateToken(accessToken, _configuration);

                // Check if the user has any of the required roles
                if (_roles.Length > 0 && !_roles.Any(role => claimsPrincipal.IsInRole(role)))
                {
                    context.Result = new ForbidResult();
                    return;
                }

                context.HttpContext.User = claimsPrincipal;
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}