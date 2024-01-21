using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class DivisionMatchResultContract
    {
        public string Name { get; set; }
        public IList<UserClassificationResultContract> Classifications { get; set; } = new List<UserClassificationResultContract>();
    }
}