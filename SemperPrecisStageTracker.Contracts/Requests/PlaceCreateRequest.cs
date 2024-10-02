using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    
   
    public class BayUpdateRequest : EntityFilterValidation
    {
        public string EntityId => PlaceId;
        [Required]
        public string Name { get; set; }
        [Required]
        public string PlaceId { get; set; }

        public string Description { get; set; }
    }
    public class StatusUpdateRequest
    {
        [Required]
        public bool Status { get; set; }
    }
    public class ReservationUpdateDataRequest
    {
        [Required]
        public TimeOnly From { get; set; }
        [Required]
        public TimeOnly To { get; set; }
        [Required]
        public DateOnly Day { get; set; }
    }

    public class ScheduleUpdateRequest : EntityFilterValidation
    {
        public string EntityId => PlaceId;
        [Required]
        public string Name { get; set; }
        [Required]
        public string PlaceId { get; set; }

        public string Description { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public DayOfWeek Day { get; set; }

    }
    public class BayCreateRequest
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class ScheduleBayCreateRequest : EntityFilterValidation
    {
        [Required]
        public string ScheduleId { get; set; }
    }
    public class ScheduleCreateRequest : EntityFilterValidation
    {
        public string EntityId => PlaceId;
        [Required]
        public string Name { get; set; }
        [Required]
        public string PlaceId { get; set; }

        public string Description { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public DayOfWeek Day { get; set; }
    }

    public class ReservationCreateRequest
    {
        public string UserId { get; set; } = string.Empty;
        [Required]
        public TimeOnly From { get; set; } = TimeOnly.MinValue;
        [Required]
        public TimeOnly To { get; set; } = TimeOnly.MinValue;
        [Required]
        public DateOnly Day { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }

    public class ReservationBlockRequest
    {
        [Required]
        public string BayId { get; set; } = string.Empty;
        [Required]
        public TimeOnly From { get; set; }
        [Required]
        public TimeOnly To { get; set; }
        [Required]
        public DateOnly Day { get; set; }
    }

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

        public string EntityId => TeamId;
    }
    public class TeamReminderUpdateRequest : EntityFilterValidation
    {
        public string TeamReminderId { get; set; }
        [Required]
        public string TeamId { get; set; }
        public string ShooterId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public DateTime ExpireDateTime { get; set; }
        public bool NotifyExpiration { get; set; }

        public string EntityId => TeamId;
    }
}