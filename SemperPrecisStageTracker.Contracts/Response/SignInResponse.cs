namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Response with Signin info
    /// </summary>
    public class SignInResponse
    {
        public UserContract User { get; set; }
        public UserPermissionContract Permissions { get; set; }
    }
}