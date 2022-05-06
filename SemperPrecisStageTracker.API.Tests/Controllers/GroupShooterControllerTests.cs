using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class GroupShooterControllerTests : ApiControllerTestsBase<GroupShooterController, SimpleScenario>
    { 
        protected override Shooter GetIdentityUser() => GetAdminUser();
        
        [TestMethod]
        public async Task FetchAvailableGroupShooterShouldBeOkHavingElements()
        {
            var group = Scenario.Groups.FirstOrDefault();

            var request = new GroupRequest
            {
                GroupId = group.Id
            };

            //Invoke del metodo
            var response = await Controller.FetchAvailableGroupShooter(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<ShooterContract>>(response);

            var shooterClassifications = parsed.Data.SelectMany(x=>x.Classifications).Count();
            var shooterTeams = parsed.Data.SelectMany(x=>x.Teams).Count();

            Assert.AreNotEqual(0,shooterClassifications);
            Assert.AreNotEqual(0,shooterTeams);
        }

    }
}