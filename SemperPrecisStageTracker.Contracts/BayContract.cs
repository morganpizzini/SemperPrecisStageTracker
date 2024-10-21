using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts
{
    public class FidelityCardTypeContract
    {
        public string FidelityCardTypeId { get; set; }
        public string Name { get; set; }
        public int MaxAccessNumber { get; set; }
        public PlaceContract Place { get; set; }
    }
    public class BayContract
    {
        public string BayId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
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