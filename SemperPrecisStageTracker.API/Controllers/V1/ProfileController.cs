using System.Collections.Generic;
using Asp.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for profile
    /// </summary>
    [ApiVersion("1.0")]
    [Obsolete]
    public class ProfileController : ApiControllerBase
    {
        /// <summary>
        /// Get specific user using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetProfile")]
        [ProducesResponseType(typeof(UserContract), 200)]
        public IActionResult GetProfile()
        {
            var userId = PlatformUtils.GetIdentityUserId(User);

            var entity = AuthorizationLayer.GetUserById(userId);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            //Serializzazione e conferma
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Updates existing user on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateProfile")]
        [ProducesResponseType(typeof(UserContract), 200)]
        public IActionResult UpdateProfile([FromBody]UserProfileUpdateRequest request)
        {
            //User id corrente
            var userId = PlatformUtils.GetIdentityUserId(User);

            //modifica solo se admin o se sè stesso o è autorizzato
            if (request.UserId != userId)
                return Unauthorized();

            //Recupero l'elemento dal business layer
            var entity = AuthorizationLayer.GetUserById(request.UserId);

            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Username = request.Username;
            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            entity.Email = request.Email;
            entity.BirthDate = request.BirthDate;

            //Salvataggio
            var validations = AuthorizationLayer.UpdateUser(entity);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Updates existing password on platform without place
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateUserPassword")]
        [ProducesResponseType(typeof(BooleanResponse), 200)]
        public IActionResult UpdateUserPassword([FromBody]UserPasswordUpdateRequest request)
        {
            //User id corrente
            var userId = PlatformUtils.GetIdentityUserId(User);

            //modifica solo se admin o se sè stesso o è autorizzato
            if (request.UserId != userId)
                return Unauthorized();

            //Recupero l'elemento dal business layer
            var entity = AuthorizationLayer.GetUserById(request.UserId);

            if (entity == null)
                return NotFound();

            //Salvataggio
            var validations = AuthorizationLayer.UpdateUserPassword(entity, request.Password);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return Ok(new BooleanResponse() { Value = true });
        }
    }
}