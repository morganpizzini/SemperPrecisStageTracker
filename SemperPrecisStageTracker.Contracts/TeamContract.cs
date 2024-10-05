using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class TeamContract
    {
        public string TeamId { get; set; }
        public string Name { get; set; }
        public IList<UserContract> TeamHolders { get; set; } = new List<UserContract>();
    }
}