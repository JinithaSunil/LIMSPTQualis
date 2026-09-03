using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class SendMail : System.Web.UI.Page
    {
        LIMS.DAL.emailCommunication _email = new LIMS.DAL.emailCommunication();
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSendEnquiryReceived_Click(object sender, EventArgs e)
        {
            _email.EnquiryReceived(59);
        }

        protected void btnSendCRFSent_Click(object sender, EventArgs e)
        {
            _email.CRFSent(10, "CustomerPTI.aspx");
        }

        protected void LocalHostSMTP_Click(object sender, EventArgs e)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "mail.smtp2go.com";   //-- Donot change.
                smtpClient.Port = 2525; //--- Donot change
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new System.Net.NetworkCredential("jinitha.s@rni-solutions.com", "Jinitha@2022");
               // smtpClient.Send(mail);
                lblMsg.Text = "Message Sent";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Message Send Failed";
                throw ex;

            }
        }
    }
}