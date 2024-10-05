using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using SemperPrecisStageTracker.Shared.Permissions;
using Microsoft.FeatureManagement.Mvc;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for match
    /// </summary>
    [ApiVersion("1.0")]
    [FeatureGate(MyFeatureFlags.MatchHandling)]
    public partial class MatchController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllMatches")]
        //[ApiAuthorizationFilter(Permissions.ManageMatches)]
        [ProducesResponseType(typeof(IList<MatchContract>), 200)]
        public Task<IActionResult> FetchAllMatches()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllMatches();
            //Ritorno i contratti
            return Reply(GenerateMatchContracts(entities));
        }


        /// <summary>
        /// Fetch list of all available matches
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAvailableMatches")]
        //[ApiAuthorizationFilter(Permissions.ManageMatches)]
        [ProducesResponseType(typeof(IList<MatchContract>), 200)]
        public Task<IActionResult> FetchAvailableMatches()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAvailableMatches(PlatformUtils.GetIdentityUserId(User));
            //Ritorno i contratti
            return Reply(GenerateMatchContracts(entities));
        }


        private IList<MatchContract> GenerateMatchContracts(IList<Match> entities)
        {
            var associationIds = entities.Select(x => x.AssociationId).ToList();
            var placeIds = entities.Select(x => x.PlaceId).ToList();
            var teamIds = entities.Select(x => x.TeamId).ToList();

            var associations = BasicLayer.FetchAssociationsByIds(associationIds);
            var teams = BasicLayer.FetchTeamsByIds(teamIds);
            var places = BasicLayer.FetchPlacesByIds(placeIds);

            //Ritorno i contratti
            return entities.As(x => ContractUtils.GenerateContract(x,teams.FirstOrDefault(p => p.Id == x.TeamId), associations.FirstOrDefault(p => p.Id == x.AssociationId), places.FirstOrDefault(p => p.Id == x.PlaceId)));
        }

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchMatchesForSO")]
        [ProducesResponseType(typeof(IList<MatchContract>), 200)]
        public async Task<IActionResult> FetchMatchSO()
        {
            //Recupero la lista dal layer
            var entities = await BasicLayer.FetchAllSoMdMatches(PlatformUtils.GetIdentityUserId(User));
            var associationIds = entities.Select(x => x.AssociationId).ToList();
            var teamIds = entities.Select(x => x.TeamId).ToList();
            var placeIds = entities.Select(x => x.PlaceId).ToList();

            var associations = BasicLayer.FetchAssociationsByIds(associationIds);
            var teams = BasicLayer.FetchTeamsByIds(teamIds);
            var places = BasicLayer.FetchPlacesByIds(placeIds);

            //Ritorno i contratti
            return Ok(entities.As(x => ContractUtils.GenerateContract(x,teams.FirstOrDefault(p => p.Id == x.TeamId), associations.FirstOrDefault(p => p.Id == x.AssociationId), places.FirstOrDefault(p => p.Id == x.PlaceId))));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetMatch")]
        [ApiAuthorizationFilter(Permissions.TeamEditPayment,Permissions.ManageTeams)]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> GetMatch([FromBody]MatchRequest request)
        {
            var entity = BasicLayer.GetMatch(request.MatchId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            var groups = BasicLayer.FetchAllGroupsWithShootersByMatchId(entity.Id);
            var stages = BasicLayer.FetchAllStagesByMatchId(entity.Id);
            
            if (groups.Any() && entity.CompetitionReady)
            {
                var now = DateTime.Now.Date;
                if( groups.All(x=> x.Item1.GroupDay.Date != now))
                {
                    // get the closest date
                    now = groups.OrderBy(x=>x.Item1.GroupDay).FirstOrDefault().Item1.GroupDay;
                }
                groups = groups.Where(x=>x.Item1.GroupDay.Date == now).ToList();
            }
            
            var stageIds = stages.Select(x => x.Id).ToList();
            //var stageStrings = BasicLayer.FetchStageStringsFromStageIds(stageIds);

            var association = BasicLayer.GetAssociation(entity.AssociationId);
            var team = BasicLayer.GetTeam(entity.TeamId);
            var place = BasicLayer.GetPlace(entity.PlaceId);

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity,team, association, place, groups, stages));
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
        public Task<IActionResult> GetMatchStats([FromBody]MatchStatsRequest request)
        {
            Match entity;
            if (string.IsNullOrEmpty(request.MatchId))
            {
                if (string.IsNullOrEmpty(request.ShortLink))
                    return Task.FromResult<IActionResult>(NotFound());
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
            var team = BasicLayer.GetTeam(entity.TeamId);
            var place = BasicLayer.GetPlace(entity.PlaceId);
            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity, team, association, place, entities));
        }

        /// <summary>
        /// Creates a match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateMatch")]
        [ApiAuthorizationFilter(Permissions.ManageMatches, Permissions.CreateMatches)]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public async Task<IActionResult> CreateMatch([FromBody]MatchCreateRequest request)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            var association = BasicLayer.GetAssociation(request.AssociationId);
            if(association == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.AssociationId).AsList()));
            }

            var place = BasicLayer.GetPlace(request.PlaceId);
            if(place == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.PlaceId).AsList()));
            }

            var team = BasicLayer.GetTeam(request.TeamId);

            if(team == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.TeamId).AsList()));
            }

            if (validations.Count > 0)
                return BadRequest(validations);
            //Creazione modello richiesto da admin
            var model = new Match
            {
                Name = request.Name,
                MatchDateTimeStart = request.MatchDateTimeStart,
                MatchDateTimeEnd = request.MatchDateTimeEnd,
                AssociationId = request.AssociationId,
                TeamId = request.TeamId,
                PlaceId = request.PlaceId,
                OpenMatch = request.OpenMatch,
                UnifyClassifications = request.UnifyClassifications,
                Cost = request.Cost,
                Kind = request.Kind,
                PaymentDetails = request.PaymentDetails
            };

            if (model.OpenMatch && model.UnifyClassifications)
            {
                return BadRequest(
                    new ValidationResult("Match can't be open and without classification at the same time").AsList());
            }

            //Invocazione del service layer
            validations = await BasicLayer.CreateMatch(model, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(model,team, association, place));
        }

        /// <summary>
        /// Updates existing match
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateMatch")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        [ApiAuthorizationFilter(Permissions.MatchHandling, Permissions.ManageMatches)]
        public async Task<IActionResult> UpdateMatch([FromBody]MatchUpdateRequest request)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            var association = BasicLayer.GetAssociation(request.AssociationId);
            if(association == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.AssociationId).AsList()));
            }

            var place = BasicLayer.GetPlace(request.PlaceId);
            if(place == null)
            {
                validations.Add(new ValidationResult("Not found",nameof(request.PlaceId).AsList()));
            }

            if (validations.Count > 0)
                return BadRequest(validations);
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetMatch(request.MatchId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.MatchDateTimeStart = request.MatchDateTimeStart;
            entity.MatchDateTimeEnd = request.MatchDateTimeEnd;
            entity.AssociationId = request.AssociationId;
            entity.PlaceId = request.PlaceId;
            entity.Kind = request.Kind;
            
            entity.Cost = request.Cost;
            entity.PaymentDetails = request.PaymentDetails;

            //Salvataggio
            validations = await BasicLayer.UpdateMatch(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            var team = BasicLayer.GetTeam(entity.TeamId);
            //Confermo
            return Ok(ContractUtils.GenerateContract(entity, team, association, place));
        }

        /// <summary>
        /// Deletes existing match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteMatch")]
        [ApiAuthorizationFilter(Permissions.ManageMatches,Permissions.MatchDelete)]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public async Task<IActionResult> DeleteMatch([FromBody]MatchRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetMatch(request.MatchId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();
            }

            //Invocazione del service layer
            var validations = await BasicLayer.DeleteMatch(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            var association = BasicLayer.GetAssociation(entity.AssociationId);
            var team = BasicLayer.GetTeam(entity.TeamId);
            var place = BasicLayer.GetPlace(entity.PlaceId);
            //Return contract
            return Ok(ContractUtils.GenerateContract(entity, team, association, place));
        }

         /// <summary>
        /// Deletes existing match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateMatchCompetitionReady")]
        [ApiAuthorizationFilter(Permissions.MatchHandling, Permissions.ManageMatches)]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> UpdateMatchCompetitionReady([FromBody]MatchCompetitionReadyRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetMatch(request.MatchId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();
            }

            entity.CompetitionReady = request.Ready;

            //Invocazione del service layer
            var validations = await BasicLayer.UpdateMatch(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse(){Status= true});
        }
    }
}
