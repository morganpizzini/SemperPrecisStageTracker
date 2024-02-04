using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SemperPrecisStageTracker.Domain.Clients;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Http;
using ZenProgramming.Chakra.Core.Mocks.Scenarios.Extensions;

namespace SemperPrecisStageTracker.Mocks.Clients
{
    /// <summary>
    /// Mock implementation for IdentityClient
    /// </summary>
    public class MockIdentityClient : IIdentityClient
    {
        /// <summary>
        /// Shared password for fake sign-in
        /// </summary>
        public readonly string SharedPassword = String.Empty;
        
        public MockIdentityClient(IConfiguration configuration)
        {
            SharedPassword = configuration["backDoorPassword"];
        }
        /// <summary>
        /// Executes sign-in on identity service
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="password">Password</param>
        /// <returns>Returns user contract if sign-in in completed</returns>
        public Task<HttpResponseMessage<User>> SignIn(string username, string password)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            //INFORMAZIONE! Utilizzo lo scenario per prelevare i dati dell'utente
            //ed eseguire il sign-in. In questo modo lo scenario comprende
            //anche tutte le eventuali situazioni (e dati) "esterni" all'applicazione
            //e si possono eseguire variazioni di situazione in maniera centalizzata

            //Estrazione dell'utente con username
            var user = GetScenario().Users
                .SingleOrDefault(u => u.Username.ToLower() == username.ToLower() || u.Email.ToLower() == username.ToLower());

            //Se non è stato trovato, ritorno unauthorized
            if (user == null)
                return Task.FromResult(HttpResponseMessage<User>.Unauthorized());

            //Se è stato trovato, verifico la password (confronto con quella condivisa)
            if (password != SharedPassword && password != user.Password)
                return Task.FromResult(HttpResponseMessage<User>.Unauthorized());

            //In tutti gli altri casi, confermo
            return Task.FromResult(new HttpResponseMessage<User>(
                new HttpResponseMessage(HttpStatusCode.OK), user));
        }

        /// <summary>
        /// Executes sign-on on identity service
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Returns user contract if sign-on in completed</returns>
        public Task<HttpResponseMessage<User>> ValidateSignUp(User user)
        {
            //Validazione argomenti
            if (user == null) throw new ArgumentNullException(nameof(user));

            //Verifico univocità dello username
            var result = GetScenario().Users
              .SingleOrDefault(u => u.Username.ToLower() == user.Username.ToLower());

            //Se è stato trovato, ritorno errore
            if (result != null)
                return Task.FromResult(HttpResponseMessage<User>.BadRequest($"Sign up not valid for username '{user.Username}'"));

            //Verifico univocità dello username
            result = GetScenario().Users
              .SingleOrDefault(u => u.Email.ToLower() == user.Email.ToLower());

            //Se è stato trovato, ritorno errore
            if (result != null)
                return Task.FromResult(HttpResponseMessage<User>.BadRequest($"Sign up not valid for email '{user.Email}'"));

            //In tutti gli altri casi, confermo
            return Task.FromResult(new HttpResponseMessage<User>(
              new HttpResponseMessage(HttpStatusCode.OK), user));
        }
        /// <summary>
        /// Converts default scenario in SemperPrecisStageTracker scenario
        /// </summary>
        /// <returns></returns>
        private ISemperPrecisStageTrackerScenario GetScenario()
        {
            var ds = SessionFactory.OpenSession();
            return ds.GetScenario<ISemperPrecisStageTrackerScenario>();

        }
        /// <summary>
        /// Count valid users
        /// </summary>
        /// <returns>Returns task with value</returns>
        public Task<HttpResponseMessage<IntResponse>> CountUsers()
        {
            //Creazione della response
            var response = new IntResponse
            {
                Value = GetScenario().Users.Count
            };

            //In tutti gli altri casi, confermo
            return Task.FromResult(new HttpResponseMessage<IntResponse>(
                new HttpResponseMessage(HttpStatusCode.OK), response));
        }

        /// <summary>
        /// Get user contract using username
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Returns task with value</returns>
        public Task<HttpResponseMessage<User>> GetUserByUserName(string userName)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));

            //Recupero l'utente dato il nome
            var response = GetScenario().Users
                .SingleOrDefault(e => e.Username.ToLower() == userName.ToLower());

            //Se non è trovato, not found
            if (response == null)
                return Task.FromResult(HttpResponseMessage<User>.NotFound());

            //In tutti i casi, confermo
            return Task.FromResult(new HttpResponseMessage<User>(
                new HttpResponseMessage(HttpStatusCode.OK), response));
        }

        /// <summary>
        /// Encrypt password
        /// </summary>
        /// <param name="password">password</param>
        /// <returns></returns>
        public string EncryptPassword(string password)
        {
            // no encryption
            return password;
        }
    }
}
