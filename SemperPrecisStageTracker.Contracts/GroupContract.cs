using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class GroupContract
    {
        [IndexDbKey]
        public string GroupId { get; set; }
        [IndexDbIndex]
        public string Name { get; set; }
        public MatchContract Match { get; set; }
        public IList<ShooterContract> Shooters { get; set; } = new List<ShooterContract>();
    }
}