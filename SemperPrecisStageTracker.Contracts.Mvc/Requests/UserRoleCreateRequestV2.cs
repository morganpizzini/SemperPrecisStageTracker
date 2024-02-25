using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Models;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Mvc.Requests;
public class UserRoleCreateRequestV2 : BaseRequestId
{
    [Required]
    [FromRoute]
    public string RoleId { get; set; } = string.Empty;
    [FromRoute]
    public string EntityId { get; set; } = string.Empty;
}
public class TakeSkipRequest
{
    [FromQuery]
    public int? Skip { get; set; }
    [FromQuery]
    public int? Take { get; set; }
}