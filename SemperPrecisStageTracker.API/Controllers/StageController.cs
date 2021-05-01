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
    /// Controller for stage
    /// </summary>
    public class StageController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all stages
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllStages")]
        [ProducesResponseType(typeof(IList<StageContract>), 200)]
        public IActionResult FetchAllStages(MatchRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllStages(request.MatchId);


            //seleziono gli id dei posti
            var matchIds = entities.Select(x => x.MatchId).ToList();

            //recupero gli utenti
            var matches = this.BasicLayer.FetchMatchsByIds(matchIds);

            //Ritorno i contratti
            return Ok(entities.As(x =>
            {
                return ContractUtils.GenerateContract(x, matches.FirstOrDefault(p => p.Id == x.MatchId));
            }));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetStage")]
        [ProducesResponseType(typeof(StageContract), 200)]
        public IActionResult GetStage(StageRequest request)
        {
            var entity = BasicLayer.GetStage(request.StageId);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            //Serializzazione e conferma
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a stage on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateStage")]
        [ProducesResponseType(typeof(StageContract), 200)]
        public IActionResult CreateStage(StageCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Stage
            {
                Name = request.Name,
                Targets = request.Targets,
                MatchId = request.MatchId
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateStage(model);

            if (validations.Count > 0)
                return BadRequest(validations);


            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing stage
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateStage")]
        [ProducesResponseType(typeof(StageContract), 200)]
        public IActionResult UpdateStage(StageUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetStage(request.StageId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.Targets = request.Targets;
            
            //Salvataggio
            var validations = BasicLayer.UpdateStage(entity);
            if (validations.Count > 0)
                return BadRequest(validations);


            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing stage on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteStage")]
        [ProducesResponseType(typeof(StageContract), 200)]
        public IActionResult DeleteStage(StageRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetStage(request.StageId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteStage(entity);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
