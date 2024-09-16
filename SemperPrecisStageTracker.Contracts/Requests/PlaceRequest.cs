using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Team request
    /// </summary>
    public class PlaceRequest : EntityFilterValidation
    {
        public string EntityId => PlaceId;
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string PlaceId { get; set; }

    }

        /// <summary>
    /// Team request
    /// </summary>
    public class TeamReminderRequest : EntityFilterValidation
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string TeamReminderId { get; set; }
        [Required]
        public string TeamId { get; set; }
        public string EntityId => TeamId;

    }
}