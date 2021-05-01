using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{

    /// <summary>
    /// Stage request
    /// </summary>
    public class StageRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string StageId { get; set; }

    }

    public class StageCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Targets { get; set; }
        [Required]
        public string MatchId { get; set; }
    }

    public class StageUpdateRequest
    {
        [Required]
        public string StageId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Targets { get; set; }
        
    }

    public class ShooterStageRequest{
        [Required]
        public string StageId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public IList<int> DownPoints { get; set; }
        public int Penalties { get; set; }
        public int Procedures { get; set; }
        public int FailureToNeutralize { get; set; }
        public int MissedEngagement { get; set; }
    }
}
