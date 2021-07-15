using System;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class IndexDbKeyAttribute : Attribute
    {
        public string Name { get; set; }
        public bool AutoIncrement { get; set; } = false;
        public bool Unique { get; set; } = true;
    }
    public class IndexDbIndexAttribute : Attribute
    {
        public bool Unique { get; set; } = false;
        public string Name { get; set; }
    }
    public class MatchContract
    {
        [IndexDbKey]
        public string MatchId { get; set; }
        [IndexDbIndex]
        public string Name { get; set; }
        public DateTime MatchDateTime { get; set; }
        public string ShortLink { get; set; }

        public DateTime CreationDateTime { get; set; }
        public bool UnifyClassifications { get; set; }
        public bool OpenMatch {get; set; }
        public AssociationContract Association {get;set;}
        public PlaceContract Place {get;set;}
        public IList<GroupContract> Groups { get; set; } = new List<GroupContract>();
        public IList<StageContract> Stages { get; set; } = new List<StageContract>();
    }
}