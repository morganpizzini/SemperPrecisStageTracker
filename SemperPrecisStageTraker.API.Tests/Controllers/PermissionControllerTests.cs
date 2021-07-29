using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class PermissionControllerTests : ApiControllerTestsBase<PermissionController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchShooterPermissionBeOkHavingProvidedData()
        {
            //Invoke del metodo
            var response = await Controller.FetchAllUserPermissions();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PermissionsResponse>(response);
            Assert.IsTrue(parsed != null &&
                          parsed.Data.AdministrationPermissions.Any(x => x.Permission == nameof(AdministrationPermissions.ManageMatches)) &&
                          parsed.Data.AdministrationPermissions.Any(x => x.Permission == nameof(AdministrationPermissions.ManageShooters)) &&
                          parsed.Data.AdministrationPermissions.Any(x => x.Permission == nameof(AdministrationPermissions.ManageShooters)) &&
                          parsed.Data.AdministrationPermissions.Any(x => x.Permission == nameof(AdministrationPermissions.ManageTeams))
            );
        }

        [TestMethod]
        public async Task ShouldFetchShooterPermissionBeOkHavingProvidedDataOnAnotherUser()
        {
            UpdateIdentityUser(GetAnotherUser());
            //Invoke del metodo
            var response = await Controller.FetchAllUserPermissions();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<PermissionsResponse>(response);

            Assert.IsTrue(parsed != null &&
                          parsed.Data.EntityPermissions.Any(x => x.Permission == nameof(EntityPermissions.EditShooter) && x.EntityId == "1")
            );
        }

    }
}