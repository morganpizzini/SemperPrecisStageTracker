using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Domain.Services;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Data;

namespace SemperPrecisStageTracker.API.Helpers
{
    public class ApiAuthorizationFilter : ActionFilterAttribute
    {
        private readonly List<Permissions> _permissions = new();
        //private readonly List<MatchPermissions> _matchPermissions = new();

        public string PermissionsString
        {
            get
            {
                var result = string.Empty;
                
                if (_permissions.Any())
                    result += " " + string.Join(" ", _permissions);

                return result;
            }
        }
        
        public ApiAuthorizationFilter(params Permissions[] entityPermissions)
        {
            _permissions.Clear();
            _permissions.AddRange(entityPermissions);
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
                var argument = context.ActionArguments.Where(p => p.Key == "request")
                    .Select(s => s.Value)
                    .FirstOrDefault();
                if (argument is EntityFilterValidation entity)
                    return entity.EntityId;
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