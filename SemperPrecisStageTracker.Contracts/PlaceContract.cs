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
        public string CompleteAddress => $"{Address}, {City} ({PostalCode}), {Region} - {Country}";
        public bool IsActive { get; set; }

    }
}