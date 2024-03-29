﻿namespace SemperPrecisStageTracker.Contracts;

public class UserRoleContract
{
    public string UserRoleId { get; set; } = string.Empty;
    public ShooterContract User { get; set; } = new();
    public RoleContract Role { get; set; } = new();
    public string EntityId { get; set; } = string.Empty;
}