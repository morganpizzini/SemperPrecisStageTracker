using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterTeamPaymentContract
    {
        public string ShooterTeamPaymentId { get; set; }
        public TeamContract Team { get; set; }
        public ShooterContract Shooter { get; set; }
        public float Amount { get; set; }
        public string Reason { get; set; } = string.Empty;

        public DateTime PaymentDateTime { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public bool NotifyExpiration { get; set; }
    }
    public class MatchDataAssociationContract
    {
        public MatchContract Match { get; set; }
        //public IList<ShooterContract> Shooters { get; set; }
        //public IList<StageContract> Stages { get; set; }
        //public IList<GroupContract> Groups { get; set; }
        public IList<ShooterStageAggregationResult> ShooterStages { get; set; }
        public IList<ShooterMatchContract> ShooterMatches { get; set; }
        public IList<ShooterSOStageContract> ShooterSoStages { get; set; }
    }

    public class AssociationContract
    {
        public string AssociationId { get; set; }
        public string Name { get; set; }
        public IList<string> Divisions { get; set; } = new List<string>();
        public IList<string> Classifications { get; set; } = new List<string>();
        public IList<string> Categories { get; set; } = new List<string>();
        public float HitOnNonThreatDownPoints { get; set; }
        public float FirstProceduralPointDown { get; set; }
        public string FirstPenaltyLabel { get; set; } = string.Empty;
        public float SecondProceduralPointDown { get; set; }
        public string SecondPenaltyLabel { get; set; } = string.Empty;
        public float ThirdProceduralPointDown { get; set; }
        public string ThirdPenaltyLabel { get; set; } = string.Empty;
        public IList<string> SoRoles { get; set; } = new List<string>();

    }

    public class EditedEntityRequest
    {
        public string EntityId { get; set; }
        public DateTime EditDateTime { get; set; } = DateTime.UtcNow;
    }
    public class UpdateDataRequest
    {
        public IList<ShooterStageContract> ShooterStages { get; set; }
        public IList<EditedEntityRequest> EditedEntities { get; set; }
    }
}