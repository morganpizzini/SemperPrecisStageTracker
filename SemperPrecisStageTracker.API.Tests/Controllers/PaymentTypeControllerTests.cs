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
    public class PaymentTypeControllerTests : ApiControllerTestsBase<PaymentTypeController, SimpleScenario>
    {
        protected override User GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldCreatePaymentTypeBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.PaymentTypes.Count;

            var existingTeam = Scenario.Teams.FirstOrDefault();

            //Composizione della request
            var request = new PaymentTypeCreateRequest
            {
                TeamId = existingTeam.Id,
                Name = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.CreatePaymentType(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.PaymentTypes.Count;
            

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PaymentTypeContract>(response);

            var updatedEntity = Scenario.PaymentTypes.FirstOrDefault(x => x.Id == parsed.Data.PaymentTypeId);

            Assert.AreEqual(countBefore + 1,countAfter);
            Assert.IsTrue(parsed != null
                          && updatedEntity.TeamId == request.TeamId
                          && parsed.Data.Name == request.Name
            );
        }

        [TestMethod]
        public async Task ShouldCreatePaymentTypeBeOkHavingProvidedNoShooter()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.PaymentTypes.Count;

            var existingTeam = Scenario.Teams.FirstOrDefault();

            //Composizione della request
            var request = new PaymentTypeCreateRequest
            {
                TeamId = existingTeam.Id,
                Name = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.CreatePaymentType(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.PaymentTypes.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PaymentTypeContract>(response);

            var updatedEntity = Scenario.PaymentTypes.FirstOrDefault(x => x.Id == parsed.Data.PaymentTypeId);

            Assert.AreEqual(countBefore + 1,countAfter);
            Assert.IsTrue(parsed != null
                          && updatedEntity.TeamId == request.TeamId
                          && parsed.Data.Name == request.Name
            );

        }
         [TestMethod]
        public async Task ShouldNotCreateReminderAddPaymentTypeBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.PaymentTypes.Count;
            

            var existingTeam = Scenario.Teams.FirstOrDefault();

            //Composizione della request
            var request = new PaymentTypeCreateRequest
            {
                TeamId = existingTeam.Id,
                Name = RandomizationUtils.GenerateRandomString(15),
            };

            //Invoke del metodo
            var response = await Controller.CreatePaymentType(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.PaymentTypes.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PaymentTypeContract>(response);

            var updatedEntity = Scenario.PaymentTypes.FirstOrDefault(x => x.Id == parsed.Data.PaymentTypeId);

            Assert.AreEqual(countBefore + 1,countAfter);
            Assert.IsTrue(parsed != null
                          && updatedEntity.TeamId == request.TeamId
                          && parsed.Data.Name == request.Name
            );

        }

        [TestMethod]
        public async Task ShouldCreatePaymentTypeBeBadRequestHavingProvidedWrongRefIds()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.PaymentTypes.Count;

            //Composizione della request
            var request = new PaymentTypeCreateRequest
            {
                TeamId = RandomizationUtils.GenerateRandomString(15),
                Name = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.CreatePaymentType(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.PaymentTypes.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);

            Assert.AreEqual(countBefore,countAfter);
            Assert.IsTrue(parsed != null );
            Assert.AreEqual(1,parsed.Data.Count);
        }

        [TestMethod]
        public async Task ShouldUpdatePaymentTypeBeOkHavingProvidedData()
        {
            var existing = Scenario.PaymentTypes.FirstOrDefault();
            if (existing == null)
            {
                Assert.Inconclusive("No shooter team payment exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.PaymentTypes.Count;


            //Composizione della request
            var request = new PaymentTypeUpdateRequest
            {
                PaymentTypeId = existing.Id,
                TeamId = existing.TeamId,
                Name = RandomizationUtils.GenerateRandomString(5)
            };

            //Invoke del metodo
            var response = await Controller.UpdatePaymentType(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.PaymentTypes.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PaymentTypeContract>(response);

            var updatedEntity = Scenario.PaymentTypes.FirstOrDefault(x => x.Id == parsed.Data.PaymentTypeId);

            Assert.IsNotNull(parsed);
            Assert.AreEqual(countAfter, countBefore);
            Assert.IsTrue(updatedEntity.TeamId == request.TeamId
                          && parsed.Data.Name == request.Name
            );

        }

        [TestMethod]
        public async Task ShouldUpdatePaymentTypeBeBadRequestHavingProvidedWrongRefIds()
        {
            var existing = Scenario.PaymentTypes.FirstOrDefault();
            if (existing == null)
            {
                Assert.Inconclusive("No shooter team payment exists");
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.PaymentTypes.Count;


            //Composizione della request
            var request = new PaymentTypeUpdateRequest
            {
                PaymentTypeId = existing.Id,
                TeamId = RandomizationUtils.GenerateRandomString(5),
                Name = RandomizationUtils.GenerateRandomString(5)
            };

            //Invoke del metodo
            var response = await Controller.UpdatePaymentType(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.PaymentTypes.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);

            Assert.AreEqual(countAfter, countBefore);
            Assert.IsNotNull(parsed);
            Assert.AreEqual(parsed.Data.Count,1);

        }

        [TestMethod]
        public async Task ShouldDeletePaymentTypeBeOkHavingProvidedData()
        {
            var existing = Scenario.PaymentTypes.FirstOrDefault();
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.PaymentTypes.Count;

            //Composizione della request
            var request = new PaymentTypeRequest
            {
                PaymentTypeId = existing.Id
            };

            //Invoke del metodo
            var response = await Controller.DeletePaymentType(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.PaymentTypes.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);


            Assert.IsTrue(parsed != null
                          // the old one should be closed with end date
                          && countAfter == countBefore - 1
            );

        }
    }
}