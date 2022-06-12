using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SemperPrecisStageTracker.Models.Commons;
using SemperPrecisStageTracker.Shared.StageResults;

namespace SemperPrecisStageTracker.Models
{
    public class ShooterStage : SemperPrecisEntity, IStageResult
    {
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string StageStringId { get; set; }
        [Required]
        public decimal Time { get; set; }
        //https://stackoverflow.com/questions/20711986/entity-framework-code-first-cant-store-liststring
        public IList<int> DownPoints { get; set; }
        /// <summary>
        /// X3
        /// </summary>
        public int Bonus { get; set; }
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