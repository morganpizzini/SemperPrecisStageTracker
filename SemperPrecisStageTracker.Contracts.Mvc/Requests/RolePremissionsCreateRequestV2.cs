using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Models;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Mvc.Requests;
public class RolePermissionCreateRequestV2 : BaseRequestId
{
    [Required]
    [FromRoute]
    public string RoleId { get; set; } = string.Empty;
}