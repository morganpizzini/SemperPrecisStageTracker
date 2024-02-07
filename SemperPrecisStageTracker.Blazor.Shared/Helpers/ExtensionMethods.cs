using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace SemperPrecisStageTracker.Blazor.Helpers
{
    public static class ExtensionMethods
    {

        public static IEnumerable<DateTime> SplitDateRange(this DateTime start, DateTime end, int dayChunkSize = 1)
        {
            DateTime chunkEnd = start;
            do
            {
                yield return start.Date;
                start = chunkEnd;
            }while ((chunkEnd = start.AddDays(dayChunkSize)) < end);
            yield return chunkEnd.Date;
        }

        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        public static string? QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }

        public static string EncodeBase64(this string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public static string DecodeBase64(this string value)
        {
            var valueBytes = System.Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }
    }
}