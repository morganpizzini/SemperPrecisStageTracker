using SemperPrecisStageTracker.Models;
using System;
using System.Security.Claims;

namespace SemperPrecisStageTracker.Domain.Identities
{
    /// <summary>
    /// Utilities for Claims Principal
    /// </summary>
    public static class ClaimsPrincipalUtils
    {
        /// <summary>
        /// Generates claims principal using provided user
        /// </summary>
        /// <param name="authenticationType">Authentication type (ex. "Basic")</param>
        /// <param name="user">User instance</param>
        /// <returns>Returns claims identity</returns>
        public static ClaimsPrincipal GeneratesClaimsPrincipal(string authenticationType, User user)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(authenticationType)) throw new ArgumentNullException(nameof(authenticationType));
            if (user == null) throw new ArgumentNullException(nameof(user));

            //Creo un'identity generica per le informazioni base e la inserisco in una claims
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(authenticationType);
            claimsIdentity.AddClaim(new Claim("Id", user.Id));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));

            //Genero il principal e lo emetto
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}