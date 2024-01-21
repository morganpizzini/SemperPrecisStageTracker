using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts;

public class UpdateDataRequest
{
    public IList<UserStageStringContract> ShooterStages { get; set; } = new List<UserStageStringContract>();
    public IList<EditedEntityRequest> EditedEntities { get; set; } = new List<EditedEntityRequest>();
}