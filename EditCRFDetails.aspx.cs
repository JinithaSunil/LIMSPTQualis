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
    public partial class EditCRFDetails : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.emailCommunication _email = new DAL.emailCommunication();
        protected void Page_Load(object sender, EventArgs e)
        {
            //HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
            //HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
            //HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
            //HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
            //HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            //HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
            //HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
            //HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            //HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            //HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");
            //HtmlControl liViewInquiries2 = (HtmlGenericControl)this.Master.FindControl("liViewInquiries2");
            //HtmlControl liInquiryStatus2 = (HtmlGenericControl)this.Master.FindControl("liInquiryStatus2");
            //HtmlControl liNewInquiry2 = (HtmlGenericControl)this.Master.FindControl("liNewInquiry2");
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
                    //liInquiries1.Attributes.Add("class", "active");
                    //liNewInquiry2.Attributes.Add("class", "active");
                    if (Request.QueryString["E"] != null)
                    {
                        int EnquiryID = Convert.ToInt32(Request.QueryString["E"].ToUpper());                        
                        BindCRFDetails();

                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        protected void BindCRFDetails()
        {
            int EnquiryID = Convert.ToInt32(Request.QueryString["E"].ToUpper());
            DataTable dtCRF = _cm.getAllCRFDetails(EnquiryID);
            if (dtCRF.Rows.Count > 0)
            {
                gvcrfsampledetails.DataSource = dtCRF;
                gvcrfsampledetails.DataBind();
            }
            else
            {
                gvcrfsampledetails.DataSource = null;
                gvcrfsampledetails.DataBind();
            }
        }


        protected void gvcrfsampledetails_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvcrfsampledetails.EditIndex = e.NewEditIndex;
            BindCRFDetails();
        }

        protected void gvcrfsampledetails_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int flag = 0;
            Label lblSampleName = (Label)gvcrfsampledetails.Rows[e.RowIndex].FindControl("lblSampleName");
            TextBox txtSampleName = (TextBox)gvcrfsampledetails.Rows[e.RowIndex].FindControl("txtSampleName");
            Label lblBatchNo = (Label)gvcrfsampledetails.Rows[e.RowIndex].FindControl("lblBatchNo");
            TextBox txtBatchNo = (TextBox)gvcrfsampledetails.Rows[e.RowIndex].FindControl("txtBatchNo");
            Label lblTestMethod = (Label)gvcrfsampledetails.Rows[e.RowIndex].FindControl("lblTestMethod");
            TextBox txtTestMethod = (TextBox)gvcrfsampledetails.Rows[e.RowIndex].FindControl("txtTestMethod");
            Label lblTestParameter = (Label)gvcrfsampledetails.Rows[e.RowIndex].FindControl("lblTestParameter");
            TextBox txtTestParameter = (TextBox)gvcrfsampledetails.Rows[e.RowIndex].FindControl("txtTestParameter");
            int CRFSampleID = int.Parse(gvcrfsampledetails.DataKeys[e.RowIndex]["CRFSampleID"].ToString());
            int CustomerRequestID = int.Parse(gvcrfsampledetails.DataKeys[e.RowIndex]["CustomerRequestID"].ToString());


            _cm.tableName = "CRFSampleDetails";
            _cm.paramID = "CRFSampleID";
            _cm.paramIDValue = CRFSampleID;

            if (txtSampleName.Text != lblSampleName.Text)
            {
                _cm.paramName = "SampleName";
                _cm.paramValue = txtSampleName.Text;
                _cm.UpdateTable(_cm);
                flag = 1;
            }
            if (txtBatchNo.Text != lblBatchNo.Text)
            {
                _cm.paramName = "BatchNo";
                _cm.paramValue = txtBatchNo.Text;
                _cm.UpdateTable(_cm);
                flag = 1;
            }
            if (txtTestMethod.Text != lblTestMethod.Text)
            {
                _cm.paramName = "TestMethod";
                _cm.paramValue = txtTestMethod.Text;
                _cm.UpdateTable(_cm);
                flag = 1;
            }
            if (txtTestParameter.Text != lblTestParameter.Text)
            {
                _cm.paramName = "TestParameter";
                _cm.paramValue = txtTestParameter.Text;
                _cm.UpdateTable(_cm);
                flag = 1;
            }

            //_cm.tableName = "CustomerCRFSampleDetails";
            //_cm.paramID = "CustomerCRFSampleID";
            //_cm.paramIDValue = CRFSampleID;

            //if (txtSampleName.Text != lblSampleName.Text)
            //{
            //    _cm.paramName = "SampleName";
            //    _cm.paramValue = txtSampleName.Text;
            //    _cm.UpdateTable(_cm);
            //    flag = 1;
            //}
            //if (txtBatchNo.Text != lblBatchNo.Text)
            //{
            //    _cm.paramName = "BatchNo";
            //    _cm.paramValue = txtBatchNo.Text;
            //    _cm.UpdateTable(_cm);
            //    flag = 1;
            //}
            //if (txtTestMethod.Text != lblTestMethod.Text)
            //{
            //    _cm.paramName = "TestMethod";
            //    _cm.paramValue = txtTestMethod.Text;
            //    _cm.UpdateTable(_cm);
            //    flag = 1;
            //}
            //if (txtTestParameter.Text != lblTestParameter.Text)
            //{
            //    _cm.paramName = "TestParameter";
            //    _cm.paramValue = txtTestParameter.Text;
            //    _cm.UpdateTable(_cm);
            //    flag = 1;
            //}
            
            gvcrfsampledetails.EditIndex = -1;
            BindCRFDetails();
            if (Request.QueryString["E"] != null)
            {
                int EnquiryID = Convert.ToInt32(Request.QueryString["E"].ToUpper());
                int CRFID = 0; string CRFLink = ""; string AllocationLink = "";
                DataTable dt1 = _cm.GetCustomerCRFSample(EnquiryID.ToString());
                if (dt1.Rows.Count > 0)
                {
                    CRFID = Convert.ToInt32(dt1.Rows[0]["CRFID"].ToString());
                }
                if (flag == 1)
                {
                    DataTable dt = _cm.getAllSampleDetails(EnquiryID);
                    if (dt.Rows.Count > 0)
                    {
                        foreach(DataRow dr in dt.Rows)
                        {
                            _cm.sampleId = Convert.ToInt32(dr["SampleID"].ToString());
                            _cm.sampleStatusID = 12;
                            _cm.updateSampleStatus(_cm);
                        }
                    }                   
                }
                switch (CRFID)
                {
                    case 1:
                        CRFLink = "http://CiscoLIMS.rnisolutions.in:8608/CreateRequestCustomerFM.aspx?ID=" + EnquiryID;
                        //CRFLink = "CloneCustomerFM.aspx?ID=" + inquiryID;
                        break;
                    case 3:
                        CRFLink = "http://CiscoLIMS.rnisolutions.in:8608/CreateRequestCustomerPTI.aspx?ID=" + EnquiryID;
                        //CRFLink = "CloneCustomerMocon.aspx?ID=" + inquiryID;
                        break;
                    default:
                        break;
                }

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('" + CRFLink + "' ,'_blank');", true);

                DataTable dt6 = _cm.GetAllocationDetails(EnquiryID, 1);
                if (dt6.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt6.Rows)
                    {
                        AllocationLink = "http://CiscoLIMS.rnisolutions.in:8608/GenerateTestAllocation.aspx?ID=" + EnquiryID;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('" + AllocationLink + "' ,'_blank');", true);
                    }
                }

            }
        }
        protected void gvcrfsampledetails_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvcrfsampledetails.EditIndex = -1;
            BindCRFDetails();
        }
    }
}