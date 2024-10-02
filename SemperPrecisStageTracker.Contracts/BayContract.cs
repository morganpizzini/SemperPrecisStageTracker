using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class BayContract
    {
        public string BayId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool HasPrivateReservation { get; set; }
    }

    public class ScheduleContract
    {
        public string ScheduleId { get; set; } = string.Empty;
        public string PlaceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public DayOfWeek Day { get; set; }
    }
}