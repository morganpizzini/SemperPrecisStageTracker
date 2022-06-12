namespace SemperPrecisStageTracker.Contracts;

public class TeamHolderContract
{
    public string TeamHolderId { get; set; }
    public TeamContract Team { get; set; }
    public ShooterContract Shooter { get; set; }
    public string Description { get; set; }

}