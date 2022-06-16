using MimeKit;

namespace SemperPrecisStageTracker.API.Models;

public class EmailConfiguration
{ 
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class Message
{
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public bool Html { get; set; }
    public IFormFileCollection Attachments { get; set; }
    
    public Message(IEnumerable<string> to, string subject, string content,IFormFileCollection attachments = null,bool html = false)
    {
        To = new List<MailboxAddress>();
        To.AddRange(to.Select(x => new MailboxAddress(x,x)));
        Subject = subject;
        Content = content;        
        if(attachments!= null)
            Attachments = attachments;
        Html = html;
    }
}