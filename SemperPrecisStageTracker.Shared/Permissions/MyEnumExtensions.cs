using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SemperPrecisStageTracker.Shared.Permissions
{
    public static class MyEnumExtensions
    {
        public static Permissions CastIntoPermission(this string numberString)
        {
            if(int.TryParse(numberString, out var number))
            {
                return number.CastIntoPermission();
            }
            throw new ArgumentOutOfRangeException(nameof(numberString), $"The value {numberString} is not a valid Permissions enum.");
        }
        public static Permissions CastIntoPermission(this int number)
        {
            // Check if the number is a defined enum and then check if it's in the list
            if (Enum.IsDefined(typeof(Permissions), number))
            {
                return (Permissions)number;
            }
            throw new ArgumentOutOfRangeException(nameof(number), $"The value {number} is not a valid Permissions enum.");
        }
        
        public static bool IsEnumInPermissions(this IList<Permissions> enumList, int number)
        {
            // Check if the number is a defined enum and then check if it's in the list
            if (Enum.IsDefined(typeof(Permissions), number))
            {
                var role = (Permissions)number;
                return enumList.Contains(role);
            }

            return false;
        }
        public static bool IsEnumInList<T>(this List<T> enumList, int number)
        {
            // Check if the number is a defined enum and then check if it's in the list
            if (Enum.IsDefined(typeof(T), number))
            {
                var role = (T)(object)number;
                return enumList.Contains(role);
            }

            return false; // Return false if the number is not a valid enum value
        }
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