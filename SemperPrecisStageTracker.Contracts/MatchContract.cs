using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Contracts
{
    public class AssociationContract
    {
        public string AssociationId { get; set; }
        public string Name { get; set; }
        public IList<string> Divisions {get;set;} = new List<string>();
        public IList<string> Ranks {get;set;} = new List<string>();
    }
    public class TeamContract
    {
        public string TeamId { get; set; }
        public string Name { get; set; }
    }
    public class MatchContract
    {
        public string MatchId { get; set; }
        public string Name { get; set; }
        public DateTime MatchDateTime { get; set; }
        public string Location {get;set;}

        public DateTime CreationDateTime { get; set; }
        public bool UnifyRanks { get; set; }
        public bool OpenMatch {get; set; }
        public AssociationContract Association {get;set;}
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

        public bool Procedural  { get; set; }
        public bool Disqualified  { get; set; }

        public decimal Total => Disqualified ? -99 : Time + DownPoints.Sum() + Procedurals*3 + HitOnNonThreat*5 + FlagrantPenalties*10 + Ftdr*20;
    }

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
    public class GroupContract
    {
        public string GroupId { get; set; }
        public string Name { get; set; }
        public MatchContract Match { get; set; }
        public IList<ShooterContract> Shooters { get; set; } = new List<ShooterContract>();
    }
    
    public class ShooterTeamContract {
        public TeamContract Team {get;set;}
        public ShooterContract Shooter {get;set;}
        public DateTime RegistrationDate { get; set; }
    }
    public class ShooterAssociationContract {
        public AssociationContract Association{get;set;}
        public string Rank { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
    public class StageContract
    {
        public string StageId { get; set; }
        public string Name { get; set; }
        public int Targets { get; set; }
        public int Index { get; set; }
        public MatchContract Match { get; set; }
        public string SO {get;set;}
        public string Scenario {get;set;}
        public string GunReadyCondition {get;set;}
        public string StageProcedure {get;set;}
        public string StageProcedureNotes {get;set;}
        public int Strings {get;set;}

        ///
        /// 12 rounds min, Unlimited
        ///
        public string Scoring {get;set;}
        ///
        /// 4 threat, 2 non threat.
        ///
        public string TargetsDescription {get;set;}
        ///
        /// Best 2 per paper
        ///
        public string ScoredHits {get;set;}
        ///
        /// Audible - Last shot
        ///
        public string StartStop {get;set;}
        ///
        /// Rulebook-2017.-3;
        ///
        public string Rules {get;set;}
        ///
        /// From 6 yds to 10 yds
        ///
        public string Distance {get;set;}
        ///
        /// Required
        ///
        public bool CoverGarment {get;set;}
    }
    
}