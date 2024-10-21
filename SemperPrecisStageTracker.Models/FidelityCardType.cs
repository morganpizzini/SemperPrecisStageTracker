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
        public int MaxAccessNumber { get; set; }
        [Required]
        public string PlaceId { get; set; }
    }
    public class UserFidelityCard : SemperPrecisEntity
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string FidelityCardTypeId { get; set; }
        /// <summary>
        /// copy for avoid any change on main property
        /// </summary>
        [Required]
        public int MaxAccessNumber { get; set; }
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