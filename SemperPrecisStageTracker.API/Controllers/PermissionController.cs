using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for permissions
    /// </summary>
    public class PermissionController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllUserPermissions")]
        [ProducesResponseType(typeof(PermissionsResponse), 200)]
        public Task<IActionResult> FetchAllUserPermissions()
        {
            var userId = PlatformUtils.GetIdentityUserId(User);
            
            //Recupero la lista dal layer
            var entities = AuthorizationLayer.GetUserPermissionById(userId);

            //Ritorno i contratti
            return Reply(new PermissionsResponse
            {
                AdministrationPermissions = entities.adminPermissions.Select(x=>new AdministrationPermissionContract
                {
                    Permission = x
                }).ToList(),
                EntityPermissions = entities.entityPermissions.As(ContractUtils.GenerateContract)
            });
        }
    }
}