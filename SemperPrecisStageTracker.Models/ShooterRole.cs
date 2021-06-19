﻿using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public abstract class ShooterRole : SemperPrecisEntity
    {
        public string ShooterId { get; set; }
        public ShooterRoleEnum Role { get; set; }
    }
}