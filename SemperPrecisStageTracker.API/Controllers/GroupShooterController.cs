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
using SemperPrecisStageTracker.Shared.Permissions;
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

            var result = shooters.As(x => ContractUtils.GenerateContract(x, null,
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

            var shooterStageStrings = BasicLayer.FetchShootersResultsOnStage(request.StageId, shooterIds);

            var shooterWarning = BasicLayer.FetchShootersWarningsDisqualifiedOnStage(request.StageId, shooterIds);

            //Return contract
            return Reply(groupShooter.As(x => ContractUtils.GenerateContract(x,
                shooters.FirstOrDefault(y => y.Id == x.ShooterId),
                request.StageId,
                shooterStageStrings.Where(y => y.ShooterId == x.ShooterId).ToList(),
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
                return Task.FromResult(BadRequest(new List<ValidationResult>() { new($"Group {request.GroupId} not found") }));
            }

            var shooterGroup = BasicLayer.FetchGroupShootersByGroupId(request.GroupId).Count;

            if (shooterGroup >= group.MaxShooterNumber)
            {
                return Task.FromResult(BadRequest(new List<ValidationResult>() { new($"Group {group.Name} is full") }));
            }

            var oldGroupId = entity.GroupId;


            entity.GroupId = request.GroupId;

            // Invocazione del service layer
            var validations = BasicLayer.UpsertGroupShooter(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);
            
            if (string.IsNullOrEmpty(oldGroupId))
            {
                return Reply(new OkResponse() { Status = true });
            }

            var match = BasicLayer.GetMatch(group.MatchId);

           

            // Return contract
            return Reply(GetGroupShooterContractByGroupId(oldGroupId, match.Id));
        }

        /// <summary>
        /// Creates a groupshooter after shooter request to join a match
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("AddGroupShooter")]
        [ProducesResponseType(typeof(IList<OkResponse>), 200)]
        public IActionResult AddGroupShooter(MatchShooterCreateRequest request)
        {
            // check entities
            var existingMatch = this.BasicLayer.GetMatch(request.MatchId);
            if (existingMatch == null)
                return BadRequest(new ValidationResult("Match not found").AsList());

            var existingShooter = this.BasicLayer.GetShooter(request.ShooterId);
            if (existingShooter == null)
                return BadRequest(new ValidationResult("Shooter not found").AsList());

            var entity = new GroupShooter
            {
                ShooterId = request.ShooterId,
                TeamId = request.TeamId,
                MatchId = request.MatchId
            };
            var validations = ValidateGroupShooter(entity, request.MatchId, request.DivisionId);

            if (validations.Count > 0)
                return BadRequest(validations);

            // Invocazione del service layer
            validations = BasicLayer.UpsertGroupShooter(entity);

            if (validations.Count > 0)
                return BadRequest(validations);


            return Ok(new OkResponse
            {
                Status = true
            });
        }

        /// <summary>
        /// Creates a groupshooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertGroupShooter")]
        [ProducesResponseType(typeof(IList<GroupShooterContract>), 200)]
        public async Task<IActionResult> UpsertGroupShooter([EntityId] GroupShooterCreateRequest request)
        {
            var entity = this.BasicLayer.GetGroupShooterByShooterAndGroup(request.ShooterId, request.GroupId);

            if (entity == null)
            {
                entity = new GroupShooter
                {
                    ShooterId = request.ShooterId,
                    GroupId = request.GroupId,
                    MatchId = request.MatchId,
                    HasPay = request.HasPay
                };
            }

            var group = this.BasicLayer.GetGroup(entity.GroupId);
            if (group == null)
            {
                return BadRequest(new List<ValidationResult>
                {
                    new ("Group not found")
                });
            }

            // check if group is full
            var shooterGroup = BasicLayer.FetchGroupShootersByGroupId(request.GroupId);

            if (shooterGroup.All(x=> x.ShooterId != entity.ShooterId) && shooterGroup.Count >= group.MaxShooterNumber)
            {
                return BadRequest(new List<ValidationResult>() { new($"Group {group.Name} is full") });
            }


            entity.TeamId = request.TeamId;
            entity.HasPay = request.HasPay;

            //check user permission
            var canEdit = await AuthorizationLayer.ValidateUserPermissions(PlatformUtils.GetIdentityUserId(User), group.MatchId,
                PermissionCtor.ManageMatches.MatchManageGroups.EditMatch);

            if (!canEdit)
            {
                return BadRequest(new List<ValidationResult>
                {
                    new ("User not allowed")
                });
            }

            var validations = ValidateGroupShooter(entity, group.MatchId, request.DivisionId);

            if (validations.Count > 0)
                return BadRequest(validations);

            

            // Invocazione del service layer
            validations = BasicLayer.UpsertGroupShooter(entity);

            if (validations.Count > 0)
                return BadRequest(validations);

            if (string.IsNullOrEmpty(entity.GroupId))
            {
                return Ok(new OkResponse() { Status = true });
            }
            // Return contract
            return Ok(GetGroupShooterContractByGroupId(entity.GroupId, group.MatchId));
        }


        private IList<ValidationResult> ValidateGroupShooter(GroupShooter entity, string matchId,string divisionId)
        {
            var match = BasicLayer.GetMatch(matchId);
            var association = BasicLayer.GetAssociation(match.AssociationId);

            var shooterAssociations = BasicLayer.FetchShooterAssociationByShooterId(entity.ShooterId, match.Id);
            if (match.OpenMatch)
            {
                var currentShooterAssociation = shooterAssociations.FirstOrDefault(x => x.AssociationId == match.AssociationId && x.Division == divisionId && x.ExpireDate == null);

                if (currentShooterAssociation == null)
                {
                    // open classification
                    if (association.Divisions.All(x => x != divisionId))
                    {
                        return new List<ValidationResult>
                        {
                            new ($"Division {divisionId} are not updated with current association specs")
                        };
                    }
                    entity.DivisionId = divisionId;
                }
                else
                {
                    // check association match division and classification
                    if (association.Classifications.All(x=>x!=currentShooterAssociation.Classification) ||
                        association.Divisions.All(x=>x!=currentShooterAssociation.Division) )
                    {
                        // register shooter in association that race for a division without classification
                        if (association.Divisions.All(x => x != divisionId))
                        {
                            return new List<ValidationResult>
                            {
                                new ($"Division {divisionId} are not updated with current association specs")
                            };
                        }
                        entity.DivisionId = divisionId;
                    }
                    else
                    {
                        entity.DivisionId = currentShooterAssociation.Division;
                        entity.Classification = currentShooterAssociation.Classification;
                    }
                }
            }
            else
            {
                if (match.UnifyClassifications)
                {
                    var currentShooterAssociation = shooterAssociations.FirstOrDefault(x => x.AssociationId == match.AssociationId && x.ExpireDate == null);
                    if (currentShooterAssociation == null)
                    {
                        return new List<ValidationResult>
                        {
                            new ("Shooter has no valid registration for match association")
                        };
                    }

                    // check association match division and classification
                    if (association.Divisions.All(x => x != divisionId))
                    {
                        return new List<ValidationResult>
                    {
                        new ($"Division {divisionId} are not updated with current association specs")
                    };
                    }
                    entity.DivisionId = divisionId;
                }
                else
                {
                    // default
                    var currentShooterAssociation = shooterAssociations.FirstOrDefault(x => x.AssociationId == match.AssociationId && x.Division == divisionId && x.ExpireDate == null);

                    if (currentShooterAssociation == null)
                    {
                        return new List<ValidationResult>
                                {
                                    new ($"Shooter has no valid registration for match association or  {divisionId} division")
                                };
                    }

                    // check association match division and classification
                    if (association.Classifications.All(x => x != currentShooterAssociation.Classification) ||
                        association.Divisions.All(x => x != currentShooterAssociation.Division))
                    {
                        return new List<ValidationResult>
                    {
                        new ($"Classification {currentShooterAssociation.Classification} or  {currentShooterAssociation.Division} Division are not updated with current association specs")
                    };
                    }

                    entity.DivisionId = currentShooterAssociation.Division;
                    entity.Classification = currentShooterAssociation.Classification;

                }

            }

            return new List<ValidationResult>();
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

            if (string.IsNullOrEmpty(groupId))
            {
                return Reply(new OkResponse() { Status = true });
            }
            //Return contract
            return Reply(GetGroupShooterContractByGroupId(groupId));
        }

        private IList<GroupShooterContract> GetGroupShooterContractByGroupId(string groupId, string matchId = null)
        {
            var shooterGroup = BasicLayer.FetchGroupShootersByGroupId(groupId);

            if (shooterGroup.Count == 0)
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
                    teams.FirstOrDefault(t => t.Id == x.TeamId),
                    shooterAssociation?.Where(s => s.ShooterId == x.ShooterId).ToList(),
                    teams.Where(s => shooterTeams.Where(st => st.ShooterId == x.ShooterId).Select(st => st.TeamId).Contains(s.Id)).ToList())
                )
                .OrderBy(x => x.Shooter.LastName).ToList();
        }
    }
}
