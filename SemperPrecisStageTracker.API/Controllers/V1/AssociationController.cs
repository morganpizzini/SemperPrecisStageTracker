using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using Asp.Versioning;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Extensions;
using Microsoft.FeatureManagement.Mvc;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for association
    /// </summary>
    [ApiVersion("1.0")]
    [FeatureGate(MyFeatureFlags.MatchHandling)]
    public class AssociationController : ApiControllerBase
    {
        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllAssociations")]
        [ProducesResponseType(typeof(IList<AssociationContract>), 200)]
        public Task<IActionResult> FetchAllAssociations()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllAssociations();

            //Ritorno i contratti
            return Reply(entities.As(x => ContractUtils.GenerateContract(x)));
        }

        /// <summary>
        /// Fetch list of all associations available, used for create classifications
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAvailableAssociationsForShooter")]
        [ProducesResponseType(typeof(IList<AssociationContract>), 200)]
        public Task<IActionResult> FetchAvailableAssociationsForShooter([FromBody]ShooterRequest request)
        {
            var associationIds = BasicLayer.FetchAllShooterAssociationInfos(request.ShooterId)
                .Select(x => x.AssociationId)
                .ToList();
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAssociationsByIds(associationIds);

            //Ritorno i contratti
            return Reply(entities.As(x => ContractUtils.GenerateContract(x)));
        }
        
        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAssociationsNotAssignedForShooter")]
        [ProducesResponseType(typeof(IList<AssociationContract>), 200)]
        public Task<IActionResult> FetchAssociationsNotAssignedForShooter([FromBody]ShooterRequest request)
        {
            var associationIds = BasicLayer.FetchAllShooterAssociationInfos(request.ShooterId)
                .Select(x => x.AssociationId)
                .ToList();
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllAssociations();

            //Ritorno i contratti
            return Reply(entities.Where(x=> !associationIds.Contains(x.Id)).As(ContractUtils.GenerateContract));
        }

        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetAssociation")]
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public Task<IActionResult> GetAssociation([FromBody]AssociationRequest request)
        {
            var entity = BasicLayer.GetAssociation(request.AssociationId);

            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a association on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateAssociation")]
        [ApiAuthorizationFilter(Permissions.ManageAssociations, Permissions.CreateAssociations)]
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public async Task<IActionResult> CreateAssociation([FromBody]AssociationCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Association
            {
                Name = request.Name,
                Classifications = request.Classifications,
                Divisions = request.Divisions,
                Categories = request.Categories,
                MatchKinds = request.MatchKinds,
                FirstPenaltyLabel = request.FirstPenaltyLabel,
                HitOnNonThreatPointDown = request.HitOnNonThreatPointDown,
                FirstProceduralPointDown = request.FirstProceduralPointDown,
                SecondPenaltyLabel = request.SecondPenaltyLabel,
                SecondProceduralPointDown = request.SecondProceduralPointDown,
                ThirdPenaltyLabel = request.ThirdPenaltyLabel,
                ThirdProceduralPointDown = request.ThirdProceduralPointDown,
                SoRoles = request.SoRoles
            };

            //Invocazione del service layer
            var validations = await BasicLayer.CreateAssociation(model, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);


            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing association
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateAssociation")]
        [ApiAuthorizationFilter(Permissions.AssociationEdit, Permissions.ManageAssociations)]
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public async Task<IActionResult> UpdateAssociation([FromBody]AssociationUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetAssociation(request.AssociationId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.Classifications = request.Classifications;
            entity.Categories = request.Categories;
            entity.Divisions = request.Divisions;
            entity.MatchKinds = request.MatchKinds;

            entity.FirstPenaltyLabel = request.FirstPenaltyLabel;
            entity.HitOnNonThreatPointDown = request.HitOnNonThreatPointDown;
            entity.FirstProceduralPointDown = request.FirstProceduralPointDown;
            entity.SecondPenaltyLabel = request.SecondPenaltyLabel;
            entity.SecondProceduralPointDown = request.SecondProceduralPointDown;
            entity.ThirdPenaltyLabel = request.ThirdPenaltyLabel;
            entity.ThirdProceduralPointDown = request.ThirdProceduralPointDown;
            entity.SoRoles = request.SoRoles;

            //Salvataggio
            var validations = await BasicLayer.UpdateAssociation(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Confermo
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Deletes existing association on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteAssociation")]
        [ApiAuthorizationFilter(Permissions.ManageAssociations,Permissions.AssociationDelete)]
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public async Task<IActionResult> DeleteAssociation([FromBody]AssociationRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetAssociation(request.AssociationId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();
            }

            //Invocazione del service layer
            var validations = await BasicLayer.DeleteAssociation(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
