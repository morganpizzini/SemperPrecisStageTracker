using System.Collections.Generic;

namespace SemperPrecisStageTracker
{
    public class UserClassificationResult
    {
        public string Classification { get; set; }
        public IList<UserMatchResult> Users { get; set; }

    }
}