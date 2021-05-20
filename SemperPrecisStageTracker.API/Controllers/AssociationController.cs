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
        public IActionResult FetchAllAssociations()
        {
            //Recupero la lista dal layer
            var entities = BasicLayer.FetchAllAssociations();

            //Ritorno i contratti
            return Ok(entities.As(x=>ContractUtils.GenerateContract(x)));
        }
    
        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetAssociation")]
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public IActionResult GetAssociation(AssociationRequest request)
        {
            var entity = BasicLayer.GetAssociation(request.AssociationId);

            //verifico validità dell'entità
            if (entity == null)
                return NotFound();

            //Serializzazione e conferma
            return Ok(ContractUtils.GenerateContract(entity));
        }

        /// <summary>
        /// Creates a association on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateAssociation")]
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public IActionResult CreateAssociation(AssociationCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Association
            {
                Name = request.Name
            };

            //Invocazione del service layer
            var validations = BasicLayer.CreateAssociation(model);

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
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public IActionResult UpdateAssociation(AssociationUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetAssociation(request.AssociationId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            
            //Salvataggio
            var validations = BasicLayer.UpdateAssociation(entity);
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
        [ProducesResponseType(typeof(AssociationContract), 200)]
        public IActionResult DeleteAssociation(AssociationRequest request)
        {

            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetAssociation(request.AssociationId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }

            //Invocazione del service layer
            var validations = BasicLayer.DeleteAssociation(entity);
            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
    }
}
