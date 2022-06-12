using System;

namespace SemperPrecisStageTracker.Contracts;

public class EditedEntityRequest
{
    public string EntityId { get; set; }
    public DateTime EditDateTime { get; set; } = DateTime.UtcNow;
}