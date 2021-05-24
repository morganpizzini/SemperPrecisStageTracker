using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        [Required]
        public int Index { get; set; }
        public string SO {get;set;}
        public string Scenario {get;set;}
        public string GunReadyCondition {get;set;}
        public string StageProcedure {get;set;}
        public string StageProcedureNotes {get;set;}
        public int Strings {get;set;}
        
        ///
        /// 12 rounds min, Unlimited
        ///
        public string Scoring {get;set;}
        ///
        /// 4 threat, 2 non threat.
        ///
        public string TargetsDescription {get;set;}
        ///
        /// Best 2 per paper
        ///
        public string ScoredHits {get;set;}
        ///
        /// Audible - Last shot
        ///
        public string StartStop {get;set;}
        ///
        /// Rulebook-2017.-3;
        ///
        public string Rules {get;set;}
        ///
        /// From 6 yds to 10 yds
        ///
        public string Distance {get;set;}
        ///
        /// Required
        ///
        public bool CoverGarment {get;set;}
    }

    public class StageUpdateRequest
    {
        [Required]
        public string StageId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Targets { get; set; }
        [Required]
        public int Index { get; set; }
        public string SO {get;set;}
        public string Scenario {get;set;}
        public string GunReadyCondition {get;set;}
        public string StageProcedure {get;set;}
        public string StageProcedureNotes {get;set;}
        public int Strings {get;set;}

        ///
        /// 12 rounds min, Unlimited
        ///
        public string Scoring {get;set;}
        ///
        /// 4 threat, 2 non threat.
        ///
        public string TargetsDescription {get;set;}
        ///
        /// Best 2 per paper
        ///
        public string ScoredHits {get;set;}
        ///
        /// Audible - Last shot
        ///
        public string StartStop {get;set;}
        ///
        /// Rulebook-2017.-3;
        ///
        public string Rules {get;set;}
        ///
        /// From 6 yds to 10 yds
        ///
        public string Distance {get;set;}
        ///
        /// Required
        ///
        public bool CoverGarment {get;set;}
        
    }

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
        public bool Procedural  { get; set; }

        public bool Disqualified  { get; set; }

        public decimal Total => Disqualified ? -99 : Time + DownPoints.Sum() + Procedurals*3 + HitOnNonThreat*5 + FlagrantPenalties*10 + Ftdr*20;

    }
}
