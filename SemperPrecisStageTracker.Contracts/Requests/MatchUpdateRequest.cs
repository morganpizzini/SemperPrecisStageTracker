using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class MatchUpdateRequest : EntityFilterValidation
    {
        [Required]
        public string MatchId { get; set; }

        public string EntityId => MatchId;

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
        public string Kind { get; set; } = string.Empty;
        public float Cost { get; set; }
        public string PaymentDetails { get; set; } = string.Empty;

    }
}