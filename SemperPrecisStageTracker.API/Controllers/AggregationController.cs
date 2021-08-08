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
        public Task<IActionResult> FetchDataForMatch(MatchRequest request)
        {
            //Match/GetMatch
            var match = BasicLayer.GetMatch(request.MatchId);
            if (match == null)
                return Task.FromResult<IActionResult>(NotFound());

            var groups = BasicLayer.FetchAllGroupsByMatchId(match.Id);
            var stages = BasicLayer.FetchAllStagesByMatchId(match.Id);

            var association = BasicLayer.GetAssociation(match.AssociationId);
            var place = BasicLayer.GetPlace(match.PlaceId);

            //Match/FetchAllMatchDirector
            //Recupero la lista dal layer
            var matchDirectors = BasicLayer.FetchShooterMatchesByMatchId(request.MatchId);

            var shooterIds = matchDirectors.Select(x => x.ShooterId).ToList();

            //shooter list to fetch
            var shooterIdsSum = new List<string>(shooterIds);

            var stageIds = stages.Select(x => x.Id).ToList();

            //Match/FetchAllShooterSOStages
            var shooterSoStages = BasicLayer.FetchShooterSOStagesByStageIds(stageIds);

            var stageShooterIds = shooterSoStages.Select(x => x.ShooterId).ToList();

            shooterIdsSum.AddRange(stageShooterIds);

            // GroupShooter/FetchGroupShooterStage

            var groupIds = groups.Select(x => x.Id).ToList();

            //var groupShootersIds = BasicLayer.FetchShootersIdsByGroupIds(groupIds);
            IList<(string groupId, IList<Shooter> shooters)> groupAggregate = new List<(string groupId, IList<Shooter>)>();
            foreach (var groupId in groupIds)
            {
                groupAggregate.Add((groupId, BasicLayer.FetchShootersByGroupId(groupId)));
            }

            var groupAggregateShooters = groupAggregate.SelectMany(x => x.shooters).ToList();

            var groupShootersIds = groupAggregateShooters.Select(x => x.Id).ToList();


            var shooterStages = BasicLayer.FetchShootersResultsOnStages(stageIds, groupShootersIds);

            var shooterWarning = BasicLayer.FetchShootersWarningsDisqualifiedOnStages(stageIds, groupShootersIds);

            // load shooters

            var shooters = BasicLayer.FetchShootersByIds(shooterIdsSum);

            var shooterMatches = shooters.Where(x => shooterIds.Contains(x.Id));
            var stageShooters = shooters.Where(x => stageShooterIds.Contains(x.Id));
            //var groupShooters = shooters.Where(x => groupShootersIds.Contains(x.Id));

            //Ritorno i contratti
            return Reply(new MatchDataAssociationContract
            {
                Match = ContractUtils.GenerateContract(match, association, place, groups, stages),

                ShooterMatchs = matchDirectors.As(x => ContractUtils.GenerateContract(x, shooterMatches.FirstOrDefault(s => s.Id == x.ShooterId))),

                ShooterSoStages = shooterSoStages.As(x =>
                    ContractUtils.GenerateContract(x, stageShooters.FirstOrDefault(s => s.Id == x.ShooterId))),

                ShooterStages = groupAggregateShooters.As(x =>
                      ContractUtils.GenerateContract(x,
                          shooterStages.FirstOrDefault(y => y.ShooterId == x.Id),
                                    shooterWarning.FirstOrDefault(y => y.ShooterId == x.Id),
                                    groupAggregate.FirstOrDefault(g => g.shooters.Any(c => c.Id == x.Id)).groupId ?? string.Empty))
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
                        StageId = x.StageId,
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
                request.EditedEntities.Select(x => (x.EntityId, x.EditDateTime)).ToList());
            return Ok(new OkResponse()
            {
                Status = validations.Count == 0
            });
        }
    }
}