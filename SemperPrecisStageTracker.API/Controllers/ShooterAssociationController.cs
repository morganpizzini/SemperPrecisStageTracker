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
using System.ComponentModel.DataAnnotations;

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
        public Task<IActionResult> FetchShooterAssociation(ShooterRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooter(request.ShooterId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound()); ;

            //Invocazione del service layer
            var shooterAssociations = BasicLayer.FetchShooterAssociationByShooterId(entity.Id);
            var associationIds = shooterAssociations.Select(x => x.AssociationId).ToList();
            var associations = BasicLayer.FetchAssociationsByIds(associationIds);

            //Return contract
            return Reply(
                shooterAssociations.As(x => ContractUtils.GenerateContract(x, associations.FirstOrDefault(a => a.Id == x.AssociationId)))
                    .OrderBy(x => x.Association.Name).ToList()
                );
        }

        /// <summary>
        /// Creates a shooterassociation on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpsertShooterAssociation")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> UpsertShooterAssociation(ShooterAssociationCreateRequest request)
        {
            var entity = this.BasicLayer.GetActiveShooterAssociationByShooterAndAssociationAndDivision(request.ShooterId, request.AssociationId, request.Division);

            IList<ValidationResult> validations;

            if (entity != null)
            {
                entity.ExpireDate = entity.RegistrationDate.AddDays(-1);
                validations = BasicLayer.UpsertShooterAssociation(entity);

                if (validations.Count > 0)
                    return BadRequestTask(validations);
            }

            entity = new ShooterAssociation();
            entity.ShooterId = request.ShooterId;
            entity.AssociationId = request.AssociationId;
            entity.CardNumber = request.CardNumber;
            entity.SafetyOfficier = request.SafetyOfficier;
            entity.Classification = request.Classification;
            entity.Division = request.Division;
            entity.RegistrationDate = request.RegistrationDate;

            //Invocazione del service layer
            validations = BasicLayer.UpsertShooterAssociation(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(new OkResponse { Status = true });
        }

        /// <summary>
        /// Creates a shooterassociation on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooterAssociation")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public Task<IActionResult> DeleteShooterAssociation(ShooterAssociationRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterAssociationById(request.ShooterAssociationId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return Task.FromResult<IActionResult>(NotFound()); ;

            }
            //Invocazione del service layer
            var validations = BasicLayer.DeleteShooterAssociation(entity);

            if (validations.Count > 0)
                return BadRequestTask(validations);

            //Return contract
            return Reply(new OkResponse { Status = true });
        }
    }
}
