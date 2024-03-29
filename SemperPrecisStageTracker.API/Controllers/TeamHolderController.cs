﻿using System.Collections.Generic;
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
    /// Controller for teamHolder
    /// </summary>
    public class TeamHolderController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchTeamHolderByTeam")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> FetchTeamHolderByTeam(TeamRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchTeamHoldersFromTeamId(request.TeamId);
            var shooterIds = entities.Select(x => x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            //Return contract
            return Reply(entities.As(x => ContractUtils.GenerateContract(x, shooters.FirstOrDefault(t => t.Id == x.ShooterId))));
        }


        /// <summary>
        /// Creates a teamHolder on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertTeamHolder")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public Task<IActionResult> UpsertTeamHolder(TeamHolderCreateRequest request)
        {
            var entity = this.BasicLayer.GetTeamHolderByTeamAndShooterId(request.TeamId, request.ShooterId);

            if (entity == null)
            {
                entity = new TeamHolder
                {
                    ShooterId = request.ShooterId,
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
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> DeleteTeamHolder(TeamHolderDeleteRequest request)
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
