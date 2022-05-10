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
    public class ShooterTeamPaymentControllerTests : ApiControllerTestsBase<ShooterTeamPaymentController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldCreateShooterTeamPaymentBeOkHavingProvidedData()
        {
            var shooterIds = Scenario.ShooterTeamPayments.Select(x => x.ShooterId).ToList();
            var existing = Scenario.Shooters.FirstOrDefault(x => !shooterIds.Contains(x.Id));
            if (existing == null)
            {
                Assert.Inconclusive("No shooter team payment without association exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterTeamPayments.Count;

            var existingTeam = Scenario.Teams.FirstOrDefault();

            //Composizione della request
            var request = new ShooterTeamPaymentCreateRequest
            {
                TeamId = existingTeam.Id,
                ShooterId = existing.Id,
                Reason = RandomizationUtils.GenerateRandomString(15),
                Amount = 1,
                PaymentDateTime = RandomizationUtils.GetRandomDate()
            };

            //Invoke del metodo
            var response = await Controller.CreateShooterTeamPayment(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterTeamPayments.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<ShooterTeamPaymentContract>(response);

            var updatedEntity = Scenario.ShooterTeamPayments.FirstOrDefault(x => x.Id == parsed.Data.ShooterTeamPaymentId);

            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && updatedEntity.TeamId == request.TeamId
                          && updatedEntity.ShooterId == request.ShooterId
                          && updatedEntity.Amount == request.Amount
                          && updatedEntity.Reason == request.Reason
                          && updatedEntity.PaymentDateTime == request.PaymentDateTime
                          && updatedEntity.ExpireDateTime == request.ExpireDateTime
                          && updatedEntity.NotifyExpiration == request.NotifyExpiration
            );

        }

        [TestMethod]
        public async Task ShouldUpdateShooterTeamPaymentBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterTeamPayments.FirstOrDefault();
            if (existing == null)
            {
                Assert.Inconclusive("No shooter team payment exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterTeamPayments.Count;


            //Composizione della request
            var request = new ShooterTeamPaymentUpdateRequest
            {
                ShooterTeamPaymentId = existing.Id,
                TeamId = existing.TeamId,
                ShooterId = existing.ShooterId,
                Reason = RandomizationUtils.GenerateRandomString(5),
                Amount = 1,
                PaymentDateTime = existing.PaymentDateTime
            };

            //Invoke del metodo
            var response = await Controller.UpdateShooterTeamPayment(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterTeamPayments.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<ShooterTeamPaymentContract>(response);

            var updatedEntity = Scenario.ShooterTeamPayments.FirstOrDefault(x => x.Id == parsed.Data.ShooterTeamPaymentId);

            Assert.IsNotNull(parsed);
            Assert.AreEqual(countAfter, countBefore);
            Assert.IsTrue(updatedEntity.TeamId == request.TeamId
                          && updatedEntity.ShooterId == request.ShooterId
                          && updatedEntity.Reason == request.Reason
                          && updatedEntity.PaymentDateTime == request.PaymentDateTime
                          && updatedEntity.Amount == request.Amount
                          && updatedEntity.ExpireDateTime == request.ExpireDateTime
                          && updatedEntity.NotifyExpiration == request.NotifyExpiration
            );

        }

        [TestMethod]
        public async Task ShouldDeleteShooterTeamPaymentBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterTeamPayments.FirstOrDefault();
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterTeamPayments.Count;

            //Composizione della request
            var request = new ShooterTeamPaymentRequest
            {
                ShooterTeamPaymentId = existing.Id
            };

            //Invoke del metodo
            var response = await Controller.DeleteShooterTeamPayment(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterTeamPayments.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);


            Assert.IsTrue(parsed != null
                          // the old one should be closed with end date
                          && countAfter == countBefore - 1
            );

        }
    }
}