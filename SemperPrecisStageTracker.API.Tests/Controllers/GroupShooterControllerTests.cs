using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class GroupShooterControllerTests : ApiControllerTestsBase<GroupShooterController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task FetchAvailableGroupShooterShouldBeOkHavingElements()
        {

            var group = Scenario.Groups.FirstOrDefault();

            var groupInMatchIds = Scenario.Groups.Where(x => x.MatchId == group.MatchId).Select(x => x.Id).ToList();

            var match = Scenario.Matches.FirstOrDefault(x => x.Id == group.MatchId);

            var association = Scenario.Associations.FirstOrDefault(x => x.Id == match.AssociationId);

            var shootersAlreadyInGroupIds = Scenario.GroupShooters.Where(x => groupInMatchIds.Contains(x.GroupId)).Select(x => x.ShooterId);

            var shootersInAssociationIds = Scenario.ShooterAssociations.Where(x => x.AssociationId == association.Id).Select(x => x.ShooterId).ToList();

            var shooterIds = shootersInAssociationIds.Count(x => !shootersAlreadyInGroupIds.Contains(x));

            var request = new GroupRequest
            {
                GroupId = group.Id
            };

            //Invoke del metodo
            var response = await Controller.FetchAvailableGroupShooter(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterContract>>(response);

            var shooterClassifications = parsed.Data.SelectMany(x => x.Classifications).Count();
            var shooterTeams = parsed.Data.SelectMany(x => x.Teams).Count();

            Assert.AreEqual(shooterIds, parsed.Data.Count);
            Assert.AreNotEqual(0, shooterClassifications);
            Assert.AreNotEqual(0, shooterTeams);
        }

        [TestMethod]
        public async Task UpsertGroupShooterShouldBeOkHavingElements()
        {
            var group = Scenario.Groups.FirstOrDefault();

            var groupInMatchIds = Scenario.Groups.Where(x => x.MatchId == group.MatchId).Select(x => x.Id).ToList();

            var match = Scenario.Matches.FirstOrDefault(x => x.Id == group.MatchId);

            var association = Scenario.Associations.FirstOrDefault(x => x.Id == match.AssociationId);

            var shootersAlreadyInGroupIds = Scenario.GroupShooters.Where(x => groupInMatchIds.Contains(x.GroupId)).Select(x => x.ShooterId);

            var shootersInAssociationIds = Scenario.ShooterAssociations.Where(x => x.AssociationId == association.Id).Select(x => x.ShooterId).ToList();

            var shooterId = shootersInAssociationIds.Where(x => !shootersAlreadyInGroupIds.Contains(x)).FirstOrDefault();

            var shooterAssociation = Scenario.ShooterAssociations.FirstOrDefault(x => x.AssociationId == association.Id && x.ShooterId == shooterId);

            var shooterTeam = Scenario.ShooterTeams.FirstOrDefault(x => x.ShooterId == shooterId);

            var request = new GroupShooterCreateRequest
            {
                GroupId = group.Id,
                ShooterId = shooterId,
                DivisionId = shooterAssociation.Division,
                TeamId = shooterTeam.Id
            };

            //Invoke del metodo
            var response = await Controller.UpsertGroupShooter(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<GroupShooterContract>>(response);

            Assert.IsTrue(parsed.Data.Any());
            Assert.IsNotNull(parsed.Data.FirstOrDefault(x => x.Shooter.ShooterId == request.ShooterId && x.Division == request.DivisionId));
        }

    }
}