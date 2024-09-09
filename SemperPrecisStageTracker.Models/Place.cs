using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Place : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class PlaceData : SemperPrecisEntity
    {
        [Required]
        public string PlaceId { get; set; }
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


    }
}