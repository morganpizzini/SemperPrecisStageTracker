using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using ZenProgramming.Chakra.Core.Utilities.Data;
using SemperPrecisStageTracker.API.Models;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class BayControllerTests : ApiControllerTestsBase<SemperPrecisStageTracker.API.Controllers.V2.PlacesController, SimpleScenario>
    {
        protected override User GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchAllPlacesBeOkHavingElements()
        {
            //conteggio esistenti generici o inseriti dall'utente
            var placeId = Scenario.Bays.Select(x => x.PlaceId).FirstOrDefault();
            var countBefore = Scenario.Bays.Count(x => x.PlaceId == placeId);

            //Invoke del metodo
            var response = await Controller.Fetch(new TakeSkipBaseRequestId { Id = placeId, Skip = 0, Take = 999 });

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<BayContract>>(response);
            Assert.AreEqual(countBefore, parsed.Data.Count);
        }

        [TestMethod]
        public async Task ShouldCreateBayBeOkHavingProvidedData()
        {
            var place = Scenario.Bays.FirstOrDefault();
            if (place == null)
                Assert.Inconclusive("Place not found");
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Bays.Count;

            //Composizione della request
            var request = new EntityBaseRequestId<BayCreateRequest>
            {
                Id = place.Id,
                Body = new BayCreateRequest
                {
                    Name = RandomizationUtils.GenerateRandomString(50),
                    Description = RandomizationUtils.GenerateRandomString(15),
                }
            };

            //Invoke del metodo
            var response = await Controller.Create(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Bays.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedCreated<BayContract>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && parsed.Data.Name == request.Body.Name
                          && parsed.Data.Description == request.Body.Description);
        }


        [TestMethod]
        public async Task ShouldCreateBayBeBadRequestOnNameDuplicate()
        {
            //utente corrente
            var user = GetIdentityUser();

            //Recupero Place esistente
            var existing = Scenario.Bays.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Place is invalid");

            var anotherExisting = Scenario.Bays.FirstOrDefault(x => x.Id != existing.Id);
            if (anotherExisting == null)
                Assert.Inconclusive("Another Place is not found");
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Bays.Count;

            //Composizione della request
            var request = new EntityBaseRequestId<BayCreateRequest>
            {
                Id = existing.Id,
                Body = new BayCreateRequest
                {
                    Name = anotherExisting.Name,
                    Description = RandomizationUtils.GenerateRandomString(15),
                }
            };

            //Invoke del metodo
            var response = await Controller.Create(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Bays.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldUpdateBayBeOkHavingProvidedData()
        {
            //Recupero una Place esistente
            var existing = Scenario.Bays.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Place does not exists");

            //conteggio esistenti
            var countBefore = Scenario.Places.Count;


            //Composizione della request
            var request = new BayUpdateRequest
            {
                PlaceId = existing.Id,
                Name = RandomizationUtils.GenerateRandomString(50),
                Description = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.Update(new BayEntityBaseRequestId<BayUpdateRequest>
            {
                Id = existing.PlaceId,
                BayId = existing.Id,
                Body = request
            });

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<BayContract>(response);

            //conteggio esistenti
            var countAfter = Scenario.Bays.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data.Name == request.Name
                          && parsed.Data.Description == request.Description);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public async Task ShouldUpdateBayBeNotFoundHavingProvidedWrongId()
        {
            //conteggio esistenti
            var countBefore = Scenario.Bays.Count;


            //Composizione della request
            var request = new BayUpdateRequest
            {
                PlaceId = RandomizationUtils.GenerateRandomString(10),
                Name = RandomizationUtils.GenerateRandomString(50),
                Description = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.Update(new BayEntityBaseRequestId<BayUpdateRequest> { 
                Id = RandomizationUtils.GenerateRandomString(10),
                BayId = RandomizationUtils.GenerateRandomString(10),
                Body = request });

            //Parsing della risposta e assert
            var parsed = ParseExpectedNotFound(response);

            //conteggio esistenti
            var countAfter = Scenario.Bays.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data == null);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public async Task ShouldUpdatePlaceBeBadRequestOnNameDuplicate()
        {
            //utente corrente
            var user = GetIdentityUser();

            //Recupero place esistente
            var existing = Scenario.Bays.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("First Bay is invalid");

            //Recupero place esistente
            var anotherExisting = Scenario.Bays.FirstOrDefault(x => x.Id != existing.Id && x.PlaceId == existing.PlaceId);
            if (anotherExisting == null)
                Assert.Inconclusive("Second bay is invalid");


            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Places.Count;

            //Composizione della request
            var request = new BayUpdateRequest
            {
                PlaceId = existing.Id,
                Name = anotherExisting.Name,
                Description = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.Update(new BayEntityBaseRequestId<BayUpdateRequest> { 
                Id = existing.PlaceId,
                BayId = existing.Id,
                Body = request });

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Bays.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeletePlaceBeOkHavingDeletedElement()
        {
            //Recupero una Place esistente non utilizzato
            var existing = Scenario.Bays.FirstOrDefault();

            if (existing == null)
                Assert.Inconclusive("Bay does not exists");

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Bays.Count;

            //Invoke del metodo
            var response = await Controller.Delete(new BayEntityBaseRequestId { Id = existing.PlaceId, BayId = existing.Id });

            //Parsing della risposta
            var parsed = ParseExpectedNoContent(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Bays.Count;

            Assert.AreEqual(countBefore - 1, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeletePlaceBeBadNotFoundHavingProvidedWrongId()
        {

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Bays.Count;


            //Invoke del metodo
            var response = await Controller.Delete(new BayEntityBaseRequestId { 
                Id = RandomizationUtils.GenerateRandomString(10),
                BayId = RandomizationUtils.GenerateRandomString(10)});

            //Parsing della risposta
            var parsed = ParseExpectedNotFound(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Bays.Count;

            Assert.AreEqual(countBefore, countAfter);
        }
    }
}