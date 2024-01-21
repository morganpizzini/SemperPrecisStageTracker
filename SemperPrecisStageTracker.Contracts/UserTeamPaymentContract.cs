using System;

namespace SemperPrecisStageTracker.Contracts;

public class UserTeamPaymentContract
{
    public string UserTeamPaymentId { get; set; }
    public TeamContract Team { get; set; }
    public UserContract User { get; set; }
    public float Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string PaymentType { get; set; }
    public DateTime PaymentDateTime { get; set; }
}

public class PaymentTypeContract
{
    public string PaymentTypeId { get; set; }
    
    public string Name { get; set; } = string.Empty;
}

public class TeamReminderContract
{
    public string TeamReminderId { get; set; }
    public TeamContract Team { get; set; }
    public UserContract User { get; set; }
    public string Reason { get; set; }
    public DateTime ExpireDateTime { get; set; }
    public bool NotifyExpiration { get; set; }
}