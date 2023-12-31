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
    /// Controller for PaymentType
    /// </summary>
    [ApiVersion("1.0")]
    public class PaymentTypeController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchPaymentTypeByTeam")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(IList<PaymentTypeContract>), 200)]
        public Task<IActionResult> FetchPaymentTypeByTeam([FromBody]TeamRequest request)
        {
            //Recupero l'elemento dal business layer
            var entities = BasicLayer.FetchPaymentTypesFromTeamId(request.TeamId);
            //Return contract
            return Reply(entities.As(x => ContractUtils.GenerateContract(x)));
        }

        /// <summary>
        /// Creates a shooter team payment on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreatePaymentType")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(PaymentTypeContract), 200)]
        public async Task<IActionResult> CreatePaymentType([FromBody]PaymentTypeCreateRequest request)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            var team = BasicLayer.GetTeam(request.TeamId);

            if(team == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.TeamId).AsList()));
            }
            if (validations.Count > 0)
                return BadRequest(validations);

            //Creazione modello richiesto da admin
            var model = new PaymentType
            {
                TeamId = request.TeamId,
                Name = request.Name
            };

            //Invocazione del service layer
            validations = await BasicLayer.CreatePaymentType(model, PlatformUtils.GetIdentityUserId(User));

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
        [Route("UpdatePaymentType")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(PaymentTypeContract), 200)]
        public async Task<IActionResult> UpdatePaymentType([FromBody]PaymentTypeUpdateRequest request)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            var team = BasicLayer.GetTeam(request.TeamId);

            if(team == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.TeamId).AsList()));
            }
            if (validations.Count > 0)
                return BadRequest(validations);

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetPaymentType(request.PaymentTypeId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.TeamId = request.TeamId;
            entity.Name = request.Name;

            //Salvataggio
            validations = await BasicLayer.UpdatePaymentType(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a PaymentType on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeletePaymentType")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public Task<IActionResult> DeletePaymentType([FromBody]PaymentTypeRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetPaymentType(request.PaymentTypeId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());

            }
            //Invocazione del service layer
            var validations = BasicLayer.DeletePaymentType(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(new OkResponse { Status = true });
        }
    }
}
