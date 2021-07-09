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
    public partial class MatchController
    {

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllMatchDirector")]
        [ProducesResponseType(typeof(IList<ShooterMatchContract>), 200)]
        public Task<IActionResult> FetchAllMatchDirector(MatchRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchShooterMatchesByMatchId(request.MatchId);
            
            var shooterIds = entities.Select(x => x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            //Ritorno i contratti
            return Reply(entities.As(x=> ContractUtils.GenerateContract(x,shooters.FirstOrDefault(s=> s.Id == x.ShooterId))));
        }

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAvailableMatchDirector")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> FetchAvailableMatchDirector(MatchRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAvailableMatchDirectorByMatchId(request.MatchId);
            
            //Ritorno i contratti
            return Reply(entities.As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAvailableMatchDirectorByAssociation")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> FetchAvailableMatchDirectorByAssociation(AssociationRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAvailableMatchDirectorByMatchId(request.AssociationId);
            
            //Ritorno i contratti
            return Reply(entities.As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Creates a match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateMatchDirector")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> CreateMatchDirector(ShooterMatchCreateRequest request)
        => this.CreateMatchDirectors(new ShooterMatchesCreateRequest()
            {
                MatchId = request.MatchId,
                ShooterIds = new List<string>{request.ShooterId}
            });
        
        /// <summary>
        /// Creates a match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateMatchDirectors")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> CreateMatchDirectors(ShooterMatchesCreateRequest request)
        {
            //Recupero l'elemento dal business layer
            var existingMatch = BasicLayer.GetMatch(request.MatchId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (existingMatch == null)
                return Task.FromResult<IActionResult>(NotFound());

            var entities = request.ShooterIds.Select(x => new ShooterMatch
            {
                MatchId = existingMatch.Id,
                ShooterId = x,
                Role = ShooterRoleEnum.MatchDirector
            });

            //Invocazione del service layer
            var validations = entities.As(BasicLayer.UpsertShooterMatch).SelectMany(x=>x).ToList();

            if (validations.Count > 0)
                return BadRequestTask(validations);

            return this.FetchAllMatchDirector(new MatchRequest {MatchId = existingMatch.Id});
            //var shooterMatches = BasicLayer.FetchShooterMatchesByMatchId(existingMatch.Id);
            //var shooterIds = shooterMatches.Select(x => x.ShooterId).ToList();
            //var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            ////Return contract
            //return Reply(shooterMatches.As(x=> ContractUtils.GenerateContract(x,shooters.FirstOrDefault(s=> s.Id == x.ShooterId))));
        }
        /// <summary>
        /// Deletes existing match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteMatchDirector")]
        [ProducesResponseType(typeof(IList<ShooterMatchContract>), 200)]
        public Task<IActionResult> DeleteMatchDirector(ShooterMatchRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterMatch(request.ShooterMatchId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Invocazione del service layer
            var validations = BasicLayer.DeleteShooterMatch(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);

            return this.FetchAllMatchDirector(new MatchRequest {MatchId = entity.MatchId});

            //var shooterMatches = BasicLayer.FetchShooterMatchesByMatchId(entity.MatchId);
            //var shooterIds = shooterMatches.Select(x => x.ShooterId).ToList();
            //var shooters = BasicLayer.FetchShootersByIds(shooterIds);
            ////Return contract
            //return Reply(shooterMatches.As(x=> ContractUtils.GenerateContract(x,shooters.FirstOrDefault(s=> s.Id == x.ShooterId))));
        }

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllShooterSOStages")]
        [ProducesResponseType(typeof(IList<ShooterSOStageContract>), 200)]
        public Task<IActionResult> FetchAllShooterSOStages(StageRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchShooterSOStagesByStageId(request.StageId);
            
            var shooterIds = entities.Select(x => x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            //Ritorno i contratti
            return Reply(entities.As(x=> ContractUtils.GenerateContract(x,shooters.FirstOrDefault(s=> s.Id == x.ShooterId))));
        }


        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAvailableStageSO")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> FetchAvailableStageSO(StageRequest request)
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAvailabelShooterSOByStageId(request.StageId);
            
            //Ritorno i contratti
            return Reply(entities.As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Creates a match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateStageSO")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> CreateStageSO(ShooterSOStageCreateRequest request)
            => this.CreateStageSOs(new ShooterSOStagesCreateRequest()
            {
                StageId = request.StageId,
                Shooters = new List<ShooterSOStageShooterContract>{request.Shooter}
            });

        /// <summary>
        /// Creates a match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateStageSOs")]
        [ProducesResponseType(typeof(MatchContract), 200)]
        public Task<IActionResult> CreateStageSOs(ShooterSOStagesCreateRequest request)
        {
            //Recupero l'elemento dal business layer
            var existingStage = BasicLayer.GetStage(request.StageId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (existingStage == null)
                return Task.FromResult<IActionResult>(NotFound());

            var entities = request.Shooters.Select(x => new ShooterSOStage
            {
                StageId = existingStage.Id,
                ShooterId = x.ShooterId,
                Role = (ShooterRoleEnum)x.Role
            });

            //Invocazione del service layer
            var validations = entities.As(BasicLayer.UpsertShooterSOStage).SelectMany(x=>x).ToList();

            if (validations.Count > 0)
                return BadRequestTask(validations);

            var shooterSOStagees = BasicLayer.FetchShooterSOStagesByStageId(existingStage.Id);
            var shooterIds = shooterSOStagees.Select(x => x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);

            //Return contract
            return Reply(shooterSOStagees.As(x=> ContractUtils.GenerateContract(x,shooters.FirstOrDefault(s=> s.Id == x.ShooterId))));
        }
        /// <summary>
        /// Deletes existing match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteStageSO")]
        [ProducesResponseType(typeof(IList<ShooterSOStageContract>), 200)]
        public Task<IActionResult> DeleteStageSO(ShooterSOStageRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterSOStage(request.ShooterSOStageId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Invocazione del service layer
            var validations = BasicLayer.DeleteShooterSOStage(entity);
            if (validations.Count > 0)
                return BadRequestTask(validations);

            var shooterSOStagees = BasicLayer.FetchShooterSOStagesByStageId(entity.StageId);
            var shooterIds = shooterSOStagees.Select(x => x.ShooterId).ToList();
            var shooters = BasicLayer.FetchShootersByIds(shooterIds);
            //Return contract
            return Reply(shooterSOStagees.As(x=> ContractUtils.GenerateContract(x,shooters.FirstOrDefault(s=> s.Id == x.ShooterId))));
        }
    }
}
