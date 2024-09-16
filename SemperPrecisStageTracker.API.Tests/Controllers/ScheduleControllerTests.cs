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
using System;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class ScheduleControllerTests : ApiControllerTestsBase<SemperPrecisStageTracker.API.Controllers.V2.SchedulesController, SimpleScenario>
    {
        protected override User GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchAllSchedulesBeOkHavingElements()
        {
            //conteggio esistenti generici o inseriti dall'utente
            var placeId = Scenario.Schedules.Select(x=>x.PlaceId).FirstOrDefault();
            var countBefore = Scenario.Schedules.Count(x => x.PlaceId == placeId);

            //Invoke del metodo
            var response = await Controller.Fetch(new EntityTakeSkipRequest { RefId= placeId, Skip = null, Take = null});

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<BaseResponse<IList<ScheduleContract>>>(response);
            Assert.AreEqual(countBefore, parsed.Data.Count);
        }

        [TestMethod]
        public async Task ShouldCreateScheduleBeOkHavingProvidedData()
        {
            var place = Scenario.Schedules.FirstOrDefault();
            if(place==null)
                Assert.Inconclusive("Place not found");
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Schedules.Count;

            //Composizione della request
            var request = new ScheduleCreateRequest
            {
                Name = RandomizationUtils.GenerateRandomString(50),
                Description = RandomizationUtils.GenerateRandomString(15),
                PlaceId = place.PlaceId,
                From = TimeOnly.FromDateTime(RandomizationUtils.GetRandomDate()),
                To = TimeOnly.FromDateTime(RandomizationUtils.GetRandomDate()),
                Day = RandomizationUtils.GetRandomDate().DayOfWeek
            };

            //Invoke del metodo
            var response = await Controller.Create(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Schedules.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedCreated<ScheduleContract>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && parsed.Data.Name == request.Name
                          && parsed.Data.From == request.From
                          && parsed.Data.To == request.To
                          && parsed.Data.Description == request.Description
                          && parsed.Data.Day == request.Day);
        }


        [TestMethod]
        public async Task ShouldCreateScheduleBeBadRequestOnNameAndTimeDuplicate()
        {
            //utente corrente
            var user = GetIdentityUser();

            //Recupero Place esistente
            var existing = Scenario.Schedules.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Place is invalid");


            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Schedules.Count;

            //Composizione della request
            var request = new ScheduleCreateRequest
            {
                Name = existing.Name,
                Description = RandomizationUtils.GenerateRandomString(15),
                PlaceId = existing.PlaceId,
                From = existing.From,
                To = existing.To,
                Day = existing.Day
            };

            //Invoke del metodo
            var response = await Controller.Create(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Schedules.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldUpdateScheduleBeOkHavingProvidedData()
        {
            //Recupero una Place esistente
            var existing = Scenario.Schedules.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Place does not exists");

            //conteggio esistenti
            var countBefore = Scenario.Schedules.Count;


            //Composizione della request
            var request = new ScheduleUpdateRequest
            {
                PlaceId = existing.Id,
                Name = RandomizationUtils.GenerateRandomString(50),
                Description = RandomizationUtils.GenerateRandomString(15),
                From = TimeOnly.FromDateTime(RandomizationUtils.GetRandomDate()),
                To = TimeOnly.FromDateTime(RandomizationUtils.GetRandomDate()),
                Day = RandomizationUtils.GetRandomDate().DayOfWeek
            };

            //Invoke del metodo
            var response = await Controller.Update(new BaseRequestId<ScheduleUpdateRequest> {Id = existing.Id, Body = request });

            //Parsing della risposta e assert
            var parsed = ParseExpectedNoContent(response);

            var entityAfter = Scenario.Schedules.FirstOrDefault(x => x.Id == existing.Id);
            //conteggio esistenti
            var countAfter = Scenario.Schedules.Count;


            Assert.IsTrue(entityAfter.Name == request.Name
                          && entityAfter.From == request.From
                          && entityAfter.To == request.To
                          && entityAfter.Day == request.Day
                          && entityAfter.Description == request.Description);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public async Task ShouldUpdatePlaceBeNotFoundHavingProvidedWrongId()
        {
            //conteggio esistenti
            var countBefore = Scenario.Schedules.Count;


            //Composizione della request
            var request = new ScheduleUpdateRequest
            {
                PlaceId = RandomizationUtils.GenerateRandomString(10),
                Name = RandomizationUtils.GenerateRandomString(50),
                Description = RandomizationUtils.GenerateRandomString(15),
                From = TimeOnly.FromDateTime(RandomizationUtils.GetRandomDate()),
                To = TimeOnly.FromDateTime(RandomizationUtils.GetRandomDate())
            };

            //Invoke del metodo
            var response = await Controller.Update(new BaseRequestId<ScheduleUpdateRequest> { Id = RandomizationUtils.GenerateRandomString(10), Body = request });

            //Parsing della risposta e assert
            var parsed = ParseExpectedNotFound(response);

            //conteggio esistenti
            var countAfter = Scenario.Schedules.Count;


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

            //Recupero schedule esistente
            var existing = Scenario.Schedules.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("First Schedule is invalid");

            //Recupero schedule esistente
            var anotherExisting = Scenario.Schedules.FirstOrDefault(x => x.Id != existing.Id && x.PlaceId == existing.PlaceId);
            if (anotherExisting == null)
                Assert.Inconclusive("Second schedule is invalid");


            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Places.Count;

            //Composizione della request
            var request = new ScheduleUpdateRequest
            {
                PlaceId = existing.Id,
                Name = anotherExisting.Name,
                Description = RandomizationUtils.GenerateRandomString(15),
                From = anotherExisting.From,
                To = anotherExisting.To,
                Day = anotherExisting.Day
            };

            //Invoke del metodo
            var response = await Controller.Update(new BaseRequestId<ScheduleUpdateRequest> { Id = existing.Id, Body = request });

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Schedules.Count;

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
            var existing = Scenario.Schedules.FirstOrDefault();

            if (existing == null)
                Assert.Inconclusive("Schedule does not exists");

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Schedules.Count;

            //Composizione della request
            var request = new DeleteEntityRefRequest { RefId = existing.PlaceId };

            //Invoke del metodo
            var response = await Controller.Delete(new BaseRequestId<DeleteEntityRefRequest> {Id = existing.Id, Body = request });

            //Parsing della risposta
            var parsed = ParseExpectedNoContent(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Schedules.Count;

            Assert.AreEqual(countBefore - 1, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeletePlaceBeBadNotFoundHavingProvidedWrongId()
        {

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Schedules.Count;

            //Composizione della request
            var request = new DeleteEntityRefRequest { RefId = RandomizationUtils.GenerateRandomString(10) };

            //Invoke del metodo
            var response = await Controller.Delete(new BaseRequestId<DeleteEntityRefRequest> { Id = RandomizationUtils.GenerateRandomString(10), Body = request });

            //Parsing della risposta
            var parsed = ParseExpectedNotFound(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Schedules.Count;

            Assert.AreEqual(countBefore, countAfter);
        }
    }
}