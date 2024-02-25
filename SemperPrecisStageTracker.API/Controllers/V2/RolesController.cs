using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using SemperPrecisStageTracker.Shared.Permissions;
using SemperPrecisStageTracker.API.Models;

namespace SemperPrecisStageTracker.API.Controllers.V2;

/// <summary>
/// Controller for Roles
/// </summary>
[ApiVersion("2.0")]
public class RolesController : ApiControllerBase
{
    /// <summary>
    /// Fetch list of all associations
    /// </summary>
    /// <returns>Returns action result</returns>
    [HttpGet]
    [ApiAuthorizationFilter(Permissions.ManagePermissions)]
    [ProducesResponseType(typeof(IList<RoleContract>), 200)]
    public IActionResult FetchAllRoles()
    {
        var entities = AuthorizationLayer.FetchRoles().As(x => ContractUtils.GenerateContract(x));
        return Ok(new BaseResponse<IList<RoleContract>>(
            entities,
            entities.Count));
    }


    /// <summary>
    /// Get specific placet ype using provided identifier
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoleContract), 200)]
    public Task<IActionResult> GetRole(BaseRequestId request)
    {
        var entity = AuthorizationLayer.GetRole(request.Id);

        //verifico validità dell'entità
        if (entity == null)
            return Task.FromResult<IActionResult>(NotFound());

        var permissions = AuthorizationLayer.FetchPermissionsOnRole(entity.Id);

        var userRoles = AuthorizationLayer.FetchUserRole(entity.Id);

        var userIds = userRoles.Select(x => x.UserId).ToList();

        var users = BasicLayer.FetchShootersByIds(userIds);

        //Serializzazione e conferma
        return Reply(ContractUtils.GenerateContract(entity, permissions, userRoles, users));
    }

    /// <summary>
    /// Creates a shooter on platform
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpPost]
    [ApiAuthorizationFilter(Permissions.ManagePermissions)]
    [ProducesResponseType(201)]
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

        return CreatedAtAction(nameof(GetRole), model.GetRouteIdentifier(), ContractUtils.GenerateContract(model));
    }

    /// <summary>
    /// Updates existing shooter
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpPut("{id}")]
    [ApiAuthorizationFilter(Permissions.ManagePermissions)]
    [ProducesResponseType(201)]
    public async Task<IActionResult> UpdateRole(BaseRequestId<RoleUpdateRequestV2> request)
    {
        //Recupero l'elemento dal business layer
        var entity = AuthorizationLayer.GetRole(request.Id);

        //modifica solo se admin o se utente richiedente è lo stesso che ha creato
        if (entity == null)
            return NotFound();

        //Aggiornamento dell'entità
        entity.Name = request.Body.Name;
        entity.Description = request.Body.Description;

        //Salvataggio
        var validations = await AuthorizationLayer.UpdateRole(entity, PlatformUtils.GetIdentityUserId(User));
        if (validations.Count > 0)
            return BadRequest(validations);

        //Confermo
        return NoContent();
    }

    /// <summary>
    /// Deletes existing match on platform
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpDelete]
    [ApiAuthorizationFilter(Permissions.ManagePermissions)]
    [ProducesResponseType(201)]
    public async Task<IActionResult> DeleteRole(BaseRequestId request)
    {
        //Recupero l'elemento dal business layer
        var entity = AuthorizationLayer.GetRole(request.Id);

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
        return NoContent();
    }
}
