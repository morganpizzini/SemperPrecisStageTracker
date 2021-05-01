using System;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterMatchResultContract
    {
        public ShooterContract Shooter { get; set; }
        public IList<ShooterStageResultContract> Results { get; set; }
    }
    public class ShooterStageResultContract
    {
        public int StageIndex { get; set; }
        public int Total { get; set; }
    }
}