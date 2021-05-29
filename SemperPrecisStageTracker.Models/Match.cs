using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Match : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string AssociationId {get;set;}
        [Required]
        public string ShortLink { get; set; }
        public string Location {get;set;}
    public DateTime MatchDateTime { get; set; }
        ///
        // Unify rank and remove any difference between Novice, Marksman, Sharpshooter, Expert, Master
        ///
        public bool UnifyRanks { get; set; }
        ///
        // Allow any shooter to partecipate to the match
        ///
        public bool OpenMatch {get; set; }
    }
    public class Association : SemperPrecisEntity {
        [Required]
        public string Name { get; set; }
        [Required]
        public IList<string> Divisions {get;set;}
        [Required]
        public IList<string> Ranks {get;set;}
    }

    public class Team : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; }
    }
}