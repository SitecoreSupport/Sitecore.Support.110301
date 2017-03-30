using Sitecore.WFFM.Abstractions.Mail;
using System;
using System.Net.Mail;

namespace Sitecore.Support.Form.Core.Pipelines.ProcessMessage
{
    public class ProcessMessage
    {
        private MailMessage GetMail(ProcessMessageArgs args)
        {
            MailMessage mail = new MailMessage(args.From.Replace(";", ","), args.To.Replace(";", ",").ToString(), args.Subject.ToString(), args.Mail.ToString())
            {
                IsBodyHtml = args.IsBodyHtml
            };
            if (args.CC.Length > 0)
            {
                char[] separator = new char[] { ',' };
                foreach (string str in args.CC.Replace(";", ",").ToString().Split(separator))
                {
                    mail.CC.Add(new MailAddress(str));
                }
            }
            if (args.BCC.Length > 0)
            {
                char[] chArray2 = new char[] { ',' };
                foreach (string str2 in args.BCC.Replace(";", ",").ToString().Split(chArray2))
                {
                    mail.Bcc.Add(new MailAddress(str2));
                }
            }
            args.Attachments.ForEach(delegate (Attachment attachment) {
                mail.Attachments.Add(attachment);
            });
            return mail;
        }

        public void SendEmail(ProcessMessageArgs args)
        {
            SmtpClient client = new SmtpClient(args.Host)
            {
                EnableSsl = args.EnableSsl
            };
            if (args.Port != 0)
            {
                client.Port = args.Port;
            }
            client.Credentials = args.Credentials;
            client.Send(this.GetMail(args));
        }
    }
}