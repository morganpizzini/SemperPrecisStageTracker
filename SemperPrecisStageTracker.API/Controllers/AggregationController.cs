using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for association
    /// </summary>
    public class AggregationController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchDataForMatch")]
        [ProducesResponseType(typeof(MatchDataAssociationContract), 200)]
        public async Task<IActionResult> FetchDataForMatch(MatchRequest request)
        {
            var availableMatches = await BasicLayer.FetchAllSoMdMatches(PlatformUtils.GetIdentityUserId(User));
            
            var match = availableMatches.FirstOrDefault(x=> x.Id == request.MatchId);

            if (match == null)
                return NotFound();

            var groups = BasicLayer.FetchAllGroupsByMatchId(match.Id);
            var stages = BasicLayer.FetchAllStagesByMatchId(match.Id);
            //var stageIds = stages.Select(x => x.Id).ToList();
            var stageStrings = BasicLayer.FetchStageStringsFromStageIds(stageIds);
            var stageStringIds = stageStrings.Select(x => x.Id).ToList();


            var association = BasicLayer.GetAssociation(match.AssociationId);
            var place = BasicLayer.GetPlace(match.PlaceId);

            //Match/FetchAllMatchDirector
            //Recupero la lista dal layer
            var matchDirectors = BasicLayer.FetchShooterMatchesByMatchId(request.MatchId);

            var shooterIds = matchDirectors.Select(x => x.ShooterId).ToList();

            //shooter list to fetch
            var shooterIdsSum = new List<string>(shooterIds);


            //Match/FetchAllShooterSOStages
            var shooterSoStages = BasicLayer.FetchShooterSOStagesByStageIds(stageIds);

            var stageShooterIds = shooterSoStages.Select(x => x.ShooterId).ToList();

            shooterIdsSum.AddRange(stageShooterIds);

            // load MDs and SOs

            var shooters = BasicLayer.FetchShootersByIds(shooterIdsSum);

            var shooterMatches = shooters.Where(x => shooterIds.Contains(x.Id)).ToList();
            var stageSoShooters = shooters.Where(x => stageShooterIds.Contains(x.Id)).ToList();

            // filter stage by user SO => SO in offline mode will see only their own stages
            var userId = PlatformUtils.GetIdentityUserId(User);

            if (shooterMatches.All(x => x.Id != userId))
            {
                var userStages = shooterSoStages.Where(x => x.ShooterId == userId).Select(x=>x.StageId).ToList();
                stages = stages.Where(x => userStages.Contains(x.Id)).ToList();
                stageIds = stages.Select(x => x.Id).ToList();
            }

            // GroupShooter/FetchGroupShooterStage

            var groupIds = groups.Select(x => x.Id).ToList();

            IList<(string groupId, IList<GroupShooter> groupShooter, IList<Shooter> shooters)> groupAggregate = new List<(string groupId, IList<GroupShooter>, IList<Shooter>)>();

            foreach (var groupId in groupIds)
            {
                var groupShooter = BasicLayer.FetchGroupShootersByGroupId(groupId);
                var shootersIds = groupShooter.Select(x => x.ShooterId).ToList();

                groupAggregate.Add((groupId,
                    groupShooter,
                    BasicLayer.FetchShootersByIds(shootersIds)
                    ));
            }

            var groupAggregateGroupShooters = groupAggregate.SelectMany(x => x.groupShooter).ToList();
            var groupAggregateShooters = groupAggregate.SelectMany(x => x.shooters).ToList();

            var groupShootersIds = groupAggregateShooters.Select(x => x.Id).ToList();

            var shooterStageResults = BasicLayer.FetchShootersResultsOnStageStrings(stageStringIds, groupShootersIds);

            var finalStageResults = new List<ShooterStage>();

            // fill empty stages results

            foreach (var stageStringId in stageStringIds)
            {
                foreach (var shooter in groupAggregateShooters)
                {
                    var existingResult = shooterStageResults.FirstOrDefault(x => x.StageStringId == stageStringId && x.ShooterId == shooter.Id);
                    if (existingResult != null)
                    {
                        finalStageResults.Add(existingResult);
                        continue;
                    }
                    finalStageResults.Add(new ShooterStage
                    {
                        StageStringId = stageStringId,
                        ShooterId = shooter.Id
                    });
                }
            }

            var shooterWarnings = BasicLayer.FetchShootersWarningsDisqualifiedOnStageStrings(match.Id, stageStringIds, groupShootersIds);

            //Ritorno i contratti
            return Ok(new MatchDataAssociationContract
            {
                Match = ContractUtils.GenerateContractCasting(match, association, place, groups.Select(x => (x, groupAggregateGroupShooters.Where(g => g.GroupId == x.Id).ToList())).ToList(), stages),

                ShooterMatches = matchDirectors.As(x => ContractUtils.GenerateContract(x, shooterMatches.FirstOrDefault(s => s.Id == x.ShooterId))),

                ShooterSoStages = shooterSoStages.As(x =>
                    ContractUtils.GenerateContract(x, stageSoShooters.FirstOrDefault(s => s.Id == x.ShooterId))),

                ShooterStages = finalStageResults.As(x =>
                      ContractUtils.GenerateContract(
                        groupAggregateGroupShooters.FirstOrDefault(y => y.ShooterId == x.ShooterId),
                        groupAggregateShooters.FirstOrDefault(y => y.Id == x.ShooterId),
                        x,
                        shooterWarnings.FirstOrDefault(y => y.ShooterId == x.ShooterId),
                        groupAggregate.FirstOrDefault(g => g.shooters.Any(c => c.Id == x.ShooterId)).groupId ?? string.Empty))

            });
        }

        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateDataForMatch")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> UpdateDataForMatch(UpdateDataRequest request)
        {
            var validations = await BasicLayer.UpsertShooterStages(
                request.ShooterStages
                    .Select(x => new ShooterStage
                    {
                        StageStringId = x.StageStringId,
                        Time = x.Time,
                        ShooterId = x.ShooterId,
                        DownPoints = x.DownPoints,
                        Procedurals = x.Procedurals,
                        HitOnNonThreat = x.HitOnNonThreat,
                        FlagrantPenalties = x.FlagrantPenalties,
                        Ftdr = x.Ftdr,
                        Warning = x.Warning,
                        Disqualified = x.Disqualified,
                        Notes = x.Notes
                    }).ToList(),
                request.EditedEntities.Select(x => (x.EntityId, x.EditDateTime)).ToList(),PlatformUtils.GetIdentityUserId(User));
            return Ok(new OkResponse()
            {
                Status = validations.Count == 0
            });
        }
    }
}