using Microsoft.AspNetCore.Authentication;

namespace SemperPrecisStageTracker.API.Middlewares
{
    /// <summary>
    /// Extensions for Basic Authentication
    /// </summary>
    public static class BasicAuthenticationExtensions
    {
        /// <summary>
        /// Add Basic Authentication to the pipeline
        /// </summary>
        /// <param name="builder">Auth builder</param>
        /// <returns>Returns builder with added schema</returns>
        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder)
        {
            //Aggiungo lo schema di autenticazione con le opzioni
            return builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(
                BasicAuthenticationOptions.Scheme,
                BasicAuthenticationOptions.Scheme,
                o => { });
        }
    }
}
