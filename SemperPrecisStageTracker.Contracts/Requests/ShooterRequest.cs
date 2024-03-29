﻿using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Shooter request
    /// </summary>
    public class ShooterRequest : EntityFilterValidation

    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string ShooterId { get; set; }


        public override string EntityId => ShooterId;
    }
}
