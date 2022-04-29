using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.Domain.Configurations;
using ZenProgramming.Chakra.Core.Configurations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for association
    /// </summary>
    [AllowAnonymous]
    public class DiagnosticController : ApiControllerBase
    {
        public DiagnosticController() : base()
        {
            
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
            return Reply(new {
                EnvironmentName = ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.EnvironmentName,
                Provider = ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.Storage.Provider
            });
        }
    }
}