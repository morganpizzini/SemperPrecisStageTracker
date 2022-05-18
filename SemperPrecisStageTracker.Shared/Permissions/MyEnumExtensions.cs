using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SemperPrecisStageTracker.Shared.Permissions
{
    public static class MyEnumExtensions
    {
        public static IList<T> AndList<T>(this T value, params T[] newValues) =>
            new List<T>
            {
                value
            }.AndList(newValues);
        

        public static IList<T> AndList<T>(this IList<T> value, params T[] newValues)
        {
            foreach (var newValue in newValues)
            {
                value.Add(newValue);
            }
            return value;
        }

        //public static IList<T> AndList<T>(this IList<T> value, T newValue)
        //{
        //    value.Add(newValue);
        //    return value;
        //}


        public static string And(this Permissions value,params Permissions[] newValues)
        {
            return $"{value.ToDescriptionString()},{string.Join(",",newValues.Select(ToDescriptionString))}";
        }

        //public static string And(this string value,params string[] newValue)
        //{
        //    return $"{value},{string.Join(",",newValue)}";
        //}


        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static string ToDescriptionString<T>(this T val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
                .GetType()
                .GetField(val.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}