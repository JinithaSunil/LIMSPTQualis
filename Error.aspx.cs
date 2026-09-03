using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string s = "";
            if (Session["ErrorMessage"] != null)
            {
                if (Request.QueryString["id"] != null)
                {
                    s = Request.QueryString["id"].ToString();
                }
                // lblErrorMessage.Text = s + " : " + Session["ErrorMessage"].ToString();
                lblErrorMessage.Text = "Ref#:" + s + " : Share this reference with RNI Team via chat";
                 lblErrorMessage.Visible = true;
            }
            else
            {
                lblErrorMessage.Visible = false;
                lblDefaultErrorMessage.Visible = true;
            }
        }
    }
}