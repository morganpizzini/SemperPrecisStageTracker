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
    /// Controller for team
    /// </summary>
    public class TeamController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all teams
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllTeams")]
        [ProducesResponseType(typeof(IList<TeamContract>), 200)]
        public IActionResult FetchAllTeams()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllTeams();

            //Ritorno i contratti
            return Ok(entities.As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetTeam")]
        [ProducesResponseType(typeof(TeamContract), 200)]
        public IActionResult GetTeam(TeamRequest request)
        {
            var entity = BasicLayer.GetTeam(request.TeamId);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            //Serializzazione e conferma
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a team on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateTeam")]
        [ProducesResponseType(typeof(TeamContract), 200)]
        public IActionResult CreateTeam(TeamCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Team
            {
                Name = request.Name
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateTeam(model);

            if (validations.Count > 0)
                return BadRequest(validations);


            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing team
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateTeam")]
        [ProducesResponseType(typeof(TeamContract), 200)]
        public IActionResult UpdateTeam(TeamUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetTeam(request.TeamId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            
            //Salvataggio
            var validations = BasicLayer.UpdateTeam(entity);
            if (validations.Count > 0)
                return BadRequest(validations);


            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing team on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteTeam")]
        [ProducesResponseType(typeof(TeamContract), 200)]
        public IActionResult DeleteTeam(TeamRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetTeam(request.TeamId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteTeam(entity);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
