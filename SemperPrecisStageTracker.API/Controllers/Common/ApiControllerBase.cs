using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SemperPrecisStageTracker.Domain.Services;
using ZenProgramming.Chakra.Core.Data;

namespace SemperPrecisStageTracker.API.Controllers.Common
{
    /// <summary>
    /// Base controller for MVC pattern
    /// </summary>
    //[Authorize]
    [Route("api/[Controller]")]
    //[TraceOnErrorFile]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase, IDisposable
    {
        #region Protected properties
        /// <summary>
        /// Session holder
        /// </summary>
        public IDataSession DataSession { get; }

        ///// <summary>
        ///// Basic service layer
        ///// </summary>
        //protected AuthenticationServiceLayer AuthorizationLayer { get; }

        /// <summary>
        /// Basic service layer
        /// </summary>
        protected MainServiceLayer BasicLayer { get; }
        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        protected ApiControllerBase()
        {
            //Inizializzo la session e il dominio
            DataSession = SessionFactory.OpenSession();
            //AuthorizationLayer = new AuthenticationServiceLayer(DataSession);
            BasicLayer = new MainServiceLayer(DataSession);
        }

        /// <summary>
        /// Compose a BarRequest (400) with provided list of validations
        /// </summary>
        /// <param name="validations">Validations</param>
        /// <returns>Returns bad request response</returns>
        protected IActionResult BadRequest(IList<ValidationResult> validations)
        {
            //Validazione argomenti
            if (validations == null) throw new ArgumentNullException(nameof(validations));

            //Scorro tutti gli errori, inserisco nel modello ed esco
            foreach (var current in validations)
                ModelState.AddModelError("", current.ErrorMessage);

            //Ritorno la request
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="isDisposing">Explicit dispose</param>
        protected virtual void Dispose(bool isDisposing)
        {
            //Se sto facendo la dispose
            if (isDisposing)
            {
                //Rilascio i layers e la sessione
                BasicLayer?.Dispose();
                DataSession?.Dispose();
                //AuthorizationLayer?.Dispose();
            }

            //Chiamo il metodo base
            //base.Dispose(isDisposing);
        }
        /// <summary>
        /// Default dispose implementation
        /// </summary>
        public void Dispose()
        {
            //Eseguo una dispose esplicita
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
