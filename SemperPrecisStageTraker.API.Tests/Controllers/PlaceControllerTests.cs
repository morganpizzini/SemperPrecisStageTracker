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
    public class PlaceControllerTests : ApiControllerTestsBase<PlaceController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchAllPlacesBeOkHavingElements()
        {
            //conteggio esistenti generici o inseriti dall'utente
            var countBefore = Scenario.Places.Count();

            //Invoke del metodo
            var response = await Controller.FetchAllPlaces();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<PlaceContract>>(response);
            Assert.AreEqual(countBefore,parsed.Data.Count);
        }

        [TestMethod]
        public async Task ShouldCreatePlaceBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Places.Count;

            //Composizione della request
            var request = new PlaceCreateRequest
            {
                Name = RandomizationUtils.GenerateRandomString(50),
                Holder = RandomizationUtils.GenerateRandomString(15),
                Phone = RandomizationUtils.GenerateRandomString(10),
                Email = RandomizationUtils.GenerateRandomEmail(),
                Address = RandomizationUtils.GenerateRandomString(15),
                City = RandomizationUtils.GenerateRandomString(15),
                Region = RandomizationUtils.GenerateRandomString(15),
                PostalZipCode = RandomizationUtils.GenerateRandomString(15),
                Country = RandomizationUtils.GenerateRandomString(15),
            };

            //Invoke del metodo
            var response = await Controller.CreatePlace(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Places.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PlaceContract>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && parsed.Data.Name == request.Name
                          && parsed.Data.Holder == request.Holder
                          && parsed.Data.Phone == request.Phone
                          && parsed.Data.Email == request.Email
                          && parsed.Data.Address == request.Address
                          && parsed.Data.City == request.City
                          && parsed.Data.Region == request.Region
                          && parsed.Data.PostalZipCode == request.PostalZipCode
                          && parsed.Data.Country == request.Country);
        }


        [TestMethod]
        public async Task ShouldCreatePlaceBeBadRequestOnNameAndCityDuplicate()
        {
            //utente corrente
            var user = GetIdentityUser();

            //Recupero Place esistente
            var existing = Scenario.Places.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Place is invalid");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Places.Count;

            //Composizione della request
            var request = new PlaceCreateRequest
            {
                Name = existing.Name,
                City = existing.City,
                Holder = RandomizationUtils.GenerateRandomString(15),
                Email = RandomizationUtils.GenerateRandomEmail(),
                Region = RandomizationUtils.GenerateRandomString(15),
                PostalZipCode = RandomizationUtils.GenerateRandomString(15),
                Country = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.CreatePlace(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Places.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldCreatePlaceBeBadRequestOnNameAndZipDuplicate()
        {
            //utente corrente
            var user = GetIdentityUser();

            //Recupero Place esistente
            var existing = Scenario.Places.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Place is invalid");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Places.Count;

            //Composizione della request
            var request = new PlaceCreateRequest
            {
                Name = existing.Name,
                PostalZipCode = existing.PostalZipCode,
                Holder = RandomizationUtils.GenerateRandomString(15),
                Email = RandomizationUtils.GenerateRandomEmail(),
                City = RandomizationUtils.GenerateRandomString(15),
                Region = RandomizationUtils.GenerateRandomString(15),
                Country = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.CreatePlace(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Places.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldUpdatePlaceBeOkHavingProvidedData()
        {
            //Recupero una Place esistente
            var existing = Scenario.Places.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Place does not exists");

            //conteggio esistenti
            var countBefore = Scenario.Places.Count;


            //Composizione della request
            var request = new PlaceUpdateRequest
            {
                PlaceId = existing.Id,
                Name = RandomizationUtils.GenerateRandomString(50),
                Holder = RandomizationUtils.GenerateRandomString(15),
                Phone = RandomizationUtils.GenerateRandomString(10),
                Email = RandomizationUtils.GenerateRandomEmail(),
                Address = RandomizationUtils.GenerateRandomString(15),
                City = RandomizationUtils.GenerateRandomString(15),
                Region = RandomizationUtils.GenerateRandomString(15),
                PostalZipCode = RandomizationUtils.GenerateRandomString(15),
                Country = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.UpdatePlace(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PlaceContract>(response);

            //conteggio esistenti
            var countAfter = Scenario.Places.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data.Name == request.Name
                          && parsed.Data.Holder == request.Holder
                          && parsed.Data.Phone == request.Phone
                          && parsed.Data.Email == request.Email
                          && parsed.Data.Address == request.Address
                          && parsed.Data.City == request.City
                          && parsed.Data.Region == request.Region
                          && parsed.Data.PostalZipCode == request.PostalZipCode
                          && parsed.Data.Country == request.Country);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public async Task ShouldUpdatePlaceBeNotFoundHavingProvidedWrongId()
        {
            //conteggio esistenti
            var countBefore = Scenario.Places.Count;


            //Composizione della request
            var request = new PlaceUpdateRequest
            {
                PlaceId = RandomizationUtils.GenerateRandomString(10),
                Name = RandomizationUtils.GenerateRandomString(50),
                Holder = RandomizationUtils.GenerateRandomString(15),
                Email = RandomizationUtils.GenerateRandomEmail(),
                City = RandomizationUtils.GenerateRandomString(15),
                Region = RandomizationUtils.GenerateRandomString(15),
                PostalZipCode = RandomizationUtils.GenerateRandomString(15),
                Country = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.UpdatePlace(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedNotFound(response);

            //conteggio esistenti
            var countAfter = Scenario.Places.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data == null);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public async Task ShouldUpdatePlaceBeBadRequestOnNameAndCityDuplicate()
        {
            //utente corrente
            var user = GetIdentityUser();

            //Recupero place esistente
            var existing = Scenario.Places.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("First place is invalid");

            //Recupero place esistente
            var anotherExisting = Scenario.Places.FirstOrDefault(x => x.Id != existing.Id);
            if (anotherExisting == null)
                Assert.Inconclusive("Second place is invalid");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Places.Count;

            //Composizione della request
            var request = new PlaceUpdateRequest
            {
                PlaceId = existing.Id,
                Name = anotherExisting.Name,
                City = anotherExisting.City,
                Holder = RandomizationUtils.GenerateRandomString(15),
                Email = RandomizationUtils.GenerateRandomEmail(),
                Region = RandomizationUtils.GenerateRandomString(15),
                PostalZipCode = RandomizationUtils.GenerateRandomString(15),
                Country = RandomizationUtils.GenerateRandomString(15)
            };

            //Invoke del metodo
            var response = await Controller.UpdatePlace(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Places.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);

        }

        [TestMethod]
        public async Task ShouldUpdatePlaceBeBadRequestOnNameZipAndDuplicate()
        {
            //utente corrente
            var user = GetIdentityUser();

            //Recupero place esistente
            var existing = Scenario.Places.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("First place is invalid");

            //Recupero place esistente
            var anotherExisting = Scenario.Places.FirstOrDefault(x => x.Id != existing.Id);
            if (anotherExisting == null)
                Assert.Inconclusive("Second place is invalid");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Places.Count;

            //Composizione della request
            var request = new PlaceUpdateRequest
            {
                PlaceId = existing.Id,
                Name = anotherExisting.Name,
                PostalZipCode = anotherExisting.PostalZipCode,
                Holder = RandomizationUtils.GenerateRandomString(15),
                Email = RandomizationUtils.GenerateRandomEmail(),
                City = RandomizationUtils.GenerateRandomString(15),
                Region = RandomizationUtils.GenerateRandomString(15),
                Country = RandomizationUtils.GenerateRandomString(15)
                
            };

            //Invoke del metodo
            var response = await Controller.UpdatePlace(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Places.Count;

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
            var existing = Scenario.Places.FirstOrDefault();

            if (existing == null)
                Assert.Inconclusive("Place does not exists");

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Places.Count;

            //Composizione della request
            var request = new PlaceRequest { PlaceId = existing.Id };

            //Invoke del metodo
            var response = await Controller.DeletePlace(request);

            //Parsing della risposta
            var parsed = ParseExpectedOk<PlaceContract>(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Places.Count;

            Assert.IsTrue(
                parsed.Data.PlaceId == existing.Id);
            Assert.AreEqual(countBefore - 1, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeletePlaceBeBadNotFoundHavingProvidedWrongId()
        {

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Places.Count;

            //Composizione della request
            var request = new PlaceRequest { PlaceId = RandomizationUtils.GenerateRandomString(10) };

            //Invoke del metodo
            var response = await Controller.DeletePlace(request);

            //Parsing della risposta
            var parsed = ParseExpectedNotFound(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Places.Count;

            Assert.IsTrue(parsed != null &&
                          parsed.Data == null);
            Assert.AreEqual(countBefore, countAfter);
        }
    }
}