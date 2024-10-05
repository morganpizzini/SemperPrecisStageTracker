using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Shared.StageResults;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class DeleteShooterStageRequest
    {
        [Required]
        public string StageStringId { get; set; }
        [Required]
        public string StageId { get; set; }
        [Required]
        public string ShooterId { get; set; } 
    }
    public class ShooterStageRequest : IStageResult
    {
        [Required]
        public string StageStringId { get; set; }
        [Required]
        public string StageId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public IList<int> DownPoints { get; set; } = new List<int>();
        [Required]
        //[Range(1, Double.MaxValue, 
        //ErrorMessage = "minValueError {0}{1}{2}")]
        [Range(1, Double.MaxValue, ErrorMessage = "minValueError| {1}")]
        public decimal Time { get; set; }

        public int Procedurals { get; set; }
        public int Bonus { get; set; }
        /// <summary>
        /// X5
        /// </summary>
        public int HitOnNonThreat { get; set; }
        /// <summary>
        /// X10
        /// </summary>
        public int FlagrantPenalties { get; set; }

        public float FirstProceduralPointDown { get; set; }
        public float SecondProceduralPointDown { get; set; }
        public float ThirdProceduralPointDown { get; set; }
        public float HitOnNonThreatPointDown { get; set; }

        /// <summary>
        /// X20
        /// </summary>
        public int Ftdr { get; set; }
        public bool Warning { get; set; }

        public bool Disqualified { get; set; }

        public string Notes { get; set; }

    }
}