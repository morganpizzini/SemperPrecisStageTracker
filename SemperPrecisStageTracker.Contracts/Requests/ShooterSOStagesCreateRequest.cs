using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterSOStagesCreateRequest
    {
        public string StageId { get; set; }
        public IList<ShooterSOStageShooterContract> Shooters { get; set; }
    }
}