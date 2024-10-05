using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class PlaceContract
    {
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public string Holder { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string CompleteAddress => !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(City) ? $"{Address}, {City} ({PostalCode}), {Region} - {Country}" : "";
        public bool IsActive { get; set; }

    }

    public class ReservationContract
    {
        public string ReservationId { get; set; } = string.Empty;
        public UserContract User { get; set; } = new UserContract();
        public BayContract Bay { get; set; } = new BayContract();
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public DateOnly Day { get; set; }
        public bool IsAccepted { get; set; } = false;
        public string Demands { get; set; } = string.Empty;

        // whenever is true, the bay is blocked for the time of the reservation, and cannot be reserved by anyone else
        public bool IsBayBlocked { get; set; } = false;
    }
}