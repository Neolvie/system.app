using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace sys.app.backup
{
  public class Mailer
  {
    private const string MailFrom = "dsad@asdsad.rr";
    private const string MailTo = "dsad@asdsad.rr";

    private const string Host = "smtp.mail.ru";

    private const string Login = "dsad@asdsad.rr";
    private const string Password = "dsad@asdsad.rr";

    private const string Subject = "Backup mail.";

    public bool Send(IEnumerable<string> files)
    {
      try
      {
        var mail = new MailMessage(MailFrom, MailTo);
        var client = new SmtpClient(Host, 465);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        client.EnableSsl = true;
        mail.Subject = $"{Subject} {DateTime.Now} {Environment.MachineName}";
        mail.Body = $"{Subject} {DateTime.Now}";
        client.Credentials = new NetworkCredential(Login, Password);

        foreach (var file in files)
        {
          var attachment = new Attachment(file);
          mail.Attachments.Add(attachment);
        }

        client.Send(mail);

        return true;
      }
      catch (Exception e)
      {
        File.AppendAllText($"log_{DateTime.Now.Date:yy-MM-dd}.txt", $"Error sending backup: {e}{Environment.NewLine}");
        return false;
      }
    }
  }
}