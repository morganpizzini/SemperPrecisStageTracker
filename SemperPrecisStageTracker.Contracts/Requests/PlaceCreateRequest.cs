﻿using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class PlaceCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Holder { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string PostalZipCode { get; set; }
        [Required]
        public string Country { get; set; }
    }
}