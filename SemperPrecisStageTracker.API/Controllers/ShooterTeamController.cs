using System.Collections.Generic;
using System.Linq;
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
        public IActionResult FetchShooterTeamByShooter(ShooterRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchTeamsFromShooterId(request.ShooterId);
            var teamIds = entities.Select(x=>x.TeamId).ToList();
            var teams = BasicLayer.FetchTeamsByIds(teamIds);
            //Return contract
            return Ok(entities.As(x=>ContractUtils.GenerateContract(x,teams.FirstOrDefault(t=>t.Id== x.TeamId))));
        }

        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterTeamByTeam")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult FetchShooterTeamByTeam(TeamRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchShootersFromTeamId(request.TeamId);
            var shooterIds = entities.Select(x=>x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);
            
            //Return contract
            return Ok(entities.As(x=>ContractUtils.GenerateContract(x,null,shooters.FirstOrDefault(t=>t.Id== x.ShooterId))));

        }

        
        /// <summary>
        /// Creates a shooterteam on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertShooterTeam")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public IActionResult UpsertShooterTeam(ShooterTeamCreateRequest request)
        {
            var entity = this.BasicLayer.GetShooterTeamByTeamAndShooterId(request.TeamId,request.ShooterId);
            
            if (entity == null){
                entity = new ShooterTeam{
                    ShooterId = request.ShooterId,
                    TeamId = request.TeamId
                };
            }
            
            entity.RegistrationDate = request.RegistrationDate;
            
            //Invocazione del service layer
            var validations = BasicLayer.UpsertShooterTeam(entity);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse{Status= true});
        }

        /// <summary>
        /// Creates a shooterteam on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooterTeam")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult DeleteShooterTeam(ShooterTeamDeleteRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterTeamByTeamAndShooterId(request.TeamId,request.ShooterId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }
            //Invocazione del service layer
            var validations = BasicLayer.DeleteShooterTeam(entity);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse{Status= true});
        }
    }
}
