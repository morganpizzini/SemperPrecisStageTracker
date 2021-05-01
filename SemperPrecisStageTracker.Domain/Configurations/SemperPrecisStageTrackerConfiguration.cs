using System.Collections.Generic;
using ZenProgramming.Chakra.Core.Configurations;

namespace SemperPrecisStageTracker.Domain.Configurations
{
    /// <summary>
    /// Application configuration
    /// </summary>
    public class SemperPrecisStageTrackerConfiguration : IApplicationConfigurationRoot
    {
        /// <summary>
        /// Name of current environment
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// Configuration for storage
        /// </summary>
        public StorageConfiguration Storage { get; set; }

        /// <summary>
        /// List of connection strings
        /// </summary>
        public IList<ConnectionStringConfiguration> ConnectionStrings { get; set; }

    }
}

