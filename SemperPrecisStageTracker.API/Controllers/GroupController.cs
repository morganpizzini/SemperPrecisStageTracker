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
    /// Controller for group
    /// </summary>
    public class GroupController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all groups
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllGroupsByMatchId")]
        [ProducesResponseType(typeof(IList<GroupContract>), 200)]
        public IActionResult FetchAllGroups(MatchRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllGroupsByMatchId(request.MatchId);


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
        [Route("GetGroup")]
        [ProducesResponseType(typeof(GroupContract), 200)]
        public IActionResult GetGroup(GroupRequest request)
        {
            var entity = BasicLayer.GetGroup(request.GroupId);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            var shooters = BasicLayer.FetchShootersByGroupId(entity.Id);

            var match = BasicLayer.GetMatch(entity.Id);

            //Serializzazione e conferma
            return Ok(ContractUtils.GenerateContract(entity,match,shooters));
        }

        /// <summary>
        /// Creates a group on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateGroup")]
        [ProducesResponseType(typeof(GroupContract), 200)]
        public IActionResult CreateGroup(GroupCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Group
            {
                Name = request.Name,
                MatchId = request.MatchId
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateGroup(model);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing group
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateGroup")]
        [ProducesResponseType(typeof(GroupContract), 200)]
        public IActionResult UpdateGroup(GroupUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetGroup(request.GroupId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            
            //Salvataggio
            var validations = BasicLayer.UpdateGroup(entity);
            if (validations.Count > 0)
                return BadRequest(validations);


            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing group on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteGroup")]
        [ProducesResponseType(typeof(GroupContract), 200)]
        public IActionResult DeleteGroup(GroupRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetGroup(request.GroupId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();
            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteGroup(entity);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
