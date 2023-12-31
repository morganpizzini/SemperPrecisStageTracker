using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Models;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class UserRoleCreateRequest
{
    [Required]
    public string RoleId { get; set; } = string.Empty;
    [Required]
    public string UserId { get; set; } = string.Empty;
        
    public string EntityId { get; set; } = string.Empty;
}

public class UserRoleCreateRequestV2 : BaseRequestId
{
    [Required]
    [FromRoute]
    public string RoleId { get; set; } = string.Empty;
    [FromRoute]
    public string EntityId { get; set; } = string.Empty;
}