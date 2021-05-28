using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SemperPrecisStageTracker.API.Helpers
{
    public static class PlatformUtils
    {
        #region private properties

        private static readonly string idClaims = "Id";

        #endregion
        /// <summary>
        /// Recovers user id from identity
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <returns>Return user identifier or null</returns>
        public static string GetIdentityUserId(IPrincipal principal)
        {
            //Validazione argomenti
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            //Nome dell'identity
            var identity = (ClaimsIdentity)principal.Identity;
            var userId = identity.Claims.FirstOrDefault(x => x.Type == idClaims)?.Value ?? "";
            if (string.IsNullOrEmpty(userId))
                return null;

            return userId;
        }

        /// <summary>
        /// Recovers username from identity
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <returns>Return username or null</returns>
        public static string GetIdentityUsername(IPrincipal principal)
        {
            //Validazione argomenti
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            //Nome dell'identity
            var identity = (ClaimsIdentity)principal.Identity;
            var userId = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? "";
            if (string.IsNullOrEmpty(userId))
                return null;

            return userId;
        }
    }
}
