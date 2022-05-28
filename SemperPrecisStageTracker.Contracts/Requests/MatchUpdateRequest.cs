using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class MatchUpdateRequest
    {
        [Required]
        public string MatchId { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime MatchDateTimeStart { get; set; }
        [Required]
        [DateGreaterThan(nameof(MatchDateTimeStart))]
        public DateTime MatchDateTimeEnd { get; set; }
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string PlaceId { get; set; }
        public float Cost { get; set; }
        public string PaymentDetails { get; set; } = string.Empty;
        public bool UnifyClassifications { get; set; }
        public bool OpenMatch { get; set; }

    }
}