using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace LIMS.DAL
{
    public class mailer
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        public void SendMail(string fromAddress, string toAddress, string ccAddress, string bccAddress, string attach, string subject, string body)
        {
            using (MailMessage mail = BuildMessageWith(fromAddress, toAddress.Replace(',', ';'), ccAddress.Replace(',', ';'), bccAddress.Replace(',', ';'), attach.Replace(',', ';'), subject, body))
            {
                SendMail(mail);
            }
        }
        public void SendMail(string fromAddress, string toAddress, string ccAddress, string bccAddress, string subject, string body)
        {
            using (MailMessage mail = BuildMessageWith(fromAddress, toAddress.Replace(',', ';'), ccAddress.Replace(',', ';'), bccAddress.Replace(',', ';'), subject, body))
            {
                SendMail(mail);
            }
        }
        public void SendMail(MailMessage mail)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "mail.smtp2go.com";   //-- Donot change.
                smtpClient.Port = 2525; //--- Donot change
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new System.Net.NetworkCredential("jinitha.s@rni-solutions.com", "Jinitha@2022");
                smtpClient.Send(mail);
                HttpContext.Current.Session["Failed"] = "0";

            }
            catch (Exception e)
            {
                HttpContext.Current.Session["Failed"] = "1";
                string ErrorMessage = e.Message;
                string Source = e.Source;
                string StackTrace = e.StackTrace;
                string Target = e.TargetSite.ToString();

                // You should fill this property with details here.
                string InnerExceptionMessage = e.InnerException.Message;

                ErrorHandler(1, ErrorMessage + "," + Source + "," + StackTrace + "," + InnerExceptionMessage);
                HttpContext.Current.Session["ErrorMessage"] = ErrorMessage + "," + Source + "," + StackTrace + "," + InnerExceptionMessage;
                throw e;

            }
        }

        
        public void SendMail(string fromAddress, string toAddress, string subject, string body)
        {
            using (MailMessage mail = BuildMessageWith(fromAddress, toAddress.Replace(',', ';'), subject, body))
            {
                try
                {
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = "mail.smtp2go.com";   //-- Donot change.
                    smtpClient.Port = 2525; //--- Donot change
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new System.Net.NetworkCredential("jinitha.s@rni-solutions.com", "Jinitha@2022");
                    smtpClient.Send(mail);
                    HttpContext.Current.Session["Failed"] = "0";

                }
                catch (Exception e)
                {
                    HttpContext.Current.Session["Failed"] = "1";
                    string ErrorMessage = e.Message;
                    string Source = e.Source;
                    string StackTrace = e.StackTrace;
                    string Target = e.TargetSite.ToString();

                    // You should fill this property with details here.
                    string InnerExceptionMessage = e.InnerException.Message;

                    ErrorHandler(1, ErrorMessage + "," + Source + "," + StackTrace + "," + InnerExceptionMessage);
                    HttpContext.Current.Session["ErrorMessage"] = ErrorMessage + "," + Source + "," + StackTrace + "," + InnerExceptionMessage;
                    throw e;

                }
            }
        }
        public void ErrorHandler(int errCode, string msg)
        {
            switch (errCode)
            {
                case 1:
                    HttpContext.Current.Session["ErrorMessage"] = msg;
                    HttpContext.Current.Response.Redirect("Error.aspx?id=" + msg);
                    break;
                default:
                    break;
            }
        }
        public void SendMail(string fromAddress, string toAddress, string subject, string body, string attach)
        {
            using (MailMessage mail = BuildMessageWith1(fromAddress, toAddress.Replace(',', ';'), subject, body, attach))
            {
                try
                {

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = "mail.smtp2go.com";   //-- Donot change.
                    smtpClient.Port = 2525; //--- Donot change
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new System.Net.NetworkCredential("jinitha.s@rni-solutions.com", "Jinitha@2022");
                    smtpClient.Send(mail);
                    HttpContext.Current.Session["Failed"] = "0";

                }
                catch (Exception e)
                {
                    HttpContext.Current.Session["Failed"] = "1";
                    string ErrorMessage = e.Message;
                        string Source = e.Source;
                        string StackTrace = e.StackTrace;
                        string Target = e.TargetSite.ToString();

                        // You should fill this property with details here.
                        string InnerExceptionMessage = e.InnerException.Message;
                    
                        _cm.ErrorHandler(1, ErrorMessage + "," + Source + "," + StackTrace + "," + InnerExceptionMessage);
                }

            }
        }
        //build a mail message
        private static MailMessage BuildMessageWith1(string fromAddress, string toAddress, string subject, string body, string attach)
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(fromAddress, "ERP Admin"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            string[] tos = toAddress.Split(';');
            foreach (string to in tos)
            {
                if (!string.IsNullOrWhiteSpace(to))
                {
                    message.To.Add(new MailAddress(to.Trim()));
                }
            }

            if (!string.IsNullOrEmpty(attach))
            {
                string[] files = attach.Split(';');
                foreach (string file in files)
                {
                    if (!string.IsNullOrEmpty(file) && File.Exists(file))
                    {
                        try
                        {
                            message.Attachments.Add(new Attachment(file));
                        }
                        catch (Exception ex)
                        {
                            // Log the error but continue
                            System.Diagnostics.Debug.WriteLine("Error attaching file: " + file + ". Error: " + ex.Message);
                        }
                    }
                }
            }

            return message;
        }
        private static MailMessage BuildMessageWith(string fromAddress, string toAddress, string subject, string body)
        {
            MailMessage message = new MailMessage
            {
                //Sender = new MailAddress(Settings.WebMasterEmail), // on Behave of When From differs
                From = new MailAddress(fromAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            //   message.CC.Add("jinitha.s@rnisolutions.in");
            string[] tos = toAddress.Split(';');

            foreach (string to in tos)
            {

                message.To.Add(new MailAddress(to));
            }
            return message;
        }

        private static MailMessage BuildMessageWith(string fromAddress, string toAddress, string subject, string body, string attach)
        {
            MailMessage message = new MailMessage
            {
                //Sender = new MailAddress(Settings.WebMasterEmail), // on Behave of When From differs
                From = new MailAddress(fromAddress, "LIMS Admin"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            //   message.CC.Add("jinitha.s@rnisolutions.in");
            string[] tos = toAddress.Split(';');

            foreach (string to in tos)
            {

                message.To.Add(new MailAddress(to));
            }
            try
            {
                message.Attachments.Add(new Attachment(attach));
            }
            catch(Exception ex)
            {

            }
            return message;
        }

        private static MailMessage BuildMessageWith(string fromAddress, string toAddress, string ccAddress, string bccAddress, string attach, string subject, string body)
        {

            MailMessage message = new MailMessage
            {
                //Sender = new MailAddress(Settings.WebMasterEmail), // on Behave of When From differs
                From = new MailAddress(fromAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };


            string[] tos = toAddress.Split(';');
            string[] ccs = ccAddress.Split(';');
            string[] bccs = bccAddress.Split(';');
            string[] attachs = attach.Split(';');

            foreach (string to in tos)
            {
                message.To.Add(new MailAddress(to));
            }
            foreach (string cc in ccs)
            {
                message.CC.Add(new MailAddress(cc));
            }
            foreach (string bcc in bccs)
            {
                message.Bcc.Add(new MailAddress(bcc));
            }
            foreach (string attach1 in attachs)
            {
                message.Attachments.Add(new Attachment(attach1));
            }

            return message;
        }
        // read the text in template file and return it as a string

        //Added on 9Apr25 - For EnquiryList - Bhargav
        private static MailMessage BuildMessageWith(string fromAddress, string toAddress, string ccAddress, string bccAddress, string subject, string body)
        {

            MailMessage message = new MailMessage
            {
                //Sender = new MailAddress(Settings.WebMasterEmail), // on Behave of When From differs
                From = new MailAddress(fromAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };


            string[] tos = toAddress.Split(';');
            string[] ccs = ccAddress.Split(';');
            string[] bccs = bccAddress.Split(';');


            foreach (string to in tos)
            {
                message.To.Add(new MailAddress(to));
            }
            foreach (string cc in ccs)
            {
                message.CC.Add(new MailAddress(cc));
            }
            foreach (string bcc in bccs)
            {
                message.Bcc.Add(new MailAddress(bcc));
            }


            return message;
        }
        private static string ReadFileFrom(string templateName)
        {
            string filePath = System.Web.HttpContext.Current.Server.MapPath(templateName);

            string body = File.ReadAllText(filePath);

            return body;
        }
        // get the template body, cache it and return the text
        private static string GetMailBodyOfTemplate(string templateName)
        {
            string cacheKey = string.Concat("mailTemplate:", templateName);
            string body;
            body = (string)System.Web.HttpContext.Current.Cache[cacheKey];
            if (string.IsNullOrEmpty(body))
            {
                //read template file text
                body = ReadFileFrom(templateName);

                if (!string.IsNullOrEmpty(body))
                {
                    System.Web.HttpContext.Current.Cache.Insert(cacheKey, body, null, DateTime.Now.AddHours(12.5).AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
                }
            }

            return body;
        }
        // replace the tokens in template body with corresponding values
        public string PrepareMailBodyWith(string templateName, params string[] pairs)
        {
            string body = GetMailBodyOfTemplate(templateName);

            for (var i = 0; i < pairs.Length; i += 2)
            {

                body = Regex.Replace(body, pairs[i], pairs[i + 1]);
                //body = body.Replace("<%={0}".FormatWith(pairs[i]), pairs[i + 1]);
            }
            return body;
        }
    }
}