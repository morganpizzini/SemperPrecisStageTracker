namespace SemperPrecisStageTracker.Contracts;

public class TeamHolderContract
{
    public string TeamHolderId { get; set; }
    public TeamContract Team { get; set; }
    public UserContract User { get; set; }
    public string Description { get; set; }

}