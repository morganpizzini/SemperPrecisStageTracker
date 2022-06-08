using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterGroupMoveRequest
    {
        [Required]

        public string GroupShooterId { get; set; }
        [Required]

        public string GroupId { get; set; }
        
        public string ShooterName { get; set; }
    }
    
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

    public class GroupShooterCreateRequest
    {
        [Required]

        public string GroupId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]  
        public string DivisionId { get; set; }
        [Required]
        public string TeamId { get; set; }
        
        public DateTime? HasPay { get; set; }

    }

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
    
}