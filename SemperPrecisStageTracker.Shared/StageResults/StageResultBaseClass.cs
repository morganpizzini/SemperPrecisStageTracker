using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Shared.StageResults;

public interface IStageResult
{
    IList<int> DownPoints { get; set; }
    
    decimal Time { get; set; }

    int Procedurals { get; set; }
    int Bonus { get; set; }
    int HitOnNonThreat { get; set; }
    int FlagrantPenalties { get; set; }
    float FirstProceduralPointDown { get; set; }
    float SecondProceduralPointDown { get; set; }
    float ThirdProceduralPointDown { get; set; }
    float HitOnNonThreatPointDown { get; set; }
    public int Ftdr { get; set; }

    bool Disqualified { get; set; }
    
    public decimal Total => Disqualified
        ? -99
        : Time - Bonus + (DownPoints?.DefaultIfEmpty(0).Sum() ?? 0) + Procedurals * (decimal)FirstProceduralPointDown +
        HitOnNonThreat * (decimal)HitOnNonThreatPointDown + FlagrantPenalties * (decimal)SecondProceduralPointDown + Ftdr * (decimal)ThirdProceduralPointDown;

    public string TotalString
    {
        get
        {
            var t = Total;
            return t > 0 ? $"{t:f2}" : t < 0 ? "Disqualified" : "";
        }
    }
}

//public static class StageResultsExtensions
//{
//    public static decimal Total(this IStageResult entity) => entity.Disqualified
//        ? -99
//        : entity.Time - entity.Bonus + entity.DownPoints?.DefaultIfEmpty(0).Sum() ?? 0 + entity.Procedurals * (decimal)entity.FirstProceduralPointDown +
//        entity.HitOnNonThreat * (decimal)entity.HitOnNonThreat + entity.FlagrantPenalties * (decimal)entity.SecondProceduralPointDown + entity.Ftdr * (decimal)entity.ThirdProceduralPointDown;
//}
