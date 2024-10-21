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
    public partial class PlacesController : ApiControllerBase
    {
        [HttpGet("{placeId}/bays/{id}/schedules")]
        [ProducesResponseType(typeof(BayContract), 200)]
        public IActionResult FetchSchedules(BaseRequestId request)
        {
            var entity = BasicLayer.GetBay(request.Id);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            var entities = BasicLayer.FetchAllSchedulesAssignedToBay(request.Id);

            //Serializzazione e conferma
            //Ritorno i contratti
            return Ok(
                new BaseResponse<IList<ScheduleContract>>(
                    entities.As(ContractUtils.GenerateContract),
                    entities.Count,
                    string.Empty
                ));
        }

        [HttpGet("{id}/bays-schedules")]
        [ProducesResponseType(typeof(BayContract), 200)]
        public IActionResult FetchBaySchedules(BaseRequestId request)
        {
            var entityIds = BasicLayer.FetchAllBays(request.Id).Select(x=>x.Id).ToList();

            var entities = BasicLayer.FetchAllSchedulesAssignedToBay(entityIds);

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
        [HttpPost("{id}/bays/{bayId}/schedules")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.EditPlace)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Create(BayEntityBaseRequestId<ScheduleBayCreateRequest> request)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();
            var existingPlace = BasicLayer.GetPlace(request.Id);
            if (existingPlace == null)
                validations.Add(new ValidationResult($"Place {request.Id} not found"));

            var existingBay = BasicLayer.GetBay(request.BayId);
            if (existingBay == null)
                validations.Add(new ValidationResult($"Bay {request.BayId} not found"));

            var existingSchedule = BasicLayer.GetSchedule(request.Body.ScheduleId);
            if (existingSchedule == null)
                validations.Add(new ValidationResult($"Schedule {request.Body.ScheduleId} not found"));

            if(validations.Count>0)
                return BadRequest(validations);

            //Creazione modello richiesto da admin
            var model = new BaySchedule
            {
                BayId = request.BayId,
                ScheduleId = request.Body.ScheduleId
            };

            //Invocazione del service layer
            validations = await BasicLayer.CreateBaySchedule(model,request.Id, PlatformUtils.GetIdentityUserId(User));

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
        [HttpDelete("{placeId}/bays/{id}/schedules/{scheduleId}")]
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
            var validations = await BasicLayer.DeleteBaySchedule(entity, request.PlaceId, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return NoContent();
        }
    }
}