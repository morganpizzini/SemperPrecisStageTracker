using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Models;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class RolePermissionCreateRequest
{
    [Required]
    public string RoleId { get; set; } = string.Empty;
    [Required]
    public string PermissionId { get; set; } = string.Empty;
        
    //public string EntityId { get; set; } = string.Empty;
}

public class RolePermissionCreateRequestV2 : BaseRequestId
{
    [Required]
    [FromRoute]
    public string RoleId { get; set; } = string.Empty;
}