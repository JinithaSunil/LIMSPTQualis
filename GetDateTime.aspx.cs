using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GetDateTime : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.AddHours(12.5).ToString();
        }
    }
}