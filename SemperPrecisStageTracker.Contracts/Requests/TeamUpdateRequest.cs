﻿using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class TeamUpdateRequest : EntityFilterValidation
    {
        public string EntityId => TeamId;
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}