namespace SemperPrecisStageTracker.Blazor
{
    public class ClientConfiguration
    {
        public string BaseAddress { get; set; } = string.Empty;

        public bool IsLocal => this.BaseAddress.Contains("localhost");

        public RecapthaConfiguration Recaptcha { get; set; } = new();
    }
}