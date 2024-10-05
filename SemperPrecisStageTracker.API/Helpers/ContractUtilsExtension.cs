namespace SemperPrecisStageTracker.API.Helpers
{

    /// <summary>
    /// Utilities of contracts
    /// </summary>
    public static partial class ContractUtils
    {
        /// <summary>
        /// Insert object into string
        /// </summary>
        public static List<T> AsList<T>(this T obj)
        {
            return new List<T> { obj };
        }
    }

}
