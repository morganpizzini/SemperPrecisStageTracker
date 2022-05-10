using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Team request
    /// </summary>
    public class PlaceRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string PlaceId { get; set; }

    }
}