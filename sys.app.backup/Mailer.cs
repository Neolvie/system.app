using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace sys.app.backup
{
  public class Mailer
  {
    private const string MailFrom = "";
    private const string MailTo = "";

    private const string Host = "smtp.mail.ru";

    private const string Login = "";
    private const string Password = "";

    private const string Subject = "Backup mail.";

    public bool Send(IEnumerable<string> files)
    {
      try
      {
        var mail = new MailMessage(MailFrom, MailTo);
        var client = new SmtpClient(Host, 465);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        mail.Subject = $"{Subject} {DateTime.Now} {Environment.MachineName}";
        mail.Body = $"{Subject} {DateTime.Now}";
        client.Credentials = new NetworkCredential(Login, Password);

        client.Send(mail);

        return true;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return false;
      }
    }
  }
}