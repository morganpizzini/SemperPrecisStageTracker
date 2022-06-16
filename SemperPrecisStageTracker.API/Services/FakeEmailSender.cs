using SemperPrecisStageTracker.API.Models;
using SemperPrecisStageTracker.API.Services.Interfaces;

namespace SemperPrecisStageTracker.API.Services;

public class FakeEmailSender : IEmailSender
{
    public void SendEmail(Message message)
    {
        //C:\Users\<user>\AppData\Local\Temp
        using var writer = File.CreateText(Path.GetTempFileName());

        writer.WriteLine(message.To.Select(x=>x.Address)); //or .Write(), if you wish
        writer.WriteLine(message.Content); //or .Write(), if you wish

    }
    public Task SendEmailAsync(Message message) {
        this.SendEmail(message);
        return Task.CompletedTask; }
}
