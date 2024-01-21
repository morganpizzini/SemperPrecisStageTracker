using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.API.Models;

public class BaseRequest
{
    //[FromHeader(Name = "Environment")]
    //[Required]
    //public string Environment { get; set; } = string.Empty;
}
public class BaseRequest<T> : BaseRequest where T : class, new()
{
    [FromBody]
    public T Body { get; set; } = new();
}
public class BaseRequestId : BaseRequest
{
    [FromRoute]
    [Required]
    public string Id { get; set; } = string.Empty;
}

public class BaseRequestId<T> : BaseRequest<T> where T : class, new()
{
    [FromRoute]
    [Required]
    public string Id { get; set; } = string.Empty;
}
