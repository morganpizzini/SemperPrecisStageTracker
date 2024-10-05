using Asp.Versioning;
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
    [ApiVersion("1.0")]
    public class AggregationController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchDataForMatch")]
        [ProducesResponseType(typeof(MatchDataAssociationContract), 200)]
        public async Task<IActionResult> FetchDataForMatch([FromBody]MatchRequest request)
        {
            var availableMatches = await BasicLayer.FetchAllSoMdMatches(PlatformUtils.GetIdentityUserId(User));

            var match = availableMatches.FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
                return NotFound();

            var groups = BasicLayer.FetchAllGroupsByMatchId(match.Id);
            if (match.CompetitionReady)
            {
                var now = DateTime.Now.Date;
                if(groups.All(x=> x.GroupDay.Date != now))
                {
                    // get the closest date
                    now = groups.OrderBy(x=>x.GroupDay).FirstOrDefault().GroupDay;
                }
                groups = groups.Where(x=>x.GroupDay.Date == now).ToList();
            }

            var stages = BasicLayer.FetchAllStagesByMatchId(match.Id);
            var stageIds = stages.Select(x => x.Id).ToList();
            var stageStrings = BasicLayer.FetchStageStringsFromStageIds(stageIds);
            var stageStringIds = stageStrings.Select(x => x.Id).ToList();


            var association = BasicLayer.GetAssociation(match.AssociationId);
            var team = BasicLayer.GetTeam(match.TeamId);
            var place = BasicLayer.GetPlace(match.PlaceId);

            //Match/FetchAllMatchDirector
            //Recupero la lista dal layer
            var matchDirectors = BasicLayer.FetchShooterMatchesByMatchId(request.MatchId);

            var shooterIds = matchDirectors.Select(x => x.UserId).ToList();

            //shooter list to fetch
            var shooterIdsSum = new List<string>(shooterIds);


            //Match/FetchAllShooterSOStages
            var shooterSoStages = BasicLayer.FetchShooterSOStagesByStageIds(stageIds);

            var stageShooterIds = shooterSoStages.Select(x => x.UserId).ToList();

            shooterIdsSum.AddRange(stageShooterIds);

            // load MDs and SOs

            var shooters = BasicLayer.FetchUsersByIds(shooterIdsSum);

            var shooterMatches = shooters.Where(x => shooterIds.Contains(x.Id)).ToList();
            var stageSoShooters = shooters.Where(x => stageShooterIds.Contains(x.Id)).ToList();

            // filter stage by user SO => SO in offline mode will see only their own stages
            var userId = PlatformUtils.GetIdentityUserId(User);

            if (shooterMatches.All(x => x.Id != userId) && !(await AuthorizationLayer.ValidateUserPermissions(PlatformUtils.GetIdentityUserId(User), PermissionCtor.ManageMatches)))
            {
                var userStages = shooterSoStages.Where(x => x.UserId == userId).Select(x => x.StageId).ToList();
                stages = stages.Where(x => userStages.Contains(x.Id)).ToList();
                stageIds = stages.Select(x => x.Id).ToList();
                stageStringIds = stageStrings.Where(x => stageIds.Contains(x.StageId)).Select(x => x.Id).ToList();
            }

            // GroupShooter/FetchGroupShooterStage

            var groupIds = groups.Select(x => x.Id).ToList();

            IList<(string groupId, IList<GroupUser> groupShooter, IList<User> shooters)> groupAggregate = new List<(string groupId, IList<GroupUser>, IList<User>)>();

            foreach (var groupId in groupIds)
            {
                var groupShooter = BasicLayer.FetchGroupShootersByGroupId(groupId);
                var shootersIds = groupShooter.Select(x => x.UserId).ToList();

                groupAggregate.Add((groupId,
                    groupShooter,
                    BasicLayer.FetchUsersByIds(shootersIds)
                    ));
            }

            var groupAggregateGroupShooters = groupAggregate.SelectMany(x => x.groupShooter).ToList();
            var groupAggregateShooters = groupAggregate.SelectMany(x => x.shooters).ToList();

            var groupShootersIds = groupAggregateShooters.Select(x => x.Id).ToList();

            var shooterStageResults = BasicLayer.FetchShootersResultsOnStageStrings(stageStringIds, groupShootersIds);

            var finalStageResults = new List<UserStageString>();

            // fill empty stages results

            foreach (var stageStringId in stageStringIds)
            {
                foreach (var shooter in groupAggregateShooters)
                {
                    var existingResult = shooterStageResults.FirstOrDefault(x => x.StageStringId == stageStringId && x.UserId == shooter.Id);
                    if (existingResult != null)
                    {
                        finalStageResults.Add(existingResult);
                        continue;
                    }
                    var currentStageString = stageStrings.FirstOrDefault(x => x.Id == stageStringId);
                    if (currentStageString == null)
                        continue;
                    finalStageResults.Add(new UserStageString
                    {
                        StageStringId = stageStringId,
                        UserId = shooter.Id,
                        StageId = currentStageString.StageId
                    });
                }
            }

            var shooterWarnings = BasicLayer.FetchShootersWarningsDisqualifiedOnStageStrings(match.Id, stageStringIds, groupShootersIds);

            //Ritorno i contratti
            return Ok(new MatchDataAssociationContract
            {
                Match = ContractUtils.GenerateContractCasting(match,team, association, place, groups.Select(x => (x, groupAggregateGroupShooters.Where(g => g.GroupId == x.Id).ToList())).ToList(), stages,stageStrings.Where(x=>stageStringIds.Contains(x.Id)).ToList()),

                ShooterMatches = matchDirectors.As(x => ContractUtils.GenerateContract(x, shooterMatches.FirstOrDefault(s => s.Id == x.UserId))),

                ShooterSoStages = shooterSoStages.As(x =>
                    ContractUtils.GenerateContract(x, stageSoShooters.FirstOrDefault(s => s.Id == x.UserId))),

                ShooterStages = finalStageResults.GroupBy(x => new { x.StageId, x.UserId }).As(x =>
                     ContractUtils.GenerateContract(
                             groupAggregateGroupShooters.FirstOrDefault(y => y.UserId == x.Key.UserId),
                             groupAggregateShooters.FirstOrDefault(y => y.Id == x.Key.UserId),
                             x.Key.StageId,
                         x.ToList(),
                         shooterWarnings.FirstOrDefault(y => y.UserId == x.Key.UserId),
                         groupAggregate.FirstOrDefault(g => g.shooters.Any(c => c.Id == x.Key.UserId)).groupId ?? string.Empty))
            });
        }

        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateDataForMatch")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> UpdateDataForMatch([FromBody]UpdateDataRequest request)
        {
            var validations = await BasicLayer.UpsertShooterStages(
                request.ShooterStages.Where(x=>x.Time>0)
                    .Select(x => new UserStageString
                    {
                        StageStringId = x.StageStringId,
                        Time = x.Time,
                        UserId = x.UserId,
                        DownPoints = x.DownPoints,
                        Procedurals = x.Procedurals,
                        HitOnNonThreat = x.HitOnNonThreat,
                        FlagrantPenalties = x.FlagrantPenalties,
                        Ftdr = x.Ftdr,
                        Warning = x.Warning,
                        Disqualified = x.Disqualified,
                        Notes = x.Notes
                    }).ToList(),
                request.EditedEntities.Select(x => (x.EntityId, x.EditDateTime)).ToList(), PlatformUtils.GetIdentityUserId(User));
            return Ok(new OkResponse()
            {
                Status = validations.Count == 0
            });
        }
        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterInformation")]
        [ProducesResponseType(typeof(ShooterInformationResponse), 200)]
        public IActionResult FetchShooterInformation([FromBody]ShooterRequest request)
        {
            var nextMatches = BasicLayer.FetchMatchRegistrationForUser(request.ShooterId);

            var associationIds = nextMatches.Item1.Select(x => x.AssociationId).ToList();
            var associations = BasicLayer.FetchAssociationsByIds(associationIds);

            var placeIds = nextMatches.Item1.Select(x => x.PlaceId).ToList();
            var places = BasicLayer.FetchPlacesByIds(placeIds);

            return Ok(new ShooterInformationResponse()
            {
                ShooterMatchInfos = nextMatches.Item1.Select(x=>new ShooterMatchInfoResponse
                {
                    Name = x.Name,
                    AssociationName = associations.FirstOrDefault(a=>a.Id== x.AssociationId)?.Name ?? string.Empty,
                    PlaceName = places.FirstOrDefault(a=>a.Id== x.PlaceId)?.Name ?? string.Empty,
                    MatchDateTimeEnd = x.MatchDateTimeEnd,
                    MatchDateTimeStart = x.MatchDateTimeStart,
                    MatchId = x.Id,
                    GroupName = nextMatches.Item2.FirstOrDefault(g=>g.MatchId == x.Id)?.Name ?? string.Empty
                }).ToList()
            });
        }
    }
}