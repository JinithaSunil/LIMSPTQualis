using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class Logout : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Abandon();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }

    }    
}