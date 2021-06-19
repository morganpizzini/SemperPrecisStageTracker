namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterMatchContract
    {
        public string ShooterMatchId { get; set; }
        public MatchContract Match { get; set; }
        public ShooterContract Shooter { get; set; }
    }
}