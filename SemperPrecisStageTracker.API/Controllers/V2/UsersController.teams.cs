using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.API.Models;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers.V2;

/// <summary>
/// Controller for Users
/// </summary>
public partial class UsersController : ApiControllerBase
{
    [HttpGet("{id}/users-in-team")]
    [ProducesResponseType(typeof(UserContract), 200)]
    public IActionResult FetchUsersInTeam(BaseRequestId request)
    {
        var users = BasicLayer.FetchUsersOnTeamBasedOnTeamHolder(request.Id);

        return OkBaseResponse(users.As(x=>ContractUtils.GenerateContract(x)));
    }
}
