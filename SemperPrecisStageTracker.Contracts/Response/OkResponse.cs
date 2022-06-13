using System;
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

    /// <summary>
    /// Shooter request
    /// </summary>
    public class ShooterInformationResponse
    {
        public IList<ShooterMatchInfoResponse> ShooterMatchInfos { get; set; } = new List<ShooterMatchInfoResponse>();
    }

    /// <summary>
    /// Shooter request
    /// </summary>
    public class ShooterMatchInfoResponse
    {
        public string MatchId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AssociationName { get; set; } = string.Empty;
        public string PlaceName { get; set; } = string.Empty;
        public DateTime MatchDateTimeStart { get; set; }
        public DateTime MatchDateTimeEnd { get; set; }
        public string GroupName { get; set; } = string.Empty;

    }
}
