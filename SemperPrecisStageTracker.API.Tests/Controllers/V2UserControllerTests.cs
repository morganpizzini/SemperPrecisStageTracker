using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using SemperPrecisStageTracker.API.Models;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class V2UserControllerTests : ApiControllerTestsBase<SemperPrecisStageTracker.API.Controllers.V2.UsersController, SimpleScenario>
    {
        protected override User GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchAllPlacesManagedByUserBeOkHavingElements()
        {
            //conteggio esistenti generici o inseriti dall'utente
            
            var countBefore = 0;
            //var countBefore = Scenario.Bays.Count(x => x.PlaceId == placeId);

            //Invoke del metodo
            var response = await Controller.FetchUserManagedPlaces(new BaseRequestId { Id = CurrentIdentityUser.Id });

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<BaseResponse<IList<PlaceContract>>>(response);
            Assert.AreEqual(countBefore, parsed.Data.Count);
        } 
    }
}