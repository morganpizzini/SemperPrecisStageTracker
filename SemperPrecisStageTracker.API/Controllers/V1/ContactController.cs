using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Domain.Containers;
using SemperPrecisStageTracker.Domain.Services;
using SemperPrecisStageTracker.Models;
using Asp.Versioning;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for contacts
    /// </summary>
    [ApiVersion("1.0")]
    public class ContactController : ApiControllerBase
    {
        /// <summary>
        /// Register contact
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateContact")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> CreateContact([FromBody]ContactCreateRequest request)
        {
            var captchaService = ServiceResolver.Resolve<ICaptchaValidatorService>();

            var captchaCheck = await captchaService.ValidateToken(request.Token);

            if (!string.IsNullOrEmpty(captchaCheck))
            {
                return BadRequest(new List<ValidationResult> { new(captchaCheck) });
            }

            var model = new Contact()
            {
                Description = request.Description,
                Email = request.Email,
                Name = request.Name,
                Subject = request.Subject
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateContract(model);

            if (validations.Count > 0)
                return BadRequest(validations);

            return Ok(new OkResponse() { Status = true });
        }
    }
}