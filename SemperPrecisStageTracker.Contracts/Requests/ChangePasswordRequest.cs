namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Request for change password
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// old password
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// new password
        /// </summary>
        public string Password { get; set; }
    }
}