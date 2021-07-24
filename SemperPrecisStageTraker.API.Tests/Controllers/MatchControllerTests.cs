using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using ZenProgramming.Chakra.Core.Utilities.Data;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class PermissionControllerTests : ApiControllerTestsBase<PermissionController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchShooterPermissionBeOkHavingProvidedData()
        {
            //Invoke del metodo
            var response = await Controller.FetchAllUserPermissions();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PermissionsResponse>(response);
            Assert.IsTrue(parsed != null &&
                          parsed.Data.AdministrationPermissions.Any(x => x.Permission == (int)AdministrationPermissions.ManageMatches) &&
                            parsed.Data.AdministrationPermissions.Any(x => x.Permission == (int)AdministrationPermissions.ManageShooters) &&
                            parsed.Data.AdministrationPermissions.Any(x => x.Permission == (int)AdministrationPermissions.ManageShooters) &&
                            parsed.Data.AdministrationPermissions.Any(x => x.Permission == (int)AdministrationPermissions.ManageTeams)
            );
        }

        [TestMethod]
        public async Task ShouldFetchShooterPermissionBeOkHavingProvidedDataOnAnotherUser()
        {
            UpdateIdentityUser(GetAnotherUser());
            //Invoke del metodo
            var response = await Controller.FetchAllUserPermissions();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PermissionsResponse>(response);

            Assert.IsTrue(parsed != null &&
                          parsed.Data.EntityPermissions.Any(x => x.Permission == (int)EntityPermissions.EditShooter && x.EntityId == "1")
            );
        }

    }


    [TestClass]
    public class ContactControllerTests : ApiControllerTestsBase<ContactController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();
        [TestMethod]
        public async Task ShouldCreateContactBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Contacts.Count;

            //Composizione della request
            var request = new ContactCreateRequest
            {
                Name = RandomizationUtils.GenerateRandomString(50),
                Token = RandomizationUtils.GenerateRandomString(50),
                Description = RandomizationUtils.GenerateRandomString(50),
                Email = RandomizationUtils.GenerateRandomEmail(),
                Subject = RandomizationUtils.GenerateRandomString(50),
                AcceptPolicy = true,
            };

            //Invoke del metodo
            var response = await Controller.CreateContact(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Contacts.Count;

            var lastInsert = Scenario.Contacts.OrderByDescending(x => x.CreationDateTime).FirstOrDefault();
            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && lastInsert != null
                          && lastInsert.Name == request.Name
                          && lastInsert.Description == request.Description
                          && lastInsert.Email == request.Email
                          && lastInsert.Subject == request.Subject
            );
        }

    }

    [TestClass]
    public class MatchControllerTests : ApiControllerTestsBase<MatchController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchAllMatchesBeOkHavingElements()
        {
            //conteggio esistenti generici o inseriti dall'utente
            var countBefore = Scenario.Matches.Count();

            //Invoke del metodo
            var response = await Controller.FetchAllMatches();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<MatchContract>>(response);
            Assert.AreEqual(countBefore, parsed.Data.Count);
        }

        [TestMethod]
        public async Task ShouldCreateMatchBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Matches.Count;

            var existingAssociation = Scenario.Associations.FirstOrDefault();
            var existingPlace = Scenario.Places.FirstOrDefault();

            //Composizione della request
            var request = new MatchCreateRequest
            {
                Name = RandomizationUtils.GenerateRandomString(50),
                AssociationId = existingAssociation.Id,
                PlaceId = existingPlace.Id,
                MatchDateTime = DateTime.Now,
                OpenMatch = true,
                UnifyClassifications = true
            };

            //Invoke del metodo
            var response = await Controller.CreateMatch(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Matches.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<MatchContract>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && parsed.Data.Name == request.Name
                          && parsed.Data.Association.AssociationId == request.AssociationId
                           && parsed.Data.Place.PlaceId == request.PlaceId
                            && parsed.Data.MatchDateTime == request.MatchDateTime
                            && parsed.Data.OpenMatch == request.OpenMatch
                            && parsed.Data.UnifyClassifications == request.UnifyClassifications
                          );
        }

        [TestMethod]
        public async Task ShouldUpdateMatchBeOkHavingProvidedData()
        {
            //Recupero una Match esistente
            var existing = Scenario.Matches.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Match does not exists");

            var existingAssociation = Scenario.Associations.FirstOrDefault(x => x.Id != existing.AssociationId);
            var existingPlace = Scenario.Places.FirstOrDefault(x => x.Id != existing.PlaceId);

            //conteggio esistenti
            var countBefore = Scenario.Matches.Count;


            //Composizione della request
            var request = new MatchUpdateRequest
            {
                MatchId = existing.Id,
                Name = RandomizationUtils.GenerateRandomString(50),
                AssociationId = existingAssociation.Id,
                PlaceId = existingPlace.Id,
                MatchDateTime = DateTime.Now,
                OpenMatch = !existing.OpenMatch,
                UnifyClassifications = !existing.UnifyClassifications
            };

            //Invoke del metodo
            var response = await Controller.UpdateMatch(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<MatchContract>(response);

            //conteggio esistenti
            var countAfter = Scenario.Matches.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data.Name == request.Name
                          && parsed.Data.Association.AssociationId == request.AssociationId
                           && parsed.Data.Place.PlaceId == request.PlaceId
                            && parsed.Data.MatchDateTime == request.MatchDateTime
                            && parsed.Data.OpenMatch == request.OpenMatch
                            && parsed.Data.UnifyClassifications == request.UnifyClassifications);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public async Task ShouldUpdateMatchBeNotFoundHavingProvidedWrongId()
        {
            //conteggio esistenti
            var countBefore = Scenario.Matches.Count;

            var existingAssociation = Scenario.Associations.FirstOrDefault();
            var existingPlace = Scenario.Places.FirstOrDefault();

            //Composizione della request
            var request = new MatchUpdateRequest
            {
                MatchId = RandomizationUtils.GenerateRandomString(10),
                Name = RandomizationUtils.GenerateRandomString(50),
                AssociationId = existingAssociation.Id,
                PlaceId = existingPlace.Id,
                MatchDateTime = DateTime.Now,
                OpenMatch = false,
                UnifyClassifications = false
            };

            //Invoke del metodo
            var response = await Controller.UpdateMatch(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedNotFound(response);

            //conteggio esistenti
            var countAfter = Scenario.Matches.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data == null);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeleteMatchBeOkHavingDeletedElement()
        {
            //Recupero una Match esistente non utilizzato
            var existing = Scenario.Matches.FirstOrDefault();

            if (existing == null)
                Assert.Inconclusive("Match does not exists");

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Matches.Count;

            //Composizione della request
            var request = new MatchRequest { MatchId = existing.Id };

            //Invoke del metodo
            var response = await Controller.DeleteMatch(request);

            //Parsing della risposta
            var parsed = ParseExpectedOk<MatchContract>(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Matches.Count;

            Assert.IsTrue(
                parsed.Data.MatchId == existing.Id);
            Assert.AreEqual(countBefore - 1, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeleteMatchBeBadNotFoundHavingProvidedWrongId()
        {

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Matches.Count;

            //Composizione della request
            var request = new MatchRequest { MatchId = RandomizationUtils.GenerateRandomString(10) };

            //Invoke del metodo
            var response = await Controller.DeleteMatch(request);

            //Parsing della risposta
            var parsed = ParseExpectedNotFound(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Matches.Count;

            Assert.IsTrue(parsed != null &&
                          parsed.Data == null);
            Assert.AreEqual(countBefore, countAfter);
        }
    }
}