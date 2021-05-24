using System;
using System.Collections.Generic;
using SemperPrecisStageTracker.Models;

namespace SemperPrecisStageTracker
{
    public class DivisionMatchResult
    {
        public string Name { get; set; }
        public IList<string> StageNumber {get; set; } = new List<string>();
        public IList<ShooterRankResult> Ranks { get; set; }
    }
    public class ShooterRankResult{
        public string Rank { get; set; }
        public IList<ShooterMatchResult> Shooters { get; set; }

    }
    public class ShooterMatchResult
    {
        public Shooter Shooter { get; set; }
        public string Rank { get; set; }
        public string TeamName { get; set; }
        public IList<ShooterStageResult> Results { get; set; }
    }
    public class ShooterStageResult
    {
        public int StageIndex { get; set; }
        public decimal Total { get; set; }
    }
}