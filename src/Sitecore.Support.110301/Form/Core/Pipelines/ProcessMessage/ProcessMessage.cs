using Sitecore.WFFM.Abstractions.Mail;
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
                // modified part of code: separator and foreach have been added to fix the issue with several CC e-mail addresses
                char[] separator = new char[] { ',' };
                foreach (string str in args.CC.Replace(";", ",").ToString().Split(separator))
                {
                    mail.CC.Add(new MailAddress(str));
                }
                // end of the modified part
            }
            if (args.BCC.Length > 0)
            {
                // modified part of code: chArray2 and foreach have been added to fix the issue with several BC e-mail addresses
                char[] chArray2 = new char[] { ',' };
                foreach (string str2 in args.BCC.Replace(";", ",").ToString().Split(chArray2))
                {
                    mail.Bcc.Add(new MailAddress(str2));
                }
                // end of the modified part
            }
            args.Attachments.ForEach(delegate (Attachment attachment)
            {
                mail.Attachments.Add(attachment);
            });
            return mail;
        }

        public static int _MaxIdleTime = -1;
        public static int MaxIdleTime
        {
            get
            {
                string s = Sitecore.Configuration.Settings.GetSetting("WFFM.ProcessMessage.SmtpClient.ServicePoint.MaxIdleTime");
                if (string.IsNullOrEmpty(s))
                    _MaxIdleTime = -1;

                if (!int.TryParse(s, out _MaxIdleTime))
                    _MaxIdleTime = -1;
                return _MaxIdleTime;
            }
        }

        public void SendEmail(ProcessMessageArgs args)
        {
            SmtpClient client = new SmtpClient(args.Host)
            {
                EnableSsl = args.EnableSsl
            };
            if (MaxIdleTime != -1)
            {
                client.ServicePoint.MaxIdleTime = _MaxIdleTime;
            }
            if (args.Port != 0)
            {
                client.Port = args.Port;
            }
            client.Credentials = args.Credentials;
            client.Send(this.GetMail(args));
        }
    }
}