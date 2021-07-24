using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SemperPrecisStageTracker.Contracts
{

    public class AssociationContract
    {
        public string AssociationId { get; set; }
        public string Name { get; set; }
        public IList<string> Divisions {get;set;} = new List<string>();
        public IList<string> Classifications {get;set;} = new List<string>();
    }
    public enum ShooterRoleEnumContract
    {
        PSO= 1,
        CSO = 2,
        SSO = 4
    }
}