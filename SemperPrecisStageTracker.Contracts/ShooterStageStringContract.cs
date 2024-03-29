﻿using System.Collections.Generic;
using System.Linq;
using SemperPrecisStageTracker.Shared.StageResults;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterStageStringContract : IStageResult
    {
        //public string ShooterStageId { get; set; }
        public string ShooterId { get; set; }

        public string StageStringId { get; set; }

        public decimal Time { get; set; }

        public IList<int> DownPoints { get; set; } = new List<int>();
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
        public string Notes { get; set; }
        public bool Disqualified { get; set; }
    }
}