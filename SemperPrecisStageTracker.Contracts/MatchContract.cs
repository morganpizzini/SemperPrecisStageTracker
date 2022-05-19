using System;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class MatchContract
    {
        [IndexDbKey]
        public string MatchId { get; set; }
        [IndexDbIndex]
        public string Name { get; set; }
        public DateTime MatchDateTimeStart { get; set; }
        public DateTime MatchDateTimeEnd { get; set; }
        public string ShortLink { get; set; }

        public DateTime CreationDateTime { get; set; }
        public bool UnifyClassifications { get; set; }
        public bool OpenMatch { get; set; }
        public AssociationContract Association { get; set; } = new();
        public PlaceContract Place { get; set; }
        public IList<GroupContract> Groups { get; set; } = new List<GroupContract>();
        public IList<StageContract> Stages { get; set; } = new List<StageContract>();
    }
}