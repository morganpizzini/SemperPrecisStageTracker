namespace SemperPrecisStageTracker.Models
{
    /// <summary>
    /// Identify role correlation between a shooter and the match
    /// </summary>
    public class UserMatch : UserRelationRole
    {
        public string MatchId { get; set; }
    }
}