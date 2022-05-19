using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class MatchCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime MatchDateTimeStart { get; set; } = DateTime.Now;
        public DateTime MatchDateTimeEnd { get; set; } = DateTime.Now;
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string PlaceId { get; set; }

        public bool UnifyClassifications { get; set; }
        public bool OpenMatch { get; set; }

    }
}