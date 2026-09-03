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
using DocumentFormat.OpenXml.Bibliography;
using ZXing;

namespace LIMS
{
    public partial class SampleList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0; int CloneCRFSampleID = 0;
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

                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());

                }


                if (!Page.IsPostBack)
                {
                    BindYear();

                    if (Session["CloneCRFSampleID"] != null)
                    {
                        CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                    }
                    Session["Id"] = 1;
                    string year = DateTime.Now.Year.ToString();
                    ddlYear.Items.FindByValue(year).Selected = true;
                    string month = DateTime.Now.Month.ToString();
                    ddlMonth.Items.FindByValue(month).Selected = true;
                    if (Request.QueryString["s"] != null)
                    {
                        int InquiryStatus = Convert.ToInt32(Request.QueryString["s"].ToString());

                        BindSampleList(InquiryStatus);
                    }
                    else
                    {
                        BindSampleList();
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

            // Bind the dropdownlist
            ddlYear.DataSource = years;
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, "Select Year");
        }
        private void BindSampleList()
        {
            string DepartmentID = "";
            string Id = Session["Id"].ToString();
            int year = Convert.ToInt32(ddlYear.SelectedValue);
            int month = Convert.ToInt32(ddlMonth.SelectedValue); 
                    {
                DataTable dt = new DataTable();
                DepartmentID = Session["DepartmentID"].ToString();
                switch (DepartmentID)
                {
                    case "2":
                        dt = _cm.GetDepartmentSampleDetails(DepartmentID, year, month);
                        break;
                    case "4":
                        dt = _cm.GetDepartmentSampleDetails(DepartmentID, year, month);
                        break;
                    case "5":
                        dt = _cm.GetDepartmentSampleDetails(DepartmentID, year, month);
                        break;
                    case "3006":
                        dt = _cm.GetDepartmentSampleDetails(DepartmentID, year, month);
                        break;
                    case "4006":
                    case "4007":
                    case "4008":
                        dt = _cm.GetDepartmentSampleDetails(DepartmentID, year, month);
                        break;
                    default:
                        dt = _cm.GetDepartmentSampleDetails(DepartmentID, year, month);
                        break;
                }
                gvSampleList.DataSource = dt;
                gvSampleList.DataBind();
            }
        }
        private void BindSampleList(int InquiryStatus)
        {
            string Id = Session["Id"].ToString();
            DataTable dt = _cm.getAssignedSampleDetails(19);
            gvSampleList.DataSource = dt;
            gvSampleList.DataBind();
        }
        protected void gvSampleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string InquiryId = ""; string ReportLink = "";
                string RequestId = ""; int DepartmentID = 0;
                int AllocationID = 0; int sampleUID = 0;
                int SupervisorID = Convert.ToInt32(Session["EmployeeID"].ToString());
                int CloneCRFSampleID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "CloneCRFSampleID").ToString());

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Button btnSampleManagement = (e.Row.FindControl("btnSampleManagement") as Button);

                    DropDownList ddlAllocatedTo = (e.Row.FindControl("ddlAllocatedTo") as DropDownList);
                    Button btnAllocate = (e.Row.FindControl("btnAllocate") as Button);
                    Button btnMultiple = (e.Row.FindControl("btnMultiple") as Button);
                    Button btnAccept = (e.Row.FindControl("btnAccept") as Button);
                    Button btnReject = (e.Row.FindControl("btnReject") as Button);
                    HyperLink hypReport = (e.Row.FindControl("hypReport") as HyperLink);

                    DataTable dt3 = _cm.GetEmployeeDetails(SupervisorID, true);
                    if (dt3.Rows.Count > 0)
                    {
                        ddlAllocatedTo.DataSource = dt3;
                        ddlAllocatedTo.DataTextField = "EmployeeName";
                        //ddlAllocatedTo.DataValueField = "EmployeeID"; changed since worklist is based on Id
                        ddlAllocatedTo.DataValueField = "ID1";
                        ddlAllocatedTo.DataBind();
                        ddlAllocatedTo.Items.Insert(0, new ListItem("Select Employee"));
                    }
                    HyperLink hypCustCRF = (e.Row.FindControl("hypCustCRF") as HyperLink);
                    HyperLink hypCRF = (e.Row.FindControl("hypCRF") as HyperLink);
                    HyperLink hypResult = (e.Row.FindControl("hypResult") as HyperLink);
                    HyperLink hypCustomerFeedback = (e.Row.FindControl("hypCustomerFeedback") as HyperLink);
                    HyperLink hypAllocation = (e.Row.FindControl("hypAllocation") as HyperLink);
                    HyperLink hypWO = (e.Row.FindControl("hypWO") as HyperLink);

                    Label lblAllocatedTo = (e.Row.FindControl("lblAllocatedTo") as Label);

                    int index = e.Row.RowIndex;

                    int SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
                    sampleUID = int.Parse(gvSampleList.DataKeys[index]["SampleID"].ToString());
                    Session["SampleId"] = sampleUID;

                    int InquiryID = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());
                    DataTable dt7 = _cm.GetAllocationFileDetails(SampleId);
                    DataTable dt4 = _cm.GetCustomerInquiryDetails(InquiryID, 1);
                    DataTable dt6 = _cm.GetAllocationDetails(SampleId);
                    DataTable dt2 = _cm.getInquiryDetails(InquiryID, 1);
                    DataTable dt8 = new DataTable();

                    if (dt2.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            CustomerUID = Convert.ToInt32(dr2["CustomerUID"].ToString());
                        }
                    }
                    //if (dt7.Rows.Count > 0)
                    //{
                    //    foreach (DataRow dr7 in dt7.Rows)
                    //    {
                    //        AllocationID = Convert.ToInt32(dr7["AllocationID"].ToString());
                    //        sampleUID = Convert.ToInt32(dr7["sampleUID"].ToString());
                    //    }
                    //}
                    if (dt4.Rows.Count > 0)
                    {
                        InquiryId = dt4.Rows[0]["InquiryId"].ToString();
                        DepartmentID = Convert.ToInt32(dt4.Rows[0]["DepartmentID"].ToString());
                        DataTable dt5 = _cm.GetCustomerRequestDetails(InquiryId);
                        if (dt5.Rows.Count > 0)
                        {
                            RequestId = dt5.Rows[0]["CustomerRequestId"].ToString();
                            _cm.requestTypeId = Convert.ToInt32(dt5.Rows[0]["RequestTypeId"].ToString());
                            Session["RequestId"] = RequestId;
                            if (_cm.requestTypeId == 1)
                            {
                                hypCRF.NavigateUrl = "~/CreateRequestFM.aspx";
                            }
                            if (_cm.requestTypeId == 2)
                            {
                                hypCRF.NavigateUrl = "~/CreateRequestMocon.aspx";
                            }
                            if (_cm.requestTypeId == 3)
                            {
                                hypCRF.NavigateUrl = "~/CreateRequestPTI.aspx";
                            }
                        }

                        Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                        switch (lblStatusName.Text)
                        {
                            case "CRF Received":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");

                                hypCRF.Visible = true;
                                btnSampleManagement.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                hypResult.Visible = false;
                                hypCustomerFeedback.Visible = false;
                                hypReport.Visible = false;
                                break;
                            case "CRF Finalized":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                btnSampleManagement.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                hypResult.Visible = false;
                                hypCustomerFeedback.Visible = false;
                                hypReport.Visible = false;
                                break;
                            case "Sample Received":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                btnSampleManagement.Visible = true;
                                ddlAllocatedTo.Visible = false;
                                hypResult.Visible = false;
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;
                                break;
                            case "Sample Accepted By Sample Management":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                btnSampleManagement.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                hypResult.Visible = false;
                                btnAccept.Visible = true;
                                btnReject.Visible = true;
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;

                                hypWO.Visible = true;
                                break;
                            case "Allocated For Testing":
                                if (dt6.Rows.Count > 0)
                                {
                                    Session["AllocationId"] = dt6.Rows[0]["AllocationId"].ToString();
                                    lblAllocatedTo.Text = dt6.Rows[0]["EmployeeName"].ToString();
                                }
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                ddlAllocatedTo.Visible = false;
                                hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                                hypAllocation.Visible = true;
                                hypResult.Visible = false;
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;

                                hypWO.Visible = true;
                                break;
                            case "Sample Rejected By Sample Management":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                hypResult.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                btnSampleManagement.Visible = true;
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;
                                break;
                            case "Sample Accepted By Lab":
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                hypResult.Visible = false;
                                if (Session["DepartmentID"].ToString() == "0")
                                {
                                    ddlAllocatedTo.Visible = false;
                                    btnAllocate.Visible = false;
                                    btnMultiple.Visible = false;
                                }
                                else
                                {
                                switch (Convert.ToInt32(Session["RoleID"].ToString()))
                                {
                                    case 1:
                                        ddlAllocatedTo.Visible = true;
                                        // btnAllocate.Visible = true;
                                        btnMultiple.Visible = true;
                                        break;
                                    case 2:
                                        ddlAllocatedTo.Visible = true;
                                        // btnAllocate.Visible = true;
                                        btnMultiple.Visible = true;
                                        break;
                                    case 3:
                                        ddlAllocatedTo.Visible = false;
                                        btnAllocate.Visible = false;
                                        btnMultiple.Visible = false;
                                        break;
                                    case 4:
                                        ddlAllocatedTo.Visible = false;
                                        btnAllocate.Visible = false;
                                        btnMultiple.Visible = false;
                                        break;
                                    case 5:
                                        ddlAllocatedTo.Visible = false;
                                        btnAllocate.Visible = false;
                                        btnMultiple.Visible = false;
                                        break;
                                    default:
                                        ddlAllocatedTo.Visible = false;
                                        btnAllocate.Visible = false;
                                        btnMultiple.Visible = false;
                                        break;
                                }

                                }
                                btnSampleManagement.Visible = false;
                                btnAccept.Visible = false;
                                btnReject.Visible = false;
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;

                                hypWO.Visible = true;
                                break;
                            case "Sample Rejected By Lab":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                hypResult.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                btnSampleManagement.Visible = false;
                                btnAccept.Visible = false;
                                btnReject.Visible = false;
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;
                                break;
                            case "Test Result Submitted":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                hypResult.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                btnSampleManagement.Visible = false;
                                btnAccept.Visible = false;
                                btnReject.Visible = false;
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;

                                hypWO.Visible = true;
                                break;
                            case "Test Report Generated":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                hypResult.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                btnSampleManagement.Visible = false;
                                btnAccept.Visible = false;
                                btnReject.Visible = false;
                                hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                                hypAllocation.Visible = true;

                                hypWO.Visible = true;
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;
                                break;
                            case "Ready to Print":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                hypResult.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                btnSampleManagement.Visible = false;
                                btnAccept.Visible = false;
                                btnReject.Visible = false;
                                hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                                hypAllocation.Visible = true;

                                hypWO.Visible = true;
                                switch (DepartmentID)
                                {
                                    case 1:
                                        ReportLink = "GenerateTestReportMechanical.aspx";
                                        break;
                                    case 2:
                                        ReportLink = "GenerateTestReportPTI.aspx";
                                        break;
                                    case 3:
                                        ReportLink = "GenerateTestReportMicro.aspx";
                                        break;
                                    case 4:
                                        ReportLink = "GenerateTestReportChemical.aspx";
                                        break;
                                    case 4006:
                                        ReportLink = "GenerateTestAllocation_PV.aspx";
                                        break;
                                    case 4007:
                                        ReportLink = "GenerateTestAllocation_mechanical.aspx";
                                        break;
                                    case 4009:
                                        ReportLink = "GenerateTestAllocation_LED.aspx";
                                        break;
                                    case 3006:
                                        ReportLink = "GenerateTestAllocation_ElectricalAccessories.aspx";
                                        break;
                                    default: break;
                                }
                                dt8 = _cm.getTestReportID(SampleId);
                                if (dt8.Rows.Count > 0)
                                {
                                    int TestResultId = Convert.ToInt32(dt8.Rows[0]["TestResultId"].ToString());
                                    if (Convert.ToInt32(Session["RoleID"].ToString()) == 5)
                                    {
                                        hypReport.NavigateUrl = string.Format("~/" + ReportLink + "?ID={0}&TestResultID={1}", SampleId, TestResultId);
                                        hypReport.Visible = true;
                                    }
                                    else
                                    {
                                        hypReport.Visible = false;
                                    }
                                    hypResult.Visible = false;
                                }
                                hypCustomerFeedback.Visible = false;
                                break;
                            case "Test Report Submitted for Review":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                hypResult.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                btnSampleManagement.Visible = false;
                                btnAccept.Visible = false;
                                btnReject.Visible = false;
                                hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                                hypAllocation.Visible = true;
                                dt8 = _cm.getTestReportID(SampleId);
                                if (dt8.Rows.Count > 0)
                                {
                                    int TestResultId = Convert.ToInt32(dt8.Rows[0]["TestResultId"].ToString());
                                    hypResult.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                                    hypResult.Visible = true;
                                }
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;

                                hypWO.Visible = true;
                                break;
                            case "Reviewed Accepted":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                hypResult.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                btnSampleManagement.Visible = false;
                                btnAccept.Visible = false;
                                btnReject.Visible = false;
                                hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                                hypAllocation.Visible = true;
                                dt8 = _cm.getTestReportID(SampleId);
                                if (dt8.Rows.Count > 0)
                                {
                                    int TestResultId = Convert.ToInt32(dt8.Rows[0]["TestResultId"].ToString());
                                    hypResult.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                                    hypResult.Visible = true;
                                }
                                hypReport.Visible = false;
                                hypCustomerFeedback.Visible = false;

                                hypWO.Visible = true;
                                break;
                            case "Feedback Received":
                                btnAllocate.Visible = false;
                                btnMultiple.Visible = false;
                                hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                                hypCustCRF.Visible = true;
                                hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                                hypCRF.Visible = true;
                                hypResult.Visible = false;
                                ddlAllocatedTo.Visible = false;
                                btnSampleManagement.Visible = false;
                                btnAccept.Visible = false;
                                btnReject.Visible = false;
                                hypReport.Visible = false;
                                hypCustomerFeedback.NavigateUrl = string.Format("CustomerFeedbackStatus.aspx?ID=" + InquiryID);
                                hypCustomerFeedback.Visible = true;

                                hypWO.Visible = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        protected void gvSampleList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = int.Parse(e.CommandArgument.ToString());
            int InquiryID = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());
            int CloneCRFSampleID = int.Parse(gvSampleList.DataKeys[index]["CloneCRFSampleID"].ToString());
            int SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
            SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
            int sampleUID = int.Parse(gvSampleList.DataKeys[index]["SampleID"].ToString());
            int InquiryId = int.Parse(gvSampleList.DataKeys[index].Values[1].ToString());

            GridView grid = (GridView)sender;
            GridViewRow row1 = grid.Rows[index];
            System.Web.UI.WebControls.Image imgageQRCode = (System.Web.UI.WebControls.Image)row1.FindControl("imgageQRCode");

            Session["CloneCRFSampleID"] = CloneCRFSampleID;

            if (e.CommandName == "Select")
            {
                GridViewRow row = gvSampleList.Rows[index];
                SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
                string SampleIdSeries = "";
                DataTable dt5 = _cm.getSampleIDSeries(SampleId);
                if (dt5.Rows.Count > 0)
                {
                    SampleIdSeries = dt5.Rows[0]["sampleIDseries"].ToString();
                }
                int rowIndex = Convert.ToInt32(e.CommandArgument);


                var QCwriter = new BarcodeWriter();
                QCwriter.Format = BarcodeFormat.QR_CODE;
                var result = QCwriter.Write(SampleIdSeries);
                string path = Server.MapPath("~/images/MyQRImage.jpg");
                var barcodeBitmap = new Bitmap(result);

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(path,
                       FileMode.Create, FileAccess.ReadWrite))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
                imgageQRCode.Visible = true;
                imgageQRCode.ImageUrl = "~/images/MyQRImage.jpg";
            }
            if (e.CommandName == "Multiple")
            {
                Session["SampleUID"] = sampleUID;
                DataTable dt2 = _cm.GetProductCategory(InquiryId);

                if (dt2.Rows.Count > 0)
                {
                    string departmentID = dt2.Rows[0]["DepartmentID"].ToString();

                    _cm.department = departmentID;


                    if (departmentID == "4007")
                    {
                        Response.Redirect("AllocateMultiple_mechanical.aspx?ID=" + SampleId);
                    }
                    else if (departmentID == "4009")
                    {
                        Response.Redirect("AllocateMultiple_LED.aspx?ID=" + SampleId);
                    }
                    else if (departmentID == "4006")
                    {
                        Response.Redirect("AllocateMultiple_PV.aspx?ID=" + SampleId);
                    }
                    else if (departmentID == "4008")
                    {
                        Response.Redirect("AllocateMultiple_HomeAppliances.aspx?ID=" + SampleId);
                    }
                    else if (departmentID == "3006")
                    {
                        Response.Redirect("AllocateMultiple_ElectricalAccessories.aspx?ID=" + SampleId);
                    }
                    else if (departmentID == "1")
                    {
                        Response.Redirect("AllocateMultiple_Transport.aspx?ID=" + SampleId);
                    }
                    else if (departmentID == "3")
                    {
                        Response.Redirect("AllocateMultiple_ToyAndBabyWear.aspx?ID=" + SampleId);
                    }
					else if (departmentID == "5020")
	                {
	                    Response.Redirect("AllocateMultiple_PhysicalSoftline.aspx?ID=" + SampleId);
	                }
                    else
                    {
                        Response.Redirect("AllocateMultiple.aspx?ID=" + SampleId);
                    }
                }
                else
                {
                    if (Session["DepartmentID"] != null)
                    {
                        if (Session["DepartmentID"].ToString() == "4007")
                        {
                            Response.Redirect("AllocateMultiple_mechanical.aspx?ID=" + SampleId);
                        }

                    }
                }
            }
            if (e.CommandName == "Save")
            {
                Session["SampleId"] = sampleUID;
                DataTable dt2 = _cm.GetProductCategory(InquiryId);
                int AllocationID = 0;

                int rowIndex = int.Parse(e.CommandArgument.ToString());
                DropDownList ddlAllocatedTo = (DropDownList)gvSampleList.Rows[rowIndex].Cells[0].FindControl("ddlAllocatedTo");
                HyperLink hypAllocation = (HyperLink)gvSampleList.Rows[rowIndex].Cells[0].FindControl("hypAllocation");
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
                        _cm.employeID = ddlAllocatedTo.SelectedItem.Value;
                        _cm.sampleId = SampleId;

                        if (_cm.department == "3006")
                        {
                            _cm.allocationID = AllocationID;
                            _cm.UpdateAllocation_mechanical(_cm);
                        }
                    }
                    AllocationID = _cm.AddAllocationDetails(_cm);
                    Session["AllocationId"] = AllocationID;

                    DataTable dt = _cm.GetCustomerInquiryDetails(SampleId);
                    if (dt.Rows.Count > 0)
                    {
                        //change CloneCRF status to 'Allocated for Testing'
                        _cm.CRFStatusID = 19;
                        _cm.CloneCRFSampleID = CloneCRFSampleID;
                        _cm.UpdateCRFStatus(_cm);

                        _cm.sampleId = SampleId;
                        _cm.sampleStatusID = 19;
                        _cm.updateSampleStatus(_cm);
                    }
                    DataTable dtCRFType = _cm.getCRFTypeDetails(InquiryId);
                    if (dtCRFType.Rows.Count > 0)
                    {
                        int CRFID = Convert.ToInt32(dtCRFType.Rows[0]["CRFID"].ToString());
                        switch (CRFID)
                        {
                            case 1:
                                Response.Redirect("TestAllocationFM.aspx");
                                break;
                            case 2:
                                Response.Redirect("TestAllocationMocon.aspx");
                                break;
                            case 3:
                                Response.Redirect("TestAllocationPTI.aspx");
                                break;
                            default: break;
                        }
                    }
                    //Response.Redirect("~/GenerateTestAllocation.aspx");
                    string url = "GenerateTestAllocation.aspx";
                    Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                    hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + CloneCRFSampleID + AllocationID + sampleUID + ".htm");
                    hypAllocation.Visible = true;
                    BindSampleList();
                }

            }
            if (e.CommandName == "Reject")
            {
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();

                //change CloneCRF status to 'Sample Rejected by Lab'
                _cm.CRFStatusID = 17;
                _cm.CloneCRFSampleID = CloneCRFSampleID;
                _cm.UpdateCRFStatus(_cm);

                _cm.sampleId = SampleId;
                _cm.sampleStatusID = 17;// Sample Rejected By Lab
                _cm.updateSampleStatus(_cm);

                BindSampleList();
            }
            if (e.CommandName == "Accept")
            {
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();

                //change CloneCRF status to 'Sample Accepted by Lab'
                _cm.CRFStatusID = 15;
                _cm.CloneCRFSampleID = CloneCRFSampleID;
                _cm.UpdateCRFStatus(_cm);

                _cm.sampleId = SampleId;
                _cm.sampleStatusID = 15;// Sample Accepted By Lab
                _cm.updateSampleStatus(_cm);

                //Change Quotation status to 'Sample Accepted By Lab'
                _cm.QuotationStatusID = 1014;//Sample Accepted By Lab
                _cm.UpdateQuotationStatus(_cm);

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