using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OpenPop.Pop3;
using OpenPop.Mime;
using System.Data;

namespace LIMS
{
    public partial class ShowMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Pop3Client pop3Client = (Pop3Client)Session["Pop3Client"];
                int messageNumber = int.Parse(Request.QueryString["MessageNumber"]);
                Message message = pop3Client.GetMessage(messageNumber);
                MessagePart messagePart = message.MessagePart.MessageParts[0];
                lblFrom.Text = message.Headers.From.Address;
                lblSubject.Text = message.Headers.Subject;
                lblBody.Text = messagePart.BodyEncoding.GetString(messagePart.Body);
            }
        }
    }
}