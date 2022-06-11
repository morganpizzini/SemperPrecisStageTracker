using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Domain.Containers;
using SemperPrecisStageTracker.Domain.Services;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;

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
        [Route("LogIn")]
        [ProducesResponseType(typeof(SignInResponse), 200)]
        public async Task<IActionResult> LogIn(LogInRequest request)
        {
            //Tento il signin ed ottengo l'utente se è completato
            var result = await AuthorizationLayer.LogIn(request.Username, request.Password);

            //Se non ho utente, unauthorized
            if (result == null)
                return Unauthorized();
            

            var shooterAssociation = BasicLayer.FetchShooterAssociationByShooterId(result.Id);
            var shooterTeams = BasicLayer.FetchTeamsFromShooterId(result.Id);

            var teamsIds = shooterTeams.Select(x => x.TeamId).ToList();
            var teams = BasicLayer.FetchTeamsByIds(teamsIds);

            //Se è tutto ok, serializzo il contratto
            return Ok(
                new SignInResponse
                {
                    Shooter = ContractUtils.GenerateContract(result,null, shooterAssociation,teams),
                    Permissions = ContractUtils.GenerateContract(await AuthorizationLayer.GetUserPermissionById(result.Id))
                });
        }

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
            var captchaService = ServiceResolver.Resolve<ICaptchaValidatorService>();

            var captchaCheck = await captchaService.ValidateToken(request.Token);

            if (!string.IsNullOrEmpty(captchaCheck))
            {
                return BadRequest(new List<ValidationResult> { new(captchaCheck) });
            }
            //Tento il signin ed ottengo l'utente se è completato
            var validations = AuthorizationLayer.CreateUser(new Shooter
            {
                Username = request.Username,
                Password = request.Password,
                BirthDate = request.BirthDate,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            });

            if (validations.Count > 0)
            {
                return BadRequest(validations);
            }

            return await LogIn(new LogInRequest() { Username = request.Username, Password = request.Password});
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
            return Reply(new OkResponse { Status = result.Count == 0, Errors = result.Select(x => x.ErrorMessage).ToList() });
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
            var result = await AuthorizationLayer.LogIn(user.Username, request.OldPassword);

            //Se non ho utente, unauthorized
            if (result == null)
                return Unauthorized();

            // recupero il profilo
            var validations = AuthorizationLayer.UpdateUserPassword(result, request.Password);

            //controllo risultato
            if (validations.Count > 0)
                return BadRequest(validations);

            //Se è tutto ok, serializzo e restituisco
            return Ok(new StringResponse { Value = request.Password });
        }
    }
}
