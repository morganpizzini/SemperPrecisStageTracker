using System;

namespace SemperPrecisStageTracker.Contracts;

public class GroupShooterContract
{
    public string GroupShooterId { get; set; } = string.Empty;
    public GroupContract Group { get; set; } = new();
    public ShooterContract Shooter { get; set; } = new();
    public string Division { get; set; } = string.Empty;
    public TeamContract Team { get; set; } = new();
    public string Classification { get; set; }
    public DateTime? HasPay { get; set; }
}