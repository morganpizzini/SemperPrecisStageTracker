using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.API.Tests.Utils;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using ZenProgramming.Chakra.Core.Utilities.Data;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class AggregationControllerTests : ApiControllerTestsBase<AggregationController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();
        
        [TestMethod]
        public async Task ShouldUpdateDataForMatchBeOkHavingProvidedData()
        { 
            var existing = Scenario.Matches.FirstOrDefault();

            var association = Scenario.Associations.FirstOrDefault(x=>x.Id == existing.AssociationId);

            var stageIds = Scenario.Stages.Where(x=>x.MatchId == existing.Id).Select(x=>x.Id).ToList();

            // all string in match
            var stageStrings = Scenario.StageStrings.Where(x=> stageIds.Contains(x.StageId)).ToList();
            var stageStringIds = stageStrings.Select(x=>x.Id).ToList();

            // all shooter result on stage strings
            var shooterResult = Scenario.ShooterStages.Where(x=>stageStringIds.Contains(x.StageStringId)).ToList();

            var groupIds = Scenario.Groups.Where(x=>x.MatchId == existing.Id).Select(x=>x.Id).ToList();

            var groupShooters = Scenario.GroupShooters.Where(x=>groupIds.Contains(x.GroupId)).ToList();
            var shootersIds = groupShooters.Select(x=>x.ShooterId).ToList();
            
            var stringWithNoResult = stageStrings.FirstOrDefault(x=> shooterResult.All(y=>y.StageStringId != x.Id));

            if(stringWithNoResult == null)
            {
                Assert.Inconclusive("No string with no results found");
                return;
            }

            var request = new UpdateDataRequest();

            for (int i = 0; i < 3; i++)
            {
                request.ShooterStages.Add(new ShooterStageStringContract()
                {
                    StageStringId = stringWithNoResult.Id,
                    ShooterId = shootersIds[i],
                    Time = RandomSeed.Next(150),
                    DownPoints = ListExtensions.Repeated(()=>RandomSeed.Next(20), stringWithNoResult.Targets),
                    Procedurals = association.FirstProceduralPointDown > 0 ? RandomSeed.Next(20) : 0,
                    HitOnNonThreat = association.SecondProceduralPointDown > 0 ? RandomSeed.Next(5) : 0,
                    FlagrantPenalties = association.ThirdProceduralPointDown > 0 ? RandomSeed.Next(3) : 0,
                    Ftdr = RandomSeed.Next(1),
                    Warning = false,
                    Disqualified = false,
                    Notes = RandomizationUtils.GenerateRandomString(10)
                });
            }
            var response = await Controller.UpdateDataForMatch(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);

            var countAfter = Scenario.ShooterStages.Where(x=>stageStringIds.Contains(x.StageStringId)).Count();

            Assert.IsTrue(parsed.Data.Status);
            Assert.AreEqual(shooterResult.Count + request.ShooterStages.Count, countAfter);
        }
    }
}