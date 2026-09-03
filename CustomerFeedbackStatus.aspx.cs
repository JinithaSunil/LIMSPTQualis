using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerFeedbackStatus : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liCustomerFeedback = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            if (Session["Usertype"] != null)
            {
                if (Session["EmployeeName"] != null)
                {
                    Label lblUserName = (Label)Master.FindControl("lblUserName");
                    lblUserName.Text = Session["EmployeeName"].ToString();
                }
                if (Session["Email"] != null)
                {
                    Label lblUserEmail = (Label)Master.FindControl("lblUserEmail");
                    lblUserEmail.Text = Session["Email"].ToString();
                }
                if (Request.QueryString["ID"] != null)
                {
                    liCustomerFeedback.Attributes.Add("class", "active");
                    int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                    BindCustomerFeedback(InquiryId);
                }
                else
                {

                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }
        private void BindCustomerFeedback(int inquiryID)
        {
            DataTable dt = _cm.getCustomerFeedback(inquiryID);

            if (dt.Rows.Count > 0)
            {
                gvCustomerFeedback.DataSource = dt;
                gvCustomerFeedback.DataBind();
            }

            DataTable dt2 = _cm.getCustomerFeedbackAverage(inquiryID);
            string average = "";
            if (dt2.Rows.Count > 0)
            {
                average = dt2.Rows[0]["average"].ToString();
                txtKnob.Attributes.Add("value", average);
            }

            DataTable dt3 = _cm.getCustomerInquiryDetails(inquiryID);
            if(dt3.Rows.Count > 0)
            {
                LstDetails.DataSource = dt3;
                LstDetails.DataBind();
            }
        }

        protected void gvCustomerFeedback_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int index = e.Row.RowIndex;
                int FeedbackID = int.Parse(gvCustomerFeedback.DataKeys[index]["FeedbackID"].ToString());
                Label lblfeedbackvalue = (e.Row.FindControl("lblfeedbackvalue") as Label);
                HtmlGenericControl divProgress = e.Row.FindControl("divProgress") as HtmlGenericControl;

                switch (lblfeedbackvalue.Text)
                {
                    case "Excellent":
                        divProgress.Attributes["class"] = "progress-bar bg-green";
                        divProgress.Attributes["style"] = "width: 100%";
                        divProgress.Attributes["aria-valuenow"] = "100";
                        break;
                    case "VeryGood":
                        divProgress.Attributes["class"] = "progress-bar bg-blue";
                        divProgress.Attributes["style"] = "width: 80%";
                        divProgress.Attributes["aria-valuenow"] = "80";
                        break;
                    case "Good":
                        divProgress.Attributes["class"] = "progress-bar bg-light-blue";
                        divProgress.Attributes["style"] = "width: 50%";
                        divProgress.Attributes["aria-valuenow"] = "50";
                        break;
                    case "Fair":
                        divProgress.Attributes["class"] = "progress-bar bg-orange";
                        divProgress.Attributes["style"] = "width: 30%";
                        divProgress.Attributes["aria-valuenow"] = "30";
                        break;
                    case "NeedsImprovement":
                        divProgress.Attributes["class"] = "progress-bar bg-red";
                        divProgress.Attributes["style"] = "width: 10%";
                        divProgress.Attributes["aria-valuenow"] = "10";
                        break;
                    default: break;
                }
            }
        }
    }
}