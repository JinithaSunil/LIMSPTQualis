using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class Viewer : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
        { 
            if (Session["Doc"] != null)
			{
                string[] s = Session["Doc"].ToString().Split(',');
                string folder = s[0]; //"~/Invoice/";       // your HTML folder
                string fileName = s[1]; //"INV-11084.htm";  // dynamic or static
                

                // 1. Physical path for checking real file
                string physicalPath = Server.MapPath(folder + fileName);

                if (File.Exists(physicalPath))
                {
                    // 2. Convert to relative URL path
                    string urlPath = ResolveUrl(folder + fileName);

                    frameHtml.Attributes["src"] = urlPath;
                }
                else
                {
                    // File not found → load a friendly HTML message
                    frameHtml.Attributes["src"] = ResolveUrl("~/FileNotFound.html");
                }
            }
		}

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            string iframeId = frameHtml.ClientID;
            string script = $@"
                        var iframe = document.getElementById('{iframeId}');
                        iframe.contentWindow.focus();
                        iframe.contentWindow.print();
                    ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "PrintIframe", script, true);
        }
    }
}