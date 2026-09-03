using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.HtmlControls;

namespace LIMS
{
    public partial class GenerateReportMocon : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0;int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
            HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
            HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
            HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
            HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");

            HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
            HtmlControl liReviewReport2 = (HtmlGenericControl)this.Master.FindControl("liReviewReport2");

            HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");

            HtmlControl liWorkList2 = (HtmlGenericControl)this.Master.FindControl("liWorkList2");
            
            HtmlControl liViewSamples2 = (HtmlGenericControl)this.Master.FindControl("liViewSamples2");
			HtmlControl liAllSamples2 = (HtmlGenericControl)this.Master.FindControl("liAllSamples2");
            
            HtmlControl liGenerateReport2 = (HtmlGenericControl)this.Master.FindControl("liGenerateReport2");
            


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
							liAllSamples2.Visible = true;
                            if (Session["DepartmentID"].ToString() == "1")
                            {
                                liViewSamples2.Visible = false;
                                liWorkList2.Visible = false;
                                liGenerateReport2.Visible = false;
                            }
                            else
                            {
                                
                                
                               
                            }
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
                    BindYear();
                    liQualityManagement1.Attributes.Add("class", "active");
                    liGenerateReport2.Attributes.Add("class", "active");
                    if (Session["CloneCRFSampleID"] != null)
                    {
                        CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                    }
                    BindSampleList();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        protected void BindYear()
        {
            DataTable dt = new DataTable();
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= currentYear - 5; year--)
            {
                years.Add(year);
            }

