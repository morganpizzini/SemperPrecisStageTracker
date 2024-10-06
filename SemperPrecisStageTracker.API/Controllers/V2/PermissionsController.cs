using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;
using SemperPrecisStageTracker.Shared.Permissions;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;

namespace SemperPrecisStageTracker.API.Controllers.V2;

/// <summary>
/// Controller for Permissions
/// </summary>
[ApiVersion("2.0")]
public class PermissionsController : ApiControllerBase
{
    /// <summary>
    /// Link permission to role
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpPost("{id}/role/{roleId}")]
    [ApiAuthorizationFilter(Permissions.ManagePermissions)]
    [ProducesResponseType(201)]
    public async Task<IActionResult> AddPermissionToRole(RolePermissionCreateRequestV2 request)
    {
        //Recupero l'elemento dal business layer
        var permission = request.Id.CastIntoPermission();

        //Recupero l'elemento dal business layer
        var role = AuthorizationLayer.GetRole(request.RoleId);

        if (role == null)
        {
            return NotFound($"Role with {request.RoleId} not found");
        }

        var entity = new PermissionRole()
        {
            PermissionId = (int)permission,
            RoleId = role.Id
        };
        //Invocazione del service layer
        var validations = await AuthorizationLayer.CreatePermissionRole(entity, PlatformUtils.GetIdentityUserId(User));

        if (validations.Count > 0)
            return BadRequest(validations);

        //Return contract
        return CreatedAtAction("GetRole", nameof(RolesController), new { id = entity.RoleId });
    }

    /// <summary>
    /// Deletes existing link between permission and role
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpDelete("{id}/role/{roleId}")]
    [ApiAuthorizationFilter(Permissions.ManagePermissions)]
    public async Task<IActionResult> DeletePermissionOnRole(RolePermissionCreateRequestV2 request)
    {
        //Recupero l'elemento dal business layer
        var permission = request.Id.CastIntoPermission();
        var entity = AuthorizationLayer.GetPermissionRole((int)permission, request.RoleId);

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
        return NoContent();
    }
}
