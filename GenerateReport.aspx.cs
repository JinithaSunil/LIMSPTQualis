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
    public partial class GenerateReport : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

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
                if (!Page.IsPostBack)
                {
                    BindYear();
                    string year = DateTime.Now.Year.ToString();
                    ddlYear.Items.FindByValue(year).Selected = true;
                    string month = DateTime.Now.Month.ToString();
                    ddlMonth.Items.FindByValue(month).Selected = true;
                    BindSampleList();
                }
                if (Request.QueryString["ID"] != null || Request.QueryString["TestResultID"] != null)
                {
                    if (Request.QueryString["ID"] != null)
                    {
                        string SampleId = Request.QueryString["ID"].ToString();
                        string TestResultId = Request.QueryString["TestResultID"].ToString(); string code = "";

                        DataTable dt = _cm.NABLOrNonNABL(SampleId);
                        if (dt.Rows.Count == 2)
                        {
                            for (int i = 1; i <= dt.Rows.Count; i++)
                            {
                                string url = "TestReportChemical.aspx?s=" + SampleId + "&t=" + TestResultId + "&t1=" + i;
                                Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                            }
                        }
                        else
                        {
                            if (dt.Rows[0]["NABLOrNonNABL"].ToString() == "NABL")
                            {
                                string url = "TestReportChemical.aspx?s=" + SampleId + "&t=" + TestResultId + "&t1=1";
                                Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                            }
                            else if (dt.Rows[0]["NABLOrNonNABL"].ToString() == "Non NABL")
                            {
                                string url = "TestReportChemical.aspx?s=" + SampleId + "&t=" + TestResultId + "&t1=2";
                                Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                            }
                        }
                    }
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
            ddlYear.DataSource = years;
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, "Select Year");
        }
        private void BindSampleList()
        {
            string SampleStatus = "";
            string SampleStatus2 = "29";
            string DepartmentID = "";
            string RoleID = Session["RoleID"].ToString();
            if(RoleID == "2") { SampleStatus = "27"; }
            if (RoleID == "5") { SampleStatus = "29"; }
            DataTable dt = new DataTable();

            DepartmentID = Session["DepartmentID"].ToString();
            if ((ddlYear.SelectedIndex != 0) && (ddlMonth.SelectedIndex != 0))
            {
                int year = Convert.ToInt32(ddlYear.SelectedValue);
                int month = Convert.ToInt32(ddlMonth.SelectedValue);
                switch (DepartmentID)
                {
                    case "1":
                        dt = _cm.getSampleDetailsForReportGeneration_transport(DepartmentID, SampleStatus, year, month);
                        break;
                    case "2":
                        DepartmentID = DepartmentID + ",4";
                        dt = _cm.getSampleDetailsForReportGeneration(DepartmentID, SampleStatus, year, month);
                        break;
                    case "3":
                        dt = _cm.getSampleDetailsForReportGeneration(DepartmentID, SampleStatus, year, month);
                        break;
                    case "4":
                        DepartmentID = DepartmentID + ",2";
                        dt = _cm.getSampleDetailsForReportGeneration(DepartmentID, SampleStatus, year, month);
                        break;
                    case "4009":
                        dt = _cm.getSampleDetailsForReportGeneration_LED(DepartmentID, SampleStatus, year, month);
                        break;
                    case "3006":
                        dt = _cm.getSampleDetailsForReportGeneration_LED(DepartmentID, SampleStatus, year, month);
                        break;
                    case "4006":
                        DepartmentID = DepartmentID + ",4006";
                        //string SampleStatusForPV = "27";
                        dt = _cm.getSampleDetailsForReportGenerationfordepartment_Pv(DepartmentID, SampleStatus, year, month);
                        break;
                    case "5020":
                        dt = _cm.getSampleDetailsForReportGeneration_Flatware(DepartmentID, SampleStatus, year, month);
                        break;

                    default:
                        dt = _cm.getSampleDetailsForReportGeneration(DepartmentID, SampleStatus, year, month);
                        break;
                }
            }

            gvSampleList.DataSource = dt;
            gvSampleList.DataBind();
        }

        protected void gvSampleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string InquiryId = ""; string ReportLink = "";
            int sampleUID = 0; int DepartmentID = 0;
            int reportId = 0; // Variable to store reportId
            DataTable dt8 = new DataTable();
            int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string CloneCRFSampleID = gvSampleList.DataKeys[e.Row.RowIndex].Values[1].ToString();
                DataTable dt3 = _cm.GetEmployeeDetails();

                HyperLink hypCRF = (e.Row.FindControl("hypCRF") as HyperLink);
                HyperLink hypAllocation = (e.Row.FindControl("hypAllocation") as HyperLink);
                HyperLink hypCustomerFeedback = (e.Row.FindControl("hypCustomerFeedback") as HyperLink);
                HyperLink hypReport = (e.Row.FindControl("hypReport") as HyperLink);
                HyperLink hypTestReport = (e.Row.FindControl("hypTestReport") as HyperLink);

                Label lblAllocatedTo = (e.Row.FindControl("lblAllocatedTo") as Label);
                Label lblReportName = (e.Row.FindControl("lblReportName") as Label);

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
                        // Fetch reportId from database if column exists
                        if (dr2.Table.Columns.Contains("reportid") && dr2["reportid"] != DBNull.Value)
                        {
                            reportId = Convert.ToInt32(dr2["reportid"].ToString());
                        }
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
                        sampleUID = Convert.ToInt32(dr3["sampleUID"].ToString());
                    }
                }
                Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                switch (lblStatusName.Text)
                {
                    case "CRF Received":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypTestReport.Visible = false;
                        break;
                    case "Sample Accepted By Sample Management":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypTestReport.Visible = false;
                        break;
                    case "Allocated For Testing":
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypTestReport.Visible = false;
                        break;
                    case "Sample Rejected By Sample Management":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypTestReport.Visible = false;
                        break;
                    case "Sample Accepted By Lab":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypTestReport.Visible = false;
                        break;
                    case "Sample Rejected By Lab":
                        hypAllocation.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypTestReport.Visible = false;
                        break;
                    case "Test Result Submitted":
                        hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypAllocation.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        switch (DepartmentID)
                        {
                            case 1:
                                ReportLink = "GenerateTestReport_Transport.aspx";
                                break;

                            case 4007:
                                ReportLink = "GenerateTestReportMechanical.aspx";
                                break;
                            case 2:
                                ReportLink = "GenerateTestReportPTI.aspx";
                                break;
                            case 3:
                                ReportLink = "GenerateTestReport_ToyAndBabyWear.aspx";
                                break;

                            case 5:
                                ReportLink = "GenerateTestReportMicro.aspx";
                                break;
                            case 4:
                                switch (lblReportName.Text)
                                {
                                    case "Truck_Cert": ReportLink = "GenerateTestReportTruckCert.aspx"; break;
                                    default: ReportLink = "GenerateTestReportChemical.aspx"; break;
                                }
                                break;

                            case 4009:
                                ReportLink = "GenerateTestReportLED.aspx";
                                break;

                            case 3006:
                                ReportLink = "GenerateTestReportEA.aspx";
                                break;

                            case 5020:
                                ReportLink = "GenerateTestReport_PhysicalSoftline.aspx";
                                break;
                            case 4006:
                                ReportLink = "GenerateTestReportPV.aspx";
                                break;
                            default: break;
                        }

                        hypReport.NavigateUrl = string.Format("~/" + ReportLink + "?ID={0}", SampleId);
                        hypReport.Visible = true;
                        hypTestReport.Visible = false;
                        break;
                    case "Ready to Print":
                        hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypAllocation.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypTestReport.Visible = false;
                        if (RoleID == 5)
                        {
                            switch (DepartmentID)
                            {
                                case 1:
                                    ReportLink = "GenerateTestReport_Transport.aspx";
                                    break;

                                case 4007:
                                    ReportLink = "GenerateTestReportMechanical.aspx";
                                    break;

                                case 2:
                                    ReportLink = "GenerateTestReportPTI.aspx";
                                    break;
                                case 3:
                                    ReportLink = "GenerateTestReport_ToyAndBabyWear.aspx";
                                    break;

                                case 4006:
                                    ReportLink = "GenerateTestReportPV.aspx";
                                    break;
                                case 5:
                                    ReportLink = "GenerateTestReportMicro.aspx";
                                    break;
                                case 4:
                                    switch (lblReportName.Text)
                                    {
                                        case "Truck_Cert": ReportLink = "GenerateTestReportTruckCert.aspx"; break;
                                        default: ReportLink = "GenerateTestReportChemical.aspx"; break;
                                    }
                                    break;
                                case 4009:
                                    ReportLink = "GenerateTestReportLED.aspx";
                                    break;
                                case 3006:
                                    ReportLink = "GenerateTestReportEA.aspx";
                                    break;

                                case 5020:
                                    ReportLink = "GenerateTestReport_PhysicalSoftline.aspx";
                                    break;

                                default: break;
                            }
                            dt8 = _cm.getTestReportID(SampleId);
                            if (dt8.Rows.Count > 0)
                            {
                                int TestResultId = Convert.ToInt32(dt8.Rows[0]["TestResultId"].ToString());
                                hypReport.NavigateUrl = string.Format("~/" + ReportLink + "?ID={0}&TestResultID={1}", SampleId, TestResultId);
                                hypReport.Visible = true;
                            }
                        }
                        else
                        {
                            hypReport.Visible = false;
                        }
                        break;
                    case "Test Report Generated":
                        hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypAllocation.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypReport.Visible = false;
                        dt8 = _cm.getTestReportID(SampleId);
                        if (dt8.Rows.Count > 0)
                        {
                            int TestResultId = Convert.ToInt32(dt8.Rows[0]["TestResultId"].ToString());
                            hypTestReport.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                            hypTestReport.Visible = true;
                        }
                        break;
                    case "Test Report Submitted for Review":
                        hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypAllocation.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypReport.Visible = false;
                        dt8 = _cm.getTestReportID(SampleId);
                        if (dt8.Rows.Count > 0)
                        {
                            int TestResultId = Convert.ToInt32(dt8.Rows[0]["TestResultId"].ToString());
                            hypTestReport.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                            hypTestReport.Visible = true;
                        }
                        break;
                    case "Feedback Received":
                        hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypAllocation.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        dt8 = _cm.getTestReportID(SampleId);
                        if (dt8.Rows.Count > 0)
                        {
                            int TestResultId = Convert.ToInt32(dt8.Rows[0]["TestResultId"].ToString());
                            hypTestReport.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                            hypTestReport.Visible = true;
                        }
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
                        //change CloneCRF status to 'Test Report Submitted for Review '
                        int CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                        _cm.CRFStatusID = 12;
                        _cm.CloneCRFSampleID = CloneCRFSampleID;
                        _cm.UpdateCRFStatus(_cm);

                        _cm.sampleId = SampleId;
                        _cm.sampleStatusID = 12;
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
                int CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
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

                //change CloneCRF status to 'Reviewed Accepted'
                int CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
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