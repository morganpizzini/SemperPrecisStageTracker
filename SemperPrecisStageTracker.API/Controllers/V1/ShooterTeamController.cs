using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using Asp.Versioning;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for shooterteam
    /// </summary>
    [ApiVersion("1.0")]
    public class ShooterTeamController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterTeamByShooter")]
        [ProducesResponseType(typeof(IList<UserContract>), 200)]
        public Task<IActionResult> FetchShooterTeamByShooter([FromBody]ShooterRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchTeamsFromShooterId(request.ShooterId);
            var teamIds = entities.Select(x => x.TeamId).ToList();
            var teams = BasicLayer.FetchTeamsByIds(teamIds);
            //Return contract
            return Reply(entities.As(x => ContractUtils.GenerateContract(x, teams.FirstOrDefault(t => t.Id == x.TeamId))));
        }

        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterTeamByTeam")]
        [ProducesResponseType(typeof(IList<UserContract>), 200)]
        public Task<IActionResult> FetchShooterTeamByTeam([FromBody]TeamRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchShootersFromTeamId(request.TeamId);
            var shooterIds = entities.Select(x => x.UserId).ToList();
            var shooters = BasicLayer.FetchUsersByIds(shooterIds);
            var shooterData = BasicLayer.FetchUserDataByUserIds(shooterIds);
            //Return contract
            return Reply(entities.As(x => ContractUtils.GenerateContract(x, null, shooters.FirstOrDefault(t => t.Id == x.UserId),shooterData.FirstOrDefault(s=>s.Id == x.UserId))).OrderBy(x=>x.User.CompleteName).ToList());

        }


        /// <summary>
        /// Creates a shooterteam on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertShooterTeam")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public Task<IActionResult> UpsertShooterTeam([FromBody]ShooterTeamCreateRequest request)
        {
            var entity = this.BasicLayer.GetShooterTeamByTeamAndShooterId(request.TeamId, request.ShooterId);

            if (entity == null)
            {
                entity = new UserTeam
                {
                    UserId = request.ShooterId,
                    TeamId = request.TeamId
                };
            }

            if (request.FromShooter)
            {
                entity.UserApprove = true;
            }
            else
            {
                // if the user has no team, user is automatically approved
                if(BasicLayer.FetchTeamsFromShooterId(request.ShooterId).Count == 0)
                    entity.UserApprove = true;
                entity.TeamApprove = true;
            }

            entity.RegistrationDate = request.RegistrationDate;

            //Invocazione del service layer
            var validations = BasicLayer.UpsertShooterTeam(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(new OkResponse { Status = true });
        }

        /// <summary>
        /// Creates a shooterteam on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooterTeam")]
        [ProducesResponseType(typeof(IList<UserContract>), 200)]
        public Task<IActionResult> DeleteShooterTeam([FromBody]ShooterTeamDeleteRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterTeamByTeamAndShooterId(request.TeamId, request.ShooterId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());

            }
            //Invocazione del service layer
            var validations = BasicLayer.DeleteShooterTeam(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(new OkResponse { Status = true });
        }
    }
}
