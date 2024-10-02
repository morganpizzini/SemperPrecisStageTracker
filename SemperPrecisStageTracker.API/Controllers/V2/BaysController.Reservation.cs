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
        [HttpGet("{placeId}/bays/{id}/reservations")]
        [ProducesResponseType(typeof(BayContract), 200)]
        public IActionResult FetchReservations(BaseRequestId request)
        {
            var entity = BasicLayer.GetBay(request.Id);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            var entities = BasicLayer.FetchAllReservations(request.Id, DateTime.Now);

            var userIds = entities.Select(x => x.UserId).ToList();
            var users = BasicLayer.FetchUsersByIds(userIds);
            //Serializzazione e conferma
            //Ritorno i contratti
            return Ok(
                new BaseResponse<IList<ReservationContract>>(
                    entities.As(x=>ContractUtils.GenerateContract(x,null,users.FirstOrDefault(u=>u.Id == x.UserId))),
                    entities.Count,
                    string.Empty
                ));
        }

        /// <summary>
        /// Creates a place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost("{placeId}/bays/{id}/reservations")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Create(BaseRequestId<ReservationCreateRequest> request)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();
            
            var userId = PlatformUtils.GetIdentityUserId(User);

            var existingBay = BasicLayer.GetBay(request.Id);
            if (existingBay == null)
                validations.Add(new ValidationResult($"Bay {request.Id} not found"));

            if(validations.Count>0)
                return BadRequest(validations);

            //Creazione modello richiesto da admin
            var model = new Reservation
            {
                UserId = string.IsNullOrEmpty(request.Body.UserId) ? userId : request.Body.UserId,
                BayId = request.Id,
                Demands = request.Body.Demands,
                From = request.Body.From,
                To = request.Body.To,
                Day = request.Body.Day
            };

            //Invocazione del service layer
            validations = await BasicLayer.CreateReservation(model, userId);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return CreatedAtAction(nameof(FetchReservations), new { placeId = existingBay.PlaceId,id = request.Id }, new BaseResponse<ReservationContract>(ContractUtils.GenerateContract(model)));
        }

        [HttpPut("{placeId}/bays/{id}/reservations/{reservationId}")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> UpdateReservation(ReservationUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetReservation(request.ReservationId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            var userId = PlatformUtils.GetIdentityUserId(User);
            if (entity.UserId != userId)
                return BadRequest(new List<ValidationResult>{new ValidationResult("User has no permissions to update reservation") });

            //Aggiornamento dell'entità
            entity.From = request.Body.From;
            entity.To = request.Body.To;
            entity.Day = request.Body.Day;
            entity.Demands = request.Body.Demands;

            //Salvataggio
            var validations = await BasicLayer.UpdateReservation(entity, userId);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return NoContent();
        }

        [HttpPut("{placeId}/bays/{id}/reservations/{reservationId}/status")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.EditPlace)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> UpdateReservationStatus(ReservationStatusUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetReservation(request.ReservationId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            var userId = PlatformUtils.GetIdentityUserId(User);
            
            //Aggiornamento dell'entità
            entity.IsAccepted = request.Body.Status;

            //Salvataggio
            var validations = await BasicLayer.UpdateReservation(entity, userId);
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
        [HttpDelete("{placeId}/bays/{id}/reservations/{reservationId}")]
        [HttpDelete("{placeId}/bays/{id}/reservations/{reservationId}/block")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> DeleteBayReservation(ReservationDeleteRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetReservation(request.ReservationId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
                return NotFound();

            //Invocazione del service layer
            var validations = await BasicLayer.DeleteReservation(entity, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return NoContent();
        }
    }
}