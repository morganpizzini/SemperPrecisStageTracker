namespace SemperPrecisStageTracker.Contracts;

public class StageStringContract
{
    public string StageStringId { get; set; }
        
    public int Targets { get; set; }
    public string Name { get; set; }
    ///
    /// 12 rounds min, Unlimited
    ///
    public string Scoring { get; set; }
    ///
    /// 4 threat, 2 non threat.
    ///
    public string TargetsDescription { get; set; }
    ///
    /// Best 2 per paper
    ///
    public string ScoredHits { get; set; }
    ///
    /// Audible - Last shot
    ///
    public string StartStop { get; set; }
    ///
    /// From 6 yds to 10 yds
    ///
    public string Distance { get; set; }
    ///
    /// Required
    ///
    public bool CoverGarment { get; set; }

}