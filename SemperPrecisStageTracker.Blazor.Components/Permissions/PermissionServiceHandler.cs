using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SemperPrecisStageTracker.Blazor.Services;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.Blazor
{
    public class PermissionServiceHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        private readonly IAuthenticationService _authService;
        public PermissionServiceHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            RolesAuthorizationRequirement requirement)
        {
            if (context.User.Identity is { IsAuthenticated: false })
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

            var permissions = requirement.AllowedRoles
                .Select(x=>x.ParseEnum<Permissions>())
                .ToList();

            // get profile id from resource, passed in from blazor 
            //  page component
            var resourceId = context.Resource?.ToString() ?? string.Empty;

            if (_authService.CheckPermissions(permissions, resourceId))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}