using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class SignInRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public DateTime BirthDate { get; set; } = DateTime.MinValue;
    [Required]
    public string Gender { get; set; } = string.Empty;
    [Required]
    public string Token { get; set; }
}

public class ResetPasswordRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required]
    public string RestorePasswordAlias { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
        [Required]
    public string Token { get; set; }
}

public class ForgotPasswordRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Token { get; set; }
}