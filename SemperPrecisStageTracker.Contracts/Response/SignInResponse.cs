namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Response with Signin info
    /// </summary>
    public class SignInResponse
    {
        public ShooterContract Shooter { get; set; }
        public PermissionsResponse Permissions { get; set; }
    }
}