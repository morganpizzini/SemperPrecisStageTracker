using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts;

public class UpdateDataRequest
{
    public IList<ShooterStageContract> ShooterStages { get; set; } = new List<ShooterStageContract>();
    public IList<EditedEntityRequest> EditedEntities { get; set; } = new List<EditedEntityRequest>();
}