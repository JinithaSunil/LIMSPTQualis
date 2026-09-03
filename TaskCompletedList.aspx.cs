using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ZXing;

namespace LIMS
{
    public partial class TaskCompletedList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
            HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
            HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
            HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
            HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
            HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
            HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");
            HtmlControl liViewInquiries2 = (HtmlGenericControl)this.Master.FindControl("liViewInquiries2");
			HtmlControl liInquiryStatus2 = (HtmlGenericControl)this.Master.FindControl("liInquiryStatus2");
            HtmlControl liTaskCompleted2 = (HtmlGenericControl)this.Master.FindControl("liTaskCompleted2");
            
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
                    switch (RoleID)
                    {
                        case 1://QC
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 2://Manager
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 3://Lab Technician
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = false;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 4://Admin
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 5://Sample Management
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        default:
                            break;
                    }
                }
                if (!Page.IsPostBack)
                {
                    liCustomerFeedback1.Attributes.Add("class", "active");
                    liTaskCompleted2.Attributes.Add("class", "active");
                    
                    if (Session["CloneCRFSampleID"] != null)
                    {
                        CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                    }
                    Session["Id"] = 1;
                    BindCompletedList();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }

        private void BindCompletedList()
        {
            string Id = Session["Id"].ToString();
            DataTable dt = _cm.getSampleDetails(24, true);//get Samples with inquirystatusid=24 => 'Reviewed Accepted'
            gvCompletedList.DataSource = dt;
            gvCompletedList.DataBind();
        }

        protected void gvCompletedList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink hypCustomerFeedback = (e.Row.FindControl("hypCustomerFeedback") as HyperLink);
                int index = e.Row.RowIndex;

                int SampleId = int.Parse(gvCompletedList.DataKeys[index]["ID"].ToString());
                Session["SampleID"] = SampleId;

                int CloneCRFSampleID = int.Parse(gvCompletedList.DataKeys[index]["CloneCRFSampleID"].ToString());
                int InquiryID = int.Parse(gvCompletedList.DataKeys[index]["InquiryID"].ToString());
                Session["InquiryID"] = InquiryID;
                Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                Button btnSendFeedbackForm = (e.Row.FindControl("btnSendFeedbackForm") as Button);
                switch (lblStatusName.Text)
                {
                    case "Feedback Received":
                        hypCustomerFeedback.NavigateUrl = string.Format("CustomerFeedbackStatus.aspx?ID=" + CloneCRFSampleID);
                        hypCustomerFeedback.Visible = true;
                        break;
                    case "Reviewed Accepted":
                        btnSendFeedbackForm.Visible = true;
                        hypCustomerFeedback.Visible = false;
                        break;
                    default:
                        hypCustomerFeedback.Visible = false;
                        btnSendFeedbackForm.Visible = false;
                        break;
                }
            }
        }

        protected void gvCompletedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SendFeedbackForm")
            {
                DAL.emailCommunication eml = new DAL.emailCommunication();
                int index = int.Parse(e.CommandArgument.ToString());
                int sampleid = int.Parse(gvCompletedList.DataKeys[index]["ID"].ToString());
                int InquiryID = int.Parse(gvCompletedList.DataKeys[index]["InquiryID"].ToString());
                int CloneCRFSampleID = int.Parse(gvCompletedList.DataKeys[index]["CloneCRFSampleID"].ToString());
                Session["InquiryId"] = InquiryID;
                string Link = "";
                //Link = "http://CiscoLIMS.rnisolutions.in:8608/CustomerFeedback.aspx?ID=" + CloneCRFSampleID;
                Link = "CustomerFeedback.aspx?ID=" + CloneCRFSampleID;
                // eml.FeedbackFormSent(CloneCRFSampleID, Link);

                //change CloneCRF status to 'Feedback form sent'
                _cm.CRFStatusID = 22;
				_cm.CloneCRFSampleID = CloneCRFSampleID;
				_cm.UpdateCRFStatus(_cm);

                _cm.sampleId = sampleid;
                _cm.sampleStatusID = 22;
                _cm.updateSampleStatus(_cm);

                _cm.inquiryId = InquiryID;
                _cm.inquiryStatusId = 22;
                _cm.UpdateInquiryStatus(_cm);
				
                Response.Redirect("CustomerFeedback.aspx?ID=" + CloneCRFSampleID);
                BindCompletedList();
            }
        }
    }
}