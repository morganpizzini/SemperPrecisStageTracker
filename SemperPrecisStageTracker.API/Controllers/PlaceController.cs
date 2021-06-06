﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
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

            //Ritorno i contratti
            return Reply(entities.As(ContractUtils.GenerateContract));
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

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreatePlace")]
        [ProducesResponseType(typeof(PlaceContract), 200)]
        public Task<IActionResult> CreatePlace(PlaceCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Place
            {
                Name = request.Name,
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
            var validations = BasicLayer.CreatePlace(model);

            if (validations.Count > 0)
                return BadRequestTask(validations);


            //Return contract
            return Reply(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing place
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdatePlace")]
        [ProducesResponseType(typeof(PlaceContract), 200)]
        public Task<IActionResult> UpdatePlace(PlaceUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetPlace(request.PlaceId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());;

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.Holder = request.Holder;
            entity.Phone = request.Phone;
            entity.Email = request.Email;
            entity.Address = request.Address;
            entity.City = request.City;
            entity.Region = request.Region;
            entity.PostalZipCode = request.PostalZipCode;
            entity.Country = request.Country;
            
            //Salvataggio
            var validations = BasicLayer.UpdatePlace(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);


            //Confermo
            return Reply(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing place on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeletePlace")]
        [ProducesResponseType(typeof(PlaceContract), 200)]
        public Task<IActionResult> DeletePlace(PlaceRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetPlace(request.PlaceId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());;

            }

            //Invocazione del service layer
            var validations = BasicLayer.DeletePlace(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(ContractUtils.GenerateContract(entity));
        }
    }
}