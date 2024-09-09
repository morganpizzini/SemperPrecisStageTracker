using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// Controller for place
    /// </summary>
    [ApiVersion("2.0")]
    public class PlaceController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all places
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<PlaceContract>), 200)]
        public Task<IActionResult> FetchPlaces(TakeSkipRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllPlaces().AsQueryable();
            var total = entities.Count();

            if (request.Skip.HasValue)
                entities = entities.Skip(request.Skip.Value);

            if (request.Take.HasValue)
                entities = entities.Take(request.Take.Value);

            var userIds = entities.Select(x => x.Id).ToList();

            var userDatas = BasicLayer.FetchUserDataByUserIds(userIds);

            var datas = BasicLayer.FetchAllMinimunPlacesData();

            //Ritorno i contratti
            return Reply(
                new BaseResponse<IList<PlaceContract>>(
                    entities.As(x => ContractUtils.GenerateContract(x, datas.FirstOrDefault(d => d.PlaceId == x.Id))),
                    total,
                    request.Take.HasValue ?
                        Url.Action(action: nameof(FetchPlaces), controller: "Place", new { take = request.Take, skip = request.Take + (request?.Skip ?? 0) }) :
                        string.Empty
                ));
        }
        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PlaceContract), 200)]
        public IActionResult GetPlace(BaseRequestId request)
        {
            var entity = BasicLayer.GetPlace(request.Id);
            var data = BasicLayer.GetPlaceData(request.Id);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            //Serializzazione e conferma
            return Ok(ContractUtils.GenerateContract(entity, data));
        }

        /// <summary>
        /// Creates a place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.CreatePlaces)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> CreatePlace([FromBody] PlaceCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Place
            {
                Name = request.Name,
                IsActive = request.IsActive
            };

            var data = new PlaceData
            {
                Holder = request.Holder,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                City = request.City,
                Region = request.Region,
                PostalCode = request.PostalCode,
                Country = request.Country
            };

            //Invocazione del service layer
            var validations = await BasicLayer.CreatePlace(model, data, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return CreatedAtAction(nameof(GetPlace), model.GetRouteIdentifier(), ContractUtils.GenerateContract(model, data));
        }

        /// <summary>
        /// Updates existing place
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPut("UpdatePlace")]
        [ApiAuthorizationFilter(Permissions.EditPlace, Permissions.ManagePlaces)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> UpdatePlace(BaseRequestId<PlaceUpdateRequest> request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetPlace(request.Id);
            var data = BasicLayer.GetPlaceData(request.Id);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            if (data == null) { }
            data = new PlaceData
            {
                PlaceId = entity.Id
            };

            //Aggiornamento dell'entità
            entity.Name = request.Body.Name;
            entity.IsActive = request.Body.IsActive;

            data.Holder = request.Body.Holder;
            data.Phone = request.Body.Phone;
            data.Email = request.Body.Email;
            data.Address = request.Body.Address;
            data.City = request.Body.City;
            data.Region = request.Body.Region;
            data.PostalCode = request.Body.PostalCode;
            data.Country = request.Body.Country;

            //Salvataggio
            var validations = await BasicLayer.UpdatePlace(entity, data, PlatformUtils.GetIdentityUserId(User));
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
        [ApiAuthorizationFilter(Permissions.ManagePlaces,Permissions.PlaceDelete)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> DeletePlace(BaseRequestId request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetPlace(request.Id);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
                return NotFound();

            //Invocazione del service layer
            var validations = await BasicLayer.DeletePlace(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return NoContent();
        }
    }
}