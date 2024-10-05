using SemperPrecisStageTracker.Models.Commons;
using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Models
{
    public class FidelityCardType : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int AccessNumber { get; set; }
        [Required]
        public string PlaceId { get; set; }
    }
    public class UserFidelityCard : SemperPrecisEntity
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string FidelityCardTypeId { get; set; }
        public bool IsExpired { get; set; }
    }
    public class UserFidelityCardAccess : SemperPrecisEntity
    {
        [Required]
        public string UserFidelityCardId { get; set; }
        [Required]
        public DateTime AccessDateTime { get; set; }
        public string ReservationId { get; set; }
    }
}