using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.API.Models;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using System.ComponentModel.DataAnnotations;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers.V2
{
    [ApiVersion("2.0")]
    public partial class TeamsController : ApiControllerBase
    {

        [HttpGet]
        [ProducesResponseType(typeof(IList<TeamContract>), 200)]
        public async Task<IActionResult> FetchAllTeams(TakeSkipRequest request)
        {
            //Recupero la lista dal layer
            var entities = await BasicLayer.FetchAllTeams(PlatformUtils.GetIdentityUserId(User));

            //Ritorno i contratti
            return Ok(new BaseResponse<IList<TeamContract>>(
                entities.As(x => ContractUtils.GenerateContract(x)),
                entities.Count,
                request.Take.HasValue ?
                    Url.Action(action: nameof(FetchAllTeams), controller: "Teams", new { take = request.Take, skip = request.Take + (request?.Skip ?? 0) }) :
                    string.Empty
            ));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TeamContract), 200)]
        public Task<IActionResult> GetTeam(BaseRequestId request)
        {
            var entity = BasicLayer.GetTeam(request.Id);
            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            var teamOwners = BasicLayer.FetchTeamHolderUsersByTeam(request.Id);
            
            return ReplyBaseResponse(ContractUtils.GenerateContract(entity, teamOwners));
        }

    }
}