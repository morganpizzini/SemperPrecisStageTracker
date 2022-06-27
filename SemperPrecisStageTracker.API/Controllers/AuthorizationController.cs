using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.API.Models;
using SemperPrecisStageTracker.API.Services.Interfaces;
using SemperPrecisStageTracker.Contracts;
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
        private readonly IEmailSender _emailSender;

        public AuthorizationController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
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

            var teamsIds = shooterTeams
                .Where(x => x.ShooterApprove && x.TeamApprove)
                .Select(x => x.TeamId)
                .ToList();
            var teams = BasicLayer.FetchTeamsByIds(teamsIds);

            //Se è tutto ok, serializzo il contratto
            return Ok(
                new SignInResponse
                {
                    Shooter = ContractUtils.GenerateContract(result, null, shooterAssociation, teams),
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

            return await LogIn(new LogInRequest() { Username = request.Username, Password = request.Password });
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
        [Route("ResetUserPassword")]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> ResetUserPassword(ShooterRequest request)
        {
            //user identifier
            var userId = PlatformUtils.GetIdentityUserId(User);

            var user = BasicLayer.GetShooter(request.ShooterId, userId);
            if (user == null)
            {
                return NotFound();
            }
            var validations = await SetPassworAliasOnShooter(user, userId);

            if (validations.Count > 0)
            {
                return BadRequest(validations);
            }

            //Se è tutto ok, serializzo e restituisco
            return Ok(new OkResponse { Status = true });
        }

        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [Route("PasswordForgot")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> PasswordForgot(ForgotPasswordRequest request)
        {
            var captchaService = ServiceResolver.Resolve<ICaptchaValidatorService>();

            var captchaCheck = await captchaService.ValidateToken(request.Token);

            if (!string.IsNullOrEmpty(captchaCheck))
            {
                return BadRequest(new List<ValidationResult> { new(captchaCheck) });
            }

            var user = BasicLayer.GetShooterFromEmailOrUsername(request.Username);
            if (user == null)
            {
                return BadRequest(new ValidationResult("User not found").AsList());
            }
            var validations = await SetPassworAliasOnShooter(user);

            if (validations.Count > 0)
            {
                return BadRequest(validations);
            }

            //Se è tutto ok, serializzo e restituisco
            return Ok(new OkResponse { Status = true });
        }

        private async Task<IList<ValidationResult>> SetPassworAliasOnShooter(Shooter user, string userId = null)
        {
            var data = BasicLayer.GetShooterData(user.Id);

            user.RestorePasswordAlias = Guid.NewGuid().ToString();

            var validations = await BasicLayer.UpdateShooter(user, data, userId,false);

            if (validations.Count > 0)
            {
                return validations;
            }

            //sendEmail
            var message = new Message(new string[] { user.Email }, "Reset assword", $"Reset password from <a href=\"https://asdsemperprecis.it/app/reset-password?data={user.RestorePasswordAlias}\">here</a>",null,true);
            _emailSender.SendEmail(message);
            //Se è tutto ok, serializzo e restituisco
            return validations;
        }

        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("GetUserFromRestorePasswordAlias")]
        [ProducesResponseType(typeof(ShooterContract), 200)]
        public IActionResult GetUserFromRestorePasswordAlias(ShooterRequest request)
        {
            var user = AuthorizationLayer.GetUserByRestorePasswordAlias(request.ShooterId);
            if (user == null)
                return NotFound();
            return Ok(ContractUtils.GenerateContract(user));
        }


        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [Route("ResetPassword")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StringResponse), 200)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var captchaService = ServiceResolver.Resolve<ICaptchaValidatorService>();

            var captchaCheck = await captchaService.ValidateToken(request.Token);

            if (!string.IsNullOrEmpty(captchaCheck))
            {
                return BadRequest(new List<ValidationResult> { new(captchaCheck) });
            }

            var user = AuthorizationLayer.GetUserById(request.UserId);

            if (user == null)
                return NotFound();
            if (user.RestorePasswordAlias != request.RestorePasswordAlias)
            {
                return BadRequest(new ValidationResult("Restore token is expire, restart the procedure").AsList());
            }

            var validations = AuthorizationLayer.UpdateUserPassword(user, request.Password);
            //Se non ho utente, unauthorized
            if (validations.Count > 0)
                return BadRequest(validations);

            //Se è tutto ok, serializzo e restituisco
            return Ok(new OkResponse { Status = true });
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
