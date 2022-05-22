namespace SemperPrecisStageTracker.Blazor.Services.Models;

public class ApiResponse<T>
{
    public T Result { get; set; }
    public bool WentWell => string.IsNullOrEmpty(Error);
    public string Error { get; set; } = string.Empty;
}
