using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class ShooterStage : SemperPrecisEntity
    {
        public string ShooterId { get; set; }
        public string StageId { get; set; }
        //https://stackoverflow.com/questions/20711986/entity-framework-code-first-cant-store-liststring
        public IList<int> DownPoints {get;set;}
        public int Penalties { get; set; }
        public int Procedures { get; set; }
        public int FailureToNeutralize { get; set; }
        public int MissedEngagement { get; set; }
        [NotMapped]
        public int Total => DownPoints.Sum() + Penalties + Procedures + FailureToNeutralize + MissedEngagement;
    }
}