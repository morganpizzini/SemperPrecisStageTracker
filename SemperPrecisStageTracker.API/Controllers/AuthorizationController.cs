using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for authorization
    /// </summary>
    public class AuthorizationController : ApiControllerBase
    {
        /// <summary>
        /// Executes sign-in on current platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("SignIn")]
        [ProducesResponseType(typeof(SignInResponse), 200)]
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            //Tento il signin ed ottengo l'utente se è completato
            var result = await AuthorizationLayer.SignIn(request.Username, request.Password);

            //Se non ho utente, unauthorized
            if (result == null)
                return Unauthorized();
            
            var permissions = await AuthorizationLayer.GetUserPermissionById(result.Id);
            
            // recupero il profilo
            //var profile = AuthorizationLayer.FetchProfilesOnUser(result.Username).FirstOrDefault();
            
            //Se è tutto ok, serializzo il contratto
            return Ok(
                new SignInResponse
                {
                    Shooter = ContractUtils.GenerateContract(result),
                    Permissions = ContractUtils.GenerateContract(permissions.adminPermissions,permissions.entityPermissions)
                });
        }


        /// <summary>
        /// Sync password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("SyncPassword")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public Task<IActionResult> SyncPassword()
        {
            //Tento il signin ed ottengo l'utente se è completato
            var result = AuthorizationLayer.SyncPasswords();
            
            //Se è tutto ok, serializzo il contratto
            return Reply(new OkResponse{ Status = result.Count == 0, Errors = result.Select(x=> x.ErrorMessage).ToList() });
        }

        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [Route("UpdatePassword")]
        [ProducesResponseType(typeof(StringResponse), 200)]
        public async Task<IActionResult> UpdatePassword(ChangePasswordRequest request)
        {
            //user identifier
            var userId = PlatformUtils.GetIdentityUserId(User);

            var user = AuthorizationLayer.GetUserById(userId);

            //Tento il signin ed ottengo l'utente se è completato
            var result = await AuthorizationLayer.SignIn(user.Username, request.OldPassword);

            //Se non ho utente, unauthorized
            if (result == null)
                return Unauthorized();

            // recupero il profilo
            var validations = AuthorizationLayer.UpdateUserPassword(result, request.Password);

            //controllo risultato
            if (validations.Count > 0)
                return BadRequest(validations);

            //Se è tutto ok, serializzo e restituisco
            return Ok(new StringResponse{Value = request.Password});
        }
    }
}
