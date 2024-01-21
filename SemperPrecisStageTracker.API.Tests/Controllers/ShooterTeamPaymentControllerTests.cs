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
        protected override User GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldCreateShooterTeamPaymentBeOkHavingProvidedData()
        {
            var shooterIds = Scenario.ShooterTeamPayments.Select(x => x.UserId).ToList();
            var existing = Scenario.Shooters.FirstOrDefault(x => !shooterIds.Contains(x.Id));
            if (existing == null)
            {
                Assert.Inconclusive("No shooter team payment without association exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterTeamPayments.Count;
            var countBeforeReminder = Scenario.TeamReminders.Count;

            var existingTeam = Scenario.Teams.FirstOrDefault();
            var existingPaymentType = Scenario.PaymentTypes.FirstOrDefault(x=>x.TeamId == existingTeam.Id);

            if (existingPaymentType == null)
            {
                Assert.Inconclusive("No payment type found");
            }

            //Composizione della request
            var request = new ShooterTeamPaymentCreateRequest
            {
                TeamId = existingTeam.Id,
                ShooterId = existing.Id,
                Reason = RandomizationUtils.GenerateRandomString(15),
                Amount = 1,
                PaymentTypeId = existingPaymentType.Id,
                PaymentDateTime = RandomizationUtils.GetRandomDate(),
                ExpireDateTime = RandomizationUtils.GetRandomDate(),
                NotifyExpiration = true
            };

            //Invoke del metodo
            var response = await Controller.CreateShooterTeamPayment(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterTeamPayments.Count;
            var countAfterReminder = Scenario.TeamReminders.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserTeamPaymentContract>(response);

            var updatedEntity = Scenario.ShooterTeamPayments.FirstOrDefault(x => x.Id == parsed.Data.UserTeamPaymentId);

            Assert.AreEqual(countBefore + 1,countAfter);
            Assert.AreEqual(countBeforeReminder + 1,countAfterReminder);
            Assert.IsTrue(parsed != null
                          && updatedEntity.TeamId == request.TeamId
                          && updatedEntity.UserId == request.ShooterId
                          && parsed.Data.Amount == request.Amount
                          && parsed.Data.PaymentType == existingPaymentType.Name
                          && parsed.Data.Reason == request.Reason
                          && parsed.Data.PaymentDateTime == request.PaymentDateTime
            );
        }

        [TestMethod]
        public async Task ShouldCreateShooterTeamPaymentBeOkHavingProvidedNoShooter()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterTeamPayments.Count;

            var existingTeam = Scenario.Teams.FirstOrDefault();
            var existingPaymentType = Scenario.PaymentTypes.FirstOrDefault(x=>x.TeamId == existingTeam.Id);

            if (existingPaymentType == null)
            {
                Assert.Inconclusive("No payment type found");
            }
            //Composizione della request
            var request = new ShooterTeamPaymentCreateRequest
            {
                TeamId = existingTeam.Id,
                Reason = RandomizationUtils.GenerateRandomString(15),
                Amount = 1,
                PaymentTypeId = existingPaymentType.Id,
                PaymentDateTime = RandomizationUtils.GetRandomDate(),
            };

            //Invoke del metodo
            var response = await Controller.CreateShooterTeamPayment(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterTeamPayments.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserTeamPaymentContract>(response);

            var updatedEntity = Scenario.ShooterTeamPayments.FirstOrDefault(x => x.Id == parsed.Data.UserTeamPaymentId);

            Assert.AreEqual(countBefore + 1,countAfter);
            Assert.IsTrue(parsed != null
                          && updatedEntity.TeamId == request.TeamId
                          && updatedEntity.UserId == request.ShooterId
                          && parsed.Data.Amount == request.Amount
                          && parsed.Data.Reason == request.Reason
                          && parsed.Data.PaymentType == existingPaymentType.Name
                          && parsed.Data.PaymentDateTime == request.PaymentDateTime
            );

        }
         [TestMethod]
        public async Task ShouldNotCreateReminderAddShooterTeamPaymentBeOkHavingProvidedData()
        {
            var shooterIds = Scenario.ShooterTeamPayments.Select(x => x.UserId).ToList();
            var existing = Scenario.Shooters.FirstOrDefault(x => !shooterIds.Contains(x.Id));
            if (existing == null)
            {
                Assert.Inconclusive("No shooter team payment without association exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterTeamPayments.Count;
            var countBeforeReminder = Scenario.TeamReminders.Count;

            var existingTeam = Scenario.Teams.FirstOrDefault();
            var existingPaymentType = Scenario.PaymentTypes.FirstOrDefault(x=>x.TeamId == existingTeam.Id);

            if (existingPaymentType == null)
            {
                Assert.Inconclusive("No payment type found");
            }
            //Composizione della request
            var request = new ShooterTeamPaymentCreateRequest
            {
                TeamId = existingTeam.Id,
                ShooterId = existing.Id,
                Reason = RandomizationUtils.GenerateRandomString(15),
                PaymentTypeId = existingPaymentType.Id,
                Amount = 1,
                PaymentDateTime = RandomizationUtils.GetRandomDate()
            };

            //Invoke del metodo
            var response = await Controller.CreateShooterTeamPayment(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterTeamPayments.Count;
            var countAfterReminder = Scenario.TeamReminders.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserTeamPaymentContract>(response);

            var updatedEntity = Scenario.ShooterTeamPayments.FirstOrDefault(x => x.Id == parsed.Data.UserTeamPaymentId);

            Assert.AreEqual(countBefore + 1,countAfter);
            Assert.AreEqual(countBeforeReminder,countAfterReminder);
            Assert.IsTrue(parsed != null
                          && updatedEntity.TeamId == request.TeamId
                          && updatedEntity.UserId == request.ShooterId
                          && parsed.Data.Amount == request.Amount
                          && parsed.Data.Reason == request.Reason
                          && parsed.Data.PaymentType == existingPaymentType.Name
                          && parsed.Data.PaymentDateTime == request.PaymentDateTime
            );

        }

        [TestMethod]
        public async Task ShouldCreateShooterTeamPaymentBeBadRequestHavingProvidedWrongRefIds()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.ShooterTeamPayments.Count;
            var countBeforeReminder = Scenario.TeamReminders.Count;

            //Composizione della request
            var request = new ShooterTeamPaymentCreateRequest
            {
                TeamId = RandomizationUtils.GenerateRandomString(15),
                ShooterId = RandomizationUtils.GenerateRandomString(15),
                Reason = RandomizationUtils.GenerateRandomString(15),
                PaymentTypeId = RandomizationUtils.GenerateRandomString(15),
                Amount = 1,
                PaymentDateTime = RandomizationUtils.GetRandomDate()
            };

            //Invoke del metodo
            var response = await Controller.CreateShooterTeamPayment(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterTeamPayments.Count;
            var countAfterReminder = Scenario.TeamReminders.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);

            Assert.AreEqual(countBefore,countAfter);
            Assert.AreEqual(countBeforeReminder,countAfterReminder);
            Assert.IsTrue(parsed != null );
            Assert.AreEqual(3,parsed.Data.Count);
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

            var existingPaymentType = Scenario.PaymentTypes.FirstOrDefault(x=>x.TeamId == existing.TeamId && x.Name != existing.PaymentType);

            if (existingPaymentType == null)
            {
                Assert.Inconclusive("No payment type found");
            }
            //Composizione della request
            var request = new ShooterTeamPaymentUpdateRequest
            {
                ShooterTeamPaymentId = existing.Id,
                TeamId = existing.TeamId,
                ShooterId = existing.UserId,
                PaymentTypeId = existingPaymentType.Id,
                Reason = RandomizationUtils.GenerateRandomString(5),
                Amount = 1,
                PaymentDateTime = existing.PaymentDateTime
            };

            //Invoke del metodo
            var response = await Controller.UpdateShooterTeamPayment(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterTeamPayments.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserTeamPaymentContract>(response);

            var updatedEntity = Scenario.ShooterTeamPayments.FirstOrDefault(x => x.Id == parsed.Data.UserTeamPaymentId);

            Assert.IsNotNull(parsed);
            Assert.AreEqual(countAfter, countBefore);
            Assert.IsTrue(updatedEntity.TeamId == request.TeamId
                          && updatedEntity.UserId == request.ShooterId
                          && parsed.Data.Reason == request.Reason
                          && parsed.Data.PaymentDateTime == request.PaymentDateTime
                          && parsed.Data.PaymentType == existingPaymentType.Name
                          && parsed.Data.Amount == request.Amount
            );

        }

        [TestMethod]
        public async Task ShouldUpdateShooterTeamPaymentBeBadRequestHavingProvidedWrongRefIds()
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
                TeamId = RandomizationUtils.GenerateRandomString(5),
                ShooterId = RandomizationUtils.GenerateRandomString(5),
                Reason = RandomizationUtils.GenerateRandomString(5),
                PaymentTypeId = RandomizationUtils.GenerateRandomString(5),
                Amount = 1,
                PaymentDateTime = existing.PaymentDateTime
            };

            //Invoke del metodo
            var response = await Controller.UpdateShooterTeamPayment(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.ShooterTeamPayments.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);

            Assert.AreEqual(countAfter, countBefore);
            Assert.IsNotNull(parsed);
            Assert.AreEqual(parsed.Data.Count,3);

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