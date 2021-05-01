using System;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class MatchContract
    {
        public string MatchId { get; set; }
        public string Name { get; set; }
        public DateTime MatchDateTime { get; set; }
        public DateTime CreationDateTime { get; set; }

    }
    public class ShooterContract
    {
        public string ShooterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class GroupContract
    {
        public string GroupId { get; set; }
        public string Name { get; set; }
        public MatchContract Match { get; set; }
        public IList<ShooterContract> Shooters { get; set; } = new List<ShooterContract>();
    }
    public class StageContract
    {
        public string StageId { get; set; }
        public string Name { get; set; }
        public int Targets { get; set; }
        public MatchContract Match { get; set; }
    }
    
}