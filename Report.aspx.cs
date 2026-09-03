using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] str = new string[] { };


            string[] alloc = new string[] {"http://localhost:41414/TestReportChemical.aspx?s=7206&c=6431&t=6548",
"http://localhost:41414/TestReportChemical.aspx?s=7335&c=6568&t=6660",
"http://localhost:41414/TestReportChemical.aspx?s=7377&c=6610&t=6677"};
            foreach (string s in alloc)
            {
                Response.Write("<script>");
                Response.Write("window.open('"+s+"' ,'_blank')");
                Response.Write("</script>");
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('"+s+"' ,'_blank');", true);
            }

        }
    }
}