            // Bind the dropdownlist
            ddlYear.DataSource = years;
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, "Select Year");
        }
        private void BindSampleList()
        {
            string SampleStatus = "27";
            string SampleStatus2 = "29";
            string DepartmentID = "";
            DataTable dt = new DataTable();
            if (Session["Type"] != null)
            {
                if (ddlYear.SelectedIndex != 0)
                {
                    int year = Convert.ToInt32(ddlYear.SelectedValue);
                    int month = Convert.ToInt32(ddlMonth.SelectedValue);
                    if (Session["Type"].ToString() == "Others")
                    {
                        dt = _cm.getAllSampleDetailsForReportGeneration(SampleStatus2,year,month);
                    }
                    else
                    {
                        DepartmentID = Session["DepartmentID"].ToString();
                        switch (DepartmentID)
                        {
                            case "2":
                                DepartmentID = DepartmentID + ",4";
                                dt = _cm.getSampleDetailsForReportGeneration(DepartmentID, SampleStatus,year,month);
                                break;
                            case "4":
                                DepartmentID = DepartmentID + ",2";
                                dt = _cm.getSampleDetailsForReportGeneration(DepartmentID, SampleStatus, year,month);
                                break;
                            default:
                                dt = _cm.getSampleDetailsForReportGeneration(DepartmentID, SampleStatus, year,month);
                                break;
                        }
                    }
                }
            }
            gvSampleList.DataSource = dt;
            gvSampleList.DataBind();
        }

        protected void gvSampleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string InquiryId = ""; string ReportLink = "";
            int AllocationID = 0; int sampleUID = 0; int DepartmentID = 0; string AllocatedTo = "";

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dt3 = _cm.GetEmployeeDetails();

                HyperLink hypCRF = (e.Row.FindControl("hypCRF") as HyperLink);
                HyperLink hypAllocation = (e.Row.FindControl("hypAllocation") as HyperLink);
                HyperLink hypCustomerFeedback = (e.Row.FindControl("hypCustomerFeedback") as HyperLink);                

                Label lblAllocatedTo = (e.Row.FindControl("lblAllocatedTo") as Label);

                int index = e.Row.RowIndex;

                int SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
                Session["SampleID"] = SampleId;// int.Parse(gvSampleList.DataKeys[index]["SampleID"].ToString());

                DataTable dt5 = _cm.getSampleDepartment(SampleId);
                DataTable dt6 = _cm.GetAllocationDetails(SampleId);
                if (dt5.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in dt5.Rows)
                    {
                        InquiryId = dr2["InquiryId"].ToString();
                        DepartmentID = Convert.ToInt32(dr2["DepartmentID"].ToString());
                    }
                }
                DataTable dt2 = _cm.getInquiryDetails(Convert.ToInt32(InquiryId), 1);
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        CustomerUID = Convert.ToInt32(dr2["CustomerUID"].ToString());
                    }
                }
                DataTable dt4 = _cm.GetAllocationFileDetails(Convert.ToInt32(SampleId));
                if (dt4.Rows.Count > 0)
                {
                    foreach (DataRow dr3 in dt4.Rows)
                    {
                        AllocationID = Convert.ToInt32(dr3["AllocationID"].ToString());
                        sampleUID = Convert.ToInt32(dr3["sampleUID"].ToString());
                        AllocatedTo = dr3["employeeName"].ToString();
                    }
                }
                Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                switch (lblStatusName.Text)
                {
                    case "CRF Received":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        break;
                    case "Sample Accepted By Sample Management":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        break;
                    case "Allocated For Testing":
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        break;
                    case "Sample Rejected By Sample Management":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        break;
                    case "Sample Accepted By Lab":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        break;
                    case "Sample Rejected By Lab":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        break;
                    case "Test Report Submitted for Review":
                        hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypAllocation.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        break;
                    case "Feedback Received":
                        hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypAllocation.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        break;
                    default:
                        break;
                }
            }

        }

        protected void gvSampleList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = int.Parse(e.CommandArgument.ToString());
            int SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
            int InquiryId = int.Parse(gvSampleList.DataKeys[index].Values[1].ToString());
            if (e.CommandName == "Save")
            {
                DataTable dt2 = _cm.GetProductCategory(InquiryId);

                int rowIndex = int.Parse(e.CommandArgument.ToString());
                DropDownList ddlAllocatedTo = (DropDownList)gvSampleList.Rows[rowIndex].Cells[0].FindControl("ddlAllocatedTo");
                if (ddlAllocatedTo.SelectedIndex == 0)
                {
                    ddlAllocatedTo.BackColor = Color.LightPink;
                }
                else
                {
                    ddlAllocatedTo.BackColor = Color.Empty;
                    foreach (DataRow dr in dt2.Rows)
                    {
                        _cm.department = dr["DepartmentID"].ToString();
                        _cm.productCategory = dr["subgroupid"].ToString();
                    }
                    _cm.employeID = ddlAllocatedTo.SelectedItem.Value;
                    _cm.sampleId = SampleId;

                    _cm.AddAllocationDetails(_cm);

                    DataTable dt = _cm.GetCustomerInquiryDetails(SampleId);
                    if (dt.Rows.Count > 0)
                    {
                        //change CloneCRF status to ' '
						_cm.CRFStatusID = 19;
						_cm.CloneCRFSampleID = CloneCRFSampleID;
						_cm.UpdateCRFStatus(_cm);

                        _cm.sampleId = SampleId;
                        _cm.sampleStatusID = 19;
                        _cm.updateSampleStatus(_cm);
                    }
                    BindSampleList();
                }

            }
            if (e.CommandName == "Reject")
            {
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();

                int InquiryID = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());

                //change CloneCRF status to 'Reviewed Rejected'
				_cm.CRFStatusID = 25;
				_cm.CloneCRFSampleID = CloneCRFSampleID;
				_cm.UpdateCRFStatus(_cm);

                _cm.sampleId = SampleId;
                _cm.sampleStatusID = 25;
                _cm.updateSampleStatus(_cm);

                BindSampleList();
            }
            if (e.CommandName == "Accept")
            {
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();

                int InquiryID = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());

                //change CloneCRF status to 'CRF Accepted'
				_cm.CRFStatusID = 24;
				_cm.CloneCRFSampleID = CloneCRFSampleID;
				_cm.UpdateCRFStatus(_cm);

                _cm.sampleId = SampleId;
                _cm.sampleStatusID = 24;
                _cm.updateSampleStatus(_cm);

                BindSampleList();
            }
            if (e.CommandName == "SampleManagement")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int inquiryID = (int)this.gvSampleList.DataKeys[rowIndex]["InquiryID"];
                Session["InquiryID"] = inquiryID;
                Response.Redirect("SampleVerification.aspx");
            }
        }
        protected void btnTestReport_Click(object sender, EventArgs e)
        {
            int count = 0; string SampleIDSeries = "";
            foreach (GridViewRow gvrow in gvSampleList.Rows)
            {
                var chkSample = gvrow.FindControl("chkSample") as CheckBox;
                if (chkSample.Checked)
                {
                    count += 1;
                }
            }
            if (count == 0)
            {
                lblMsg.Text = "Select Sample For Test Report Generation";
            }
            else
            {
                foreach (GridViewRow gvrow in gvSampleList.Rows)
                {
                    var chkSample = gvrow.FindControl("chkSample") as CheckBox;
                    if (chkSample.Checked)
                    {
                        SampleIDSeries = chkSample.Text;
                        Session["SampleIDForTestReport"] += SampleIDSeries + ",";
                    }
                }
                Response.Redirect("GenerateTestReportMechanical.aspx");
            }
        }
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMonth.SelectedIndex == 0)
            {
                ddlMonth.BorderColor = Color.Red;
            }
            else
            {
                BindSampleList();
            }

        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMonth.SelectedIndex == 0)
            {
                ddlMonth.BorderColor = Color.Red;
            }
            else
            {
                BindSampleList();
            }
        }
    }
}