using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using SemperPrecisStageTracker.API.Models;
using ZenProgramming.Chakra.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Contracts.Mvc.Requests;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SemperPrecisStageTracker.API.Controllers.V2;

/// <summary>
/// Controller for Users
/// </summary>
[ApiVersion("2.0")]
public class UsersController : ApiControllerBase
{
    private readonly IEmailSender _emailSender;

    public UsersController(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }
    /// <summary>
    /// Fetch list of all users
    /// </summary>
    /// <returns>Returns action result</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IList<UserContract>), 200)]
    public Task<IActionResult> FetchUsers(UserFetchRequest request)
    {
        //Recupero la lista dal layer
        var entities = BasicLayer.FetchAllUsers().AsQueryable();
        var total = entities.Count();

        if(request.Skip.HasValue)
            entities = entities.Skip(request.Skip.Value);

        if(request.Take.HasValue)
            entities = entities.Take(request.Take.Value);

        var userIds = entities.Select(x => x.Id).ToList();

        var userDatas = BasicLayer.FetchUserDataByUserIds(userIds);

        //Ritorno i contratti
        return Reply(
            new BaseResponse<IList<UserContract>>(
                entities.As(x => ContractUtils.GenerateContract(x, userDatas.FirstOrDefault(y => y.UserId == x.Id), null, null, false)),
                total,
                request.Take.HasValue ? 
                    Url.Action(action: nameof(FetchUsers), controller: "User", new { take = request.Take, skip = request.Take + (request?.Skip ?? 0)}) : 
                    string.Empty
            ));
    }
    /// <summary>
    /// Get user by id
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserContract), 200)]
    public IActionResult GetUser(BaseRequestId request)
    {
        var entity = AuthorizationLayer.GetUserById(request.Id);

        //verifico validità dell'entità
        if (entity == null)
            return NotFound();

        var data = BasicLayer.GetUserData(entity.Id);

        //Serializzazione e conferma
        return Ok(ContractUtils.GenerateContract(entity, data));
    }

    /// <summary>
    /// Creates a user
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpPost]
    [ApiAuthorizationFilter(Permissions.ManageUsers, Permissions.CreateUser)]
    [ProducesResponseType(201)]
    public async Task<IActionResult> CreateUser([AsParameters]UserCreateRequest request)
    {
        //Creazione modello richiesto da admin
        var model = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            Email = request.Email,
            Username = request.Username
        };
        var data = new UserData
        {
            FirearmsLicence = request.FirearmsLicence,
            FirearmsLicenceExpireDate = request.FirearmsLicenceExpireDate,
            FirearmsLicenceReleaseDate = request.FirearmsLicenceReleaseDate,
            MedicalExaminationExpireDate = request.MedicalExaminationExpireDate,
            BirthLocation = request.BirthLocation,
            Address = request.Address,
            City = request.City,
            PostalCode = request.PostalCode,
            Province = request.Province,
            Country = request.Country,
            Phone = request.Phone,
            FiscalCode = request.FiscalCode
        };

        //Invocazione del service layer
        var validations = await BasicLayer.CreateUser(model, data, PlatformUtils.GetIdentityUserId(User));

        if (validations.Count > 0)
            return BadRequest(validations);

        //Return contract
        return CreatedAtAction(nameof(GetUser), model.GetRouteIdentifier(), ContractUtils.GenerateContract(model, data));
    }

    /// <summary>
    /// Updates existing user
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpPut("{id}")]
    [ApiAuthorizationFilter(Permissions.EditUser, Permissions.ManageUsers)]
    [ProducesResponseType(201)]
    public async Task<IActionResult> UpdateUser(BaseRequestId<UserUpdateRequest> request)
    {
        //Recupero l'elemento dal business layer
        var entity = AuthorizationLayer.GetUserById(request.Id);
        
        if(entity == null)
            return NotFound();

        var data = BasicLayer.GetUserData(entity.Id);

        //modifica solo se admin o se utente richiedente è lo stesso che ha creato
        if (entity == null || data == null)
            return NotFound();

        //Aggiornamento dell'entità
        entity.FirstName = request.Body.FirstName;
        entity.LastName = request.Body.LastName;
        entity.BirthDate = request.Body.BirthDate;
        entity.Email = request.Body.Email;
        entity.Username = request.Body.Username;
        data.FirearmsLicenceExpireDate = request.Body.FirearmsLicenceExpireDate;
        data.FirearmsLicenceReleaseDate = request.Body.FirearmsLicenceReleaseDate;
        data.FirearmsLicence = request.Body.FirearmsLicence;
        data.MedicalExaminationExpireDate = request.Body.MedicalExaminationExpireDate;
        data.BirthLocation = request.Body.BirthLocation;
        data.Address = request.Body.Address;
        data.City = request.Body.City;
        data.PostalCode = request.Body.PostalCode;
        data.Province = request.Body.Province;
        data.Country = request.Body.Country;
        data.Phone = request.Body.Phone;
        data.FiscalCode = request.Body.FiscalCode;

        //Salvataggio
        var validations = await BasicLayer.UpdateUser(entity, data, PlatformUtils.GetIdentityUserId(User));
        if (validations.Count > 0)
            return BadRequest(validations);

        //Confermo
        return NoContent();
    }

    /// <summary>
    /// Deletes existing user on platform
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpDelete("{id}")]
    [ApiAuthorizationFilter(Permissions.ManageUsers, Permissions.UserDelete)]
    [ProducesResponseType(201)]
    public async Task<IActionResult> DeleteUser(BaseRequestId request)
    {
        //Recupero l'elemento dal business layer
        var entity = AuthorizationLayer.GetUserById(request.Id);

        //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
        if (entity == null)
        {
            return NotFound();
        }

        //Invocazione del service layer
        var validations = await BasicLayer.DeleteUser(entity, PlatformUtils.GetIdentityUserId(User));
        if (validations.Count > 0)
            return BadRequest(validations);

        //Return contract
        return NoContent();
    }

    /// <summary>
    /// Update user password
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    /// <response code="200">Ok</response>
    /// <response code="400">Bad request</response>
    [HttpPost("{id}/reset-password")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> ResetUserPassword(BaseRequestId request)
    {
        //user identifier
        var userId = PlatformUtils.GetIdentityUserId(User);

        var user = AuthorizationLayer.GetUserById(request.Id);
        if (user == null)
        {
            return NotFound();
        }
        var validations = await SetPassworAliasOnUser(user, userId);

        if (validations.Count > 0)
        {
            return BadRequest(validations);
        }

        //Se è tutto ok, serializzo e restituisco
        return NoContent();
    }

    private async Task<IList<ValidationResult>> SetPassworAliasOnUser(User user, string userId = null)
    {
        var data = BasicLayer.GetUserData(user.Id);

        user.RestorePasswordAlias = Guid.NewGuid().ToString();

        var validations = await BasicLayer.UpdateUser(user, data, userId, false);

        if (validations.Count > 0)
        {
            return validations;
        }

        //sendEmail
        var message = new Message(new string[] { user.Email }, "Reset assword", $"Reset password from <a href=\"https://asdsemperprecis.it/app/reset-password?data={user.RestorePasswordAlias}\">here</a>", null, true);
        _emailSender.SendEmail(message);
        //Se è tutto ok, serializzo e restituisco
        return validations;
    }

    /// <summary>
    /// Update user password
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    /// <response code="200">Ok</response>
    /// <response code="400">Bad request</response>
    [HttpGet("alias/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserContract), 200)]
    public IActionResult GetUserFromRestorePasswordAlias(BaseRequestId request)
    {
        var user = AuthorizationLayer.GetUserByRestorePasswordAlias(request.Id);
        if (user == null)
            return NotFound();
        return Ok(ContractUtils.GenerateContract(user));
    }

    /// <summary>
    /// Updates existing password
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpPut("{id}/password")]
    [ApiAuthorizationFilter(Permissions.ManageUsers)]
    [ProducesResponseType(201)]
    public IActionResult UpdateUserPassword(BaseRequestId<UserPasswordUpdateRequest> request)
    {
        //Recupero l'elemento dal business layer
        var entity = AuthorizationLayer.GetUserById(request.Id);

        if (entity == null)
            return NotFound();

        //Salvataggio
        var validations = AuthorizationLayer.UpdateUserPassword(entity, request.Body.Password);
        if (validations.Count > 0)
            return BadRequest(validations);

        //Confermo
        return NoContent();
    }
    /// <summary>
    /// Updates existing user profile
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpPost("{id}/profile")]
    [ProducesResponseType(typeof(UserContract), 200)]
    public IActionResult UpdateProfile(BaseRequestId<UserProfileUpdateRequestV2> request)
    {
        //Recupero l'elemento dal business layer
        var entity = AuthorizationLayer.GetUserById(request.Id);

        if (entity == null)
            return NotFound();

        //Aggiornamento dell'entità
        entity.Username = request.Body.Username;
        entity.FirstName = request.Body.FirstName;
        entity.LastName = request.Body.LastName;
        entity.Email = request.Body.Email;
        entity.BirthDate = request.Body.BirthDate;

        //Salvataggio
        var validations = AuthorizationLayer.UpdateUser(entity);
        if (validations.Count > 0)
            return BadRequest(validations);

        //Confermo
        return CreatedAtAction(nameof(GetUser),entity.GetRouteIdentifier(), ContractUtils.GenerateContract(entity));
    }

    

    /// <summary>
    /// Fetch all permissions on user
    /// </summary>
    /// <returns>Returns action result</returns>
    [HttpGet("{id}/permissions")]
    [ProducesResponseType(typeof(UserPermissionContract), 200)]
    public async Task<IActionResult> FetchAllPermissionsOnUser(BaseRequestId request) =>
        Ok(ContractUtils.GenerateContract(await AuthorizationLayer.GetUserPermissionById(request.Id)));

    /// <summary>
    /// Link user to role
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpPost("{id}/roles/{roleId}/{entityId?}")]
    [ApiAuthorizationFilter(Permissions.ManagePermissions)]
    [ProducesResponseType(typeof(IList<UserRoleContract>), 200)]
    public async Task<IActionResult> CreateUserRole(UserRoleCreateRequestV2 request)
    {
        //Recupero l'elemento dal business layer
        var user = BasicLayer.GetShooter(request.Id);

        if (user == null)
        {
            return NotFound($"User with {request.Id} not found");
        }

        //Recupero l'elemento dal business layer
        var role = AuthorizationLayer.GetRole(request.RoleId);

        if (role == null)
        {
            return NotFound($"Role with {request.RoleId} not found");
        }

        var entity = new UserRole()
        {
            UserId = user.Id,
            RoleId = role.Id,
            EntityId = request.EntityId
        };
        //Invocazione del service layer
        var validations = await AuthorizationLayer.CreateUserRole(entity, PlatformUtils.GetIdentityUserId(User));

        if (validations.Count > 0)
            return BadRequest(validations);

        //Return contract
        return CreatedAtAction(nameof(GetUser), new { id = entity.UserId});
    }

    /// <summary>
    /// Deletes existing link between user and role
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns action result</returns>
    [HttpDelete("{id}/roles/{roleId}")]
    [ApiAuthorizationFilter(Permissions.ManagePermissions)]
    [ProducesResponseType(typeof(IList<UserRoleContract>), 200)]
    public async Task<IActionResult> DeleteUserRole(UserRoleCreateRequestV2 request)
    {
        //Recupero l'elemento dal business layer
        var entity = AuthorizationLayer.GetUserRole(request.Id,request.RoleId);

        //Se l'utente non hai i permessi non posso rimuovere entità con userId nullo
        if (entity == null)
        {
            return NotFound();
        }

        //Invocazione del service layer
        var validations = await AuthorizationLayer.DeleteUserRole(entity, PlatformUtils.GetIdentityUserId(User));
        if (validations.Count > 0)
            return BadRequest(validations);

        return NoContent();
    }
}
