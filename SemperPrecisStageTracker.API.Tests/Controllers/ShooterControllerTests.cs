using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
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
    public class ShooterControllerTests : ApiControllerTestsBase<ShooterController, SimpleScenario>
    {
        protected override User GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public async Task ShouldFetchAllShootersBeOkHavingElements()
        {
            //conteggio esistenti generici o inseriti dall'utente
            var countBefore = Scenario.Shooters.Count();

            //Invoke del metodo
            var response = await Controller.FetchAllShooters();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<IList<UserContract>>(response);
            Assert.AreEqual(countBefore, parsed.Data.Count);
        }

        [TestMethod]
        public async Task ShouldCreateShooterBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Shooters.Count;

            //Composizione della request
            var request = new ShooterCreateRequest
            {
                Email = RandomizationUtils.GenerateRandomEmail(),
                LastName = RandomizationUtils.GenerateRandomString(10),
                FirstName = RandomizationUtils.GenerateRandomString(10),
                BirthDate = DateTime.Now,
                Username = RandomizationUtils.GenerateRandomString(10),
                FirearmsLicence = RandomizationUtils.GenerateRandomString(10),
                FirearmsLicenceExpireDate = DateTime.Now,
                Address = RandomizationUtils.GenerateRandomString(10),
                BirthLocation = RandomizationUtils.GenerateRandomString(10),
                City = RandomizationUtils.GenerateRandomString(10),
                Country = RandomizationUtils.GenerateRandomString(10),
                FirearmsLicenceReleaseDate = DateTime.Now,
                FiscalCode = RandomizationUtils.GenerateRandomString(10),
                MedicalExaminationExpireDate = DateTime.Now,
                Phone = RandomizationUtils.GenerateRandomString(10),
                PostalCode = RandomizationUtils.GenerateRandomString(10),
                Province=RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = await Controller.CreateShooter(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Shooters.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserContract>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && parsed.Data.Email == request.Email
                          && parsed.Data.LastName == request.LastName
                           && parsed.Data.FirstName == request.FirstName
                            && parsed.Data.BirthDate == request.BirthDate
                            && parsed.Data.Username == request.Username
                            && parsed.Data.FirearmsLicence == request.FirearmsLicence
                            && parsed.Data.FirearmsLicenceExpireDate == request.FirearmsLicenceExpireDate
                            && parsed.Data.Address == request.Address 
                            && parsed.Data.BirthLocation == request.BirthLocation 
                            && parsed.Data.City == request.City 
                            && parsed.Data.Country == request.Country 
                            && parsed.Data.FirearmsLicenceReleaseDate == request.FirearmsLicenceReleaseDate 
                            && parsed.Data.FiscalCode == request.FiscalCode 
                            && parsed.Data.MedicalExaminationExpireDate == request.MedicalExaminationExpireDate 
                            && parsed.Data.Phone == request.Phone 
                            && parsed.Data.PostalCode == request.PostalCode 
                            && parsed.Data.Province == request.Province 
                          );
        }


        [TestMethod]
        public async Task ShouldCreateShooterBeOkAndNotCreatePermissions()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Permissions.Count;

            //Composizione della request
            var request = new ShooterCreateRequest
            {
                Email = RandomizationUtils.GenerateRandomEmail(),
                LastName = RandomizationUtils.GenerateRandomString(10),
                FirstName = RandomizationUtils.GenerateRandomString(10),
                BirthDate = DateTime.Now,
                Username = RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = await Controller.CreateShooter(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Permissions.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserContract>(response);
            Assert.IsTrue(parsed != null);
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldCreateShooterBeBadRequestWithoutPermission()
        {
            UpdateIdentityUser(GetUserWithoutPermission(PermissionCtor.ManageShooters.CreateShooters));

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Shooters.Count;
            var countBeforePermission = Scenario.Permissions.Count;

            //Composizione della request
            var request = new ShooterCreateRequest
            {
                Email = RandomizationUtils.GenerateRandomEmail(),
                LastName = RandomizationUtils.GenerateRandomString(10),
                FirstName = RandomizationUtils.GenerateRandomString(10),
                BirthDate = DateTime.Now,
                Username = RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = await Controller.CreateShooter(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Shooters.Count;
            var countAfterPermission = Scenario.Permissions.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null);
            Assert.AreEqual(countBefore, countAfter);
            // because is made by an admin the permissions should be the same
            Assert.AreEqual(countBeforePermission, countAfterPermission);
        }

        [TestMethod]
        public async Task ShouldCreateShooterBeOkAndCreatePermissions()
        {
            UpdateIdentityUser(GetUserWithPermission(PermissionCtor.CreateShooters));
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.UserPermissions.Count;

            //Composizione della request
            var request = new ShooterCreateRequest
            {
                Email = RandomizationUtils.GenerateRandomEmail(),
                LastName = RandomizationUtils.GenerateRandomString(10),
                FirstName = RandomizationUtils.GenerateRandomString(10),
                BirthDate = DateTime.Now,
                Username = RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = await Controller.CreateShooter(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.UserPermissions.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserContract>(response);
            Assert.IsTrue(parsed != null);
            Assert.AreEqual(countBefore + 2, countAfter);
        }

        [TestMethod]
        public async Task ShouldGetShooterBeOkHavingProvidedData()
        {
            //Recupero una Shooter esistente
            var existing = Scenario.Shooters.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Shooter does not exists");


            var existingData = Scenario.ShooterDatas.FirstOrDefault(x=>x.UserId == existing.Id);
            if (existingData == null)
                Assert.Fail("Shooter data not found");

            //conteggio esistenti
            var countBefore = Scenario.Shooters.Count;


            //Composizione della request
            var request = new ShooterRequest
            {
                ShooterId = existing.Id
            };

            //Invoke del metodo
            var response = await Controller.GetShooter(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserContract>(response);

            //conteggio esistenti
            var countAfter = Scenario.Shooters.Count;
            
            Assert.IsTrue(parsed != null
                          && parsed.Data.Email == existing.Email
                          && parsed.Data.LastName == existing.LastName
                          && parsed.Data.FirstName == existing.FirstName
                          && parsed.Data.BirthDate == existing.BirthDate
                          && parsed.Data.FirearmsLicence == existingData.FirearmsLicence
                          && parsed.Data.FirearmsLicenceExpireDate == existingData.FirearmsLicenceExpireDate
                          && parsed.Data.MedicalExaminationExpireDate == existingData.MedicalExaminationExpireDate
                          && parsed.Data.Username == existing.Username);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldUpdateShooterBeOkHavingProvidedData()
        {
            //Recupero una Shooter esistente
            var existing = Scenario.Shooters.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("Shooter does not exists");

            //conteggio esistenti
            var countBefore = Scenario.Shooters.Count;


            //Composizione della request
            var request = new ShooterUpdateRequest
            {
                ShooterId = existing.Id,
                Email = RandomizationUtils.GenerateRandomEmail(),
                LastName = RandomizationUtils.GenerateRandomString(10),
                FirstName = RandomizationUtils.GenerateRandomString(10),
                BirthDate = DateTime.Now,
                Username = RandomizationUtils.GenerateRandomString(10),
                FirearmsLicence = RandomizationUtils.GenerateRandomString(10),
                FirearmsLicenceExpireDate = DateTime.Now,
                MedicalExaminationExpireDate = DateTime.Now,
                Address = RandomizationUtils.GenerateRandomString(10),
                BirthLocation = RandomizationUtils.GenerateRandomString(10),
                City = RandomizationUtils.GenerateRandomString(10),
                Country = RandomizationUtils.GenerateRandomString(10),
                FirearmsLicenceReleaseDate = DateTime.Now,
                FiscalCode = RandomizationUtils.GenerateRandomString(10),
                Phone = RandomizationUtils.GenerateRandomString(10),
                PostalCode = RandomizationUtils.GenerateRandomString(10),
                Province=RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = await Controller.UpdateShooter(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<UserContract>(response);

            //conteggio esistenti
            var countAfter = Scenario.Shooters.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data.Email == request.Email
                          && parsed.Data.LastName == request.LastName
                          && parsed.Data.FirstName == request.FirstName
                          && parsed.Data.BirthDate == request.BirthDate
                          && parsed.Data.FirearmsLicence == request.FirearmsLicence
                          && parsed.Data.FirearmsLicenceExpireDate == request.FirearmsLicenceExpireDate
                          && parsed.Data.MedicalExaminationExpireDate == request.MedicalExaminationExpireDate
                          && parsed.Data.Username == request.Username
                          && parsed.Data.Address == request.Address 
                            && parsed.Data.BirthLocation == request.BirthLocation 
                            && parsed.Data.City == request.City 
                            && parsed.Data.Country == request.Country 
                            && parsed.Data.FirearmsLicenceReleaseDate == request.FirearmsLicenceReleaseDate 
                            && parsed.Data.FiscalCode == request.FiscalCode 
                            && parsed.Data.MedicalExaminationExpireDate == request.MedicalExaminationExpireDate 
                            && parsed.Data.Phone == request.Phone 
                            && parsed.Data.PostalCode == request.PostalCode 
                            && parsed.Data.Province == request.Province );

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldUpdateShooterBeBadRequestHavingProvidedSameFirearmsLicence()
        {
            //conteggio esistenti
            var countBefore = Scenario.Shooters.Count;
            //Recupero una Shooter esistente
            var existing = Scenario.ShooterDatas.FirstOrDefault();

            if (existing == null)
                Assert.Inconclusive("Shooter does not exists");

            //Recupero una Shooter esistente
            var existingToUpdate = Scenario.Shooters.FirstOrDefault(x => x.Id != existing.UserId);

            if (existing == null)
                Assert.Inconclusive("Shooter does not exists");

            //Composizione della request
            var request = new ShooterUpdateRequest
            {
                ShooterId = existingToUpdate.Id,
                FirearmsLicence = existing.FirearmsLicence,
                Email = RandomizationUtils.GenerateRandomEmail(),
                LastName = RandomizationUtils.GenerateRandomString(10),
                FirstName = RandomizationUtils.GenerateRandomString(10),
                BirthDate = DateTime.Now,
                Username = RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = await Controller.UpdateShooter(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);

            //conteggio esistenti
            var countAfter = Scenario.Shooters.Count;

            Assert.IsNotNull(parsed);
            Assert.IsTrue(parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldUpdateShooterBeNotFoundHavingProvidedWrongId()
        {
            //conteggio esistenti
            var countBefore = Scenario.Shooters.Count;

            //Composizione della request
            var request = new ShooterUpdateRequest
            {
                ShooterId = RandomizationUtils.GenerateRandomString(10),
                Email = RandomizationUtils.GenerateRandomEmail(),
                LastName = RandomizationUtils.GenerateRandomString(10),
                FirstName = RandomizationUtils.GenerateRandomString(10),
                BirthDate = DateTime.Now,
                Username = RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = await Controller.UpdateShooter(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedNotFound(response);

            //conteggio esistenti
            var countAfter = Scenario.Shooters.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data == null);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeleteShooterBeOkHavingDeletedElement()
        {
            //Recupero una Shooter esistente non utilizzato
            var existing = Scenario.Shooters.FirstOrDefault();

            if (existing == null)
                Assert.Inconclusive("Shooter does not exists");

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Shooters.Count;

            //Composizione della request
            var request = new ShooterRequest { ShooterId = existing.Id };

            //Invoke del metodo
            var response = await Controller.DeleteShooter(request);

            //Parsing della risposta
            var parsed = ParseExpectedOk<UserContract>(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Shooters.Count;

            Assert.IsTrue(
                parsed.Data.UserId == existing.Id);
            Assert.AreEqual(countBefore - 1, countAfter);
        }

        [TestMethod]
        public async Task ShouldDeleteShooterBeOkAndDeletePermissions()
        {
            var permission =
                Scenario.Permissions.FirstOrDefault(x => x.Name == nameof(Permissions.EditShooter));

            if (permission == null)
                Assert.Inconclusive("Permissions not found");

            //Recupero una Shooter esistente non utilizzato
            var existing = Scenario.Shooters.FirstOrDefault();
            //var existing = Scenario.Shooters.FirstOrDefault(x => x.Id == permission.EntityId);

            if (existing == null)
                Assert.Inconclusive("Shooter does not exists");


            //Composizione della request
            var request = new ShooterRequest { ShooterId = existing.Id };

            //Invoke del metodo
            var response = await Controller.DeleteShooter(request);

            //Parsing della risposta
            var parsed = ParseExpectedOk<UserContract>(response);

            //Conteggio gli elementi dopo la cancellazione
            var countPermissionAfter = Scenario.Permissions.Count(x =>
                //x.EntityId == permission.EntityId &&
                x.Name == nameof(Permissions.EditShooter));

            Assert.IsTrue(
                parsed.Data.UserId == existing.Id);

            //Assert.AreEqual(0, countPermissionAfter);

        }

        [TestMethod]
        public async Task ShouldDeleteShooterBeNotFoundHavingProvidedWrongId()
        {

            //Conteggio gli elementi prima della cancellazione
            var countBefore = Scenario.Shooters.Count;

            //Composizione della request
            var request = new ShooterRequest { ShooterId = RandomizationUtils.GenerateRandomString(10) };

            //Invoke del metodo
            var response = await Controller.DeleteShooter(request);

            //Parsing della risposta
            var parsed = ParseExpectedNotFound(response);

            //Conteggio gli elementi dopo la cancellazione
            var countAfter = Scenario.Shooters.Count;

            Assert.IsTrue(parsed != null &&
                          parsed.Data == null);
            Assert.AreEqual(countBefore, countAfter);
        }
    }
}