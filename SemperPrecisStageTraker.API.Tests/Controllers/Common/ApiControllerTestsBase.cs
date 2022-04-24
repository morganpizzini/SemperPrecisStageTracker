using System;
using System.Collections.Generic;
using System.Linq;
using SemperPrecisStageTracker.Domain.Identities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemperPrecisStageTracker.API.Controllers.Common;
using SemperPrecisStageTracker.Domain.Cache;
using SemperPrecisStageTracker.Domain.Clients;
using SemperPrecisStageTracker.Domain.Containers;
using SemperPrecisStageTracker.Domain.Services;
using SemperPrecisStageTracker.Mocks.Clients;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Mocks.Data;
using ZenProgramming.Chakra.Core.Mocks.Scenarios.Extensions;
using ZenProgramming.Chakra.Core.Mocks.Scenarios.Options;

namespace SemperPrecisStageTraker.API.Tests.Controllers.Common
{
    /// <summary>
    /// Base asbtract class for test API controller
    /// </summary>
    /// <typeparam name="TApiController">Type of API controller</typeparam>
    /// <typeparam name="TScenario">Type of scenario to use</typeparam>
    public abstract class ApiControllerTestsBase<TApiController, TScenario>
        where TApiController : ApiControllerBase, new()
        where TScenario : class, ISemperPrecisStageTrackerScenario, new()
    {
        #region Protected properties
        /// <summary>
        /// Scenario used on tests
        /// </summary>
        protected TScenario Scenario { get; set; }

        /// <summary>
        /// Controller instance
        /// </summary>
        protected TApiController Controller { get; set; }

        /// <summary>
        /// Used identity Shooter
        /// </summary>
        protected Shooter CurrentIdentityUser { get; set; }

        /// <summary>
        /// Random seed
        /// </summary>
        protected Random RandomSeed { get; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        protected ApiControllerTestsBase()
        {
            //Inizializzo il randomizzatore
            RandomSeed = new Random();
        }

        #region Helper Methods

        /// <summary>
        /// Get admin that will be used for ASP.NET Identity
        /// </summary>
        /// <returns>Returns Shooter instance</returns>
        protected Shooter GetAdminUser()
        {
            //Recupero l'utente amministratore
            return Scenario.Shooters
                .SingleOrDefault(u => u.Username == SimpleScenario.AdminUser);
        }
        /// <summary>
        /// Get admin that will be used for ASP.NET Identity
        /// </summary>
        /// <returns>Returns Shooter instance</returns>
        protected Shooter GetAnotherUser(string userId = null)
        {
            //Recupero l'utente amministratore
            return
                string.IsNullOrEmpty(userId) ?
                Scenario.Shooters
                .SingleOrDefault(u => u.Username == SimpleScenario.AnotherUser)
                :
                Scenario.Shooters
                .SingleOrDefault(u => u.Id == userId);
        }

        /// <summary>
        /// Get admin that will be used for ASP.NET Identity
        /// </summary>
        /// <returns>Returns Shooter instance</returns>
        protected Shooter GetUserNotIn(IList<string> userIds)
        {
            //Recupero l'utente amministratore
            return
                Scenario.Shooters
                .FirstOrDefault(u => !userIds.Contains(u.Id));
        }

        protected Shooter GetUserWithoutPermission(IList<AdministrationPermissions> adminPermissions)
            => GetUserWithPermission(adminPermissions,false);
        
        /// <summary>
        /// Get admin that will be used for ASP.NET Identity
        /// </summary>
        /// <returns>Returns Shooter instance</returns>
        protected Shooter GetUserWithPermission(IList<AdministrationPermissions> adminPermissions, bool inOrOut = true)
        {
            var userWIthPermissionsIds = new List<string>();
            if (adminPermissions != null)
                userWIthPermissionsIds.AddRange(Scenario.AdministrationPermissions
                .Select(x => new
                {
                    x.ShooterId,
                    Permission = x.Permission.ParseEnum<AdministrationPermissions>()
                })
                .Where(x => adminPermissions.Contains(x.Permission))
                .Select(x => x.ShooterId)
                .ToList());

            if (inOrOut)
            {
                var selectedUser = userWIthPermissionsIds.FirstOrDefault();
                if (selectedUser == null)
                {
                    Assert.Inconclusive("no user with privileges found");
                    return null;
                }
                return GetAnotherUser(selectedUser);
            }
            else
            {
                return GetUserNotIn(userWIthPermissionsIds);
            }
        }

        #endregion

        /// <summary>
        /// Get Shooter that will be used for ASP.NET Identity
        /// </summary>
        /// <returns>Returns Shooter instance</returns>
        protected abstract Shooter GetIdentityUser();

        /// <summary>
        /// On initialize
        /// </summary>
        [TestInitialize]
        public virtual void OnInitialize()
        {
            //Registrazione della sessione di default
            SessionFactory.RegisterDefaultDataSession<MockDataSession<TScenario, TransientScenarioOption<TScenario>>>();

            //Registrazione dei servizi
            ServiceResolver.Register<IIdentityClient, MockIdentityClient>();
            ServiceResolver.Register<ICaptchaValidatorService, MockCaptchaValidatorService>();

            ServiceResolver.Register<ISemperPrecisMemoryCache, SemperPrecisMemoryCache>();

            //Creazione del controller dichiarato
            Controller = new TApiController();

            Scenario = Controller.DataSession.GetScenario<TScenario>();

            //Recupero l'utente da usare nel testa
            var defaultShooterIdentity = GetIdentityUser();
            if (defaultShooterIdentity == null)
                throw new InvalidProgramException("Shooter for identity is invalid");

            //Inizializzazione del controller context e impostazione dell'identity
            UpdateIdentityUser(defaultShooterIdentity);
        }

        /// <summary>
        /// Updates identity set on controller context
        /// </summary>
        /// <param name="Shooter">Shooter instance</param>
        protected void UpdateIdentityUser(Shooter Shooter)
        {
            //Validazione argomenti
            if (Shooter == null) throw new ArgumentNullException(nameof(Shooter));

            //impostazione local dell'identità
            CurrentIdentityUser = Shooter;

            //Generazione del principal
            var identity = ClaimsPrincipalUtils.GeneratesClaimsPrincipal("Mock", CurrentIdentityUser);

            Controller.ControllerContext = new ControllerContext
            {
                //HTTP context default
                HttpContext = new DefaultHttpContext
                {
                    //Imposto l'identity generata
                    User = identity
                }
            };
        }

        /// <summary>
        /// Parses provided response as "Ok - 200" and returns contained data
        /// </summary>
        /// <typeparam name="TData">Type of data</typeparam>
        /// <param name="response">Response to parse</param>
        /// <returns>Returns parsed response</returns>
        protected ActionResultStructure<OkObjectResult, TData> ParseExpectedOk<TData>(IActionResult response)
        {
            //Validazione argomenti
            if (response == null) throw new ArgumentNullException(nameof(response));

            //Mi attendo un 200
            if (!(response is OkObjectResult castedResponse))
                throw new InvalidProgramException($"Response should be of type {typeof(OkObjectResult).Name}");

            //Attendo che il risultato del tipo atteso
            if (!(castedResponse.Value is TData castedResult))
                throw new InvalidProgramException($"Response data should be of type {typeof(TData).FullName}");

            //Se i cast sono avvenuti con successo, ritorno la struttura
            return new ActionResultStructure<OkObjectResult, TData>
            {
                Response = castedResponse,
                Data = castedResult
            };
        }

        /// <summary>
        /// Parses provided response as "NotFound - 404"
        /// </summary>
        /// <param name="response">Response to parse</param>
        /// <returns>Returns parsed response</returns>
        protected ActionResultStructure<NotFoundResult, object> ParseExpectedNotFound(IActionResult response)
        {
            //Validazione argomenti
            if (response == null) throw new ArgumentNullException(nameof(response));

            //Mi attendo un 200
            if (!(response is NotFoundResult castedResponse))
                throw new InvalidProgramException($"Response should be of type {typeof(NotFoundResult).Name}");

            //Creo la struttura di uscita
            return new ActionResultStructure<NotFoundResult, object>
            {
                Response = castedResponse,
                Data = null
            };
        }

        /// <summary>
        /// Parses provided response as "Forbid - 403"
        /// </summary>
        /// <param name="response">Response to parse</param>
        /// <returns>Returns parsed response</returns>
        protected ActionResultStructure<ForbidResult, object> ParseExpectedForbid(IActionResult response)
        {
            //Validazione argomenti
            if (response == null) throw new ArgumentNullException(nameof(response));

            //Mi attendo un 200
            if (!(response is ForbidResult castedResponse))
                throw new InvalidProgramException($"Response should be of type {typeof(ForbidResult).Name}");

            //Creo la struttura di uscita
            return new ActionResultStructure<ForbidResult, object>
            {
                Response = castedResponse,
                Data = null
            };
        }

        /// <summary>
        /// Parses provided response as "Forbid - 403"
        /// </summary>
        /// <param name="response">Response to parse</param>
        /// <returns>Returns parsed response</returns>
        protected ActionResultStructure<UnauthorizedResult, object> ParseExpectedUnauthorized(IActionResult response)
        {
            //Validazione argomenti
            if (response == null) throw new ArgumentNullException(nameof(response));

            //Mi attendo un 200
            if (!(response is UnauthorizedResult castedResponse))
                throw new InvalidProgramException($"Response should be of type {typeof(UnauthorizedResult).Name}");

            //Creo la struttura di uscita
            return new ActionResultStructure<UnauthorizedResult, object>
            {
                Response = castedResponse,
                Data = null
            };
        }

        /// <summary>
        /// Parses provided response as "BadRequest - 400"
        /// </summary>
        /// <param name="response">Response to parse</param>
        /// <returns></returns>
        protected ActionResultStructure<BadRequestObjectResult, SerializableError> ParseExpectedBadRequest(IActionResult response)
        {
            //Validazione argomenti
            if (response == null) throw new ArgumentNullException(nameof(response));

            //Mi attendo un 200
            if (!(response is BadRequestObjectResult castedResponse))
                throw new InvalidProgramException($"Response should be of type {typeof(BadRequestObjectResult).Name}");

            //Attendo che il risultato del tipo atteso
            if (!(castedResponse.Value is SerializableError castedResult))
                throw new InvalidProgramException($"Response data should be of type {typeof(SerializableError).FullName}");

            //Creo la struttura di uscita
            return new ActionResultStructure<BadRequestObjectResult, SerializableError>
            {
                Response = castedResponse,
                Data = castedResult
            };
        }

        /// <summary>
        /// On cleanup
        /// </summary>
        [TestInitialize]
        public virtual void OnCleanup()
        {
            //Cleanup del controller e scenario, layer
            Controller?.Dispose();
            Controller = null;
            Scenario = null;
        }
    }
}
