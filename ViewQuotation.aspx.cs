using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace LIMS
{
	public partial class ViewQuotation : System.Web.UI.Page
	{
        DAL.CM _cm = new DAL.CM();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID1"] != null)
                {
                    string VendorQuotationID = Request.QueryString["ID1"].ToString();
                    if (Request.QueryString["ID2"] != null)
                    {
                        string VendorID = Request.QueryString["ID2"].ToString();
                        DataTable dt = _cm.GetVQFileName(VendorQuotationID, VendorID);
                        if (dt.Rows.Count > 0)
                        {
                            string ReportFileName = dt.Rows[0]["ReportFileName1"].ToString();
                            if (ReportFileName.ToString() != "")
                            {
                                try
                                {
                                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//VendorQuotation/" + ReportFileName + ".html"));
                                    divQuotation.InnerHtml = html;
                                }
                                catch { }
                            }
                            else
                            {
                                divQuotation.InnerHtml = "<h4>File not Available</h4>";
                            }
                        }
                    }
                    else
                    {
                        Response.Redirect("VendorQuotationManagement.aspx");
                    }
                }
                else
                {
                    Response.Redirect("VendorQuotationManagement.aspx");
                }
            }
        }
    }
}