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