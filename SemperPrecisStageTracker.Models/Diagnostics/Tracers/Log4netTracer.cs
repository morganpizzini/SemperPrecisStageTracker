using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using ZenProgramming.Chakra.Core.Diagnostic;

namespace SemperPrecisStageTracker.Models.Diagnostics.Tracers
{
    /// <summary>
    /// Tracer for Log4Net
    /// </summary>
    public class Log4NetTracer : ITracer
    {
        #region Private fields

        private ILog _Log;

        #endregion

        /// <summary>
        /// Execute initialization of logger
        /// </summary>
        private ILog GetLogger()
        {
            //Se la variabile statica è nulla, la inizializzo
            if (_Log == null)
            {
                //Avvio la configurazione di Log4net
                var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                    typeof(log4net.Repository.Hierarchy.Hierarchy));
                string targetConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
                FileInfo configFile = new FileInfo(targetConfigFile);
                XmlConfigurator.Configure(repo, configFile);

                //Eseguo l'istanziamento di un logger
                _Log = LogManager.GetLogger(Assembly.GetEntryAssembly(),
                    typeof(log4net.Repository.Hierarchy.Hierarchy));
            }

            //Ritorno il logger
            return _Log;
        }

        /// <summary>
        /// Format for trace
        /// </summary>
        public string TraceFormat { get; set; }

        /// <summary>
        /// Trace an information message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="formatParams">Format parameters</param>
        public void Info(string message, params object[] formatParams)
        {
            //Utilizzo log4net per la scrittura del log
            GetLogger().InfoFormat(message, formatParams);
        }

        /// <summary>
        /// Trace an error message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="formatParams">Format parameters</param>
        public void Error(string message, params object[] formatParams)
        {
            //Utilizzo log4net per la scrittura del log
            GetLogger().ErrorFormat(message, formatParams);
        }

        /// <summary>
        /// Trace a warning message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="formatParams">Format parameters</param>
        public void Warn(string message, params object[] formatParams)
        {
            //Utilizzo log4net per la scrittura del log
            GetLogger().WarnFormat(message, formatParams);
        }

        /// <summary>
        /// Trace a debug message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="formatParams">Format parameters</param>
        public void Debug(string message, params object[] formatParams)
        {
            //Utilizzo log4net per la scrittura del log
            GetLogger().DebugFormat(message, formatParams);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            //Nessuna risorsa da rilasciare
            //in questa implementazione
        }
    }
}
