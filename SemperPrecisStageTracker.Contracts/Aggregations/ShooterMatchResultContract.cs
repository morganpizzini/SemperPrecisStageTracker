using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterMatchResultContract
    {
        public ShooterContract Shooter { get; set; }
        public string Classification { get; set; }
        public string TeamName { get; set; }

        public IList<ShooterStageResultContract> Results { get; set; } = new List<ShooterStageResultContract>();
        public decimal Total => Results.Any(x => x.Total < 0) ? -99 : Results.Sum(x => x.Total);
    }
}