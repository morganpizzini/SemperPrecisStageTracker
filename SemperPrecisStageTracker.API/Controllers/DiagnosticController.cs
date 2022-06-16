using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.Domain.Configurations;
using ZenProgramming.Chakra.Core.Configurations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Contracts.Requests;
using Microsoft.Extensions.Configuration;
using SemperPrecisStageTracker.API.Services.Interfaces;
using SemperPrecisStageTracker.API.Models;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for association
    /// </summary>
    [AllowAnonymous]
    public class DiagnosticController : ApiControllerBase
    {
        private readonly string adminUser;
        private readonly string[] cors;

        public DiagnosticController(IConfiguration configuration)
        {
            adminUser = configuration["adminUsername"];
            cors = configuration.GetSection("blazorEndpoints").Get<string[]>();
        }

        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetDiagnostic")]
        [ProducesResponseType(typeof(object), 200)]
        public Task<IActionResult> GetDiagnostic()
        {
            return Reply(new
            {
                EnvironmentName = ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.EnvironmentName,
                Provider = ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.Storage.Provider,
                Cors = cors
            });
        }

        /// <summary>
        /// Init database
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpGet]
        [Route("InitDatabase")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult InitDatabase()
        {
            var validations = BasicLayer.InitDatabase(adminUser);
            if (validations.Count > 0)
            {
                return BadRequest(validations);
            }
            return Ok(new OkResponse() { Status = true });
        }
    }
}