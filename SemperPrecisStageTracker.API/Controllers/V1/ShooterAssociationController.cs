using Asp.Versioning;
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
    [ApiVersion("1.0")]
    public class ShooterAssociationController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join association
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterAssociation")]
        [ProducesResponseType(typeof(IList<UserAssociationContract>), 200)]
        public Task<IActionResult> FetchShooterAssociation([FromBody]ShooterRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetUser(request.ShooterId);

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
        [ProducesResponseType(typeof(IList<UserContract>), 200)]
        public Task<IActionResult> UpsertShooterAssociation([FromBody]UserAssociationCreateRequest request)
        {
            var availableAssociationIds = BasicLayer.FetchAllShooterAssociationInfos(request.UserId).Select(x=>x.AssociationId);

            IList<ValidationResult> validations = new List<ValidationResult>();
            if (!availableAssociationIds.Contains(request.AssociationId))
            {
                validations.Add(new ValidationResult("User is not registered for the selected association"));
                return BadRequestTask(validations);
            }

            var entity = this.BasicLayer.GetActiveShooterAssociationByShooterAndAssociationAndDivision(request.UserId, request.AssociationId, request.Division);


            if (entity != null)
            {
                entity.ExpireDate = entity.RegistrationDate.AddDays(-1);
                validations = BasicLayer.UpsertShooterAssociation(entity);

                if (validations.Count > 0)
                    return BadRequestTask(validations);
            }

            entity = new UserAssociation();
            entity.UserId = request.UserId;
            entity.AssociationId = request.AssociationId;
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
        [ProducesResponseType(typeof(OkResponse), 200)]
        public Task<IActionResult> DeleteShooterAssociation([FromBody]ShooterAssociationRequest request)
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
