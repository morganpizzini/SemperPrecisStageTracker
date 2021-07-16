namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterSOStageContract
    {
        [IndexDbKey]
        public string ShooterSOStageId { get; set; }
        public int Role { get; set; }
        public ShooterContract Shooter { get; set; }
        public StageContract Stage { get; set; }
    }
}