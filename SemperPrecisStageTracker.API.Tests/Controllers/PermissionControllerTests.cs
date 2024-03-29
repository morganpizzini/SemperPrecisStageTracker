﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.Contracts;
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
            var response = await Controller.FetchAllPermissionsOnUser();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserPermissionContract>(response);
            Assert.IsTrue(parsed != null &&
                          parsed.Data.GenericPermissions.Any(x => x == Permissions.ManageMatches) &&
                          parsed.Data.GenericPermissions.Any(x => x == Permissions.ManageShooters) &&
                          parsed.Data.GenericPermissions.Any(x => x == Permissions.ManageShooters) &&
                          parsed.Data.GenericPermissions.Any(x => x == Permissions.ManageTeams)
            );
        }

        [TestMethod]
        public async Task ShouldFetchShooterPermissionBeOkHavingProvidedDataOnAnotherUser()
        {
            UpdateIdentityUser(GetAnotherUser());
            //Invoke del metodo
            var response = await Controller.FetchAllPermissionsOnUser();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserPermissionContract>(response);

            // TODO: controllare permesso su entità
            Assert.IsTrue(parsed != null &&
                          parsed.Data.EntityPermissions.Any(x => x.Permissions.Contains(Permissions.EditShooter))
            );
        }

    }
}