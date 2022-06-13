using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for shooterteam
    /// </summary>
    public class ShooterTeamController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterTeamByShooter")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> FetchShooterTeamByShooter(ShooterRequest request)
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
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> FetchShooterTeamByTeam(TeamRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchShootersFromTeamId(request.TeamId);
            var shooterIds = entities.Select(x => x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            //Return contract
            return Reply(entities.As(x => ContractUtils.GenerateContract(x, null, shooters.FirstOrDefault(t => t.Id == x.ShooterId))).OrderBy(x=>x.Shooter.CompleteName).ToList());

        }


        /// <summary>
        /// Creates a shooterteam on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertShooterTeam")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public Task<IActionResult> UpsertShooterTeam(ShooterTeamCreateRequest request)
        {
            var entity = this.BasicLayer.GetShooterTeamByTeamAndShooterId(request.TeamId, request.ShooterId);

            if (entity == null)
            {
                entity = new ShooterTeam
                {
                    ShooterId = request.ShooterId,
                    TeamId = request.TeamId
                };
            }

            if (request.FromShooter)
            {
                entity.ShooterApprove = true;
            }
            else
            {
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
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> DeleteShooterTeam(ShooterTeamDeleteRequest request)
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
