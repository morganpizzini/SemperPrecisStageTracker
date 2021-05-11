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
    /// Controller for shootergroup
    /// </summary>
    public class ShooterGroupController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterGroup")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult FetchShooterGroup(GroupRequest request)
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
        [Route("FetchShooterGroupStage")]
        [ProducesResponseType(typeof(IList<ShooterStageAggregationResult>), 200)]
        public IActionResult FetchShooterGroupStage(GroupStageRequest request)
        {
            //Invocazione del service layer
            var shooters = BasicLayer.FetchShootersByGroupId(request.GroupId);
            
            var shooterIds = shooters.Select(x => x.Id).ToList();
            var shooterStages = BasicLayer.FetchShootersResultsOnStage(request.StageId,shooterIds);

            //Return contract
            return Ok(shooters.As(x=>ContractUtils.GenerateContract(x,shooterStages.FirstOrDefault(y=>y.ShooterId == x.Id ))));
        }

        /// <summary>
        /// Creates a shootergroup on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertShooterGroup")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public IActionResult UpsertShooterGroup(ShooterGroupRequest request)
        {
            //Invocazione del service layer
            var validations = BasicLayer.UpsertShootersInGroup(request.GroupId, request.ShooterIds);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse{ Status= true});
        }

        /// <summary>
        /// Creates a shootergroup on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("AddShooterGroup")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult AddShooterGroup(GroupCreateDeleteShooterRequest request)
        {
            var shooterIds = this.BasicLayer.FetchShootersByGroupId(request.GroupId).Select(x=> x.Id).ToList();

            shooterIds.Add(request.ShooterId);
            
            //Invocazione del service layer
            var validations = BasicLayer.UpsertShootersInGroup(request.GroupId, shooterIds);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(this.BasicLayer.FetchShootersByGroupId(request.GroupId).As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Creates a shootergroup on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooterGroup")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult DeleteShooterGroup(GroupCreateDeleteShooterRequest request)
        {
            var shooterIds = this.BasicLayer.FetchShootersByGroupId(request.GroupId).Select(x=> x.Id).ToList();

            shooterIds.Remove(request.ShooterId);
            
            //Invocazione del service layer
            var validations = BasicLayer.UpsertShootersInGroup(request.GroupId, shooterIds);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(this.BasicLayer.FetchShootersByGroupId(request.GroupId).As(ContractUtils.GenerateContract));
        }
    }
}
