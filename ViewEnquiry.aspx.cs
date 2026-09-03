using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ViewEnquiry : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    if (Request.QueryString["ID"] != null)
                    {
                        string EnquiryID = Request.QueryString["ID"].ToString();
                        DataTable dt = _cm.GetEnquiryDetails(EnquiryID);
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0]["ReportFileName"].ToString() != "")
                            {
                                string ReportFileName = dt.Rows[0]["ReportFileName"].ToString();
                                try
                                {
                                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//Enquiries/" + ReportFileName + ".html"));
                                    divEnquiry.InnerHtml = html;
                                }
                                catch { }
                               
                            }
                        }
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
    }
}