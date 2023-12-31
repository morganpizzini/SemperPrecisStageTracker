using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class UserUpdateRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; } = new(1980, 1, 1);
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
    }
    public class UserUpdateRequestV2
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; } = new(1980, 1, 1);
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
    }
}