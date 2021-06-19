using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterContract
    {
        public string ShooterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string AuthData {get;set;}
        public string Username {get;set;}
        public string Email {get;set;}
    }
}