using System;

namespace SemperPrecisStageTracker.Contracts;

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