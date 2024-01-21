namespace SemperPrecisStageTracker.Contracts
{
    public class UserMatchContract
    {
        [IndexDbKey]
        public string UserMatchId { get; set; }
        public MatchContract Match { get; set; }
        public UserContract User { get; set; }
    }
}