using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Contracts
{
    public class UserMatchResultContract
    {
        public UserContract User { get; set; }
        public string Classification { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;

        public IList<UserStageResultContract> Results { get; set; } = new List<UserStageResultContract>();
        public decimal Total => Results.Any(x => x.Total < 0) ? -99 : Results.Sum(x => x.Total);
        public decimal RawTime => Results.Sum(x => x.RawTime);
    }
}