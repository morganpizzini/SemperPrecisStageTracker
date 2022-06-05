using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using System.Threading.Tasks;
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
        public Task<IActionResult> FetchAvailableGroupShooter(GroupRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetGroup(request.GroupId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Invocazione del service layer
            var shooters = BasicLayer.FetchAvailableShooters(entity);

            var shooterIds = shooters.Select(s => s.Id).ToList();

            var shooterAssociation = BasicLayer.FetchShooterAssociationByShooterIds(shooterIds, entity.MatchId);
            var shooterTeams = BasicLayer.FetchTeamsFromShooterIds(shooterIds);

            var teamsIds = shooterTeams.Select(x => x.TeamId).ToList();
            var teams = BasicLayer.FetchTeamsByIds(teamsIds);

            var result = shooters.As(x => ContractUtils.GenerateContract(x,null,
                shooterAssociation.Where(s => s.ShooterId == x.Id).ToList(),
                teams.Where(s => shooterTeams.Where(st => st.ShooterId == x.Id).Select(st => st.TeamId).Contains(s.Id))
                    .ToList()));
            //Return contract
            return Reply(result);
        }

        /// <summary>
        /// Fetch shooter in group and stage with their score
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchGroupShooterStage")]
        [ProducesResponseType(typeof(IList<ShooterStageAggregationResult>), 200)]
        public Task<IActionResult> FetchGroupShooterStage(GroupStageRequest request)
        {
            //Invocazione del service layer
            var groupShooter = BasicLayer.FetchGroupShootersByGroupId(request.GroupId);
            var shooterIds = groupShooter.Select(x => x.ShooterId).ToList();

            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            var shooterStages = BasicLayer.FetchShootersResultsOnStage(request.StageId, shooterIds);

            var shooterWarning = BasicLayer.FetchShootersWarningsDisqualifiedOnStage(request.StageId, shooterIds);

            //Return contract
            return Reply(groupShooter.As(x => ContractUtils.GenerateContract(x,
                shooters.FirstOrDefault(y => y.Id == x.ShooterId),
                shooterStages.FirstOrDefault(y => y.ShooterId == x.ShooterId),
                shooterWarning.FirstOrDefault(y => y.ShooterId == x.ShooterId))));
        }

        /// <summary>
        /// Creates a groupshooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("MoveGroupShooter")]
        [ProducesResponseType(typeof(IList<GroupShooterContract>), 200)]
        public Task<IActionResult> MoveGroupShooter(ShooterGroupMoveRequest request)
        {
            var entity = this.BasicLayer.GetGroupShooterById(request.GroupShooterId);

            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }

            var group = this.BasicLayer.GetGroup(request.GroupId);


            if (group == null)
            {
                return Task.FromResult(BadRequest(new List<ValidationResult>() { new($"Group {request.GroupId} not found")}));
            }
            var oldGroupId = entity.GroupId;


            entity.GroupId = request.GroupId;

            // Invocazione del service layer
            var validations = BasicLayer.UpsertGroupShooter(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            var match = BasicLayer.GetMatch(group.MatchId);
            // Return contract
            return Reply(GetGroupShooterContractByGroupId(oldGroupId, match.Id));
        }
        /// <summary>
        /// Creates a groupshooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertGroupShooter")]
        [ProducesResponseType(typeof(IList<GroupShooterContract>), 200)]
        public Task<IActionResult> UpsertGroupShooter(GroupShooterCreateRequest request)
        {
            var entity = this.BasicLayer.GetGroupShooterByShooterAndGroup(request.ShooterId, request.GroupId);

            if (entity == null)
            {
                entity = new GroupShooter
                {
                    ShooterId = request.ShooterId,
                    GroupId = request.GroupId,
                    HasPay = request.HasPay
                };
            }

            var group = this.BasicLayer.GetGroup(entity.GroupId);
            if (group == null)
            {
                return Task.FromResult(BadRequest(new List<ValidationResult>
                {
                    new ("Group not found")
                }));
            }

            var match = BasicLayer.GetMatch(group.MatchId);

            var shooterAssociations = BasicLayer.FetchShooterAssociationByShooterId(entity.ShooterId, match.Id);

            var currentShooterAssociation = shooterAssociations.FirstOrDefault(x => x.Division == request.DivisionId && x.ExpireDate == null);

            if (currentShooterAssociation == null)
            {
                return Task.FromResult(BadRequest(new List<ValidationResult>
                {
                    new ($"Shooter has no {request.DivisionId} division")
                }));
            }

            entity.DivisionId = currentShooterAssociation.Division;
            
            entity.Classification = currentShooterAssociation.Classification;
            entity.TeamId = request.TeamId;
            entity.HasPay = request.HasPay;

            // Invocazione del service layer
            var validations = BasicLayer.UpsertGroupShooter(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            // Return contract
            return Reply(GetGroupShooterContractByGroupId(entity.GroupId,match.Id));
        }

        /// <summary>
        /// Creates a groupshooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteGroupShooter")]
        [ProducesResponseType(typeof(IList<GroupShooterContract>), 200)]
        public Task<IActionResult> DeleteGroupShooter(GroupShooterRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetGroupShooterById(request.GroupShooterId);
            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }
            var groupId = entity.GroupId;
            //Invocazione del service layer
            var validations = BasicLayer.DeleteGroupShooter(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(GetGroupShooterContractByGroupId(groupId));
        }

        private IList<GroupShooterContract> GetGroupShooterContractByGroupId(string groupId,string matchId = null)
        {
            var shooterGroup = BasicLayer.FetchGroupShootersByGroupId(groupId);

            if (shooterGroup.Count==0)
                return new List<GroupShooterContract>();

            var shooterIds = shooterGroup.Select(x => x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);


            if (string.IsNullOrEmpty(matchId))
                matchId = BasicLayer.GetGroup(groupId)?.MatchId;
            
            if (string.IsNullOrEmpty(matchId))
                return new List<GroupShooterContract>();
            
            var shooterAssociation = BasicLayer.FetchShooterAssociationByShooterIds(shooterIds, matchId);
            var shooterTeams = BasicLayer.FetchTeamsFromShooterIds(shooterIds);

            var teamsIds = shooterTeams.Select(x => x.TeamId).ToList();
            var teams = BasicLayer.FetchTeamsByIds(teamsIds);
            
            return shooterGroup.As(x => ContractUtils.GenerateContract(x, 
                    shooters.FirstOrDefault(s => s.Id == x.ShooterId),
                    null,
                    teams.FirstOrDefault(t=>t.Id== x.TeamId),
                    shooterAssociation?.Where(s=>s.ShooterId == x.ShooterId).ToList(),
                    teams.Where(s => shooterTeams.Where(st => st.ShooterId == x.ShooterId).Select(st => st.TeamId).Contains(s.Id)).ToList())
                )
                .OrderBy(x => x.Shooter.LastName).ToList();
        }
    }
}
