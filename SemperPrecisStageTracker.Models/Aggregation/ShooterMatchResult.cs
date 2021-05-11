using System;
using System.Collections.Generic;
using SemperPrecisStageTracker.Models;

namespace SemperPrecisStageTracker
{
    public class ShooterMatchResult
    {
        public Shooter Shooter { get; set; }
        public IList<ShooterStageResult> Results { get; set; }
    }
    public class ShooterStageResult
    {
        public int StageIndex { get; set; }
        public decimal Total { get; set; }
    }
}