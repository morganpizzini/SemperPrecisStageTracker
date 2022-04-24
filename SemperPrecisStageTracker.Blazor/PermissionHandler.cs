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
        public PermissionHandler(IAuthenticationService authService)
        {
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
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}