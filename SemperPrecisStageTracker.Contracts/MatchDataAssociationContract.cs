using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts;

public class MatchDataAssociationContract
{
    public MatchContract Match { get; set; }
    //public IList<ShooterContract> Shooters { get; set; }
    //public IList<StageContract> Stages { get; set; }
    //public IList<GroupContract> Groups { get; set; }
    public IList<ShooterStageAggregationResult> ShooterStages { get; set; }
    public IList<ShooterMatchContract> ShooterMatches { get; set; }
    public IList<ShooterSOStageContract> ShooterSoStages { get; set; }
}