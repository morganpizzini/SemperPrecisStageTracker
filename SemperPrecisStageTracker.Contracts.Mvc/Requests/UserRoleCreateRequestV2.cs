﻿using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.API.Models;
using SemperPrecisStageTracker.Contracts.Requests;
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
public class EntityTakeSkipRequest : TakeSkipRequest
{
    [FromQuery]
    public string RefId { get; set; } = string.Empty;
}
public class GetScheduleRequest : BaseRequestId, EntityFilterValidation
{
    public string EntityId => RefId;
    [Required]
    [FromQuery]
    public string RefId { get; set; } = string.Empty;
}
public class DeleteEntityRefRequest : BaseRequestId, EntityFilterValidation
{
    public string EntityId => RefId;
    [Required]
    [FromQuery]
    public string RefId { get; set; } = string.Empty;
}
public class EntityBaseRequestId<T> : BaseRequest<T>, EntityFilterValidation where T : class, new()
{
    public string EntityId => Id;

    [FromRoute]
    [Required]
    public string Id { get; set; } = string.Empty;
}
public class EntityBaseRequestId : BaseRequestId, EntityFilterValidation
{
    public string EntityId => Id;
}
public class BayEntityBaseRequestId : EntityBaseRequestId
{
    [FromRoute]
    [Required]
    public string BayId { get; set; } = string.Empty;
}
public class BayEntityBaseRequestId<T> : EntityBaseRequestId<T> where T : class, new()
{
    [FromRoute]
    [Required]
    public string BayId { get; set; } = string.Empty;
}
public class TakeSkipBaseRequestId : BaseRequestId, EntityFilterValidation
{
    public string EntityId => Id;
    [FromQuery]
    public int? Skip { get; set; }
    [FromQuery]
    public int? Take { get; set; }
}
public class TakeSkipRequest
{
    [FromQuery]
    public int? Skip { get; set; }
    [FromQuery]
    public int? Take { get; set; }
}
public class BayScheduleDeleteRequest : BaseRequestId, EntityFilterValidation
{
    public string EntityId => RefId;
    [FromRoute]
    [Required]
    public string ScheduleId { get; set; } = string.Empty;
    [Required]
    [FromQuery]
    public string RefId { get; set; } = string.Empty;
}

public class ReservationUpdateRequest : BaseRequestId<ReservationUpdateDataRequest>
{
    [FromRoute]
    [Required]
    public string ReservationId { get; set; } = string.Empty;
}

public class ReservationDeleteRequest : BaseRequestId
{
    [FromRoute]
    [Required]
    public string ReservationId { get; set; } = string.Empty;
}