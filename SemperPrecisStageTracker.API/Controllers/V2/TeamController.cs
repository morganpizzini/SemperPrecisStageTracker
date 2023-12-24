using System.Collections.Generic;
using Asp.Versioning;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;

using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.API.Controllers.V2;

/// <summary>
/// Controller for TeamReminder
/// </summary>
[ApiVersion("2.0")]
public class TeamReminderController : ApiControllerBase
{
    /// <summary>
    /// Fetch list of all TeamReminders
    /// </summary>
    /// <returns>Returns action result</returns>
    [HttpPost]
    [Route("FetchAllTeamReminders")]
    [ApiAuthorizationFilter(Permissions.TeamEditPayment)]
    [ProducesResponseType(typeof(IList<TeamReminderContract>), 200)]
    public async Task<IActionResult> FetchAllTeamReminders(TeamRequest request)
    {
        //Recupero la lista dal layer
        var entities = await BasicLayer.FetchAllTeamReminders(request.TeamId, PlatformUtils.GetIdentityUserId(User));

        //Ritorno i contratti
        return Ok(entities.As(x => ContractUtils.GenerateContract(x)));
    } 
}