using System;

namespace SemperPrecisStageTracker.Contracts;

public class GroupUserContract
{
    public string GroupUserId { get; set; } = string.Empty;
    public GroupContract Group { get; set; } = new();
    public UserContract User { get; set; } = new();
    public string Division { get; set; } = string.Empty;
    public TeamContract Team { get; set; } = new();
    public string Classification { get; set; }
    public DateTime? HasPay { get; set; }
}