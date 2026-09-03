using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class LocalhostEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string from = txtFrom.Text.Trim();
                string to = txtTo.Text.Trim();
                string subject = txtSubject.Text.Trim();
                string message = txtMessage.Text.Trim();

                SmtpClient objSmtpClient = new SmtpClient();
                objSmtpClient.Host = "localhost";
                objSmtpClient.Port = 25;
                objSmtpClient.Send(from, to, subject, message);

                lblStatus.Text = "<b style='color:green'>Email has been sent successfully!!!</b>";
            }
            catch (SmtpException ex) { lblStatus.Text = "<b style='color:red'>" + ex.Message + "</b>"; }
        }
    }
}