using System;
using System.Linq;
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
    public class ProfileControllerTests : ApiControllerTestsBase<ProfileController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();

        [TestMethod]
        public void ShouldGetUserBeOkHavingProvidedIdentifier()
        {
            var existing = GetIdentityUser();

            //Invoke del metodo
            var response = Controller.GetProfile();

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<ShooterContract>(response);
            Assert.IsTrue(parsed.Data != null
                          && parsed.Data.ShooterId == existing.Id
                          && parsed.Data.FirstName == existing.FirstName
                          && parsed.Data.LastName == existing.LastName
                          && parsed.Data.Username == existing.Username
                          && parsed.Data.Email == existing.Email);
        }

        [TestMethod]
        public void ShouldUpdateUserBeOkHavingProvidedData()
        {
            //Recupero un user esistente
            var existing = GetIdentityUser();

            //conteggio esistenti
            var countBefore = Scenario.Shooters.Count;


            //Composizione della request
            var request = new UserUpdateRequest
            {
                UserId = existing.Id,
                Username = RandomizationUtils.GenerateRandomString(10),
                FirstName = RandomizationUtils.GenerateRandomString(10),
                LastName = RandomizationUtils.GenerateRandomString(10),
                Email = RandomizationUtils.GenerateRandomEmail(),
                BirthDate = DateTime.Now
            };

            //Invoke del metodo
            var response = Controller.UpdateProfile(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<ShooterContract>(response);

            //conteggio esistenti
            var countAfter = Scenario.Shooters.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data.ShooterId == request.UserId
                          && parsed.Data.FirstName == request.FirstName
                          && parsed.Data.LastName == request.LastName
                          && parsed.Data.Username == request.Username
                          && parsed.Data.Email == request.Email
                          && parsed.Data.BirthDate == request.BirthDate);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public void ShouldUpdateUserBeUnauthorizedHavingProvidedWrongId()
        {
            //conteggio esistenti
            var countBefore = Scenario.Shooters.Count;

            //Composizione della request
            var request = new UserUpdateRequest
            {
                UserId = RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = Controller.UpdateProfile(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedUnauthorized(response);

            //conteggio esistenti
            var countAfter = Scenario.Shooters.Count;


            Assert.IsTrue(parsed != null
                          && parsed.Data == null);

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public void ShouldUpdateUserBeBadRequestOnNameDuplicate()
        {
            //Recupero un user esistente
            var existing = GetAdminUser();


            //Recupero un altro utente esistente
            var anotherExisting = Scenario.Shooters.FirstOrDefault(x => x.Id != existing.Id);
            if (anotherExisting == null)
                Assert.Inconclusive("User is invalid");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Shooters.Count;

            //Composizione della request
            var request = new UserUpdateRequest
            {
                UserId = existing.Id,
                Username = anotherExisting.Username,
                Email = existing.Email
            };

            //Invoke del metodo
            var response = Controller.UpdateProfile(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Shooters.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public void ShouldUpdateUserBeBadRequestOnEmailDuplicate()
        {
            //Recupero un user esistente
            var existing = GetAdminUser();


            //Recupero altro utente esistente
            var anotherExisting = Scenario.Shooters.FirstOrDefault(x => x.Id != existing.Id);
            if (anotherExisting == null)
                Assert.Inconclusive("Another user is invalid");

            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Shooters.Count;

            //Composizione della request
            var request = new UserUpdateRequest
            {
                UserId = existing.Id,
                Username = existing.Username,
                Email = anotherExisting.Email
            };

            //Invoke del metodo
            var response = Controller.UpdateProfile(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Shooters.Count;

            //Parsing della risposta e assert
            var parsed = ParseExpectedBadRequest(response);
            Assert.IsTrue(parsed != null
                          && parsed.Data.Any());

            //verifica contatori
            Assert.AreEqual(countBefore, countAfter);
        }


        [TestMethod]
        public void ShouldUpdateUserPasswordBeOkHavingProvidedData()
        {
            //Recupero un user esistente
            var existing = Scenario.Shooters.FirstOrDefault();
            if (existing == null)
                Assert.Inconclusive("User does not exists");

            var oldPassword = existing.Password;

            //Composizione della request
            var request = new UserPasswordUpdateRequest
            {
                UserId = existing.Id,
                Password = RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = Controller.UpdateUserPassword(request);

            var newPassword = Scenario.Shooters.FirstOrDefault(x => x.Id == existing.Id)?.Password;

            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<BooleanResponse>(response);

            Assert.IsNotNull(newPassword);
            Assert.IsTrue(parsed != null
                          && oldPassword != newPassword);
        }

        [TestMethod]
        public void ShouldUpdateUserPasswordBeUnauthorizedHavingProvidedWrongId()
        {
            //Composizione della request
            var request = new UserPasswordUpdateRequest
            {
                UserId = RandomizationUtils.GenerateRandomString(10),
                Password = RandomizationUtils.GenerateRandomString(10)
            };

            //Invoke del metodo
            var response = Controller.UpdateUserPassword(request);

            //Parsing della risposta e assert
            var parsed = ParseExpectedUnauthorized(response);

            //verifica contatori
            Assert.IsNull(parsed.Data);
        }
    }
}