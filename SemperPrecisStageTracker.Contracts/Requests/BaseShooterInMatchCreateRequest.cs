using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

/// <summary>
/// used for component
/// </summary>
public class BaseShooterInMatchCreateRequest 
{
    [Required]

    public string GroupId { get; set; }
    [Required]

    public string MatchId { get; set; }
    [Required]
    public string ShooterId { get; set; }
    [Required]  
    public string DivisionId { get; set; }
    [Required]
    public string TeamId { get; set; }
        
    public DateTime? HasPay { get; set; }
}