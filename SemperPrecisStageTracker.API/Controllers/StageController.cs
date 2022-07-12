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

            var stageStrings = BasicLayer.FetchStageStringsFromStageId(entity.Id);

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity,stageStrings));
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
                Description = request.Description,
                MatchId = request.MatchId,
                Index = request.Index,
                Scenario = request.Scenario,
                GunReadyCondition = request.GunReadyCondition,
                StageProcedure = request.StageProcedure,
                StageProcedureNotes = request.StageProcedureNotes,
                Rules = request.Rules
            };

            var strings = request.Strings.Select(x => new StageString()
            {
                Targets = x.Targets,
                Scoring = x.Scoring,
                TargetsDescription = x.TargetsDescription,
                ScoredHits = x.ScoredHits,
                StartStop = x.StartStop,
                Distance = x.Distance,
                CoverGarment = x.CoverGarment,
                MuzzleSafePlane = x.MuzzleSafePlane,
                Name = x.Name
            }).ToList();

            //Invocazione del service layer
            var validations = BasicLayer.CreateStage(model,strings);

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

            var strings = BasicLayer.FetchStageStringsFromStageId(request.StageId);

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Index = request.Index;
            entity.Scenario = request.Scenario;
            entity.GunReadyCondition = request.GunReadyCondition;
            entity.StageProcedure = request.StageProcedure;
            entity.StageProcedureNotes = request.StageProcedureNotes;
            entity.Rules = request.Rules;


            var newStrings = new List<StageString>();
            // set new
            // update existing
            foreach (var stageString in request.Strings)
            {
                var existing = strings.FirstOrDefault(x => x.Id == stageString.StageStringId) ?? new StageString();

                existing.Targets = stageString.Targets;
                existing.Scoring = stageString.Scoring;
                existing.TargetsDescription = stageString.TargetsDescription;
                existing.ScoredHits = stageString.ScoredHits;
                existing.StartStop = stageString.StartStop;
                existing.Distance = stageString.Distance;
                existing.MuzzleSafePlane = stageString.MuzzleSafePlane;
                existing.CoverGarment = stageString.CoverGarment;

                newStrings.Add(existing);
            }
            
            //Salvataggio
            var validations = BasicLayer.UpdateStage(entity,newStrings);
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
        public IActionResult DeleteStage(StageRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetStage(request.StageId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
                return NotFound();

            //Invocazione del service layer
            var validations = BasicLayer.DeleteStage(entity);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
