using System;
using System.Collections.Generic;
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
    public class TeamControllerTests : ApiControllerTestsBase<TeamController, SimpleScenario>
    {
        protected override User GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchAllTeamsBeOkHavingElements()
        {
            //conteggio esistenti generici o inseriti dall'utente
            var countBefore = Scenario.Teams.Count();

            //Invoke del metodo
            var response = await Controller.FetchAllTeams();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<TeamContract>>(response);
            Assert.AreEqual(countBefore, parsed.Data.Count);
        }

        [TestMethod]
        public async Task ShouldCreateTeamBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Teams.Count;

            //Composizione della request
            var request = new TeamCreateRequest
            {
                Name = RandomizationUtils.GenerateRandomString(50)
            };

            //Invoke del metodo
            var response = await Controller.CreateTeam(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Teams.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<TeamContract>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && parsed.Data.Name == request.Name);
        }


        [TestMethod]
        public async Task ShouldCreateTeamBeBadRequestOnNameDuplicate()
        {
            //utente corrente
            var user = GetIdentityUser();

            //Recupero Team esistente
            var existing = Scenario.Teams.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Team is invalid");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Teams.Count;

            //Composizione della request
            var request = new TeamCreateRequest
            {
                Name = existing.Name
            };

            //Invoke del metodo
            var response = await Controller.CreateTeam(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Teams.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldUpdateTeamBeOkHavingProvidedData()
        {
            //Recupero una Team esistente
            var existing = Scenario.Teams.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Team does not exists");

            //conteggio esistenti
            var countBefore = Scenario.Teams.Count;


            //Composizione della request
            var request = new TeamUpdateRequest
            {
                TeamId = existing.Id,
                Name = RandomizationUtils.GenerateRandomString(50)
            };

            //Invoke del metodo
            var response = await Controller.UpdateTeam(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<TeamContract>(response);

            //conteggio esistenti
            var countAfter = Scenario.Teams.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data.Name == request.Name);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public async Task ShouldUpdateTeamBeNotFoundHavingProvidedWrongId()
        {
            //conteggio esistenti
            var countBefore = Scenario.Teams.Count;


            //Composizione della request
            var request = new TeamUpdateRequest
            {
                TeamId = RandomizationUtils.GenerateRandomString(10),
                Name = RandomizationUtils.GenerateRandomString(50)
            };

            //Invoke del metodo
            var response = await Controller.UpdateTeam(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedNotFound(response);

            //conteggio esistenti
            var countAfter = Scenario.Teams.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data == null);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public async Task ShouldUpdateTeamBeBadRequestOnNameDuplicate()
        {
            //utente corrente
            var user = GetIdentityUser();

            //Recupero team esistente
            var existing = Scenario.Teams.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("First team is invalid");

            //Recupero team esistente
            var anotherExisting = Scenario.Teams.FirstOrDefault(x => x.Id != existing.Id);
            if (anotherExisting == null)
                Assert.Inconclusive("Second team is invalid");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Teams.Count;

            //Composizione della request
            var request = new TeamUpdateRequest
            {
                TeamId = existing.Id,
                Name = anotherExisting.Name
            };

            //Invoke del metodo
            var response = await Controller.UpdateTeam(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Teams.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeleteTeamBeOkHavingDeletedElement()
        {
            //Recupero una Team esistente non utilizzato
            var existing = Scenario.Teams.FirstOrDefault();

            if (existing == null)
                Assert.Inconclusive("Team does not exists");

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Teams.Count;

            //Composizione della request
            var request = new TeamRequest { TeamId = existing.Id };

            //Invoke del metodo
            var response = await Controller.DeleteTeam(request);

            //Parsing della risposta
            var parsed = ParseExpectedOk<TeamContract>(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Teams.Count;

            Assert.IsTrue(
              parsed.Data.TeamId == existing.Id);
            Assert.AreEqual(countBefore - 1, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeleteTeamBeBadNotFoundHavingProvidedWrongId()
        {

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Teams.Count;

            //Composizione della request
            var request = new TeamRequest { TeamId = RandomizationUtils.GenerateRandomString(10) };

            //Invoke del metodo
            var response = await Controller.DeleteTeam(request);

            //Parsing della risposta
            var parsed = ParseExpectedNotFound(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Teams.Count;

            Assert.IsTrue(parsed != null &&
              parsed.Data == null);
            Assert.AreEqual(countBefore, countAfter);
        }
    }
}
