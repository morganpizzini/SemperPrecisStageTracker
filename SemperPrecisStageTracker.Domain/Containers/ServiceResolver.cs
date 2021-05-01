using System;
using Ninject;

namespace SemperPrecisStageTracker.Domain.Containers
{
    /// <summary>
    /// Service resolver
    /// </summary>
    public class ServiceResolver
    {
        //Inizializzazione statica
        private static readonly Lazy<IKernel> Kernel = new Lazy<IKernel>(() => new StandardKernel());

        /// <summary>
        /// Registers provided interface to concrete type
        /// </summary>
        /// <typeparam name="TServiceInterface">Type of service interface</typeparam>
        /// <typeparam name="TServiceInstance">Type of concrete instance</typeparam>
        public static void Register<TServiceInterface, TServiceInstance>()
            where TServiceInstance : class, TServiceInterface
        {
            //Espongo il metodo ed ottengo la sintassi per il bindind
            //di destinazione per l'interfaccia passata
            var bindingToSyntax = Kernel.Value.Rebind<TServiceInterface>();

            //Eseguo il binding della sintassi al target
            var bindingOnSyntax = bindingToSyntax.To<TServiceInstance>();

            //Applico la policy di singleton per la cache
            bindingOnSyntax.InSingletonScope();
        }

        /// <summary>
        /// Resolve specified interface
        /// </summary>
        /// <typeparam name="TServiceInterface">Type of service interface</typeparam>
        /// <returns>Returns resolution</returns>
        public static TServiceInterface Resolve<TServiceInterface>()
        {
            //Ritorno la risoluzione
            return Kernel.Value.Get<TServiceInterface>();
        }
    }
}
