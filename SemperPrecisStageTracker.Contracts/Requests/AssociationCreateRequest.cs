using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class AssociationCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IList<string> Divisions {get;set;} = new List<string>();
        [Required]
        public IList<string> Classifications {get;set;} = new List<string>();
    }

    public class CallShooterRequest
    {
        [Required]
        public string MatchId { get; set; }

        [Required]
        public string ShooterId { get; set; }
        
        [Required]
        public CallShooterContextEnum Context { get; set; }
    }

    public enum CallShooterContextEnum
    {
        MatchDirector = 0
    }
}