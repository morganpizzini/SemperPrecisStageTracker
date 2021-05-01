using System.Collections.Generic;
using System.Linq;
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
    /// Controller for shooter
    /// </summary>
    public class ShooterController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all shooters
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllShooters")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult FetchAllShooters()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllShooters();

            //Ritorno i contratti
            return Ok(entities.As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetShooter")]
        [ProducesResponseType(typeof(ShooterContract), 200)]
        public IActionResult GetShooter(ShooterRequest request)
        {
            var entity = BasicLayer.GetShooter(request.ShooterId);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            //Serializzazione e conferma
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a shooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateShooter")]
        [ProducesResponseType(typeof(ShooterContract), 200)]
        public IActionResult CreateShooter(ShooterCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Shooter
            {
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateShooter(model);

            if (validations.Count > 0)
                return BadRequest(validations);


            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing shooter
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateShooter")]
        [ProducesResponseType(typeof(ShooterContract), 200)]
        public IActionResult UpdateShooter(ShooterUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooter(request.ShooterId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            
            //Salvataggio
            var validations = BasicLayer.UpdateShooter(entity);
            if (validations.Count > 0)
                return BadRequest(validations);


            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing shooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooter")]
        [ProducesResponseType(typeof(ShooterContract), 200)]
        public IActionResult DeleteShooter(ShooterRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooter(request.ShooterId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteShooter(entity);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
