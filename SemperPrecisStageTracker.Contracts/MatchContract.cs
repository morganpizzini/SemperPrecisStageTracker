﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Contracts
{
    public class MatchContract
    {
        public string MatchId { get; set; }
        public string Name { get; set; }
        public DateTime MatchDateTime { get; set; }
        public DateTime CreationDateTime { get; set; }
        public IList<GroupContract> Groups { get; set; } = new List<GroupContract>();
        public IList<StageContract> Stages { get; set; } = new List<StageContract>();
    }
    public class ShooterStageContract
    {
        public string ShooterStageId { get; set; }
        public string ShooterId { get; set; }
        
        public string StageId { get; set; }
        
        public decimal Time { get; set; }
        
        public IList<int> DownPoints {get;set;} = new List<int>();
        /// <summary>
        /// X3
        /// </summary>
        public int Procedurals { get; set; }
        /// <summary>
        /// X5
        /// </summary>
        public int HitOnNonThreat { get; set; }
        /// <summary>
        /// X10
        /// </summary>
        public int FlagrantPenalties { get; set; }
        /// <summary>
        /// X20
        /// </summary>
        public int Ftdr { get; set; }

        public bool Disqualified  { get; set; }

        public decimal Total => Disqualified ? -99 : Time + DownPoints.Sum() + Procedurals*3 + HitOnNonThreat*5 + FlagrantPenalties*10 + Ftdr*20;
    }

    public class ShooterContract
    {
        public string ShooterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
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