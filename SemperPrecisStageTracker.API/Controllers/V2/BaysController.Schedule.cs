using System.ComponentModel.DataAnnotations;
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
    public partial class BaysController : ApiControllerBase
    {
        [HttpGet("{id}/schedules")]
        [ProducesResponseType(typeof(BayContract), 200)]
        public IActionResult FetchSchedules(BaseRequestId request)
        {
            var entity = BasicLayer.GetBay(request.Id);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            var entities = BasicLayer.FetchAllBaySchedules(request.Id);

            //Serializzazione e conferma
            //Ritorno i contratti
            return Ok(
                new BaseResponse<IList<ScheduleContract>>(
                    entities.As(ContractUtils.GenerateContract),
                    entities.Count,
                    string.Empty
                ));
        }

        /// <summary>
        /// Creates a place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost("{id}/schedules")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.EditPlace)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Create([FromBody]ScheduleBayCreateRequest request)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();
            var existingPlace = BasicLayer.GetPlace(request.PlaceId);
            if (existingPlace == null)
                validations.Add(new ValidationResult($"Place {request.PlaceId} not found"));

            var existingBay = BasicLayer.GetBay(request.BayId);
            if (existingBay == null)
                validations.Add(new ValidationResult($"Bay {request.BayId} not found"));

            var existingSchedule = BasicLayer.GetSchedule(request.ScheduleId);
            if (existingSchedule == null)
                validations.Add(new ValidationResult($"Schedule {request.ScheduleId} not found"));

            if(validations.Count>0)
                return BadRequest(validations);

            //Creazione modello richiesto da admin
            var model = new BaySchedule
            {
                BayId = request.BayId,
                ScheduleId = request.ScheduleId
            };

            //Invocazione del service layer
            validations = await BasicLayer.CreateBaySchedule(model,request.PlaceId, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return NoContent();
        }
        
        /// <summary>
        /// Deletes existing place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpDelete("{id}/schedules/{scheduleId}")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces,Permissions.EditPlace)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteBaySchedule(BayScheduleDeleteRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetBaySchedule(request.Id,request.ScheduleId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
                return NotFound();

            //Invocazione del service layer
            var validations = await BasicLayer.DeleteBaySchedule(entity, request.RefId, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return NoContent();
        }
    }
}