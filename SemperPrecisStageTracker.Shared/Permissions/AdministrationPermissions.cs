using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Shared.Permissions
{

    public static class MyEnumExtensions
    {
        public static IList<string> AndList(this object value, object newValue) => new List<string>
        {
            value.ToString(),
            newValue.ToString()
        };
        public static IList<T> AndList<T>(this T value, T newValue) => new List<T>
            {
                value,
                newValue
            };

        public static IList<T> AndList<T>(this IList<T> value, T newValue)
        {
            value.Add(newValue);
            return value;
        }


        public static string And(this string value, string newValue)
        {
            return $"{value},{newValue}";
        }
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
    // ranged permissions
    public enum AdministrationPermissions
    {
        [Description("ManageMatches")]
        ManageMatches = 1,
        [Description("ManageShooters")]
        ManageShooters = 2,
        [Description("ManageTeams")]
        ManageTeams = 3,
        [Description("ManageStages")]
        ManageStages = 4,
        [Description("CreateMatches")]
        CreateMatches = 5,
        [Description("CreateShooters")]
        CreateShooters = 6,
        [Description("CreateTeams")]
        CreateTeams = 7,
        [Description("CreateStages")]
        CreateStages = 8
    }
    // singularity permissions
}
