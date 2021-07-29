using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.OpenApi.Models;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Domain.Services;
using SemperPrecisStageTracker.Models.Commons;
using SemperPrecisStageTracker.Shared.Permissions;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZenProgramming.Chakra.Core.Data;

namespace SemperPrecisStageTracker.API.Helpers
{
    // https://ekobit.com/blog/asp-netcore-custom-authorization-attributes/
   
    public class UserPermissions
    {
        public ICollection<AdministrationPermissions> AdministratorPermissions { get; set; } =
            new List<AdministrationPermissions>();

        public IDictionary<string, List<EntityPermissions>> EntityPermissions { get; set; } =
            new Dictionary<string, List<EntityPermissions>>();
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class EntityIdAttribute : Attribute
    {
    }

    public class ApiAuthorizationFilter : ActionFilterAttribute
    {
        private readonly List<AdministrationPermissions> _adminPermission = new();
        private readonly List<EntityPermissions> _shooterPermission = new();
        //private readonly List<MatchPermissions> _matchPermissions = new();

        public string PermissionsString
        {
            get
            {
                var result = string.Empty;
                if (_adminPermission.Any())
                    result += string.Join(" ", _adminPermission);

                if (_shooterPermission.Any())
                    result += " " + string.Join(" ", _shooterPermission);

                return result;
            }
        }


        public ApiAuthorizationFilter(params EntityPermissions[] permissions)
        {
            _shooterPermission.Clear();
            _shooterPermission.AddRange(permissions);
        }

        public ApiAuthorizationFilter(params AdministrationPermissions[] permissions)
        {
            _adminPermission.Clear();
            _adminPermission.AddRange(permissions);
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

            var permissions = await GetUserPermissions(userId);

            var valueFound = CheckPermissionsForValue(permissions,entityId);

            if (valueFound)
            {
                await next();
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
        
        private async Task<UserPermissions> GetUserPermissions(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new();
            //Transazione isolata per il database con il solo scopo di identificare
            //l'accesso di emergenza, subito chiusa al termine dell'operazione
            using IDataSession isolatedSession = SessionFactory.OpenSession();

            //Service layer base
            using var serviceLayer = new AuthenticationServiceLayer(isolatedSession);

            var userPermissions = await serviceLayer.GetUserPermissionById(userId);

            return new UserPermissions()
            {
                AdministratorPermissions =
                    userPermissions.adminPermissions.Select(x => x.ParseEnum<AdministrationPermissions>()).ToList(),
                EntityPermissions = userPermissions.entityPermissions.GroupBy(x => x.EntityId)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(e => e.Permission.ParseEnum<EntityPermissions>()).ToList())
            };
        }

        private bool CheckPermissionsForValue(UserPermissions permissions, string entityId = null)
        {
            if (permissions.AdministratorPermissions.Any(p => _adminPermission.Contains(p)))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(entityId) && _shooterPermission.Any() && permissions.EntityPermissions.ContainsKey(entityId))
            {

                var existingPermission = permissions.EntityPermissions
                    .First(sp => sp.Key == entityId).Value;

                if (existingPermission.Any(p => _shooterPermission.Contains(p)))
                    return true;
            }

            return false;
        }
    }


    public class PermissionsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var customAuthorizationFilter = (ApiAuthorizationFilter)filterDescriptors
                .Select(filterInfo => filterInfo.Filter)
                .FirstOrDefault(filter => filter is ApiAuthorizationFilter);
            if (customAuthorizationFilter != null)
            {
                operation.Description += $"Permissions: {customAuthorizationFilter.PermissionsString}";
            }
        }
    }

    public static class PlatformUtils
    {
        #region private properties

        private static readonly string idClaims = "Id";

        #endregion
        /// <summary>
        /// Recovers user id from identity
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <returns>Return user identifier or null</returns>
        public static string GetIdentityUserId(IPrincipal principal)
        {
            //Validazione argomenti
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            //Nome dell'identity
            var identity = (ClaimsIdentity)principal.Identity;
            var userId = identity.Claims.FirstOrDefault(x => x.Type == idClaims)?.Value ?? "";
            if (string.IsNullOrEmpty(userId))
                return null;

            return userId;
        }

        /// <summary>
        /// Recovers username from identity
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <returns>Return username or null</returns>
        public static string GetIdentityUsername(IPrincipal principal)
        {
            //Validazione argomenti
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            //Nome dell'identity
            var identity = (ClaimsIdentity)principal.Identity;
            var userId = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? "";
            if (string.IsNullOrEmpty(userId))
                return null;

            return userId;
        }
    }
}
