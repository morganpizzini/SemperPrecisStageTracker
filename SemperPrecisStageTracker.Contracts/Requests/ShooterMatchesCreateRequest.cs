using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterMatchCreateRequest
    {
        public string MatchId { get; set; }
        public string ShooterId { get; set; }
    }
    public class ShooterMatchesCreateRequest
    {
        public string MatchId { get; set; }
        public IList<string> ShooterIds { get; set; }
    }
}