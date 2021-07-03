using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for configuration
    /// </summary>
    [Route("api/[Controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Register contact
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpGet]
        [Route("GetConfig")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetConfig()
        {
            return Ok(
                new
                {
                    recaptcha = new
                    {
                        recaptchaToken = _configuration.GetSection("recaptcha")["recaptchaToken"]
                    }
                });
        }
    }
}