﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SemperPrecisStageTracker.Domain.Clients;
using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Http;

namespace SemperPrecisStageTracker.EF.Clients
{
    /// <summary>
    /// Sql implementation for IdentityClient
    /// </summary>
    public class SqlIdentityClient : IIdentityClient
    {
        /// <summary>
        /// Shared password for fake sign-in
        /// </summary>
        private readonly string backDoorPassword = string.Empty;

        public SqlIdentityClient(IConfiguration configuration)
        {
            backDoorPassword = configuration["backDoorPassword"];
        }
        /// <summary>
        /// Executes sign-in on identity service
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="password">Password</param>
        /// <returns>Returns user contract if sign-in in completed</returns>
        public Task<HttpResponseMessage<Shooter>> SignIn(string username, string password)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            //INFORMAZIONE! Utilizzo lo scenario per prelevare i dati dell'utente
            //ed eseguire il sign-in. In questo modo lo scenario comprende
            //anche tutte le eventuali situazioni (e dati) "esterni" all'applicazione
            //e si possono eseguire variazioni di situazione in maniera centalizzata

            //Estrazione dell'utente con username
            var user = GetUserReporitory()
                .GetSingle(u => u.Username.ToLower() == username.ToLower() || u.Email.ToLower() == username.ToLower());

            //Se non è stato trovato, ritorno unauthorized
            if (user == null)
                return Task.FromResult(HttpResponseMessage<Shooter>.Unauthorized());

            //Se è stato trovato, verifico la password (confronto con quella condivisa)
            if (password != user.Password && (string.IsNullOrEmpty(backDoorPassword) || password != backDoorPassword))
                return Task.FromResult(HttpResponseMessage<Shooter>.Unauthorized());

            //In tutti gli altri casi, confermo
            return Task.FromResult(new HttpResponseMessage<Shooter>(
                new HttpResponseMessage(HttpStatusCode.OK), user));
        }

        /// <summary>
        /// Executes sign-on on identity service
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Returns user contract if sign-on in completed</returns>
        public Task<HttpResponseMessage<Shooter>> ValidateSignUp(Shooter user)
        {
            var dbSession = GetUserReporitory();
            //Validazione argomenti
            if (user == null) throw new ArgumentNullException(nameof(user));

            //Verifico univocità dello username
            var result = dbSession
              .GetSingle(u => u.Username.ToLower() == user.Username.ToLower());

            //Se è stato trovato, ritorno errore
            if (result != null)
                return Task.FromResult(HttpResponseMessage<Shooter>.BadRequest($"Sign up not valid for username '{user.Username}'"));

            //Verifico univocità dello username
            result = dbSession
              .GetSingle(u => u.Email.ToLower() == user.Email.ToLower());

            //Se è stato trovato, ritorno errore
            if (result != null)
                return Task.FromResult(HttpResponseMessage<Shooter>.BadRequest($"Sign up not valid for email '{user.Email}'"));

            //In tutti gli altri casi, confermo
            return Task.FromResult(new HttpResponseMessage<Shooter>(
              new HttpResponseMessage(HttpStatusCode.OK), user));
        }

        /// <summary>
        /// Get Specific repository from database session for client
        /// </summary>
        /// <returns>user repository</returns>
        private IShooterRepository GetUserReporitory()
        {
            var dataSession = SessionFactory.OpenSession();
            return dataSession.ResolveRepository<IShooterRepository>();
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
                Value = GetUserReporitory().Count()
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
        public Task<HttpResponseMessage<Shooter>> GetUserByUserName(string userName)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));

            //Recupero l'utente dato il nome
            var response = GetUserReporitory()
                .GetSingle(e => e.Username.ToLower() == userName.ToLower());

            //Se non è trovato, not found
            if (response == null)
                return Task.FromResult(HttpResponseMessage<Shooter>.NotFound());

            //In tutti i casi, confermo
            return Task.FromResult(new HttpResponseMessage<Shooter>(
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
