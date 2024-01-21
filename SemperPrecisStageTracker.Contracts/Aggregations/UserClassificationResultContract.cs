using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class UserClassificationResultContract
    {
        public string Classification { get; set; }
        public IList<UserMatchResultContract> Users { get; set; } = new List<UserMatchResultContract>();

    }
}