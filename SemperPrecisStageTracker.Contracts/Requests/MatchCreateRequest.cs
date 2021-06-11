﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class MatchCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime MatchDateTime { get; set; } = DateTime.Now;
        [Required]
        public string AssociationId { get; set; }
        public string Location {get;set;}
        public bool UnifyClassifications { get; set; }
        public bool OpenMatch {get; set; }

    }
}