using SemperPrecisStageTracker.Shared.Permissions;
using System.Security.Principal;

namespace SemperPrecisStageTracker.Blazor.BackOffice.Models;

public class Gender
{
    public string Code { get; set; }
    public string Description { get; set; }

    public static IEnumerable<Gender> Genders = new List<Gender>()
    {
        new()
        {
            Code = null,
            Description = string.Empty
        },
        new()
        {
            Code = "M",
            Description = "Male"
        },
        new()
        {
            Code = "F",
            Description = "Female"
        },
        new()
        {
            Code = "D",
            Description = "Diverse"
        }
    };
}
public class PermissionsEnumViewModel
{
    public PermissionsEnumViewModel()
    {
        
    }
    public PermissionsEnumViewModel(int intValue, string stringValue)
    {
        IntValue = intValue;
        StringValue = stringValue;
    }
    public int IntValue { get; set; }
    public string StringValue { get; set; } = string.Empty;

    public static IEnumerable<PermissionsEnumViewModel> List =
        Enum.GetValues(typeof(Permissions)).Cast<Permissions>()
        .Select(x=>new PermissionsEnumViewModel((int)x,x.ToDescriptionString())).ToList();
}