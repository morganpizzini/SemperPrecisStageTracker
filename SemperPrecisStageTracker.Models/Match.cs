using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Match : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShortLink { get; set; }
        public string PlaceId { get; set; }
        public float Cost { get; set; }
        public bool CompetitionReady {get; set; }
        public string PaymentDetails { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public DateTime MatchDateTimeStart { get; set; }
        public DateTime MatchDateTimeEnd { get; set; }
        ///
        // Unify classification and remove any difference between Novice, Marksman, Sharpshooter, Expert, Master
        ///
        public bool UnifyClassifications { get; set; }
        ///
        // Allow any shooter to partecipate to the match
        ///
        public bool OpenMatch { get; set; }
    }
}