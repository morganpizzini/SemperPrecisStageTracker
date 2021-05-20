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
    /// Controller for shooterassociation
    /// </summary>
    public class ShooterAssociationController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join association
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterAssociation")]
        [ProducesResponseType(typeof(IList<ShooterAssociationContract>), 200)]
        public IActionResult FetchShooterAssociation(ShooterRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooter(request.ShooterId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Invocazione del service layer
            var shooterAssociations = BasicLayer.FetchShooterAssociationByShooterId(entity.Id);
            var associationIds = shooterAssociations.Select(x=>x.Id).ToList();
            var associations = BasicLayer.FetchAssociationsByIds(associationIds);

            //Return contract
            return Ok(shooterAssociations.As(x=>ContractUtils.GenerateContract(x,associations.FirstOrDefault(a=>a.Id== x.AssociationId))));
        }

        /// <summary>
        /// Creates a shooterassociation on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertShooterAssociation")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult UpsertShooterAssociation(ShooterAssociationCreateRequest request)
        {
            var entity = this.BasicLayer.GetShooterAssociationByShooterAndAssociation(request.ShooterId,request.AssociationId);
            
            if (entity == null){
                entity = new ShooterAssociation{
                    ShooterId = request.ShooterId,
                    AssociationId = request.AssociationId
                };
            }
            entity.Rank = request.Rank;
            entity.RegistrationDate = request.RegistrationDate;
            
            //Invocazione del service layer
            var validations = BasicLayer.UpsertShooterAssociation(entity);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse{Status= true});
        }

        /// <summary>
        /// Creates a shooterassociation on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooterAssociation")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public IActionResult DeleteShooterAssociation(ShooterAssociationDeleteRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterAssociationByShooterAndAssociation(request.ShooterId,request.AssociationId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }
            //Invocazione del service layer
            var validations = BasicLayer.DeleteShooterAssociation(entity);

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse{Status= true});
        }
    }
}
