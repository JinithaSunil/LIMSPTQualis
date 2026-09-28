using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int flag = 0;string result = "";
            if (txtUserName.Text == "") { flag=1; txtUserName.BorderColor=Color.Red; }
            if (flag == 0)
            {
                result = _cm.ResetPassword(txtUserName.Text);
                lblMsg.Text = result;
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Enter Username to reset password')", true);
            }
        }
    }
}