using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts;

public class MatchDataAssociationContract
{
    public MatchContract Match { get; set; }
    //public IList<ShooterContract> Shooters { get; set; }
    //public IList<StageContract> Stages { get; set; }
    //public IList<GroupContract> Groups { get; set; }
    public IList<UserStageAggregationResult> ShooterStages { get; set; }
    public IList<UserMatchContract> ShooterMatches { get; set; }
    public IList<UserSOStageContract> ShooterSoStages { get; set; }
}