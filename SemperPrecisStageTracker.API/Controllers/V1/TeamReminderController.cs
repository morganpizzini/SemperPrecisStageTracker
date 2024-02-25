using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Extensions;


namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for TeamReminder
    /// </summary>
    [ApiVersion("1.0")]
    public class TeamReminderController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all TeamReminders
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllTeamReminders")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment)]
        [ProducesResponseType(typeof(IList<TeamReminderContract>), 200)]
        public async Task<IActionResult> FetchAllTeamReminders([FromBody]TeamRequest request)
        {
            //Recupero la lista dal layer
            var entities = await  BasicLayer.FetchAllTeamReminders(request.TeamId,PlatformUtils.GetIdentityUserId(User));

            //Ritorno i contratti
            return Ok(entities.As(x=>ContractUtils.GenerateContract(x)));
        }
        /// <summary>
        /// Get specific TeamRemindert ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetTeamReminder")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment)]
        [ProducesResponseType(typeof(TeamReminderContract), 200)]
        public Task<IActionResult> GetTeamReminder([FromBody]TeamReminderRequest request)
        {
            var entity = BasicLayer.GetTeamReminder(request.TeamReminderId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a TeamReminder on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateTeamReminder")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(TeamReminderContract), 200)]
        public async Task<IActionResult> CreateTeamReminder([FromBody]TeamReminderCreateRequest request)
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

            if (validations.Count > 0)
                return BadRequest(validations);

            //Creazione modello richiesto da admin
            var model = new TeamReminder
            {
                UserId = request.ShooterId,
                TeamId = request.TeamId,
                ExpireDateTime = request.ExpireDateTime,
                NotifyExpiration = request.NotifyExpiration,
                Reason = request.Reason
            };

            //Invocazione del service layer
            validations = await BasicLayer.CreateTeamReminder(model, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing TeamReminder
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateTeamReminder")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(TeamReminderContract), 200)]
        public async Task<IActionResult> UpdateTeamReminder([FromBody]TeamReminderUpdateRequest request)
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

            if (validations.Count > 0)
                return BadRequest(validations);

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetTeamReminder(request.TeamReminderId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.UserId = request.ShooterId;
            entity.TeamId = request.TeamId;
            entity.ExpireDateTime = request.ExpireDateTime;
            entity.NotifyExpiration = request.NotifyExpiration;
            entity.Reason = request.Reason;
           

            //Salvataggio
            validations = await BasicLayer.UpdateTeamReminder(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing TeamReminder on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteTeamReminder")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(TeamReminderContract), 200)]
        public async Task<IActionResult> DeleteTeamReminder([FromBody]TeamReminderRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetTeamReminder(request.TeamReminderId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();
            }
            //Invocazione del service layer
            var validations = await BasicLayer.DeleteTeamReminder(entity, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}