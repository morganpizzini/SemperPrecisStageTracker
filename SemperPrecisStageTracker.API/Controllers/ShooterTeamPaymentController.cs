using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for shooterTeamPayment
    /// </summary>
    public class ShooterTeamPaymentController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterTeamPaymentByTeam")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> FetchShooterTeamPaymentByTeam(TeamRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchShooterTeamPaymentsFromTeamId(request.TeamId);
            var shooterIds = entities.Select(x => x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            //Return contract
            return Reply(entities.As(x => ContractUtils.GenerateContract(x, shooters.FirstOrDefault(t => t.Id == x.ShooterId))));
        }

        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterTeamPaymentByShooterAndTeam")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> FetchShooterTeamPaymentByShooterAndTeam(ShooterTeamRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchShooterTeamPaymentByTeamAndShooterId(request.TeamId, request.ShooterId);

            //Return contract
            return Reply(entities.As(x => ContractUtils.GenerateContract(x)));

        }

        /// <summary>
        /// Creates a shooter team payment on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateShooterTeamPayment")]
        [ProducesResponseType(typeof(ShooterTeamPaymentContract), 200)]
        public async Task<IActionResult> CreateShooterTeamPayment(ShooterTeamPaymentCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new ShooterTeamPayment
            {
                ShooterId = request.ShooterId,
                TeamId = request.TeamId,
                Amount = request.Amount,
                Reason = request.Reason,
                PaymentDateTime = request.PaymentDateTime,
                ExpireDateTime = request.ExpireDateTime,
                NotifyExpiration = request.NotifyExpiration
            };

            //Invocazione del service layer
            var validations = await BasicLayer.CreateShooterTeamPayment(model, PlatformUtils.GetIdentityUserId(User));

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
        [ProducesResponseType(typeof(ShooterTeamPaymentContract), 200)]
        public async Task<IActionResult> UpdateShooterTeamPayment([EntityId] ShooterTeamPaymentUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterTeamPayment(request.ShooterTeamPaymentId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.TeamId = request.TeamId;
            entity.ShooterId = request.ShooterId;
            entity.Amount = request.Amount;
            entity.Reason = request.Reason;
            entity.PaymentDateTime = request.PaymentDateTime;
            entity.ExpireDateTime = request.ExpireDateTime;
            entity.NotifyExpiration = request.NotifyExpiration;

            //Salvataggio
            var validations = await BasicLayer.UpdateShooterTeamPayment(entity, PlatformUtils.GetIdentityUserId(User));
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
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> DeleteShooterTeamPayment(ShooterTeamPaymentRequest request)
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
