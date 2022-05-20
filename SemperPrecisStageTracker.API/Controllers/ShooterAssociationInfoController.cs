using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for shooterassociation
    /// </summary>
    public class ShooterAssociationInfoController : ApiControllerBase
    {
        /// <summary>
        /// Fetch shooter available for join association
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchShooterAssociationInfo")]
        [ProducesResponseType(typeof(IList<ShooterAssociationInfoContract>), 200)]
        public Task<IActionResult> FetchShooterAssociationInfo(ShooterRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooter(request.ShooterId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound()); ;

            //Invocazione del service layer
            var shooterAssociations = BasicLayer.FetchShooterAssociationInfoByShooterId(entity.Id);
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
        [Route("CreateShooterAssociationInfo")]
        [ProducesResponseType(typeof(ShooterAssociationInfoContract), 200)]
        public async Task<IActionResult> CreateShooterAssociationInfo(ShooterAssociationInfoCreateRequest request)
        {
            var (validations,shooter,association) = CheckRequest(request.ShooterId, request.AssociationId);
            if (validations.Count > 0)
                return BadRequest(validations);

            var entity = new ShooterAssociationInfo();
            entity.ShooterId = request.ShooterId;
            entity.AssociationId = request.AssociationId;
            entity.CardNumber = request.CardNumber;
            entity.SafetyOfficier = request.SafetyOfficier;

            //Invocazione del service layer
            validations = await BasicLayer.UpdateShooterAssociationInfo(entity, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity,association,shooter));
        }

        /// <summary>
        /// Creates a shooterassociation on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateShooterAssociationInfo")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public async Task<IActionResult> UpdateShooterAssociationInfo(ShooterAssociationInfoUpdateRequest request)
        {
            var entity = this.BasicLayer.GetShooterAssociationInfo(request.ShooterAssociationInfoId);

            if (entity == null)
            {
                return NotFound();
            }

            var (validations, shooter, association) = CheckRequest(request.ShooterId, request.AssociationId);
            if (validations.Count > 0)
                return BadRequest(validations);

            entity = new ShooterAssociationInfo();
            entity.ShooterId = request.ShooterId;
            entity.AssociationId = request.AssociationId;
            entity.CardNumber = request.CardNumber;
            entity.SafetyOfficier = request.SafetyOfficier;
            

            //Invocazione del service layer
            validations = await BasicLayer.UpdateShooterAssociationInfo(entity, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(entity,association,shooter));
        }


        private (IList<ValidationResult> validations, Shooter shooter,Association association) CheckRequest(string shooterId,string associationId)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            // check shooter
            var shooter = BasicLayer.GetShooter(shooterId);
            if (shooter == null)
                validations.Add(new ValidationResult("Shooter not found"));
            // check association
            var association = BasicLayer.GetAssociation(associationId);
            if (association == null)
                validations.Add(new ValidationResult("Association not found"));
            return (validations,shooter,association);
        }

        /// <summary>
        /// Creates a shooterassociation on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteShooterAssociationInfo")]
        [ProducesResponseType(typeof(IList<ShooterContract>), 200)]
        public async Task<IActionResult> DeleteShooterAssociationInfo(ShooterAssociationInfoRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = BasicLayer.GetShooterAssociationInfo(request.ShooterAssociationInfoId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();

            }
            //Invocazione del service layer
            var validations = await BasicLayer.DeleteShooterAssociationInfo(entity, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(new OkResponse { Status = true });
        }
    }
}
