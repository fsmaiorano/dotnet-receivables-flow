namespace Application.Common.Utils;

using MailKit.Net.Smtp;
using MimeKit;

public static class SendMail
{
    public static void Send(string to, string subject, string body)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress("Sender Name", "communication@receivablesflow.com"));
        email.To.Add(new MailboxAddress("Receiver Name", to));

        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = body };

        using var smtp = new SmtpClient();
        smtp.Connect("smtp.server.address", 587, false);

        smtp.Authenticate("smtp_username", "smtp_password");

        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
