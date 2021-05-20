using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Match : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string AssociationId {get;set;}
        public string Location {get;set;}
        public DateTime MatchDateTime { get; set; }
    }
    public class Association : SemperPrecisEntity {
        [Required]
        public string Name { get; set; }
        [Required]
        public IList<string> Divisions {get;set;}
        [Required]
        public IList<string> Classes {get;set;}
    }
    
    public class Team : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; }
    }
}