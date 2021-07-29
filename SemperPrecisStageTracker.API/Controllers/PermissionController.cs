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
        public async Task<IActionResult> FetchAllUserPermissions()
        {
            var userId = PlatformUtils.GetIdentityUserId(User);
            
            //Recupero la lista dal layer
            var entities = await AuthorizationLayer.GetUserPermissionById(userId);

            //Ritorno i contratti
            return Ok(ContractUtils.GenerateContract(entities.adminPermissions,entities.entityPermissions));
        }
    }
}