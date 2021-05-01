using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZenProgramming.Chakra.Core.Data;

namespace SemperPrecisStageTracker.API.Middlewares
{
    /// <summary>
    /// Handler for Basi Authentication
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="logger">Logger</param>
        /// <param name="encoder">Encoder</param>
        /// <param name="clock">Clock</param>
        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        /// <summary>
        /// Handle process for current authentication
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //Se non ho headers o non ho "Authentication", esco
            if (string.IsNullOrEmpty(Request.Headers?["Authorization"]))
            {
                //Fallisco l'autenticazione
                //return AuthenticateResult.Fail("Header 'Authorization' was not provided");
                return AuthenticateResult.NoResult();
            }

            //Recupero il valore e split
            string authValue = Request.Headers["Authorization"];
            var segments = authValue.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            //Se non ho due elementi, esco
            if (segments.Length != 2)
            {
                //Fallisco l'autenticazione
                //return AuthenticateResult.Fail("Header 'Authorization' should contains two items: schema and value");
                return AuthenticateResult.NoResult();
            }

            //Se il lo schema non è Basic, esco
            if (segments[0] != "Basic" || string.IsNullOrEmpty(segments[1]))
            {
                //Fallisco l'autenticazione
                //return AuthenticateResult.Fail($"Provided schema is not '{Scheme.Name}'");
                return AuthenticateResult.NoResult();
            }

            string credentials;
            try
            {
                //Il valore dell'intestazione va decodificato dalla sua forma Base64
                //Per i dettagli, vedere: http://www.w3.org/Protocols/HTTP/1.0/spec.html#BasicAA
                credentials = Encoding.UTF8.GetString(Convert.FromBase64String(segments[1]));
            }
            catch
            {
                //Probabilmente la stringa base64 non era valida
                credentials = string.Empty;
            }

            //Username e password sono separati dal carattere delimitatore ":"
            //Terminiamo l'esecuzione se non è presente o se è in posizione non valida
            var indexOfSeparator = credentials.IndexOf(":", StringComparison.Ordinal);
            if (indexOfSeparator < 1 || indexOfSeparator > credentials.Length - 2)
            {
                //Fallisco l'autenticazione
                return AuthenticateResult.Fail("Base64 encoded values should be separated by char ':'");
            }

            //Estraiamo finalmente le credenziali
            var username = credentials.Substring(0, indexOfSeparator);
            var password = credentials.Substring(indexOfSeparator + 1);

            //Se username o password sono vuoti, esco
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                //Fallisco l'autenticazione
                return AuthenticateResult.Fail("Username and/or password should not be empty or null");
            }

            //Transazione isolata per il database con il solo scopo di identificare
            //l'accesso di emergenza, subito chiusa al termine dell'operazione
            using (IDataSession isolatedSession = SessionFactory.OpenSession())
            {
                //Service layer base
                //using (var serviceLayer = new AuthenticationServiceLayer(isolatedSession))
                //{
                //    //Tento di eseguire il sign-in dell'utente
                //    var signedInUser = await serviceLayer.SignIn(username, password);

                //    //Se non ho l'utente, esco
                //    if (signedInUser == null)
                //    {
                //        //Fallisco
                //        return AuthenticateResult.Fail("Provided credentials are invalid");
                //    }

                //    //Eseguo la generazione del principal
                //    var principal = ClaimsPrincipalUtils.GeneratesClaimsPrincipal(
                //        BasicAuthenticationOptions.Scheme, signedInUser);
                //    //Creo il ticket di autenticazione
                //    var authTicket = new AuthenticationTicket(principal, new AuthenticationProperties(), Scheme.Name);

                //    //Imposto il principal sul thread corrente
                //    Thread.CurrentPrincipal = principal;
                //    //Confermo l'autenticazione
                //    return AuthenticateResult.Success(authTicket);
                //}
                return null;
            }
        }
    }
}

