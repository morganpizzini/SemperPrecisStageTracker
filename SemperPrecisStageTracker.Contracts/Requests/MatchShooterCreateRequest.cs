using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class MatchShooterCreateRequest
{
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