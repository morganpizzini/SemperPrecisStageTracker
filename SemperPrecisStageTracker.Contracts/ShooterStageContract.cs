using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterStageContract
    {
        public string ShooterStageId { get; set; }
        public string ShooterId { get; set; }

        public string StageId { get; set; }

        public decimal Time { get; set; }

        public IList<int> DownPoints {get;set;} = new List<int>();
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

        public bool Warning  { get; set; }
        public string Notes { get; set; }
        public bool Disqualified  { get; set; }

        public decimal Total => Disqualified ? -99 : Time + DownPoints.Sum() + Procedurals*3 + HitOnNonThreat*5 + FlagrantPenalties*10 + Ftdr*20;
    }
}