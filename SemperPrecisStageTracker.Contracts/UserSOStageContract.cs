namespace SemperPrecisStageTracker.Contracts
{
    public class UserSOStageContract
    {
        [IndexDbKey]
        public string UserSOStageId { get; set; }
        public string Role { get; set; }
        public UserContract User { get; set; }
        public StageContract Stage { get; set; }
    }
}