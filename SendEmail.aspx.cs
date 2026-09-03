using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class SendEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SendMail("jinitha.s@rni-solutions.com", "jinitha.s@rni-solutions.com", "Test from https://hemetekLIMS.rnisolutions.in/", "https://hemetekLIMS.rnisolutions.in/");
        }
        public void SendMail(string fromAddress, string toAddress, string subject, string body)
        {
            using (MailMessage mail = BuildMessageWith(fromAddress, toAddress.Replace(',', ';'), subject, body))
            {
                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtpClient = new SmtpClient();
                    string msg = string.Empty;
                    MailAddress from = new MailAddress(fromAddress);
                    message.From = from;
                    message.To.Add(toAddress);
                    message.Bcc.Add("jinitha.s@rni-solutions.com");
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    message.Body = body;
                    smtpClient.Host = "relay-hosting.secureserver.net";   //-- Donot change.
                    smtpClient.Port = 25; //--- Donot change
                    smtpClient.EnableSsl = false;//--- Donot change
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new System.Net.NetworkCredential("support@rni-solutions.com", "jinitha123");
                    smtpClient.Send(message);
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
                }
            }
        }
        public void ErrorHandler(int errCode, string msg)
        {
            switch (errCode)
            {
                case 1:
                    HttpContext.Current.Session["ErrorMessage"] = msg;
                   // HttpContext.Current.Response.Redirect("Error.aspx?id=" + msg);
                    break;
                default:
                    break;
            }
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
    }
}