using System.Collections;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Response with Signin info
    /// </summary>
    public class SignInResponse
    {
        public ShooterContract Shooter { get; set; }
        public UserPermissionContract Permissions { get; set; }
    }
}