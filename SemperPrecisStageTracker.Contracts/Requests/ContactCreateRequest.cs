using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ContactCreateRequest
    {
        [Required]
        public string Token { get; set; }
    }
}