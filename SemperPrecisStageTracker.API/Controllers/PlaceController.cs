using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// Controller for place
    /// </summary>
    public class PlaceController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all places
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllPlaces")]
        [ProducesResponseType(typeof(IList<PlaceContract>), 200)]
        public Task<IActionResult> FetchAllPlaces()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllPlaces();

            var datas = BasicLayer.FetchAllMinimunPlacesData();

            //Ritorno i contratti
            return Reply(entities.As(x=>ContractUtils.GenerateContract(x,datas.FirstOrDefault(d=>d.PlaceId== x.Id))));
        }
        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetPlace")]
        [ProducesResponseType(typeof(PlaceContract), 200)]
        public Task<IActionResult> GetPlace(PlaceRequest request)
        {
            var entity = BasicLayer.GetPlace(request.PlaceId);
            var data = BasicLayer.GetPlaceData(request.PlaceId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity,data));
        }

        /// <summary>
        /// Creates a place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreatePlace")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces, Permissions.CreatePlaces)]
        [ProducesResponseType(typeof(PlaceContract), 200)]
        public async Task<IActionResult> CreatePlace(PlaceCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Place
            {
                Name = request.Name
            };

            var data = new PlaceData
            {
                Holder = request.Holder,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                City = request.City,
                Region = request.Region,
                PostalZipCode = request.PostalZipCode,
                Country = request.Country
            };

            //Invocazione del service layer
            var validations = await BasicLayer.CreatePlace(model,data, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(model,data));
        }

        /// <summary>
        /// Updates existing place
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdatePlace")]
        [ApiAuthorizationFilter(Permissions.EditPlace, Permissions.ManagePlaces )]
        [ProducesResponseType(typeof(PlaceContract), 200)]
        public async Task<IActionResult> UpdatePlace([EntityId] PlaceUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetPlace(request.PlaceId);
            var data = BasicLayer.GetPlaceData(request.PlaceId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            if (data == null){}
                data = new PlaceData
                {
                    PlaceId = entity.Id
                };

            //Aggiornamento dell'entità
            entity.Name = request.Name;

            data.Holder = request.Holder;
            data.Phone = request.Phone;
            data.Email = request.Email;
            data.Address = request.Address;
            data.City = request.City;
            data.Region = request.Region;
            data.PostalZipCode = request.PostalZipCode;
            data.Country = request.Country;

            //Salvataggio
            var validations = await BasicLayer.UpdatePlace(entity,data, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return Ok(ContractUtils.GenerateContract(entity,data));
        }

        /// <summary>
        /// Deletes existing place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeletePlace")]
        [ApiAuthorizationFilter(Permissions.ManagePlaces )]
        [ProducesResponseType(typeof(PlaceContract), 200)]
        public async Task<IActionResult> DeletePlace([EntityId] PlaceRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetPlace(request.PlaceId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }

            //Invocazione del service layer
            var validations = await BasicLayer.DeletePlace(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}