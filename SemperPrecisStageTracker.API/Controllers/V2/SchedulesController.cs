using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.API.Models;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers.V2
{
    /// <summary>
    /// Muovere tutto sotto controller place
    /// </summary>
    [ApiVersion("2.0")]
    public class SchedulesController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list
        /// </summary>
        /// <returns>Returns action result</returns>
        //[HttpGet]
        //[ProducesResponseType(typeof(IList<ScheduleContract>), 200)]
        //public Task<IActionResult> Fetch(EntityTakeSkipRequest request)
        //{
        //    //Recupero la lista dal layer
        //    var entities = BasicLayer.FetchAllSchedules(request.RefId).AsQueryable();
        //    var total = entities.Count();

        //    if (request.Skip.HasValue)
        //        entities = entities.Skip(request.Skip.Value);

        //    if (request.Take.HasValue)
        //        entities = entities.Take(request.Take.Value);

        //    //Ritorno i contratti
        //    return Reply(
        //        new BaseResponse<IList<ScheduleContract>>(
        //            entities.As(ContractUtils.GenerateContract),
        //            total,
        //            request.Take.HasValue ?
        //                Url.Action(action: nameof(Fetch), controller: "Schedules", new { take = request.Take, refId= request.RefId, skip = request.Take + (request?.Skip ?? 0) }) :
        //                string.Empty
        //        ));
        //}
        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ScheduleContract), 200)]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.EditPlace)]
        public IActionResult Get(GetScheduleRequest request)
        {
            var entity = BasicLayer.GetSchedule(request.Id);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            //Serializzazione e conferma
            return Ok(new BaseResponse<ScheduleContract>(ContractUtils.GenerateContract(entity)));
        }

        /// <summary>
        /// Creates a place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.EditPlace)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Create([FromBody] ScheduleCreateRequest request)
        {
            var existingPlace = BasicLayer.GetPlace(request.PlaceId);
            if (existingPlace == null)
                return BadRequest(new List<ValidationResult> { new ValidationResult($"Place {request.PlaceId} not found") });

            //Creazione modello richiesto da admin
            var model = new Schedule
            {
                Name = request.Name,
                PlaceId = request.PlaceId,
                Description = request.Description,
                From = request.From,
                To = request.To,
                Day = request.Day
            };

            //Invocazione del service layer
            var validations = await BasicLayer.CreateSchedule(model, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return CreatedAtAction(nameof(Get), model.GetRouteIdentifier(), ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing place
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPut("{id}")]
        [ApiAuthorizationFilter(Permissions.EditPlace, Permissions.ManagePlaces)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Update(BaseRequestId<ScheduleUpdateRequest> request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetSchedule(request.Id);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Body.Name;
            entity.Description = request.Body.Description;
            entity.From = request.Body.From;
            entity.To = request.Body.To;
            entity.Day = request.Body.Day;

            //Salvataggio
            var validations = await BasicLayer.UpdateSchedule(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return NoContent();
        }

        /// <summary>
        /// Deletes existing place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpDelete("{id}")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces,Permissions.EditPlace)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Delete(DeleteEntityRefRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetSchedule(request.Id);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
                return NotFound();

            //Invocazione del service layer
            var validations = await BasicLayer.DeleteSchedule(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return NoContent();
        }
    }
}