using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.API.Tests.Utils;
internal static class ListExtensions
{
    internal static List<T> RepeatedDefault<T>(int count)
    {
        return Repeated(default(T), count);
    }

    internal static List<T> Repeated<T>(T value, int count)
    {
        List<T> ret = new List<T>(count);
        ret.AddRange(Enumerable.Repeat(value, count));
        return ret;
    }
    internal static List<T> Repeated<T>(Func<T> value, int count)
    {
        List<T> ret = new List<T>(count);
        ret.AddRange(Enumerable.Repeat(value.Invoke(), count));
        return ret;
    }
}
