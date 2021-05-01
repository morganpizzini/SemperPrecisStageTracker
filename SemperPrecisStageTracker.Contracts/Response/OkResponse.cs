using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Shooter request
    /// </summary>
    public class OkResponse
    {
        public bool Status { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
       
    }
}
