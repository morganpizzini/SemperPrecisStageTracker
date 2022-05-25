using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for association
    /// </summary>
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
        public Task<IActionResult> FetchAvailableAssociationsForShooter(ShooterRequest request)
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
        public Task<IActionResult> FetchAssociationsNotAssignedForShooter(ShooterRequest request)
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
        public Task<IActionResult> GetAssociation(AssociationRequest request)
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
        public async Task<IActionResult> CreateAssociation(AssociationCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Association
            {
                Name = request.Name,
                Classifications = request.Classifications,
                Divisions = request.Divisions,
                Categories = request.Categories,
                FirstPenaltyLabel = request.FirstPenaltyLabel,
                FirstPenaltyDownPoints = request.FirstPenaltyDownPoints,
                SecondPenaltyLabel = request.SecondPenaltyLabel,
                SecondPenaltyDownPoints = request.SecondPenaltyDownPoints,
                ThirdPenaltyLabel = request.ThirdPenaltyLabel,
                ThirdPenaltyDownPoints = request.ThirdPenaltyDownPoints,
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
        [ApiAuthorizationFilter(Permissions.EditAssociation, Permissions.ManageAssociations)]
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public async Task<IActionResult> UpdateAssociation([EntityId] AssociationUpdateRequest request)
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

            entity.FirstPenaltyLabel = request.FirstPenaltyLabel;
            entity.FirstPenaltyDownPoints = request.FirstPenaltyDownPoints;
            entity.SecondPenaltyLabel = request.SecondPenaltyLabel;
            entity.SecondPenaltyDownPoints = request.SecondPenaltyDownPoints;
            entity.ThirdPenaltyLabel = request.ThirdPenaltyLabel;
            entity.ThirdPenaltyDownPoints = request.ThirdPenaltyDownPoints;
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
        [ApiAuthorizationFilter(Permissions.DeleteAssociation, Permissions.ManageAssociations)]
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public async Task<IActionResult> DeleteAssociation([EntityId] AssociationRequest request)
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
