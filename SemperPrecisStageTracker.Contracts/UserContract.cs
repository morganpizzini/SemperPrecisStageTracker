using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class UserContract
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Username {get;set;}
        public string Email {get;set;}
    }
}