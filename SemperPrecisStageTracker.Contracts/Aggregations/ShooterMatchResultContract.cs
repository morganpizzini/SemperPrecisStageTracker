using System;
using System.Collections.Generic;
using System.Linq;

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
        public decimal Total { get; set; }
    }
    public class ShooterStageAggregationResult
    {
        public ShooterContract Shooter { get; set; }
        public ShooterStageContract ShooterStage { get; set; }
    }
    
}