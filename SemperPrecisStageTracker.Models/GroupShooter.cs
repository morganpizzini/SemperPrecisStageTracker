﻿using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class GroupShooter : SemperPrecisEntity
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string DivisionId {get; set;}
        [Required]
        public string TeamId {get; set;}
    }
}