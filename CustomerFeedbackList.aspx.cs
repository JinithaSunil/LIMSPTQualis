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
    public partial class CustomerFeedbackList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
   //         HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
   //         HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
   //         HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
   //         HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
   //         HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
   //         HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
   //         HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
   //         HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
   //         HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
   //         HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");
   //         HtmlControl liViewInquiries2 = (HtmlGenericControl)this.Master.FindControl("liViewInquiries2");
			//HtmlControl liInquiryStatus2 = (HtmlGenericControl)this.Master.FindControl("liInquiryStatus2");
   //         HtmlControl liCustomerFeedbackList = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedbackList2");
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
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                    //switch (RoleID)
                    //{
                    //    case 1://QC
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = false;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 2://Manager
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = false;
                    //        liInquiries1.Visible = false;
                    //        liCRFList1.Visible = false;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 3://Lab Technician
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = false;
                    //        liInquiries1.Visible = false;
                    //        liCRFList1.Visible = false;
                    //        liSampleManagement1.Visible = false;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = false;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 4://Admin
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 5://Sample Management
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = false;
                    //        liQualityManagement1.Visible = false;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
                if (!Page.IsPostBack)
                {
                    //liCustomerFeedback1.Attributes.Add("class", "active");
                    //liCustomerFeedbackList.Attributes.Add("class", "active");
                    BindCustomerFeedback();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        private void BindCustomerFeedback()
        {
            DataTable dtCustomerFeedback = _cm.getCustomerFeedbackList();
            gvCustomerFeedbackList.DataSource = dtCustomerFeedback;
            gvCustomerFeedbackList.DataBind();
        }

        protected void gvCustomerFeedbackList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int index = e.Row.RowIndex;
                //int FeedbackID = int.Parse(gvCustomerFeedbackList.DataKeys[index]["FeedbackID"].ToString());
                Label lblfeedbackvalue = (e.Row.FindControl("lblfeedbackweightage") as Label);
                HtmlGenericControl divProgress = e.Row.FindControl("divProgress") as HtmlGenericControl;
                int average = Convert.ToInt32(lblfeedbackvalue.Text);
                string feedback = "";
                if(average > 90 && average <= 100)
                {
                    feedback = "Excellent";
                }
                else
                if (average > 80 && average <= 90)
                {
                    feedback = "Very Good";
                }
                else
                if (average > 50 && average <= 80)
                {
                    feedback = "Good";
                }
                else
                if (average > 30 && average <= 50)
                {
                    feedback = "Fair";
                }
                else
                {
                    feedback = "NeedsImprovement";
                }
                switch (feedback)
                {
                    case "Excellent":
                        divProgress.Attributes["class"] = "progress-bar bg-green";
                        divProgress.Attributes["style"] = "width:" + average + "%";
                        divProgress.Attributes["aria-valuenow"] = average.ToString();
                        break;
                    case "VeryGood":
                        divProgress.Attributes["class"] = "progress-bar bg-blue";
                        divProgress.Attributes["style"] = "width:" + average + "%";
                        divProgress.Attributes["aria-valuenow"] = average.ToString();
                        break;
                    case "Good":
                        divProgress.Attributes["class"] = "progress-bar bg-light-blue";
                        divProgress.Attributes["style"] = "width:" + average + "%";
                        divProgress.Attributes["aria-valuenow"] = average.ToString();
                        break;
                    case "Fair":
                        divProgress.Attributes["class"] = "progress-bar bg-orange";
                        divProgress.Attributes["style"] = "width:" + average + "%";
                        divProgress.Attributes["aria-valuenow"] = average.ToString();
                        break;
                    case "NeedsImprovement":
                        divProgress.Attributes["class"] = "progress-bar bg-red";
                        divProgress.Attributes["style"] = "width:" + average + "%";
                        divProgress.Attributes["aria-valuenow"] = average.ToString();
                        break;
                    default: break;
                }
            }
        }
    }
}