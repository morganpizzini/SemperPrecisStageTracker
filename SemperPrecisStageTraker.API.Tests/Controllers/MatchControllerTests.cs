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
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using ZenProgramming.Chakra.Core.Utilities.Data;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
  [TestClass]
  public class ShooterAssociationControllerTests : ApiControllerTestsBase<ShooterAssociationController, SimpleScenario>
  {
    protected override Shooter GetIdentityUser() => GetAdminUser();

    [TestMethod]
    public async Task ShouldCreateShooterAssociationBeOkHavingProvidedData()
    {
      var shooterIds = Scenario.ShooterAssociations.Select(x => x.ShooterId).ToList();
      var existing = Scenario.Shooters.FirstOrDefault(x=>!shooterIds.Contains(x.Id));
      if(existing == null){
          Assert.Inconclusive("No shooter without association exists");
      }
      //Conteggio gli elementi prima della creazione
      var countBefore = Scenario.ShooterAssociations.Count;

      var existingAssociation = Scenario.Associations.FirstOrDefault();


      //Composizione della request
      var request = new ShooterAssociationCreateRequest
            {
                AssociationId = existingAssociation.Id,
                ShooterId = existing.Id,
                SafetyOfficier = true,
                CardNumber= RandomizationUtils.GenerateRandomString(5),
                RegistrationDate= DateTime.Now,
                Classification = existingAssociation.Classifications.FirstOrDefault()
            };

            //Invoke del metodo
            var response = await Controller.UpsertShooterAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociations.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);

      var updatedEntity = Scenario.ShooterAssociations.FirstOrDefault(x => x.AssociationId == request.AssociationId
      && x.ShooterId == request.ShooterId);

      Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && updatedEntity.AssociationId == request.AssociationId
                          && updatedEntity.ShooterId == request.ShooterId
                          && updatedEntity.SafetyOfficier ==request.SafetyOfficier
                          && updatedEntity.CardNumber==request.CardNumber
                          && updatedEntity.RegistrationDate ==request.RegistrationDate
                          && updatedEntity.Classification==request.Classification
                          );

    }
    [TestMethod]
    public async Task ShouldUpdateShooterAssociationBeOkHavingProvidedData()
    {
      var existing = Scenario.ShooterAssociations.FirstOrDefault();
      //Conteggio gli elementi prima della creazione
      var countBefore = Scenario.ShooterAssociations.Count;


      //Composizione della request
      var request = new ShooterAssociationCreateRequest
            {
                AssociationId = existing.AssociationId,
                ShooterId = existing.ShooterId,
                SafetyOfficier = !existing.SafetyOfficier,
                CardNumber= RandomizationUtils.GenerateRandomString(5),
                RegistrationDate= DateTime.Now,
                Classification = existing.Classification
            };

            //Invoke del metodo
            var response = await Controller.UpsertShooterAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociations.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);

      var updatedEntity = Scenario.ShooterAssociations.FirstOrDefault(x => x.Id == existing.Id);
      Assert.IsTrue(parsed != null
                          && countAfter == countBefore
                          && updatedEntity.AssociationId == request.AssociationId
                          && updatedEntity.ShooterId == request.ShooterId
                           && updatedEntity.SafetyOfficier ==request.SafetyOfficier
                            && updatedEntity.CardNumber==request.CardNumber
                            && updatedEntity.RegistrationDate ==request.RegistrationDate
                            && updatedEntity.Classification==request.Classification
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
            Assert.AreEqual(countBefore,parsed.Data.Count);
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
                MatchDateTime=DateTime.Now,
                OpenMatch = true,
                UnifyClassifications= true
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
                           && parsed.Data.Place.PlaceId ==request.PlaceId
                            && parsed.Data.MatchDateTime==request.MatchDateTime
                            && parsed.Data.OpenMatch ==request.OpenMatch
                            && parsed.Data.UnifyClassifications==request.UnifyClassifications
                          );
        }

        [TestMethod]
        public async Task ShouldUpdateMatchBeOkHavingProvidedData()
        {
            //Recupero una Match esistente
            var existing = Scenario.Matches.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Match does not exists");

            var existingAssociation = Scenario.Associations.FirstOrDefault(x=>x.Id!= existing.AssociationId);
            var existingPlace = Scenario.Places.FirstOrDefault(x=>x.Id!= existing.PlaceId);

            //conteggio esistenti
            var countBefore = Scenario.Matches.Count;


            //Composizione della request
            var request = new MatchUpdateRequest
            {
                MatchId = existing.Id,
                Name = RandomizationUtils.GenerateRandomString(50),
                AssociationId = existingAssociation.Id,
                PlaceId = existingPlace.Id,
                MatchDateTime=DateTime.Now,
                OpenMatch = !existing.OpenMatch,
                UnifyClassifications= !existing.UnifyClassifications
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
                           && parsed.Data.Place.PlaceId ==request.PlaceId
                            && parsed.Data.MatchDateTime==request.MatchDateTime
                            && parsed.Data.OpenMatch ==request.OpenMatch
                            && parsed.Data.UnifyClassifications==request.UnifyClassifications);

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
                MatchDateTime=DateTime.Now,
                OpenMatch = false,
                UnifyClassifications= false
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