namespace SemperPrecisStageTracker
{
    public class UserStageResult
    {
        public string UserId { get; set; }
        public string StageName { get; set; }
        public decimal RawTime { get; set; }
        public decimal Total { get; set; }
    }
}