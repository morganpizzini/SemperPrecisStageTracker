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
    /// Controller for groupshooter
    /// </summary>
    public class GroupShooterController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAvailableGroupShooter")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult FetchAvailableGroupShooter(GroupRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetGroup(request.GroupId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Invocazione del service layer
            var shooters = BasicLayer.FetchAvailableShooters(entity);

            //Return contract
            return Ok(shooters.As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Fetch shooter in group and stage with their score
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchGroupShooterStage")]
        [ProducesResponseType(typeof(IList<ShooterStageAggregationResult>), 200)]
        public IActionResult FetchGroupShooterStage(GroupStageRequest request)
        {
            //Invocazione del service layer
            var shooters = BasicLayer.FetchShootersByGroupId(request.GroupId);
            
            var shooterIds = shooters.Select(x => x.Id).ToList();
            var shooterStages = BasicLayer.FetchShootersResultsOnStage(request.StageId,shooterIds);

            //Return contract
            return Ok(shooters.As(x=>ContractUtils.GenerateContract(x,shooterStages.FirstOrDefault(y=>y.ShooterId == x.Id ))));
        }

        /// <summary>
        /// Creates a groupshooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertGroupShooter")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult UpsertGroupShooter(GroupShooterCreateRequest request)
        {
            var entity = this.BasicLayer.GetGroupShooterByShooterAndGroup(request.ShooterId,request.GroupId);
            
            if (entity == null){
                entity = new GroupShooter{
                    ShooterId = request.ShooterId,
                    GroupId = request.GroupId
                };
            }
            
            entity.DivisionId = request.DivisionId;
            entity.TeamId = request.TeamId;
            
            //Invocazione del service layer
            var validations = BasicLayer.UpsertGroupShooter(entity);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(this.BasicLayer.FetchShootersByGroupId(request.GroupId).As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Creates a groupshooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteGroupShooter")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult DeleteGroupShooter(GroupShooterDeleteRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetGroupShooterByShooterAndGroup(request.ShooterId,request.GroupId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }
            //Invocazione del service layer
            var validations = BasicLayer.DeleteGroupShooter(entity);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(this.BasicLayer.FetchShootersByGroupId(request.GroupId).As(ContractUtils.GenerateContract));
        }
    }
}
