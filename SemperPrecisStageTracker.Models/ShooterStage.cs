using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class ShooterStage : SemperPrecisEntity
    {
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string StageId { get; set; }
        [Required]
        public decimal Time { get; set; }
        //https://stackoverflow.com/questions/20711986/entity-framework-code-first-cant-store-liststring
        public IList<int> DownPoints {get;set;}
        /// <summary>
        /// X3
        /// </summary>
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
        public bool Procedural  { get; set; }
        public bool Disqualified  { get; set; }
        [NotMapped]
        public decimal Total => Disqualified ? -99 : Time + DownPoints.Sum() + Procedurals*3 + HitOnNonThreat*5 + FlagrantPenalties*10 + Ftdr*20;
    }
}