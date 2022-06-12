using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts;

public class UpdateDataRequest
{
    public IList<ShooterStageStringContract> ShooterStages { get; set; } = new List<ShooterStageStringContract>();
    public IList<EditedEntityRequest> EditedEntities { get; set; } = new List<EditedEntityRequest>();
}