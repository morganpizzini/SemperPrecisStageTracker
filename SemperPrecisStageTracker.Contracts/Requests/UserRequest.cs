using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class UserRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string UserId { get; set; }
    }
}