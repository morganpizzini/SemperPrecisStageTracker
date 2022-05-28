using System.Collections.Generic;
using System.Linq;
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
    /// Controller for permissions
    /// </summary>
    public class PermissionController : ApiControllerBase
    {
        [HttpPost]
        [Route("FetchPermissions")]
        [ProducesResponseType(typeof(PermissionContract), 200)]
        public IActionResult FetchPermissions() =>
            Ok((AuthorizationLayer.FetchPermission()).As(ContractUtils.GenerateContract));

        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllPermissionsOnUser")]
        [ProducesResponseType(typeof(UserPermissionContract), 200)]
        public async Task<IActionResult> FetchAllPermissionsOnUser() =>
            Ok(ContractUtils.GenerateContract(await AuthorizationLayer.GetUserPermissionById(PlatformUtils.GetIdentityUserId(User))));
        
        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("FetchAllRoles")]
        [ApiAuthorizationFilter(Permissions.ManagePermissions)]
        [ProducesResponseType(typeof(IList<RoleContract>), 200)]
        public IActionResult FetchAllRoles() =>
            Ok(AuthorizationLayer.FetchRoles().As(x=>ContractUtils.GenerateContract(x)));


        /// <summary>
        /// Get specific placet ype using provided identifier
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("GetRole")]
        [ProducesResponseType(typeof(RoleContract), 200)]
        public Task<IActionResult> GetRole(RoleRequest request)
        {
            var entity = AuthorizationLayer.GetRole(request.RoleId);
            
            //verifico validità dell'entità
            if (entity == null)
                return Task.FromResult<IActionResult>(NotFound());

            var permissions = AuthorizationLayer.FetchPermissionsOnRole(entity.Id);

            //Serializzazione e conferma
            return Reply(ContractUtils.GenerateContract(entity,permissions));
        }

        /// <summary>
        /// Creates a shooter on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreateRole")]
        [ApiAuthorizationFilter(Permissions.ManagePermissions)]
        [ProducesResponseType(typeof(RoleContract), 200)]
        public async Task<IActionResult> CreateRole(RoleCreateRequest request)
        {
            //Creazione modello richiesto da admin
            var model = new Role
            {
                Name = request.Name,
                Description = request.Description
            };

            //Invocazione del service layer
            var validations = await AuthorizationLayer.CreateRole(model, PlatformUtils.GetIdentityUserId(User));

            if (validations.Count > 0)
                return BadRequest(validations);

            //Return contract
            return Ok(ContractUtils.GenerateContract(model));
        }

        /// <summary>
        /// Updates existing shooter
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("UpdateRole")]
        [ApiAuthorizationFilter(Permissions.ManagePermissions)]
        [ProducesResponseType(typeof(RoleContract), 200)]
        public async Task<IActionResult> UpdateRole(RoleUpdateRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = AuthorizationLayer.GetRole(request.RoleId);

            //modifica solo se admin o se utente richiedente è lo stesso che ha creato
            if (entity == null)
                return NotFound();

            //Aggiornamento dell'entità
            entity.Name = request.Name;
            entity.Description = request.Description;
            
            //Salvataggio
            var validations = await AuthorizationLayer.UpdateRole(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);


            var permissions = AuthorizationLayer.FetchPermissionsOnRole(entity.Id);

            //Confermo
            return Ok(ContractUtils.GenerateContract(entity,permissions));
        }
        
        /// <summary>
        /// Deletes existing match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("CreatePermissionOnRole")]
        [ApiAuthorizationFilter(Permissions.ManagePermissions)]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> CreatePermissionOnRole(RolePermissionCreateRequest request)
        {
            //Recupero l'elemento dal business layer
            var permission = AuthorizationLayer.GetPermission(request.PermissionId);

            if (permission == null)
            {
                return NotFound($"Permission with {request.PermissionId} not found");
            }

            //Recupero l'elemento dal business layer
            var role = AuthorizationLayer.GetRole(request.RoleId);

            if (role== null)
            {
                return NotFound($"Role with {request.RoleId} not found");
            }
            
            var entity = new PermissionRole()
            {
                PermissionId = permission.Id,
                RoleId = role.Id
            };
            //Invocazione del service layer
            var validations = await AuthorizationLayer.CreatePermissionRole(entity, PlatformUtils.GetIdentityUserId(User));
            
            if (validations.Count > 0)
                return BadRequest(validations);
            
            //Return contract
            return Ok(new OkResponse{Status = true});
        }
        /// <summary>
        /// Deletes existing match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeletePermissionOnRole")]
        [ApiAuthorizationFilter(Permissions.ManagePermissions)]
        [ProducesResponseType(typeof(OkResponse), 200)]
        public async Task<IActionResult> DeletePermissionOnRole(RolePermissionRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = AuthorizationLayer.GetPermissionRole(request.PermissionId,request.RoleId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();
            }

            //Invocazione del service layer
            var validations = await AuthorizationLayer.DeletePermissionRole(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);
            
            //Return contract
            return Ok(new OkResponse{Status = true});
        }

        /// <summary>
        /// Deletes existing match on platform
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Returns action result</returns>
        [HttpPost]
        [Route("DeleteRole")]
        [ApiAuthorizationFilter(Permissions.ManagePermissions)]
        [ProducesResponseType(typeof(RoleContract), 200)]
        public async Task<IActionResult> DeleteRole(RoleRequest request)
        {
            //Recupero l'elemento dal business layer
            var entity = AuthorizationLayer.GetRole(request.RoleId);

            //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
            if (entity == null)
            {
                return NotFound();
            }

            //Invocazione del service layer
            var validations = await AuthorizationLayer.DeleteRole(entity, PlatformUtils.GetIdentityUserId(User));
            if (validations.Count > 0)
                return BadRequest(validations);
            
            //Return contract
            return Ok(ContractUtils.GenerateContract(entity));
        }
        
    }
}