﻿using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterStageAggregationResult
    {
        [IndexDbKey]
        public string EditedEntityId { get; set; } = Guid.NewGuid().ToString();
        public GroupShooterContract GroupShooter { get; set; }
        public string GroupId { get; set; }
        public ShooterStageContract ShooterStage { get; set; }
        public ShooterStatusEnum ShooterStatus { get; set; }
    }
}