﻿using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Stage : SemperPrecisEntity
    {
        public string MatchId { get; set; }
        public string Name { get; set; }
        ///
        /// Stage index for sorting
        ///
        public int Index { get; set; }
        public int Targets { get; set; }

        [StringLength(1000)]
        public string Scenario { get; set; }
        [StringLength(1000)]
        public string GunReadyCondition { get; set; }
        [StringLength(1000)]
        public string StageProcedure { get; set; }
        [StringLength(1000)]
        public string StageProcedureNotes { get; set; }
        public int Strings { get; set; }

        ///
        /// 12 rounds min, Unlimited
        ///
        public string Scoring { get; set; }
        ///
        /// 4 threat, 2 non threat.
        ///
        public string TargetsDescription { get; set; }
        ///
        /// Best 2 per paper
        ///
        public string ScoredHits { get; set; }
        ///
        /// Audible - Last shot
        ///
        public string StartStop { get; set; }
        ///
        /// Rulebook-2017.-3;
        ///
        public string Rules { get; set; }
        ///
        /// From 6 yds to 10 yds
        ///
        public string Distance { get; set; }
        ///
        /// Required
        ///
        public bool CoverGarment { get; set; }

    }
}