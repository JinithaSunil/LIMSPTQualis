using LIMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class openlink : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            for (int i= 16420; i <= 16915; i++)
            {
                    string url = "http://hemetekLIMS.rnisolutions.in/generatetestallocation.aspx?ID=" + i;
                    Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
               
            }

        }

    }
}