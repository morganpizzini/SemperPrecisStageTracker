namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterSOStageCreateRequest
    {
        public string StageId { get; set; }
        public ShooterSOStageShooterContract Shooter { get; set; } = new();
    }
}