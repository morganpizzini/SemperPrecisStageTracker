using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Contracts
{
    public class DivisionMatchResultContract{
        public string Name { get; set; }
        public IList<string> StageNumber {get; set; } = new List<string>();
        public IList<ShooterRankResultContract> Ranks { get; set; } = new List<ShooterRankResultContract>();
    }
    public class ShooterRankResultContract{
        public string Rank { get; set; }
        public IList<ShooterMatchResultContract> Shooters { get; set; } = new List<ShooterMatchResultContract>();

    }
    public class ShooterMatchResultContract
    {
        public ShooterContract Shooter { get; set; }
        public string Rank { get; set; }
        public string TeamName { get; set; }

        public IList<ShooterStageResultContract> Results { get; set; } = new List<ShooterStageResultContract>();
        public decimal Total => Results.Any(x=> x.Total<0) ? -99 : Results.Sum(x=>x.Total);
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