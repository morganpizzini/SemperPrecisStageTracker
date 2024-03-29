﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{

    public class Association : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Range(0, int.MaxValue)]
        public float HitOnNonThreatPointDown { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public float FirstProceduralPointDown { get; set; }

        [Required(AllowEmptyStrings = true)] 
        public string FirstPenaltyLabel { get; set; } = string.Empty;
        
        [Required]
        [Range(0, int.MaxValue)]
        public float SecondProceduralPointDown { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string SecondPenaltyLabel { get; set; } = string.Empty;
        
        [Required]
        [Range(0, int.MaxValue)]
        public float ThirdProceduralPointDown { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string ThirdPenaltyLabel { get; set; } = string.Empty;
        
        
        [Required]
        public IList<string> MatchKinds { get; set; } = new List<string>();
        [Required]
        public IList<string> Divisions { get; set; } = new List<string>();
        [Required]
        public IList<string> Classifications { get; set; } = new List<string>();
        [Required]
        public IList<string> Categories { get; set; } = new List<string>();
        [Required]
        public IList<string> SoRoles { get; set; } = new List<string>();
    }
}