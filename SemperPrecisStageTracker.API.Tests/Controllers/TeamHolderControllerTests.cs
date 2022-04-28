using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using ZenProgramming.Chakra.Core.Utilities.Data;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class TeamHolderControllerTests : ApiControllerTestsBase<TeamHolderController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldCreateTeamHolderBeOkHavingProvidedData()
        {
            var shooterIds = Scenario.TeamHolders.Select(x => x.ShooterId).ToList();
            var existing = Scenario.Shooters.FirstOrDefault(x=>!shooterIds.Contains(x.Id));
            if(existing == null){
                Assert.Inconclusive("No shooter without association exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.TeamHolders.Count;

            var existingTeam = Scenario.Teams.FirstOrDefault();

            //Composizione della request
            var request = new TeamHolderCreateRequest
            {
                TeamId = existingTeam.Id,
                ShooterId = existing.Id,
                Description = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.UpsertTeamHolder(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.TeamHolders.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<TeamHolderContract>(response);

            var updatedEntity = Scenario.TeamHolders.FirstOrDefault(x => x.Id == parsed.Data.TeamHolderId);

            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && updatedEntity.TeamId == request.TeamId
                          && updatedEntity.ShooterId == request.ShooterId
                          && updatedEntity.Description ==request.Description
            );

        }
        
        [TestMethod]
        public async Task ShouldUpdateTeamHolderBeOkHavingProvidedData()
        {
            var existing = Scenario.TeamHolders.FirstOrDefault();
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.TeamHolders.Count;

            //Composizione della request
            var request = new TeamHolderCreateRequest
            {
                TeamId = existing.TeamId,
                ShooterId = existing.ShooterId,
                Description= RandomizationUtils.GenerateRandomString(5)
            };

            //Invoke del metodo
            var response = await Controller.UpsertTeamHolder(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.TeamHolders.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<TeamHolderContract>(response);

            var updatedEntity = Scenario.TeamHolders.FirstOrDefault(x => x.Id == parsed.Data.TeamHolderId);
            Assert.IsNotNull(parsed);
            Assert.AreEqual(countAfter,countBefore);
            Assert.IsTrue(updatedEntity.TeamId == request.TeamId
                          && updatedEntity.ShooterId == request.ShooterId
                          && updatedEntity.Description ==request.Description
            );

        }

        [TestMethod]
        public async Task ShouldDeleteTeamHolderBeOkHavingProvidedData()
        {
            var existing = Scenario.TeamHolders.FirstOrDefault();
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.TeamHolders.Count;

            //Composizione della request
            var request = new TeamHolderDeleteRequest
            {
                ShooterId = existing.ShooterId,
                TeamId = existing.TeamId
            };

            //Invoke del metodo
            var response = await Controller.DeleteTeamHolder(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.TeamHolders.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);

            
            Assert.IsTrue(parsed != null
                            // the old one should be closed with end date
                          && countAfter == countBefore -1
            );

        }
    }
}