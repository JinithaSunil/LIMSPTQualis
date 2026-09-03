using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace LIMS.Models
{
    public class EmailService
    {
        private static int sentEmailCount;

        public static bool SendEmail(string toEmail, string ccEmail, string bccEmail, string subject, string body, byte[] attachmentData, string attachmentName, out string errorMessage)
        {
            sentEmailCount = 0;
            errorMessage = "";
            var smtpName = HttpContext.Current.Session["SMTP_NAME"].ToString();
            var smtpPortNumber = Convert.ToInt32(HttpContext.Current.Session["SMTP_PORT"]);
            var smtpEmail = HttpContext.Current.Session["SMTP_EMAIL"].ToString();
            var smtpPassword = HttpContext.Current.Session["SMTP_PASSWORD"].ToString();
            var fromEmailAddress = HttpContext.Current.Session["FROM_EMAIL"].ToString();

            try
            {
                SmtpClient client = new SmtpClient(smtpName, smtpPortNumber);
                client.EnableSsl = true;
                client.Timeout = 900000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpEmail, smtpPassword);

                MailMessage msg = new MailMessage();

                if (!string.IsNullOrEmpty(toEmail))
                {
                    sentEmailCount++;
                    msg.To.Add(toEmail);
                }

                if (!string.IsNullOrEmpty(ccEmail))
                {
                    sentEmailCount++;
                    msg.CC.Add(ccEmail);
                }


                if (!string.IsNullOrEmpty(bccEmail))
                {
                    sentEmailCount++;
                    msg.Bcc.Add(bccEmail);
                }

                msg.From = new MailAddress(fromEmailAddress, "PTQualis ADMIN");
                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.Body = body;

                if (attachmentData != null && !string.IsNullOrEmpty(attachmentName))
                {
                    msg.Attachments.Add(new Attachment(new MemoryStream(attachmentData), attachmentName, "application/pdf"));
                }

                client.Send(msg);
                msg.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static int GetSentEmailCount()
        {
            return sentEmailCount;
        }


    }
}