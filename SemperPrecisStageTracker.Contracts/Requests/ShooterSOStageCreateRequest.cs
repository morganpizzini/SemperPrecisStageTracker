using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterSOStageCreateRequest
    {
        public string StageId { get; set; }
        public IList<ShooterSOStageShooterContract> Shooters { get; set; }
    }
}