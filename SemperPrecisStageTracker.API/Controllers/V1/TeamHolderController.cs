using Asp.Versioning;
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
    /// Controller for teamHolder
    /// </summary>
    [ApiVersion("1.0")]
    public class TeamHolderController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchTeamHolderByTeam")]
        [ProducesResponseType(typeof(IList<UserContract>), 200)]
        public Task<IActionResult> FetchTeamHolderByTeam([FromBody]TeamRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchTeamHoldersFromTeamId(request.TeamId);
            var shooterIds = entities.Select(x => x.UserId).ToList();
            var shooters = BasicLayer.FetchUsersByIds(shooterIds);

            //Return contract
            return Reply(entities.As(x => ContractUtils.GenerateContract(x, shooters.FirstOrDefault(t => t.Id == x.UserId))));
        }


        /// <summary>
        /// Creates a teamHolder on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertTeamHolder")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public Task<IActionResult> UpsertTeamHolder([FromBody]TeamHolderCreateRequest request)
        {
            var entity = this.BasicLayer.GetTeamHolderByTeamAndShooterId(request.TeamId, request.ShooterId);

            if (entity == null)
            {
                entity = new TeamHolder
                {
                    UserId = request.ShooterId,
                    TeamId = request.TeamId
                };
            }

            entity.Description = request.Description;

            //Invocazione del service layer
            var validations = BasicLayer.UpsertTeamHolder(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a teamHolder on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteTeamHolder")]
        [ProducesResponseType(typeof(IList<UserContract>), 200)]
        public Task<IActionResult> DeleteTeamHolder([FromBody]TeamHolderDeleteRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetTeamHolderByTeamAndShooterId(request.TeamId, request.ShooterId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());

            }
            //Invocazione del service layer
            var validations = BasicLayer.DeleteTeamHolder(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(new OkResponse { Status = true });
        }
    }
}
