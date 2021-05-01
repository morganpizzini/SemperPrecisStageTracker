using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.API.Helpers
{
    /// <summary>
    /// Extensions class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Sum timespan into single timespan
        /// </summary>
        /// <param name="collection">timespan collection</param>
        /// <returns>Returns timespan</returns>
        public static TimeSpan TotalTime(this IEnumerable<TimeSpan> collection)
        {
            int i = 0;
            int TotalSeconds = 0;

            var ArrayDuration = collection.ToArray();

            for (i = 0; i < ArrayDuration.Length; i++)
            {
                TotalSeconds = (int)(ArrayDuration[i].TotalSeconds) + TotalSeconds;
            }

            return TimeSpan.FromSeconds(TotalSeconds);
        }

        /// <summary>
        /// Get next day of the week
        /// </summary>
        /// <param name="from">base date</param>
        /// <param name="dayOfWeek">day of the week, default sunday</param>
        /// <returns>Datetime</returns>
        public static DateTime NextDay(this DateTime from, DayOfWeek dayOfWeek = DayOfWeek.Sunday )
        {
            var start = (int)from.DayOfWeek;
            var target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }
        
        /// <summary>
        /// Get first day of next week
        /// </summary>
        /// <param name="from">base date</param>
        /// <returns>Datetime</returns>
        public static DateTime NextMonth(this DateTime from)
        {
            return new DateTime(from.AddMonths(1).Year, from.AddMonths(1).Month, 1);
        }
    }
}
