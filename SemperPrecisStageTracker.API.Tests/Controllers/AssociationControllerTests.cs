using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.API.Tests.Utils;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using ZenProgramming.Chakra.Core.Utilities.Data;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class AssociationControllerTests : ApiControllerTestsBase<AssociationController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldCreateAssociationBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Associations.Count;
            var countPermission = Scenario.UserPermissions.Count;

            //Composizione della request
            var request = new AssociationCreateRequest
            {
                Name = RandomizationUtils.GenerateRandomString(50),
                FirstPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                SecondPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                ThirdPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                FirstProceduralPointDown = RandomSeed.Next(10),
                SecondProceduralPointDown = RandomSeed.Next(10),
                ThirdProceduralPointDown = RandomSeed.Next(10),
                HitOnNonThreatPointDown = RandomSeed.Next(10),
                Categories = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Classifications= ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Divisions = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                MatchKinds = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                SoRoles = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3)
            };

            //Invoke del metodo
            var response = await Controller.CreateAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Associations.Count;
            var countPermissionAfter = Scenario.UserPermissions.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<AssociationContract>(response);
            Assert.AreEqual(countAfter, countBefore + 1);
            Assert.AreEqual(countPermission,countPermissionAfter);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Name == request.Name
                          && parsed.Data.FirstPenaltyLabel == request.FirstPenaltyLabel
                          && parsed.Data.SecondPenaltyLabel == request.SecondPenaltyLabel
                          && parsed.Data.ThirdPenaltyLabel == request.ThirdPenaltyLabel
                          && parsed.Data.FirstProceduralPointDown == request.FirstProceduralPointDown
                          && parsed.Data.SecondProceduralPointDown == request.SecondProceduralPointDown
                          && parsed.Data.ThirdProceduralPointDown == request.ThirdProceduralPointDown
                          && parsed.Data.HitOnNonThreatPointDown == request.HitOnNonThreatPointDown
                          && parsed.Data.Divisions.All(x=> request.Divisions.Contains(x))
                          && parsed.Data.Categories.All(x=> request.Categories.Contains(x))
                          && parsed.Data.Classifications.All(x=> request.Classifications.Contains(x))
                          && parsed.Data.MatchKinds.All(x=> request.MatchKinds.Contains(x))
                          && parsed.Data.SoRoles.All(x=> request.SoRoles.Contains(x)));
        }

        [TestMethod]
        public async Task ShouldCreateAssociationBeOkHavingProvidedDataAndCreatePermission()
        {
            UpdateIdentityUser(GetUserWithPermission(PermissionCtor.CreateAssociations));

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Associations.Count;
            var countPermission = Scenario.UserPermissions.Count;

            //Composizione della request
            var request = new AssociationCreateRequest
            {
                Name = RandomizationUtils.GenerateRandomString(50),
                FirstPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                SecondPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                ThirdPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                FirstProceduralPointDown = RandomSeed.Next(10),
                SecondProceduralPointDown = RandomSeed.Next(10),
                ThirdProceduralPointDown = RandomSeed.Next(10),
                HitOnNonThreatPointDown = RandomSeed.Next(10),
                Categories = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Classifications= ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Divisions = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                MatchKinds = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                SoRoles = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3)
            };

            //Invoke del metodo
            var response = await Controller.CreateAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Associations.Count;
            var countPermissionAfter = Scenario.UserPermissions.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<AssociationContract>(response);
            Assert.AreEqual(countAfter, countBefore + 1);
            Assert.AreEqual(countPermission+2,countPermissionAfter);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Name == request.Name
                          && parsed.Data.FirstPenaltyLabel == request.FirstPenaltyLabel
                          && parsed.Data.SecondPenaltyLabel == request.SecondPenaltyLabel
                          && parsed.Data.ThirdPenaltyLabel == request.ThirdPenaltyLabel
                          && parsed.Data.FirstProceduralPointDown == request.FirstProceduralPointDown
                          && parsed.Data.SecondProceduralPointDown == request.SecondProceduralPointDown
                          && parsed.Data.ThirdProceduralPointDown == request.ThirdProceduralPointDown
                          && parsed.Data.HitOnNonThreatPointDown == request.HitOnNonThreatPointDown
                          && parsed.Data.Divisions.All(x=> request.Divisions.Contains(x))
                          && parsed.Data.Categories.All(x=> request.Categories.Contains(x))
                          && parsed.Data.Classifications.All(x=> request.Classifications.Contains(x))
                          && parsed.Data.MatchKinds.All(x=> request.MatchKinds.Contains(x))
                          && parsed.Data.SoRoles.All(x=> request.SoRoles.Contains(x)));
        }

        [TestMethod]
        public async Task ShouldCreateAssociationBeBadRequestWithNoPrivileges()
        {
            UpdateIdentityUser(GetUserWithoutPermission(PermissionCtor.ManageAssociations.CreateAssociations));

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Associations.Count;
            var countPermission = Scenario.UserPermissions.Count;

            //Composizione della request
            var request = new AssociationCreateRequest
            {
                Name = RandomizationUtils.GenerateRandomString(50),
                FirstPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                SecondPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                ThirdPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                FirstProceduralPointDown = RandomSeed.Next(10),
                SecondProceduralPointDown = RandomSeed.Next(10),
                ThirdProceduralPointDown = RandomSeed.Next(10),
                HitOnNonThreatPointDown = RandomSeed.Next(10),
                Categories = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Classifications= ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Divisions = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                MatchKinds = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                SoRoles = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3)
            };

            //Invoke del metodo
            var response = await Controller.CreateAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Associations.Count;
            var countPermissionAfter = Scenario.UserPermissions.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);

            Assert.AreEqual(countAfter, countBefore);
            Assert.AreEqual(countPermissionAfter, countPermission);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());
        }

        [TestMethod]
        public async Task ShouldUpdateAssociationBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Associations.Count;
            var countPermission = Scenario.UserPermissions.Count;

            var existing = Scenario.Associations[0];
            
            if(existing == null)
            {
                Assert.Inconclusive("No association found");
                return;
            }
            //Composizione della request
            var request = new AssociationUpdateRequest
            {
                AssociationId = existing.Id,
                Name = RandomizationUtils.GenerateRandomString(50),
                FirstPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                SecondPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                ThirdPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                FirstProceduralPointDown = RandomSeed.Next(10),
                SecondProceduralPointDown = RandomSeed.Next(10),
                ThirdProceduralPointDown = RandomSeed.Next(10),
                HitOnNonThreatPointDown = RandomSeed.Next(10),
                Categories = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Classifications= ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Divisions = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                MatchKinds = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                SoRoles = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3)
            };

            //Invoke del metodo
            var response = await Controller.UpdateAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Associations.Count;
            var countPermissionAfter = Scenario.UserPermissions.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<AssociationContract>(response);
            Assert.AreEqual(countAfter, countBefore);
            Assert.AreEqual(countPermission,countPermissionAfter);
            Assert.IsTrue(parsed != null
                          && parsed.Data.AssociationId == request.AssociationId
                          && parsed.Data.Name == request.Name
                          && parsed.Data.FirstPenaltyLabel == request.FirstPenaltyLabel
                          && parsed.Data.SecondPenaltyLabel == request.SecondPenaltyLabel
                          && parsed.Data.ThirdPenaltyLabel == request.ThirdPenaltyLabel
                          && parsed.Data.FirstProceduralPointDown == request.FirstProceduralPointDown
                          && parsed.Data.SecondProceduralPointDown == request.SecondProceduralPointDown
                          && parsed.Data.ThirdProceduralPointDown == request.ThirdProceduralPointDown
                          && parsed.Data.HitOnNonThreatPointDown == request.HitOnNonThreatPointDown
                          && parsed.Data.Divisions.All(x=> request.Divisions.Contains(x))
                          && parsed.Data.Categories.All(x=> request.Categories.Contains(x))
                          && parsed.Data.Classifications.All(x=> request.Classifications.Contains(x))
                          && parsed.Data.MatchKinds.All(x=> request.MatchKinds.Contains(x))
                          && parsed.Data.SoRoles.All(x=> request.SoRoles.Contains(x)));
        }

        [TestMethod]
        public async Task ShouldUpdateAssociationBeOkHavingProvidedDataAndPermission()
        {
            var basePermission = PermissionCtor.AssociationEdit;
            var currentUser = GetUserWithPermission(basePermission);
            UpdateIdentityUser(currentUser);

            var entityId = FindEntityWithPermission(currentUser.Id,basePermission).FirstOrDefault();

            var existingAssociation = Scenario.Associations.FirstOrDefault(x=>x.Id == entityId);

            if (existingAssociation == null) {
                Assert.Inconclusive("Associaiton not found");
                return; 
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Associations.Count;
            var countPermission = Scenario.UserPermissions.Count;

            //Composizione della request
            var request = new AssociationUpdateRequest
            {
                AssociationId = existingAssociation.Id,
                Name = RandomizationUtils.GenerateRandomString(50),
                FirstPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                SecondPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                ThirdPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                FirstProceduralPointDown = RandomSeed.Next(10),
                SecondProceduralPointDown = RandomSeed.Next(10),
                ThirdProceduralPointDown = RandomSeed.Next(10),
                HitOnNonThreatPointDown = RandomSeed.Next(10),
                Categories = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Classifications= ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Divisions = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                MatchKinds = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                SoRoles = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3)
            };

            //Invoke del metodo
            var response = await Controller.UpdateAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Associations.Count;
            var countPermissionAfter = Scenario.UserPermissions.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<AssociationContract>(response);
            Assert.AreEqual(countAfter, countBefore);
            Assert.AreEqual(countPermission,countPermissionAfter);
            Assert.IsTrue(parsed != null
                          && parsed.Data.AssociationId == request.AssociationId
                          && parsed.Data.Name == request.Name
                          && parsed.Data.FirstPenaltyLabel == request.FirstPenaltyLabel
                          && parsed.Data.SecondPenaltyLabel == request.SecondPenaltyLabel
                          && parsed.Data.ThirdPenaltyLabel == request.ThirdPenaltyLabel
                          && parsed.Data.FirstProceduralPointDown == request.FirstProceduralPointDown
                          && parsed.Data.SecondProceduralPointDown == request.SecondProceduralPointDown
                          && parsed.Data.ThirdProceduralPointDown == request.ThirdProceduralPointDown
                          && parsed.Data.HitOnNonThreatPointDown == request.HitOnNonThreatPointDown
                          && parsed.Data.Divisions.All(x=> request.Divisions.Contains(x))
                          && parsed.Data.Categories.All(x=> request.Categories.Contains(x))
                          && parsed.Data.Classifications.All(x=> request.Classifications.Contains(x))
                          && parsed.Data.MatchKinds.All(x=> request.MatchKinds.Contains(x))
                          && parsed.Data.SoRoles.All(x=> request.SoRoles.Contains(x)));
        }

        [TestMethod]
        public async Task ShouldUpdateAssociationBeBadRequestHavingProvidedDataAndBadPermission()
        {
             var basePermission = PermissionCtor.ManageAssociations.AssociationEdit;
            var currentUser = GetUserWithoutPermission(basePermission);
            UpdateIdentityUser(currentUser);

            var entityId = FindEntityWithPermission(currentUser.Id,basePermission).FirstOrDefault();

            var existingAssociation = Scenario.Associations.FirstOrDefault();

            if (existingAssociation == null) {
                Assert.Inconclusive("Associaiton not found");
                return; 
            }
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Associations.Count;
            var countPermission = Scenario.UserPermissions.Count;

            //Composizione della request
            var request = new AssociationUpdateRequest
            {
                AssociationId = existingAssociation.Id,
                Name = RandomizationUtils.GenerateRandomString(50),
                FirstPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                SecondPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                ThirdPenaltyLabel = RandomizationUtils.GenerateRandomString(50),
                FirstProceduralPointDown = RandomSeed.Next(10),
                SecondProceduralPointDown = RandomSeed.Next(10),
                ThirdProceduralPointDown = RandomSeed.Next(10),
                HitOnNonThreatPointDown = RandomSeed.Next(10),
                Categories = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Classifications= ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                Divisions = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                MatchKinds = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3),
                SoRoles = ListExtensions.Repeated(()=>RandomizationUtils.GenerateRandomString(15),3)
            };

            //Invoke del metodo
            var response = await Controller.UpdateAssociation(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Associations.Count;
            var countPermissionAfter = Scenario.UserPermissions.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.AreEqual(countAfter, countBefore);
            Assert.AreEqual(countPermission,countPermissionAfter);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());
        }

        [TestMethod]
        public async Task ShouldDeleteAssociationBeOkHavingDeletedElement()
        {
            //Recupero una Match esistente non utilizzato
            var existing = Scenario.Associations.FirstOrDefault();

            if (existing == null)
                Assert.Inconclusive("Association does not exists");

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Associations.Count;

            //Composizione della request
            var request = new AssociationRequest { AssociationId = existing.Id };

            //Invoke del metodo
            var response = await Controller.DeleteAssociation(request);

            //Parsing della risposta
            var parsed = ParseExpectedOk<AssociationContract>(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Associations.Count;

            Assert.IsTrue(
                parsed.Data.AssociationId == existing.Id);
            Assert.AreEqual(countBefore - 1, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeleteAssociationBeOkAndDeletePermissions()
        {
            var basePermission = PermissionCtor.AssociationDelete;
            var currentUser = GetUserWithPermission(basePermission);
            UpdateIdentityUser(currentUser);

            var entityId = FindEntityWithPermission(currentUser.Id,basePermission).FirstOrDefault();

            if (entityId == null)
                Assert.Inconclusive("No permission found");

            var countBefore = Scenario.UserPermissions.Count(x=>x.UserId == currentUser.Id);

            //Recupero una Match esistente non utilizzato
            var existing = Scenario.Associations.FirstOrDefault(x=> x.Id == entityId);

            if (existing == null)
                Assert.Inconclusive("Association does not exists");

            //Composizione della request
            var request = new AssociationRequest { AssociationId = existing.Id };

            //Invoke del metodo
            var response = await Controller.DeleteAssociation(request);

            //Parsing della risposta
            var parsed = ParseExpectedOk<AssociationContract>(response);

            var countAfter = Scenario.UserPermissions.Count(x=>x.UserId == currentUser.Id);

            Assert.IsTrue(parsed.Data.AssociationId == existing.Id);

            Assert.AreEqual(countBefore -2, countAfter);
        }

         [TestMethod]
        public async Task ShouldDeleteAssociationBeBadRequestWithoutPermissions()
        {
            var currentUser = GetUserWithoutPermission(PermissionCtor.ManageAssociations.AssociationDelete);
            UpdateIdentityUser(currentUser);

            var countBefore = Scenario.UserPermissions.Count(x=>x.UserId == currentUser.Id);

            //Recupero una Match esistente non utilizzato
            var existing = Scenario.Associations.FirstOrDefault();

            if (existing == null)
                Assert.Inconclusive("Association does not exists");

            //Composizione della request
            var request = new AssociationRequest { AssociationId = existing.Id };

            //Invoke del metodo
            var response = await Controller.DeleteAssociation(request);

            //Parsing della risposta
            var parsed = ParseExpectedBadRequest(response);

            var countAfter = Scenario.UserPermissions.Count(x=>x.UserId == currentUser.Id);

            Assert.IsTrue(parsed.Data.Any());

            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldFetchAvailableShooterAssociationBeOkHavingProvidedData()
        {
            var existing = Scenario.ShooterAssociationInfos.FirstOrDefault();

            if (existing == null)
            {
                Assert.Inconclusive("No shooter association exists");
            }

            var count = Scenario.ShooterAssociationInfos.Count(x => x.ShooterId == existing.ShooterId);

            var request = new ShooterRequest
            {
                ShooterId = existing.ShooterId
            };

            //Invoke del metodo
            var response = await Controller.FetchAvailableAssociationsForShooter(request);


            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<AssociationContract>>(response);

            Assert.AreEqual(count, parsed.Data.Count);
            Assert.IsTrue(parsed.Data.All(x => !string.IsNullOrEmpty(x.Name)));

        }

    }
}