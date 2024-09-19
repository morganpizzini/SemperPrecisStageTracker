using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using SemperPrecisStageTracker.API.Models;
using ZenProgramming.Chakra.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;

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
