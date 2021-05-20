using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Association request
    /// </summary>
    public class AssociationRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string AssociationId { get; set; }
       
    }

    public class AssociationCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IList<string> Divisions {get;set;} = new List<string>();
        [Required]
        public IList<string> Classes {get;set;} = new List<string>();
    }

    public class AssociationUpdateRequest
    {
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public IList<string> Divisions {get;set;} = new List<string>();
        [Required]
        public IList<string> Classes {get;set;} = new List<string>();
    }

}
