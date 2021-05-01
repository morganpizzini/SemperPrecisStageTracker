namespace SemperPrecisStageTracker.Domain.Configurations
{
    /// <summary>
    /// Configuration of single connection string
    /// </summary>
    public class ConnectionStringConfiguration
    {
        /// <summary>
        /// Name of connection string
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Provider name (ex. System.Data.SqlClient)
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
