using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Association request
    /// </summary>
    public class AssociationRequest : EntityFilterValidation
    {
        public override string EntityId => AssociationId;
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string AssociationId { get; set; }

    }
}
