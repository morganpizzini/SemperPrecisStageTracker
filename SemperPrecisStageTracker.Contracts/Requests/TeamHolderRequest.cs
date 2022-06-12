using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

/// <summary>
/// TeamHolder request
/// </summary>
public class TeamHolderRequest
{
    /// <summary>
    /// Identifier
    /// </summary>
    [Required]
    public string TeamHolderId { get; set; }

}