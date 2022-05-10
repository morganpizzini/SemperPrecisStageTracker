using System;
using System.Collections.Generic;
using System.Linq;
using SemperPrecisStageTracker.Models;

namespace SemperPrecisStageTracker
{
    public class ShooterMatchResult
    {
        public Shooter Shooter { get; set; }
        public string Classification { get; set; }
        public string TeamName { get; set; }
        public IList<ShooterStageResult> Results { get; set; }
        public decimal TotalTime => Results.Sum(x => x.Total);
    }
}