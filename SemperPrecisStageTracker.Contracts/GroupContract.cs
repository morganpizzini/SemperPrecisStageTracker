using System;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    
    public class GroupContract
    {
        [IndexDbKey]
        public string GroupId { get; set; }
        [IndexDbIndex]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime GroupDay {get; set; }
        public int Index { get; set; }
        public int MaxUserNumber { get; set; }
        public MatchContract Match { get; set; }
        public IList<GroupUserContract> Users { get; set; } = new List<GroupUserContract>();
    }
}