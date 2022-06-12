using System;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts;

public class ShooterAssociationInfoContract
{
    public string ShooterAssociationInfoId { get; set; }
    public AssociationContract Association { get; set; }
    public ShooterContract Shooter { get; set; }
    public IList<string> Categories { get; set; } = new List<string>();
    public string CardNumber { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool SafetyOfficier { get; set; }
}