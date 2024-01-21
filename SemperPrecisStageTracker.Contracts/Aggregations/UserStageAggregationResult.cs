using System;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class UserStageAggregationResult
    {
        [IndexDbKey]
        public string EditedEntityId { get; set; } = Guid.NewGuid().ToString();
        public GroupUserContract GroupUser { get; set; }
        public string GroupId { get; set; }
        public string StageId { get; set; }
        public IList<UserStageStringContract> UserStage { get; set; }
        public UserStatusEnum UserStatus { get; set; }
    }
}