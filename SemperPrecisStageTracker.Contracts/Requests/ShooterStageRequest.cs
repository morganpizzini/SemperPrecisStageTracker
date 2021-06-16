using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterStageRequest{
        
        [Required]
        public string StageId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public IList<int> DownPoints { get; set; } = new List<int>();
        [Required]
        public decimal Time { get; set; }
        
        public int Procedurals { get; set; }
        /// <summary>
        /// X5
        /// </summary>
        public int HitOnNonThreat { get; set; }
        /// <summary>
        /// X10
        /// </summary>
        public int FlagrantPenalties { get; set; }
        /// <summary>
        /// X20
        /// </summary>
        public int Ftdr { get; set; }
        public bool Warning  { get; set; }

        public bool Disqualified  { get; set; }

        public string Notes  { get; set; }

        public decimal Total => Disqualified ? -99 : Time + DownPoints.Sum() + Procedurals*3 + HitOnNonThreat*5 + FlagrantPenalties*10 + Ftdr*20;

    }
}