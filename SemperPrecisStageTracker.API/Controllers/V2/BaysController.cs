using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers.V2
{
    /// <summary>
    /// muovere bays sotto place
    /// </summary>
    [ApiVersion("2.0")]
    public partial class PlacesController : ApiControllerBase
    {
        [HttpGet("{id}/bays")]
        [ProducesResponseType(typeof(IList<BayContract>), 200)]
        public Task<IActionResult> Fetch(TakeSkipBaseRequestId request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllBays(request.Id).AsQueryable();
            var total = entities.Count();

            if (request.Skip.HasValue)
                entities = entities.Skip(request.Skip.Value);

            if (request.Take.HasValue)
                entities = entities.Take(request.Take.Value);
            
            
            //Ritorno i contratti
            return Reply(
                new BaseResponse<IList<BayContract>>(
                    entities.As(ContractUtils.GenerateContract),
                    total,
                    request.Take.HasValue ?
                        Url.Action(action: nameof(Fetch), controller: "Bay", new { take = request.Take, skip = request.Take + (request?.Skip ?? 0) }) :
                        string.Empty
                ));
        }
        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpGet("{id}/bays/{bayId}")]
        [ProducesResponseType(typeof(BayContract), 200)]
        public IActionResult Get(BayEntityBaseRequestId request)
        {
            var entity = BasicLayer.GetBay(request.BayId);

            //verifico validità dell'entità
            if (entity == null || entity.PlaceId != request.Id)
                return NotFound();

            //Serializzazione e conferma
            return OkBaseResponse(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost("{id}/bays")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.EditPlace)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Create(EntityBaseRequestId<BayCreateRequest> request)
        {
            var existingPlace = BasicLayer.GetPlace(request.Id);
            if (existingPlace == null)
                return BadRequest(new List<ValidationResult> { new ValidationResult($"Place {request.Id} not found") });

            //Creazione modello richiesto da admin
            var model = new Bay
            {
                Name = request.Body.Name,
                PlaceId = request.Id,
                Description = request.Body.Description,
                HasPrivateReservation = request.Body.HasPrivateReservation
            };

            //Invocazione del service layer
            var validations = await BasicLayer.CreateBay(model, PlatformUtils.GetIdentityUserId(User));

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
        [HttpPost("{id}/bays/{bayId}")]
        [ApiAuthorizationFilter(Permissions.EditPlace, Permissions.ManagePlaces)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Update(BayEntityBaseRequestId<BayUpdateRequest> request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetBay(request.BayId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null || entity.PlaceId != request.Id)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Body.Name;
            entity.Description = request.Body.Description;
            entity.HasPrivateReservation = request.Body.HasPrivateReservation;

            //Salvataggio
            var validations = await BasicLayer.UpdateBay(entity, PlatformUtils.GetIdentityUserId(User));
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
        [HttpDelete("{id}/bays/{bayId}")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces,Permissions.EditPlace)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Delete(BayEntityBaseRequestId request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetBay(request.BayId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null || entity.PlaceId != request.Id)
                return NotFound();

            //Invocazione del service layer
            var validations = await BasicLayer.DeleteBay(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return NoContent();
        }
    }
}