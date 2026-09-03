using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ViewPR : System.Web.UI.Page
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
                        string PRID = Request.QueryString["ID"].ToString();
                        DataTable dt = _cm.GetPurchaseRequisition(PRID);
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0]["ReportFileName"].ToString() != "")
                            {
                                string ReportFileName = dt.Rows[0]["ReportFileName"].ToString();
                                string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//PurchaseRequisition/" + ReportFileName + ".html"));
                                divPR.InnerHtml = html;
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