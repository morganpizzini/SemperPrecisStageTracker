using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SemperPrecisStageTracker.Shared.Permissions
{
    public static class MyEnumExtensions
    {
        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static string ToDescriptionString<T>(this T val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
                .GetType()
                .GetField(val.ToString() ?? string.Empty)
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}