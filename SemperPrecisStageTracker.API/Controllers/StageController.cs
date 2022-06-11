using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using System.Threading.Tasks;

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
        public Task<IActionResult> FetchAllStages(MatchRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllStages(request.MatchId);


            //seleziono gli id dei posti
            var matchIds = entities.Select(x => x.MatchId).ToList();

            //recupero gli utenti
            var matches = this.BasicLayer.FetchMatchesByIds(matchIds);

            var associationIds = matches.Select(x => x.AssociationId).ToList();
            var associations = BasicLayer.FetchAssociationsByIds(associationIds);

            //Ritorno i contratti
            return Reply(entities.As(x =>
            {
                var match = matches.FirstOrDefault(p => p.Id == x.MatchId);
                return ContractUtils.GenerateContract(x, match, associations.FirstOrDefault(p => p.Id == match.AssociationId));
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
        public Task<IActionResult> GetStage(StageRequest request)
        {
            var entity = BasicLayer.GetStage(request.StageId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a stage on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateStage")]
        [ProducesResponseType(typeof(StageContract), 200)]
        public Task<IActionResult> CreateStage(StageCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Stage
            {
                Name = request.Name,
                Targets = request.Targets,
                MatchId = request.MatchId,
                Index = request.Index,
                Scenario = request.Scenario,
                GunReadyCondition = request.GunReadyCondition,
                StageProcedure = request.StageProcedure,
                StageProcedureNotes = request.StageProcedureNotes,
                Strings = request.Strings,
                Scoring = request.Scoring,
                TargetsDescription = request.TargetsDescription,
                ScoredHits = request.ScoredHits,
                StartStop = request.StartStop,
                Rules = request.Rules,
                Distance = request.Distance,
                CoverGarment = request.CoverGarment
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateStage(model);

            if (validations.Count > 0)
                return BadRequestTask(validations);


            //Return contract
            return Reply(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing stage
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateStage")]
        [ProducesResponseType(typeof(StageContract), 200)]
        public Task<IActionResult> UpdateStage(StageUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetStage(request.StageId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.Targets = request.Targets;
            entity.Index = request.Index;
            entity.Scenario = request.Scenario;
            entity.GunReadyCondition = request.GunReadyCondition;
            entity.StageProcedure = request.StageProcedure;
            entity.StageProcedureNotes = request.StageProcedureNotes;
            entity.Strings = request.Strings;
            entity.Scoring = request.Scoring;
            entity.TargetsDescription = request.TargetsDescription;
            entity.ScoredHits = request.ScoredHits;
            entity.StartStop = request.StartStop;
            entity.Rules = request.Rules;
            entity.Distance = request.Distance;
            entity.CoverGarment = request.CoverGarment;

            //Salvataggio
            var validations = BasicLayer.UpdateStage(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);


            //Confermo
            return Reply(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing stage on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteStage")]
        [ProducesResponseType(typeof(StageContract), 200)]
        public Task<IActionResult> DeleteStage(StageRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetStage(request.StageId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound());

            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteStage(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(ContractUtils.GenerateContract(entity));
        }
    }
}
