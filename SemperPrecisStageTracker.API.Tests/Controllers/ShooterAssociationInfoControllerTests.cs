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
    public class ShooterAssociationInfoControllerTests : ApiControllerTestsBase<ShooterAssociationInfoController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchAllShooterAssociationInfoBeOkHavingProvidedData()
        {
            var shooterId = Scenario.ShooterAssociationInfos.FirstOrDefault()?.ShooterId;
            if (string.IsNullOrEmpty(shooterId))
            {
                Assert.Inconclusive("Shooter not found");
            }
            //conteggio esistenti generici o inseriti dall'utente
            var countBefore = Scenario.ShooterAssociationInfos.Count(x=>x.ShooterId == shooterId);

            //Invoke del metodo
            var response = await Controller.FetchShooterAssociationInfo(new ShooterRequest
            {
                ShooterId = shooterId
            });

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterAssociationInfoContract>>(response);
            Assert.AreEqual(countBefore, parsed.Data.Count);
        }

        [TestMethod]
        public async Task ShouldCreateShooterAssociationInfoBeOkHavingProvidedData()
        {
            var shooterIds = Scenario.ShooterAssociationInfos.Select(x => x.ShooterId).ToList();
            var existing = Scenario.Shooters.FirstOrDefault(x => !shooterIds.Contains(x.Id));
            if (existing == null)
            {
                Assert.Inconclusive("No shooter without association exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociationInfos.Count;

            var existingAssociation = Scenario.Associations.FirstOrDefault();

            //Composizione della request
            var request = new ShooterAssociationInfoCreateRequest
            {
                AssociationId = existingAssociation.Id,
                ShooterId = existing.Id,
                SafetyOfficier = true,
                CardNumber = RandomizationUtils.GenerateRandomString(5),
                Categories = new List<string> { existingAssociation.Categories.FirstOrDefault() }
            };

            //Invoke del metodo
            var response = await Controller.CreateShooterAssociationInfo(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociationInfos.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<ShooterAssociationInfoContract>(response);

            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && parsed != null
                          && parsed.Data.Association.AssociationId == request.AssociationId
                          && parsed.Data.Shooter.ShooterId == request.ShooterId
                          && parsed.Data.SafetyOfficier == request.SafetyOfficier
                          && parsed.Data.CardNumber == request.CardNumber
                          && parsed.Data.Categories.All(x => request.Categories.Contains(x))
            );

        }

        [TestMethod]
        public async Task ShouldFetchShooterAssociationInfoBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterAssociationInfos.FirstOrDefault();

            if (existing == null)
            {
                Assert.Inconclusive("No shooter association exists");
            }

            var count = Scenario.ShooterAssociationInfos.Count(x => x.ShooterId == existing.ShooterId);

            var request = new ShooterRequest
            {
                ShooterId = existing.ShooterId
            };

            //Invoke del metodo
            var response = await Controller.FetchShooterAssociationInfo(request);


            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterAssociationInfoContract>>(response);

            Assert.AreEqual(count, parsed.Data.Count);
            Assert.IsTrue(parsed.Data.All(x => !string.IsNullOrEmpty(x.Association.Name)));

        }
        [TestMethod]
        public async Task ShouldUpdateShooterAssociationInfoBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterAssociationInfos.FirstOrDefault();
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociationInfos.Count;

            var existingAssociation = Scenario.Associations.FirstOrDefault(x => x.Id == existing.AssociationId);

            //Composizione della request
            var request = new ShooterAssociationInfoUpdateRequest
            {
                ShooterAssociationInfoId = existing.Id,
                AssociationId = existing.AssociationId,
                ShooterId = existing.ShooterId,
                SafetyOfficier = !existing.SafetyOfficier,
                CardNumber = RandomizationUtils.GenerateRandomString(5),
                Categories = new List<string> { existingAssociation.Categories.LastOrDefault() }
            };

            //Invoke del metodo
            var response = await Controller.UpdateShooterAssociationInfo(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociationInfos.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<ShooterAssociationInfoContract>(response);

            var oldEntity = Scenario.ShooterAssociationInfos.FirstOrDefault(x => x.Id == existing.Id);

            

            Assert.AreEqual(countBefore + 1, countAfter);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Association.AssociationId == request.AssociationId
                          && parsed.Data.Shooter.ShooterId == request.ShooterId
                          && parsed.Data.SafetyOfficier == request.SafetyOfficier
                          && parsed.Data.CardNumber == request.CardNumber
                          && parsed.Data.Categories.All(x=>request.Categories.Contains(x))
            );

        }

        [TestMethod]
        public async Task ShouldUpdateShooterAssociationInfoBeBadRequestHavingProvidedWrongCategories()
        {
            var existing = Scenario.ShooterAssociationInfos.FirstOrDefault();

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociationInfos.Count;

            //Composizione della request
            var request = new ShooterAssociationInfoUpdateRequest
            {
                AssociationId = existing.AssociationId,
                ShooterId = existing.ShooterId,
                SafetyOfficier = !existing.SafetyOfficier,
                CardNumber = existing.CardNumber,
                Categories = new List<string> { RandomizationUtils.GenerateRandomString(5) }
            };

            //Invoke del metodo
            var response = await Controller.UpdateShooterAssociationInfo(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociationInfos.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);


            Assert.AreEqual(countBefore, countAfter);
            Assert.IsTrue(parsed != null &&
                          // the old one should be closed with end date
                          parsed.Data.Any()
            );

        }

        [TestMethod]
        public async Task ShouldCreateShooterAssociationInfoBeBadRequestHavingProvidedWrongCategories()
        {
            
            var shooterIds = Scenario.ShooterAssociationInfos.Select(x => x.ShooterId).ToList();
            var existing = Scenario.Shooters.FirstOrDefault(x => !shooterIds.Contains(x.Id));
            if (existing == null)
            {
                Assert.Inconclusive("No shooter without association exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociationInfos.Count;

            var existingAssociation = Scenario.Associations.FirstOrDefault();

            //Composizione della request
            var request = new ShooterAssociationInfoCreateRequest
            {
                AssociationId = existingAssociation.Id,
                ShooterId = existing.Id,
                SafetyOfficier = false,
                CardNumber = RandomizationUtils.GenerateRandomString(5),
                Categories = new List<string> { RandomizationUtils.GenerateRandomString(5) }
            };

            //Invoke del metodo
            var response = await Controller.CreateShooterAssociationInfo(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociationInfos.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);


            Assert.AreEqual(countBefore, countAfter);
            Assert.IsTrue(parsed != null &&
                          // the old one should be closed with end date
                          parsed.Data.Any()
            );

        }

        [TestMethod]
        public async Task ShouldUpdateShooterAssociationInfoBeBadRequestHavingProvidedSameCardId()
        {
            var existing = Scenario.ShooterAssociationInfos.FirstOrDefault();
            var another = Scenario.ShooterAssociationInfos.FirstOrDefault(x => x.ShooterId != existing.ShooterId && x.AssociationId == existing.AssociationId);
            if (another == null)
            {
                Assert.Inconclusive("Another shooter association not found");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociationInfos.Count;

            //Composizione della request
            var request = new ShooterAssociationInfoUpdateRequest
            {
                AssociationId = existing.AssociationId,
                ShooterId = existing.ShooterId,
                SafetyOfficier = !existing.SafetyOfficier,
                CardNumber = another.CardNumber,
            };

            //Invoke del metodo
            var response = await Controller.UpdateShooterAssociationInfo(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociationInfos.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);


            Assert.AreEqual(countBefore, countAfter);
            Assert.IsTrue(parsed != null &&
                          // the old one should be closed with end date
                          parsed.Data.Any()
            );

        }

        [TestMethod]
        public async Task ShouldDeleteShooterAssociationInfoBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterAssociationInfos.FirstOrDefault();
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterAssociationInfos.Count;

            //Composizione della request
            var request = new ShooterAssociationInfoRequest
            {
                ShooterAssociationInfoId = existing.Id
            };

            //Invoke del metodo
            var response = await Controller.DeleteShooterAssociationInfo(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterAssociationInfos.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);


            Assert.IsTrue(parsed != null
                          // the old one should be closed with end date
                          && countAfter == countBefore - 1
            );

        }
    }
}