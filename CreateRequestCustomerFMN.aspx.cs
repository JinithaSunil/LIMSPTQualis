using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CreateRequestCustomerFMN : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int InquiryID = 0; int ReportID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["ID"] != null)
            {
                InquiryID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                ReportID = Convert.ToInt32(Request.QueryString["r"].ToString());
                Session["InquiryID"] = InquiryID;
                Session["ReportID"] = ReportID;
            }
            StringBuilder sb = new StringBuilder();
            mailer objEmail = new mailer();
            StringReader sr = new StringReader(sb.ToString());
            string CustomerUID = "";

            if (Session["InquiryID"] != null)
            {
                string body = objEmail.PrepareMailBodyWith("TAF.html");
                string str = body.ToString();
                string filename = Server.MapPath("//CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                System.IO.File.WriteAllText(@filename, str);

                //change customer inquiry Status to CRF Received
                _cm.inquiryStatusId = 6;
                _cm.UpdateInquiryStatus(_cm);
                GC.Collect();
                string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm"));
                divRequest.InnerHtml = html;
               // Response.Redirect("CustomerFM.aspx?ID=" + InquiryID + "&r=" + ReportID);
            }

        }

        protected void rptParent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            InquiryID = Convert.ToInt32(Request.QueryString["ID"].ToString());

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblTestParameter = (Label)e.Item.FindControl("lblTestParameter");
                if (lblTestParameter != null)
                {
                    DataTable dt = _cm.getCustomerCRFSampleDetails(InquiryID, lblTestParameter.Text);
                    if (dt.Rows.Count > 0)
                    {
                        Repeater rptChild = (Repeater)e.Item.FindControl("rptChild");
                        rptChild.DataSource = dt;
                        rptChild.DataBind();
                    }

                }
              
            }
        }
    }
}
