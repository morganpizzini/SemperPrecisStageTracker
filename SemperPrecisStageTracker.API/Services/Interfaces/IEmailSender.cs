using SemperPrecisStageTracker.API.Models;

namespace SemperPrecisStageTracker.API.Services.Interfaces;

public interface IEmailSender
{
    void SendEmail(Message message);
    Task SendEmailAsync(Message message);
}
