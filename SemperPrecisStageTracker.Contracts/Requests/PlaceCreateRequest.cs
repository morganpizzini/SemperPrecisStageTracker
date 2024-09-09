using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class PlaceCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Holder { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Country { get; set; }
        public bool IsActive { get; set; }
    }

    public class TeamReminderCreateRequest : EntityFilterValidation
    {
        [Required]
        public string TeamId { get; set; }
        public string ShooterId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public DateTime ExpireDateTime { get; set; }
        public bool NotifyExpiration { get; set; }

        public override string EntityId => TeamId;
    }
    public class TeamReminderUpdateRequest : EntityFilterValidation
    {
        public string TeamReminderId {get; set;}
        [Required]
        public string TeamId { get; set; }
        public string ShooterId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public DateTime ExpireDateTime { get; set; }
        public bool NotifyExpiration { get; set; }

        public override string EntityId => TeamId;
    }
}