using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Response with string value
    /// </summary>
    public class MatchGroupResponse
    {
        public MatchContract Match { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public IList<GroupContract> Groups { get; set; }
        
        /// <summary>
        /// List of 
        /// </summary>
        public IList<GroupShooterContract> UnGrouped { get; set; }
    }
    /// <summary>
    /// Response with string value
    /// </summary>
    public class StringResponse
    {
        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }
    }
}