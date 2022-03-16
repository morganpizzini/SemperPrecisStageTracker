using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterTeamPaymentRequest{
        [Required]
        public string ShooterTeamPaymentId { get; set; }
    }
    public class ShooterTeamPaymentCreateRequest
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public float Amount { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public DateTime PaymentDateTime { get; set; }
    }
    public class ShooterTeamPaymentUpdateRequest
    {
        [Required]
        public string ShooterTeamPaymentId { get; set; }
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public float Amount { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public DateTime PaymentDateTime { get; set; }
    }
    public class TeamHolderCreateRequest
    {
        [Required]

        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
    public class ShooterTeamCreateRequest
    {
        [Required]

        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}