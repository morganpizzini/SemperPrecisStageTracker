using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using Microsoft.AspNetCore.Authorization;

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
        public Task<IActionResult> FetchAllMatchs()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllMatchs();
            var associationIds = entities.Select(x=>x.AssociationId).ToList();

            var associations = BasicLayer.FetchAssociationsByIds(associationIds);

            //Ritorno i contratti
            return Reply(entities.As(x=>ContractUtils.GenerateContract(x,associations.FirstOrDefault(p => p.Id == x.AssociationId))));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> GetMatch(MatchRequest request)
        {
            var entity = BasicLayer.GetMatch(request.MatchId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());;

            var groups = BasicLayer.FetchAllGroupsByMatchId(entity.Id);
            var stages = BasicLayer.FetchAllStagesByMatchId(entity.Id);

            var association = BasicLayer.GetAssociation(entity.AssociationId);
            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity,association,groups,stages));
        }

                /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GetMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> GetMatchFromShortLink(MatchRequest request)
        {
            var entity = BasicLayer.GetMatchFromShortLink(request.MatchId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());;

            var groups = BasicLayer.FetchAllGroupsByMatchId(entity.Id);
            var stages = BasicLayer.FetchAllStagesByMatchId(entity.Id);

            var association = BasicLayer.GetAssociation(entity.AssociationId);
            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity,association,groups,stages));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GetMatchStats")]
        [ProducesResponseType(typeof(IList<DivisionMatchResultContract>), 200)]
        public Task<IActionResult> GetMatchStats(MatchRequest request)
        {
            var entities = BasicLayer.GetMatchStats(request.MatchId);
            //Serializzazione e conferma
            return Reply(entities.As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Creates a match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> CreateMatch(MatchCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Match
            {
                Name = request.Name,
                MatchDateTime = request.MatchDateTime,
                AssociationId= request.AssociationId,
                Location = request.Location,
                OpenMatch = request.OpenMatch,
                UnifyRanks = request.UnifyRanks
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateMatch(model);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            var association = BasicLayer.GetAssociation(model.AssociationId);

            //Return contract
            return Reply(ContractUtils.GenerateContract(model,association));
        }

        /// <summary>
        /// Updates existing match
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> UpdateMatch(MatchUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetMatch(request.MatchId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.MatchDateTime = request.MatchDateTime;
            entity.AssociationId = request.AssociationId;
            entity.Location = request.Location;
            entity.OpenMatch = request.OpenMatch;
            entity.UnifyRanks = request.UnifyRanks;

            //Salvataggio
            var validations = BasicLayer.UpdateMatch(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);

            var association = BasicLayer.GetAssociation(entity.AssociationId);
            //Confermo
            return Reply(ContractUtils.GenerateContract(entity,association));
        }

        /// <summary>
        /// Deletes existing match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> DeleteMatch(MatchRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetMatch(request.MatchId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());;
            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteMatch(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);

            var association = BasicLayer.GetAssociation(entity.AssociationId);

            //Return contract
            return Reply(ContractUtils.GenerateContract(entity,association));
        }
    }
}
