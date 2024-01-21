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
    public class ShooterAssociationControllerTests : ApiControllerTestsBase<ShooterAssociationController, SimpleScenario>
    {
        protected override User GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldCreateShooterAssociationBeOkHavingProvidedData()
        {
            // select a shooter with shooterassociationInfo but withtout any classification
            var match = Scenario.ShooterAssociationInfos.Select(x => new { x.UserId, x.AssociationId })
                        .GroupJoin(Scenario.ShooterAssociations,x=>x,x=> new { x.UserId, x.AssociationId },
                        (x,y)=>new
                        {
                            x,
                            match = y.Any()
                        }).FirstOrDefault(x=>!x.match)?.x;

            if (match == null)
            {
                Assert.Inconclusive("No shooter without association exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociations.Count;

            var existingAssociation = Scenario.Associations.FirstOrDefault(x=>x.Id == match.AssociationId);

            //Composizione della request
            var request = new UserAssociationCreateRequest
            {
                AssociationId = existingAssociation.Id,
                UserId = match.UserId,
                RegistrationDate = DateTime.Now,
                Classification = existingAssociation.Classifications.FirstOrDefault(),
                Division = existingAssociation.Divisions.FirstOrDefault()
            };

            //Invoke del metodo
            var response = await Controller.UpsertShooterAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociations.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);

            var updatedEntity = Scenario.ShooterAssociations.FirstOrDefault(x => x.AssociationId == request.AssociationId && x.UserId == request.UserId);

            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && updatedEntity != null
                          && updatedEntity.AssociationId == request.AssociationId
                          && updatedEntity.UserId == request.UserId
                          && updatedEntity.RegistrationDate == request.RegistrationDate
                          && updatedEntity.Classification == request.Classification
                          && updatedEntity.Division == request.Division
            );

        }

        [TestMethod]
        public async Task ShouldFetchShooterAssociationBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterAssociations.FirstOrDefault();

            if (existing == null)
            {
                Assert.Inconclusive("No shooter association exists");
            }

            var count = Scenario.ShooterAssociations.Count(x => x.UserId == existing.UserId);

            var request = new ShooterRequest
            {
                ShooterId = existing.UserId
            };

            //Invoke del metodo
            var response = await Controller.FetchShooterAssociation(request);


            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<UserAssociationContract>>(response);

            Assert.AreEqual(count, parsed.Data.Count);
            Assert.IsTrue(parsed.Data.All(x => !string.IsNullOrEmpty(x.Association.Name)));

        }


        [TestMethod]
        public async Task ShouldUpdateShooterAssociationBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterAssociations.FirstOrDefault();
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociations.Count;

            //Composizione della request
            var request = new UserAssociationCreateRequest
            {
                AssociationId = existing.AssociationId,
                UserId = existing.UserId,
                RegistrationDate = DateTime.Now,
                Classification = existing.Classification,
                Division = existing.Division
            };

            //Invoke del metodo
            var response = await Controller.UpsertShooterAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociations.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);

            var oldEntity = Scenario.ShooterAssociations.FirstOrDefault(x => x.Id == existing.Id);

            var updatedEntity = Scenario.ShooterAssociations.FirstOrDefault(x => x.AssociationId == request.AssociationId && x.UserId == request.UserId && !x.ExpireDate.HasValue);

            Assert.AreEqual(countBefore + 1, countAfter);
            Assert.IsTrue(parsed != null
                          // the old one should be closed with end date
                          && oldEntity.ExpireDate != null
                          && updatedEntity.AssociationId == request.AssociationId
                          && updatedEntity.UserId == request.UserId
                          && updatedEntity.RegistrationDate == request.RegistrationDate
                          && updatedEntity.Classification == request.Classification
                          && updatedEntity.Division == request.Division
            );
        }

        [TestMethod]
        public async Task ShouldCreateShooterAssociationBeBadRequestHaavingProvidedWrongAssociation()
        {
            var existing = Scenario.Shooters.FirstOrDefault();
            
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociations.Count;

            //Composizione della request
            var request = new UserAssociationCreateRequest
            {
                AssociationId = RandomizationUtils.GenerateRandomString(5),
                UserId = existing.Id,
                RegistrationDate = DateTime.Now,
                Classification = RandomizationUtils.GenerateRandomString(5),
                Division = RandomizationUtils.GenerateRandomString(5)
            };

            //Invoke del metodo
            var response = await Controller.UpsertShooterAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociations.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);

            Assert.AreEqual(countBefore, countAfter);
            Assert.IsTrue(parsed != null
                            && parsed.Data.Count > 0);
        }

        [TestMethod]
        public async Task ShouldDeleteShooterAssociationBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterAssociations.FirstOrDefault();
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociations.Count;

            //Composizione della request
            var request = new ShooterAssociationRequest
            {
                ShooterAssociationId = existing.Id
            };

            //Invoke del metodo
            var response = await Controller.DeleteShooterAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociations.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);


            Assert.IsTrue(parsed != null
                          // the old one should be closed with end date
                          && countAfter == countBefore - 1
            );

        }
    }
}