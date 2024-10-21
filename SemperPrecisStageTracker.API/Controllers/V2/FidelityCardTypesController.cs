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
    /// muovere fidelityCardTypes sotto place
    /// </summary>
    [ApiVersion("2.0")]
    public partial class PlacesController : ApiControllerBase
    {
        [HttpGet("{id}/fidelityCardTypes")]
        [ProducesResponseType(typeof(IList<FidelityCardTypeContract>), 200)]
        public Task<IActionResult> FetchFidelityCardType(TakeSkipBaseRequestId request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllFidelityCardTypes(request.Id).AsQueryable();
            var total = entities.Count();

            if (request.Skip.HasValue)
                entities = entities.Skip(request.Skip.Value);

            if (request.Take.HasValue)
                entities = entities.Take(request.Take.Value);
            
            
            //Ritorno i contratti
            return Reply(
                new BaseResponse<IList<FidelityCardTypeContract>>(
                    entities.As(x=>ContractUtils.GenerateContract(x)),
                    total,
                    request.Take.HasValue ?
                        Url.Action(action: nameof(Fetch), controller: "FidelityCardType", new { take = request.Take, skip = request.Take + (request?.Skip ?? 0) }) :
                        string.Empty
                ));
        }
        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpGet("{id}/fidelityCardTypes/{fidelityCardTypeId}")]
        [ProducesResponseType(typeof(FidelityCardTypeContract), 200)]
        public IActionResult Get(FidelityCardTypeEntityBaseRequestId request)
        {
            var entity = BasicLayer.GetFidelityCardType(request.FidelityCardTypeId);

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
        [HttpPost("{id}/fidelityCardTypes")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.EditPlace)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Create(EntityBaseRequestId<FidelityCardTypeCreateRequest> request)
        {
            var existingPlace = BasicLayer.GetPlace(request.Id);
            if (existingPlace == null)
                return BadRequest(new List<ValidationResult> { new ValidationResult($"Place {request.Id} not found") });

            //Creazione modello richiesto da admin
            var model = new FidelityCardType
            {
                Name = request.Body.Name,
                PlaceId = request.Id,
                MaxAccessNumber = request.Body.MaxAccessNumber
            };

            //Invocazione del service layer
            var validations = await BasicLayer.CreateFidelityCardType(model, PlatformUtils.GetIdentityUserId(User));

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
        [HttpPost("{id}/fidelityCardTypes/{fidelityCardTypeId}")]
        [ApiAuthorizationFilter(Permissions.EditPlace, Permissions.ManagePlaces)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Update(FidelityCardTypeEntityBaseRequestId<FidelityCardTypeUpdateRequest> request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetFidelityCardType(request.FidelityCardTypeId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null || entity.PlaceId != request.Id)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Body.Name;
            entity.MaxAccessNumber = request.Body.MaxAccessNumber;

            //Salvataggio
            var validations = await BasicLayer.UpdateFidelityCardType(entity, PlatformUtils.GetIdentityUserId(User));
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
        [HttpDelete("{id}/fidelityCardTypes/{fidelityCardTypeId}")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces,Permissions.EditPlace)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Delete(FidelityCardTypeEntityBaseRequestId request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetFidelityCardType(request.FidelityCardTypeId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null || entity.PlaceId != request.Id)
                return NotFound();

            //Invocazione del service layer
            var validations = await BasicLayer.DeleteFidelityCardType(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return NoContent();
        }
    }
}