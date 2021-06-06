namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterStageAggregationResult
    {
        public ShooterContract Shooter { get; set; }
        public ShooterStageContract ShooterStage { get; set; }
        public ShooterStatusEnum ShooterStatus { get; set; }
    }
}