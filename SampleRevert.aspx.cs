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
    public partial class SampleRevert : System.Web.UI.Page
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
            if (Session["Usertype"] != null)
            {
    //            HtmlControl liSampleManagement = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
    //            HtmlControl liViewSamples2 = (HtmlGenericControl)this.Master.FindControl("liViewSamples2");
				//HtmlControl liAllSamples2 = (HtmlGenericControl)this.Master.FindControl("liAllSamples2");
    //            HtmlControl liViewSamplesMocon2 = (HtmlGenericControl)this.Master.FindControl("liViewSamplesMocon2");
    //            HtmlControl liWorkList2 = (HtmlGenericControl)this.Master.FindControl("liWorkList2");
    //            HtmlControl liWorkListMocon2 = (HtmlGenericControl)this.Master.FindControl("liWorkListMocon2");
    //            HtmlControl liGenerateReport2 = (HtmlGenericControl)this.Master.FindControl("liGenerateReport2");
    //            HtmlControl liGenerateReportMocon2 = (HtmlGenericControl)this.Master.FindControl("liGenerateReportMocon2");
    //            HtmlControl liReviewReport2 = (HtmlGenericControl)this.Master.FindControl("liReviewReport2");

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
       //             switch (RoleID)
       //             {
       //                 case 1://QC
       //                     liHome1.Visible = true;
       //                     liCustomers1.Visible = true;
       //                     liInquiries1.Visible = true;
       //                     liCRFList1.Visible = true;
       //                     liSampleManagement1.Visible = true;
       //                     liTestManagement1.Visible = false;
       //                     liQualityManagement1.Visible = true;
       //                     liCustomerFeedback1.Visible = false;
       //                     liUserManagement1.Visible = false;
       //                     liSupport1.Visible = false;
       //                     break;
       //                 case 2://Manager
       //                     liHome1.Visible = true;
       //                     liCustomers1.Visible = false;
       //                     liInquiries1.Visible = false;
       //                     liCRFList1.Visible = false;
       //                     liSampleManagement1.Visible = true;
							//liAllSamples2.Visible = true;
       //                     if (Session["DepartmentID"].ToString() == "1")
       //                     {
       //                         liViewSamples2.Visible = false;
       //                         liWorkList2.Visible = false;
       //                         liGenerateReport2.Visible = false;

       //                         liViewSamplesMocon2.Visible = true;
       //                         liWorkListMocon2.Visible = true;
       //                         liGenerateReportMocon2.Visible = true;
       //                     }
       //                     else
       //                     {
       //                         liViewSamples2.Visible = true;
       //                         liWorkList2.Visible = true;
       //                         liGenerateReport2.Visible = true;

       //                         liViewSamplesMocon2.Visible = false;
       //                         liWorkListMocon2.Visible = false;
       //                         liGenerateReportMocon2.Visible = false;
       //                     }
       //                     liTestManagement1.Visible = true;
       //                     liQualityManagement1.Visible = true;
       //                     liCustomerFeedback1.Visible = false;
       //                     liUserManagement1.Visible = false;
       //                     liSupport1.Visible = false;
       //                     break;
       //                 case 3://Lab Technician
       //                     liHome1.Visible = true;
       //                     liCustomers1.Visible = false;
       //                     liInquiries1.Visible = false;
       //                     liCRFList1.Visible = false;
       //                     liSampleManagement1.Visible = false;
       //                     liTestManagement1.Visible = true;
       //                     liQualityManagement1.Visible = false;
       //                     liCustomerFeedback1.Visible = false;
       //                     liUserManagement1.Visible = false;
       //                     liSupport1.Visible = false;
       //                     break;
       //                 case 4://Admin
       //                     liHome1.Visible = true;
       //                     liCustomers1.Visible = true;
       //                     liInquiries1.Visible = true;
       //                     liCRFList1.Visible = true;
       //                     liSampleManagement1.Visible = true;
       //                     liTestManagement1.Visible = true;
       //                     liQualityManagement1.Visible = true;
       //                     liCustomerFeedback1.Visible = false;
       //                     liUserManagement1.Visible = false;
       //                     liSupport1.Visible = false;
       //                     break;
       //                 case 5://Sample Management
       //                     liHome1.Visible = true;
       //                     liCustomers1.Visible = true;
       //                     liInquiries1.Visible = true;
       //                     liCRFList1.Visible = true;
       //                     liSampleManagement1.Visible = true;
       //                     liViewSamplesMocon2.Visible = true;
       //                     liWorkListMocon2.Visible = true;
       //                     liQualityManagement1.Visible = true;
       //                     liGenerateReport2.Visible = true;
       //                     liGenerateReportMocon2.Visible = false;
       //                     liReviewReport2.Visible = false;
       //                     liTestManagement1.Visible = false;
       //                     liCustomerFeedback1.Visible = false;
       //                     liUserManagement1.Visible = false;
       //                     liSupport1.Visible = false;
       //                     break;
       //                 default:
       //                     break;
       //             }
                }
                if (!Page.IsPostBack)
                {
                    //liSampleManagement1.Attributes.Add("class", "active");
                    //liViewSamplesMocon2.Visible = false;
                    //liViewSamples2.Attributes.Add("class", "active");
                    BindSamples();                    
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        private void BindSamples()
        {
            DataTable dt = _cm.GetRevertSampleList();
            ddlSampleID.DataSource = dt;
            ddlSampleID.DataTextField = "SampleIDSeries";
            ddlSampleID.DataValueField = "ID";
            ddlSampleID.DataBind();
            ddlSampleID.Items.Insert(0, new ListItem("Select SampleID"));
        }
        private void BindStatus(string StatusID)
        {
            DataTable dt = _cm.GetRevertSampleStatus(StatusID);
            ddlNewStatus.DataSource = dt;
            ddlNewStatus.DataTextField = "StatusName";
            ddlNewStatus.DataValueField = "InquiryStatusID";
            ddlNewStatus.DataBind();
            ddlNewStatus.Items.Insert(0, new ListItem("Select New Status"));
        }
        protected void ddlSampleID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sampleID = ddlSampleID.SelectedValue;
            DataTable dt = _cm.GetSampleStatus(sampleID);
            if (dt.Rows.Count > 0)
            {
                lblSampleStatus.Text = dt.Rows[0]["StatusName"].ToString();
                hdSampleStatusID.Value = dt.Rows[0]["Position"].ToString();
                BindStatus(hdSampleStatusID.Value);
            }
        }

        protected void ddlNewStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string NewStatusID = ddlNewStatus.SelectedValue;            
        }

        protected void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            int CloneCRFSampleID = 0;int SampleId = 0;
            string NewStatusID = ddlNewStatus.SelectedValue;
            DataTable dt = _cm.getCloneCRFIDFromSampleID(ddlSampleID.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                CloneCRFSampleID = Convert.ToInt32(dt.Rows[0]["CloneCRFSampleID"].ToString());
                SampleId = Convert.ToInt32(dt.Rows[0]["SampleId"].ToString());
                //change CloneCRF &status to selected status
                _cm.CRFStatusID = Convert.ToInt32(NewStatusID);
                _cm.CloneCRFSampleID = CloneCRFSampleID;
                _cm.UpdateCRFStatus(_cm);

                _cm.sampleId = SampleId;
                _cm.sampleStatusID = Convert.ToInt32(NewStatusID);
                _cm.updateSampleStatus(_cm);

                BindSamples();
            }
            
        }
    }
}