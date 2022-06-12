using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SemperPrecisStageTracker.Contracts
{
    public class AssociationContract
    {
        public string AssociationId { get; set; }
        public string Name { get; set; }
        public IList<string> Divisions { get; set; } = new List<string>();
        public IList<string> Classifications { get; set; } = new List<string>();
        public IList<string> Categories { get; set; } = new List<string>();
        public float HitOnNonThreatPointDown { get; set; }
        public float FirstProceduralPointDown { get; set; }
        public string FirstPenaltyLabel { get; set; } = string.Empty;
        public float SecondProceduralPointDown { get; set; }
        public string SecondPenaltyLabel { get; set; } = string.Empty;
        public float ThirdProceduralPointDown { get; set; }
        public string ThirdPenaltyLabel { get; set; } = string.Empty;
        public IList<string> SoRoles { get; set; } = new List<string>();

    }
}