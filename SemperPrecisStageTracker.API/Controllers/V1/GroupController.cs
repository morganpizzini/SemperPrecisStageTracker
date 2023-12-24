using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using Asp.Versioning;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for group
    /// </summary>
    [ApiVersion("1.0")]
    public class GroupController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all groups
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllGroupsByMatchId")]
        [ProducesResponseType(typeof(IList<GroupContract>), 200)]
        public Task<IActionResult> FetchAllGroups(MatchRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllGroupsByMatchId(request.MatchId);

            //Ritorno i contratti
            return Reply(entities.As(x => ContractUtils.GenerateContract(x)));
        }

        /// <summary>
        /// Fetch list of all groups
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllGroupsWithDetailsByMatchId")]
        [ProducesResponseType(typeof(MatchGroupResponse), 200)]
        public Task<IActionResult> FetchAllGroupsWithDetailsByMatchId(MatchRequest request)
        {
            //Recupero la lista dal layer
            var match = BasicLayer.GetMatch(request.MatchId);
            if (match == null)
                return Reply(NotFound());

            var association = BasicLayer.GetAssociation(match.AssociationId);

            var entities = BasicLayer.FetchAllGroupsByMatchId(match.Id);

            var groupIds = entities.Select(x => x.Id).ToList();
            //var shooters = BasicLayer.FetchShootersByGroupId(entity.Id);
            var shooterGroup = BasicLayer.FetchGroupShootersByGroupIds(groupIds);

            var shooterWithNoGroup = BasicLayer.FetchGroupShootersWithoutGroupByMatchIds(match.Id);
            
            var shooterIds = shooterGroup.Select(x => x.ShooterId)
                .Concat(shooterWithNoGroup.Select(x => x.ShooterId).ToList()).ToList();

            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            var teamIds = shooterGroup.Select(x => x.TeamId).Concat(shooterWithNoGroup.Select(x=>x.TeamId)).ToList();
            var teams = BasicLayer.FetchTeamsByIds(teamIds);

            //Ritorno i contratti
            return Reply(
                new MatchGroupResponse
                {
                    Match = ContractUtils.GenerateContract(match,null,association),
                    Groups = entities.As(x =>
                    {
                        var groupShooter = shooterGroup.Where(e => e.GroupId == x.Id).ToList();
                        var sIds = groupShooter.Select(x => x.ShooterId).ToList();
                        return ContractUtils.GenerateContract(x, null, null, null,
                            groupShooter, shooters.Where(s => sIds.Contains(s.Id)).ToList(),null,null,teams);
                    }),
                    UnGrouped = shooterWithNoGroup.As(x=>ContractUtils.GenerateContract(x,shooters.FirstOrDefault(s=>s.Id == x.ShooterId),null,teams.FirstOrDefault(t=>t.Id == x.TeamId)))
                });
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetGroup")]
        [ProducesResponseType(typeof(GroupContract), 200)]
        public Task<IActionResult> GetGroup(GroupRequest request)
        {
            var entity = BasicLayer.GetGroup(request.GroupId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //var shooters = BasicLayer.FetchShootersByGroupId(entity.Id);
            var shooterGroup = BasicLayer.FetchGroupShootersByGroupId(entity.Id);

            var shooterIds = shooterGroup.Select(x => x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            var match = BasicLayer.GetMatch(entity.MatchId);
            var association = BasicLayer.GetAssociation(match.AssociationId);
            
            var shooterAssociation = BasicLayer.FetchShooterAssociationByShooterIds(shooterIds, entity.MatchId);
            var shooterTeams = BasicLayer.FetchTeamsFromShooterIds(shooterIds);

            var teamsIds = shooterTeams.Select(x => x.TeamId).ToList();
            var teams = BasicLayer.FetchTeamsByIds(teamsIds);

            var result = ContractUtils.GenerateContract(entity, match, association, null, shooterGroup, shooters,
                shooterAssociation, shooterTeams, teams);
            //Serializzazione e conferma
            return Reply(result);
        }

        /// <summary>
        /// Creates a group on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateGroup")]
        [ProducesResponseType(typeof(GroupContract), 200)]
        public Task<IActionResult> CreateGroup(GroupCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Group
            {
                Name = request.Name,
                MatchId = request.MatchId,
                Description = request.Description,
                GroupDay = request.GroupDay,
                MaxShooterNumber = request.MaxShooterNumber,
                Index = request.Index
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateGroup(model);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateGroup")]
        [ProducesResponseType(typeof(GroupContract), 200)]
        public Task<IActionResult> UpdateGroup(GroupUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetGroup(request.GroupId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.GroupDay = request.GroupDay;
            entity.MaxShooterNumber = request.MaxShooterNumber;
            entity.Index = request.Index;

            //Salvataggio
            var validations = BasicLayer.UpdateGroup(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);


            //Confermo
            return Reply(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing group on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteGroup")]
        [ProducesResponseType(typeof(GroupContract), 200)]
        public Task<IActionResult> DeleteGroup(GroupRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetGroup(request.GroupId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteGroup(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(ContractUtils.GenerateContract(entity));
        }
    }
}
