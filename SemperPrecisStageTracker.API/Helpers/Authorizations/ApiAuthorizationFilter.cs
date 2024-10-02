using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Domain.Services;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Data;

namespace SemperPrecisStageTracker.API.Helpers
{
    public class ApiAuthorizationFilter : ActionFilterAttribute
    {
        private readonly IPermissionInterface _permissions;

        public string PermissionsString => _permissions.ToString();

        public ApiAuthorizationFilter(params Permissions[] entityPermissions)
        {
            _permissions = PermissionCtor.Compose(entityPermissions);
        }

        public ApiAuthorizationFilter(IPermissionInterface entityPermissions)
        {
            _permissions = entityPermissions;
        }

        /// <summary>
        /// extract EntityId value attribute
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetPermissionIdValue(ActionExecutingContext context)
        {
            if (context.ActionArguments.Any())
            {
                 var parameterName = context.ActionDescriptor.Parameters
                    .Where(p => p.BindingInfo?.BindingSource == BindingSource.Body)
                    .FirstOrDefault()?.Name;
                
                if(string.IsNullOrEmpty(parameterName))
                    return string.Empty;

                var argument = context.ActionArguments.Where(p => p.Key == parameterName)
                    .Select(s => s.Value)
                    .FirstOrDefault();

                if (argument is EntityFilterValidation entity)
                    return entity.EntityId;
                else
                    return string.Empty;
                    //throw new ArgumentException($"Parameter {parameterName} must inherit from {nameof(EntityFilterValidation)}");
            }
            return string.Empty;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = PlatformUtils.GetIdentityUserId(context.HttpContext.User);
            if (userId == null)
            {
                context.Result = new ForbidResult();
            }

            var entityId = GetPermissionIdValue(context);

            var valueFound = await CheckPermissionsOnUser(userId, entityId);

            if (valueFound)
            {
                await next();
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }

        private Task<bool> CheckPermissionsOnUser(string userId, string entityId = null)
        {
            if (string.IsNullOrEmpty(userId))
                return Task.FromResult(false);
            //Transazione isolata per il database con il solo scopo di identificare
            //l'accesso di emergenza, subito chiusa al termine dell'operazione
            using IDataSession isolatedSession = SessionFactory.OpenSession();

            //Service layer base
            using var authServiceLayer = new AuthenticationServiceLayer(isolatedSession);

            return authServiceLayer.ValidateUserPermissions(userId, entityId, _permissions);
        }
    }
}