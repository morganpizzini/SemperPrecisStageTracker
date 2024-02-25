using System.Collections.Generic;
using Asp.Versioning;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for shooterTeamPayment
    /// </summary>
    [ApiVersion("1.0")]
    public class ShooterTeamPaymentController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterTeamPaymentByTeam")]
        [ProducesResponseType(typeof(IList<UserContract>), 200)]
        public Task<IActionResult> FetchShooterTeamPaymentByTeam([FromBody]TeamRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchShooterTeamPaymentsFromTeamId(request.TeamId);
            var shooterIds = entities.Select(x => x.UserId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            //Return contract
            return Reply(entities.As(x => ContractUtils.GenerateContract(x, shooters.FirstOrDefault(t => t.Id == x.UserId))));
        }

        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterTeamPaymentByShooterAndTeam")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(IList<UserContract>), 200)]
        public IActionResult FetchShooterTeamPaymentByShooterAndTeam([FromBody]ShooterTeamRequest request)
        {
            var shooter = BasicLayer.GetUser(request.ShooterId);
            if(shooter == null)
                return NotFound();
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchShooterTeamPaymentByTeamAndShooterId(request.TeamId, request.ShooterId);
            //Return contract
            return Ok(entities.As(x => ContractUtils.GenerateContract(x,shooter)));

        }

        /// <summary>
        /// Creates a shooter team payment on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateShooterTeamPayment")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(UserTeamPaymentContract), 200)]
        public async Task<IActionResult> CreateShooterTeamPayment([FromBody]ShooterTeamPaymentCreateRequest request)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            User shooter = null;
            if (!string.IsNullOrEmpty(request.ShooterId))
            {
                shooter = BasicLayer.GetUser(request.ShooterId);
                if(shooter == null)
                {
                    validations.Add(new ValidationResult("Not found",nameof(request.ShooterId).AsList()));
                }
            }
            var team = BasicLayer.GetTeam(request.TeamId);

            if(team == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.TeamId).AsList()));
            }
            
            var paymentTypes = BasicLayer.FetchPaymentTypesFromTeamId(request.TeamId);

            var currentPaymentType = paymentTypes.FirstOrDefault(x=>x.Id == request.PaymentTypeId);

            if(currentPaymentType == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.PaymentTypeId).AsList()));
            }

            if (validations.Count > 0)
                return BadRequest(validations);

            //Creazione modello richiesto da admin
            var model = new TeamPayment
            {
                UserId = request.ShooterId ?? "",
                TeamId = request.TeamId,
                Amount = request.Amount,
                Reason = request.Reason,
                PaymentType = currentPaymentType.Name,
                PaymentDateTime = request.PaymentDateTime
            };

            TeamReminder reminder = null;
            if (request.ExpireDateTime.HasValue)
            {
                reminder = new TeamReminder()
                {
                    Reason = shooter != null ? $"{shooter.LastName} {shooter.FirstName} - {request.Reason}" : request.Reason,
                    TeamId = request.TeamId,
                    UserId=request.ShooterId,
                    ExpireDateTime = request.ExpireDateTime.Value,
                    NotifyExpiration = request.NotifyExpiration
                };
            }

            //Invocazione del service layer
            validations = await BasicLayer.CreateShooterTeamPayment(model, reminder, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing shooter team payment
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateShooterTeamPayment")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(UserTeamPaymentContract), 200)]
        public async Task<IActionResult> UpdateShooterTeamPayment([FromBody]ShooterTeamPaymentUpdateRequest request)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            var shooter = BasicLayer.GetUser(request.ShooterId);
            if(shooter == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.ShooterId).AsList()));
            }
            var team = BasicLayer.GetTeam(request.TeamId);

            if(team == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.TeamId).AsList()));
            }
             var paymentTypes = BasicLayer.FetchPaymentTypesFromTeamId(request.TeamId);

            var currentPaymentType = paymentTypes.FirstOrDefault(x=>x.Id == request.PaymentTypeId);

            if(currentPaymentType == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.PaymentTypeId).AsList()));
            }

            if (validations.Count > 0)
                return BadRequest(validations);

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterTeamPayment(request.ShooterTeamPaymentId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.TeamId = request.TeamId;
            entity.UserId = request.ShooterId;
            entity.Amount = request.Amount;
            entity.PaymentType = currentPaymentType.Name;
            entity.Reason = request.Reason;
            entity.PaymentDateTime = request.PaymentDateTime;

            //Salvataggio
            validations = await BasicLayer.UpdateShooterTeamPayment(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a shooterTeamPayment on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooterTeamPayment")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public Task<IActionResult> DeleteShooterTeamPayment([FromBody]ShooterTeamPaymentRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterTeamPayment(request.ShooterTeamPaymentId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());

            }
            //Invocazione del service layer
            var validations = BasicLayer.DeleteShooterTeamPayment(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(new OkResponse { Status = true });
        }
    }
}
