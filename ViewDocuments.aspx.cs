using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;

using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ViewDocuments : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    string DocumentID = Request.QueryString["ID"].ToString();
                    DataTable dt = new DataTable();

                    dt = _cm.GetDocument(DocumentID);
                    if (dt.Rows.Count > 0)
                    {
                        string Filename = dt.Rows[0]["DocumentName"].ToString();
                        string documentUrl = "/VendorDocument/" + Filename;
                        Response.Redirect(ResolveUrl(documentUrl));
                    }
                }
                if (Request.QueryString["Inv"] != null)
                {
                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("/Invoice/MergedOutput.html"));
                    divDocument.InnerHtml = html;
                }
            }
        }
    }

}