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
    /// Controller for match
    /// </summary>
    public class MatchController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllMatches")]
        [ProducesResponseType(typeof(IList<MatchContract>), 200)]
        public IActionResult FetchAllMatchs()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllMatchs();

            //Ritorno i contratti
            return Ok(entities.As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public IActionResult GetMatch(MatchRequest request)
        {
            var entity = BasicLayer.GetMatch(request.MatchId);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            //Serializzazione e conferma
            return Ok(ContractUtils.GenerateContract(entity));
        }
        
        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetMatchStats")]
        [ProducesResponseType(typeof(IList<ShooterMatchResultContract>), 200)]
        public IActionResult GetMatchStats(MatchRequest request)
        {
            var entities = BasicLayer.GetMatchStats(request.MatchId);
            //Serializzazione e conferma
            return Ok(entities.As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Creates a match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public IActionResult CreateMatch(MatchCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Match
            {
                Name = request.Name,
                MatchDateTime = request.MatchDateTime
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateMatch(model);

            if (validations.Count > 0)
                return BadRequest(validations);


            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing match
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public IActionResult UpdateMatch(MatchUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetMatch(request.MatchId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.MatchDateTime = request.MatchDateTime;
            
            //Salvataggio
            var validations = BasicLayer.UpdateMatch(entity);
            if (validations.Count > 0)
                return BadRequest(validations);


            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public IActionResult DeleteMatch(MatchRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetMatch(request.MatchId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteMatch(entity);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
