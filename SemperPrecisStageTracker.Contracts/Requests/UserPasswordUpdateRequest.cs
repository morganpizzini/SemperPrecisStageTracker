using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Update user password request
    /// </summary>
    public class UserPasswordUpdateRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// User new password
        /// </summary>
        [Required]
        public string Password { get; set; }

    }
}