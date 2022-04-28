using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
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
}