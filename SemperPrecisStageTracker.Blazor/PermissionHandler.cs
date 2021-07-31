using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using SemperPrecisStageTracker.Blazor.Services;

namespace SemperPrecisStageTracker.Blazor
{
    public class PermissionHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<PermissionHandler> _logger;
        public PermissionHandler(IAuthenticationService authService, ILogger<PermissionHandler> logger)
        {
            _logger = logger;
            _authService = authService;
        }
        

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            RolesAuthorizationRequirement requirement)
        {
            
            if (context.User.Identity is {IsAuthenticated: false})
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (!requirement.AllowedRoles.Any())
            {
                // it means any logged in user is allowed to access the resource    
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var roles = requirement.AllowedRoles;
            
            // get profile id from resource, passed in from blazor 
            //  page component
            var resourceId = context.Resource?.ToString() ?? string.Empty;
            
            if (_authService.CheckPermissions(roles, resourceId))
            {
                context.Succeed(requirement);
                _logger.LogInformation("OOOKKK");
                return Task.CompletedTask;
            }

            _logger.LogInformation("NOOOO");

            context.Fail();
            return Task.CompletedTask;
        }
    }
}