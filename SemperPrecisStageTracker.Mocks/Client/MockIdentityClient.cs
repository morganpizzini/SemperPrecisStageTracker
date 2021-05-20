// using System;
// using System.Net;
// using System.Net.Http;
// using System.Threading.Tasks;
// using Bonebat.Client.Contracts;
// using Bonebat.Clients;
// using Bonebat.Data.Repositories;
// using ZenProgramming.Chakra.Core.Data;
// using ZenProgramming.Chakra.Core.Http;

// namespace Bonebat.Sql.Clients
// {
//     /// <summary>
//     /// Sql implementation for IdentityClient
//     /// </summary>
//     public class SqlIdentityClient : IIdentityClient
//     {
//         /// <summary>
//         /// Shared password for fake sign-in
//         /// </summary>
//         public const string SharedPassword = "password";

//         /// <summary>
//         /// Executes sign-in on identity service
//         /// </summary>
//         /// <param name="username">User name</param>
//         /// <param name="password">Password</param>
//         /// <returns>Returns user contract if sign-in in completed</returns>
//         public Task<HttpResponseMessage<User>> SignIn(string username, string password)
//         {
//             //Validazione argomenti
//             if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
//             if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

//             //INFORMAZIONE! Utilizzo lo scenario per prelevare i dati dell'utente
//             //ed eseguire il sign-in. In questo modo lo scenario comprende
//             //anche tutte le eventuali situazioni (e dati) "esterni" all'applicazione
//             //e si possono eseguire variazioni di situazione in maniera centalizzata

//             //Estrazione dell'utente con username
//             var user = GetUserReporitory()
//                 .GetSingle(u => u.Username.ToLower() == username.ToLower());

//             //Se non è stato trovato, ritorno unauthorized
//             if (user == null)
//                 return Task.FromResult(HttpResponseMessage<User>.Unauthorized());

//             //Se è stato trovato, verifico la password (confronto con quella condivisa)
//             if (password != SharedPassword && password != user.Password)
//                 return Task.FromResult(HttpResponseMessage<User>.Unauthorized());

//             //In tutti gli altri casi, confermo
//             return Task.FromResult(new HttpResponseMessage<User>(
//                 new HttpResponseMessage(HttpStatusCode.OK), user));
//         }

//         /// <summary>
//         /// Executes sign-on on identity service
//         /// </summary>
//         /// <param name="user">User</param>
//         /// <returns>Returns user contract if sign-on in completed</returns>
//         public Task<HttpResponseMessage<User>> ValidateSignUp(User user)
//         {
//             var dbSession = GetUserReporitory();
//             //Validazione argomenti
//             if (user == null) throw new ArgumentNullException(nameof(user));

//             //Verifico univocità dello username
//             var result = dbSession
//               .GetSingle(u => u.Username.ToLower() == user.Username.ToLower());

//             //Se è stato trovato, ritorno errore
//             if (result != null)
//                 return Task.FromResult(HttpResponseMessage<User>.BadRequest($"Sign up not valid for username '{user.Username}'"));

//             //Verifico univocità dello username
//             result = dbSession
//               .GetSingle(u => u.Email.ToLower() == user.Email.ToLower());

//             //Se è stato trovato, ritorno errore
//             if (result != null)
//                 return Task.FromResult(HttpResponseMessage<User>.BadRequest($"Sign up not valid for email '{user.Email}'"));

//             //In tutti gli altri casi, confermo
//             return Task.FromResult(new HttpResponseMessage<User>(
//               new HttpResponseMessage(HttpStatusCode.OK), user));
//         }

//         /// <summary>
//         /// Get Specific repository from database session for client
//         /// </summary>
//         /// <returns>user repository</returns>
//         private IUserRepository GetUserReporitory()
//         {
//             var dataSession = SessionFactory.OpenSession();
//             var userRepository = dataSession.ResolveRepository<IUserRepository>();


//             //Ritorno lo scenario
//             return userRepository;
//         }
//         /// <summary>
//         /// Count valid users
//         /// </summary>
//         /// <returns>Returns task with value</returns>
//         public Task<HttpResponseMessage<IntResponse>> CountUsers()
//         {
//             //Creazione della response
//             var response = new IntResponse
//             {
//                 Value = GetUserReporitory().Count()
//             };

//             //In tutti gli altri casi, confermo
//             return Task.FromResult(new HttpResponseMessage<IntResponse>(
//                 new HttpResponseMessage(HttpStatusCode.OK), response));
//         }

//         /// <summary>
//         /// Get user contract using username
//         /// </summary>
//         /// <param name="userName">User name</param>
//         /// <returns>Returns task with value</returns>
//         public Task<HttpResponseMessage<User>> GetUserByUserName(string userName)
//         {
//             //Validazione argomenti
//             if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));

//             //Recupero l'utente dato il nome
//             var response = GetUserReporitory()
//                 .GetSingle(e => e.Username.ToLower() == userName.ToLower());

//             //Se non è trovato, not found
//             if (response == null)
//                 return Task.FromResult(HttpResponseMessage<User>.NotFound());

//             //In tutti i casi, confermo
//             return Task.FromResult(new HttpResponseMessage<User>(
//                 new HttpResponseMessage(HttpStatusCode.OK), response));
//         }

//         /// <summary>
//         /// Encrypt password
//         /// </summary>
//         /// <param name="password">password</param>
//         /// <returns></returns>
//         public string EncryptPassword(string password)
//         {
//             // no encryption
//             return password;
//         }
//     }
// }
