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
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for match
    /// </summary>
    public partial class MatchController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllMatches")]
        [ApiAuthorizationFilter(AdministrationPermissions.ManageMatches)]
        [ProducesResponseType(typeof(IList<MatchContract>), 200)]
        public Task<IActionResult> FetchAllMatches()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllMatches();
            var associationIds = entities.Select(x=>x.AssociationId).ToList();
            var placeIds = entities.Select(x=>x.PlaceId).ToList();

            var associations = BasicLayer.FetchAssociationsByIds(associationIds);
            var places = BasicLayer.FetchPlacesByIds(placeIds);

            //Ritorno i contratti
            return Reply(entities.As(x=>ContractUtils.GenerateContract(x,associations.FirstOrDefault(p => p.Id == x.AssociationId),places.FirstOrDefault(p => p.Id == x.PlaceId))));
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
                return Task.FromResult<IActionResult>(NotFound());

            var groups = BasicLayer.FetchAllGroupsByMatchId(entity.Id);
            var stages = BasicLayer.FetchAllStagesByMatchId(entity.Id);

            var association = BasicLayer.GetAssociation(entity.AssociationId);
            var place = BasicLayer.GetPlace(entity.PlaceId);
            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity,association,place,groups,stages));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GetMatchStats")]
        [ProducesResponseType(typeof(MatchStatsResultContract), 200)]
        public Task<IActionResult> GetMatchStats(MatchStatsRequest request)
        {
            Match entity;
            if (string.IsNullOrEmpty(request.MatchId))
            {
                if (string.IsNullOrEmpty(request.ShortLink))
                {
                    return Task.FromResult<IActionResult>(NotFound());
                }
                entity = BasicLayer.GetMatchFromShortLink(request.ShortLink);
            }
            else
            {
                entity = BasicLayer.GetMatch(request.MatchId);
            }

            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            var entities = BasicLayer.GetMatchStats(entity.Id);
            var association = BasicLayer.GetAssociation(entity.AssociationId);
var place = BasicLayer.GetPlace(entity.PlaceId);
            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity,association,place,entities));
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
                PlaceId = request.PlaceId,
                OpenMatch = request.OpenMatch,
                UnifyClassifications = request.UnifyClassifications
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateMatch(model);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            var association = BasicLayer.GetAssociation(model.AssociationId);
var place = BasicLayer.GetPlace(model.PlaceId);
            //Return contract
            return Reply(ContractUtils.GenerateContract(model,association,place));
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
            entity.PlaceId = request.PlaceId;
            entity.OpenMatch = request.OpenMatch;
            entity.UnifyClassifications = request.UnifyClassifications;

            //Salvataggio
            var validations = BasicLayer.UpdateMatch(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);

            var association = BasicLayer.GetAssociation(entity.AssociationId);
            var place = BasicLayer.GetPlace(entity.PlaceId);
            //Confermo
            return Reply(ContractUtils.GenerateContract(entity,association,place));
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
                return Task.FromResult<IActionResult>(NotFound());
            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteMatch(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);

            var association = BasicLayer.GetAssociation(entity.AssociationId);
            var place = BasicLayer.GetPlace(entity.PlaceId);
            //Return contract
            return Reply(ContractUtils.GenerateContract(entity,association,place));
        }
    }
}
