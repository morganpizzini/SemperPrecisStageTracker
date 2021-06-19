namespace SemperPrecisStageTracker.Models
{
    /// <summary>
    /// Identify role correlation between a shooter and the match
    /// </summary>
    public class ShooterMatch : ShooterRole
    {
        public string MatchId { get; set; }
    }
}