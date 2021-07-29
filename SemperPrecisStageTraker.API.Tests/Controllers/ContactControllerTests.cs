using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTraker.API.Tests.Controllers.Common;
using ZenProgramming.Chakra.Core.Utilities.Data;

namespace SemperPrecisStageTraker.API.Tests.Controllers
{
    [TestClass]
    public class ContactControllerTests : ApiControllerTestsBase<ContactController, SimpleScenario>
    {
        protected override Shooter GetIdentityUser() => GetAdminUser();
        [TestMethod]
        public async Task ShouldCreateContactBeOkHavingProvidedData()
        {
            //Conteggio gli elementi prima della creazione
            var countBefore = Scenario.Contacts.Count;

            //Composizione della request
            var request = new ContactCreateRequest
            {
                Name = RandomizationUtils.GenerateRandomString(50),
                Token = RandomizationUtils.GenerateRandomString(50),
                Description = RandomizationUtils.GenerateRandomString(50),
                Email = RandomizationUtils.GenerateRandomEmail(),
                Subject = RandomizationUtils.GenerateRandomString(50),
                AcceptPolicy = true,
            };

            //Invoke del metodo
            var response = await Controller.CreateContact(request);

            //Conteggio gli elementi dopo la creazione
            var countAfter = Scenario.Contacts.Count;

            var lastInsert = Scenario.Contacts.OrderByDescending(x => x.CreationDateTime).FirstOrDefault();
            //Parsing della risposta e assert
            var parsed = ParseExpectedOk<OkResponse>(response);
            Assert.IsTrue(parsed != null
                          && countAfter == countBefore + 1
                          && lastInsert != null
                          && lastInsert.Name == request.Name
                          && lastInsert.Description == request.Description
                          && lastInsert.Email == request.Email
                          && lastInsert.Subject == request.Subject
            );
        }

    }
}