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
    public partial class ReviewReport : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0; int inquiryID = 0; int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            //HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
            //HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
            //HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
            //HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
            //HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            //HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");

            //HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
            //HtmlControl liGenerateReport2 = (HtmlGenericControl)this.Master.FindControl("liGenerateReport2");
            //HtmlControl liReviewReport2 = (HtmlGenericControl)this.Master.FindControl("liReviewReport2");

            //HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            //HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            //HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");

            //HtmlControl liWorkList2 = (HtmlGenericControl)this.Master.FindControl("liWorkList2");
            //HtmlControl liWorkListMocon2 = (HtmlGenericControl)this.Master.FindControl("liWorkListMocon2");
            //HtmlControl liViewSamples2 = (HtmlGenericControl)this.Master.FindControl("liViewSamples2");
            //HtmlControl liAllSamples2 = (HtmlGenericControl)this.Master.FindControl("liAllSamples2");
            //HtmlControl liViewSamplesMocon2 = (HtmlGenericControl)this.Master.FindControl("liViewSamplesMocon2");
            //HtmlControl liGenerateReportMocon2 = (HtmlGenericControl)this.Master.FindControl("liGenerateReportMocon2");
            //HtmlControl liWorkListCompleted2 = (HtmlGenericControl)this.Master.FindControl("liWorkListCompleted2");

            //HtmlControl liQRAnalyzer = (HtmlGenericControl)this.Master.FindControl("liQRAnalyzer");
            //HtmlControl liReportManagement1 = (HtmlGenericControl)this.Master.FindControl("liReportManagement1");
            //HtmlControl liGroupManagement = (HtmlGenericControl)this.Master.FindControl("liGroupManagement");
            
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
                    //        if (Session["DepartmentID"].ToString() == "1")
                    //        {
                    //            liViewSamples2.Visible = false;
                    //            liWorkList2.Visible = false;
                    //            liGenerateReport2.Visible = false;

                    //            liViewSamplesMocon2.Visible = true;
                    //            liWorkListMocon2.Visible = true;
                    //            liGenerateReportMocon2.Visible = true;
                    //        }
                    //        else
                    //        {
                    //            liViewSamples2.Visible = true;
                    //            liWorkList2.Visible = true;
                    //            liGenerateReport2.Visible = true;

                    //            liViewSamplesMocon2.Visible = false;
                    //            liWorkListMocon2.Visible = false;
                    //            liGenerateReportMocon2.Visible = false;
                    //        }
                    //        liTestManagement1.Visible = false;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        liReportManagement1.Visible = false;
                    //        liQRAnalyzer.Visible = false;
                    //        liGroupManagement.Visible = false;
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
                    //        liTestManagement1.Visible = true;
                    //        liReportManagement1.Visible = false;
                    //        liGroupManagement.Visible = false;
                    //        liQRAnalyzer.Visible = false;
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
                    //        liViewSamplesMocon2.Visible = true;
                    //        liWorkListMocon2.Visible = true;
                    //        liQualityManagement1.Visible = true;
                    //        liGenerateReport2.Visible = true;
                    //        liGenerateReportMocon2.Visible = false;
                    //        liReviewReport2.Visible = false;
                    //        liTestManagement1.Visible = false;
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
                    BindYear();
                    //liQualityManagement1.Attributes.Add("class", "active");
                    //liReviewReport2.Attributes.Add("class", "active");
                    
                    if (Session["CloneCRFSampleID"] != null)
                    {
                        CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                    }
                    string year = DateTime.Now.Year.ToString();
                    ddlYear.Items.FindByValue(year).Selected = true;
                    string month = DateTime.Now.Month.ToString();
                    ddlMonth.Items.FindByValue(month).Selected = true;
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
            if (Session["DepartmentID"] != null)
            {
                string DepartmentID = Session["DepartmentID"].ToString();
                switch (DepartmentID)
                {
                    case "2":
                        DepartmentID = DepartmentID + ",4";
                        break;
                    case "4":
                        DepartmentID = DepartmentID + ",2";
                        break;
                    default:
                        break;
                }
                string SampleStatus = "12,24,25,26,29";
                if ((ddlYear.SelectedIndex != 0)&& (ddlMonth.SelectedIndex != 0))
                {
                    int year = Convert.ToInt32(ddlYear.SelectedValue);
                    int month = Convert.ToInt32(ddlMonth.SelectedValue);
                    DataTable dt = _cm.getSampleDetailsForReportReview(DepartmentID,SampleStatus, year,month);
                    gvSampleList.DataSource = dt;
                    gvSampleList.DataBind();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void gvSampleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {            
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dt3 = _cm.GetEmployeeDetails();

                
                HyperLink hypReport = (e.Row.FindControl("hypReport") as HyperLink);
                HyperLink hypTestResultsNN = (e.Row.FindControl("hypTestResultsNN") as HyperLink);

                Button btnAccept = (e.Row.FindControl("btnAccept") as Button);
                //Button btnReject = (e.Row.FindControl("btnReject") as Button);

                int index = e.Row.RowIndex;

                int TestResultID = int.Parse(gvSampleList.DataKeys[index]["TestResultID"].ToString());
                int sampleUID = int.Parse(gvSampleList.DataKeys[index]["sampleID"].ToString());

                bool nabl1 = false;bool nonnabl1 = false;
                int nabl = 0; int nonnabl = 0;
                DataTable dtReport = _cm.GetReportType(TestResultID);
                if (dtReport.Rows.Count > 0)
                {
                    if (dtReport.Rows[0]["ReportType"].ToString() == "NABL_Cert_PDP")
                    {
                        DataTable dtNABL = _cm.NABLOrNonNABL(dtReport.Rows[0]["ID"].ToString());
                        if (dtNABL.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtNABL.Rows)
                            {
                                if (row["NABLorNonNABL"].ToString() == "Non NABL")
                                {
                                    nonnabl++;
                                    
                                }
                                if (row["NABLorNonNABL"].ToString() == "NABL")
                                {
                                    nabl++;
                                }
                            }
                            if (nabl > 0) { nabl1 = true; } else { nabl1 = false; }
                            if (nonnabl > 0) { nonnabl1 = true; } else { nonnabl1 = false; }
                        }
                    }
                    else
                    {
                        nabl1 = true;
                        nonnabl1 = false;
                    }
                    
                }
                Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                switch (lblStatusName.Text)
                {
                    case "Test Report Submitted for Review":
                        btnAccept.Visible = true;
                        //btnReject.Visible = true;
                        hypReport.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultID + sampleUID + ".htm");
                        hypReport.Visible = nabl1;

                        hypTestResultsNN.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultID + sampleUID + "_NN.htm");
                        hypTestResultsNN.Visible = nonnabl1;
                        break;
                    case "Reviewed Accepted":
                    case "Reviewed Rejected":
                    case "Test Report Generated":
                        hypReport.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultID + sampleUID + ".htm");
                        hypReport.Visible = nabl1;

                        hypTestResultsNN.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultID + sampleUID + "_NN.htm");
                        hypTestResultsNN.Visible = nonnabl1;
                        break;                   
                    default:
                        hypReport.Visible = false;
                        break;
                }
            }

        }
        protected void gvSampleList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = int.Parse(e.CommandArgument.ToString());
            int SampleId = int.Parse(gvSampleList.DataKeys[index]["sampleID"].ToString());
            int EnquiryID = int.Parse(gvSampleList.DataKeys[index]["EnquiryID"].ToString());
            int ID = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
            CloneCRFSampleID = int.Parse(gvSampleList.DataKeys[index]["CloneCRFSampleID"].ToString());
            int InquiryId = int.Parse(gvSampleList.DataKeys[index].Values[1].ToString());
            int TestResultID = int.Parse(gvSampleList.DataKeys[index]["TestResultID"].ToString());
            DataTable dt = _cm.GetCustomerInquiryDetails(SampleId);
            if (dt.Rows.Count > 0)
            {
                inquiryID = Convert.ToInt32(dt.Rows[0]["InquiryId"]);
            }
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
                    
					//change CloneCRF status to 'Allocated For Testing'
					_cm.CRFStatusID = 19;
					_cm.CloneCRFSampleID = CloneCRFSampleID;
					_cm.UpdateCRFStatus(_cm);

                    _cm.sampleId = ID;
                    _cm.sampleStatusID = 19;
                    _cm.updateSampleStatus(_cm);
                    BindSampleList();
                }

            }
            if (e.CommandName == "Reject")
            {
                DataTable dt2 = new DataTable();

                //change CloneCRF status to 'Reviewed Rejected'
				_cm.CRFStatusID = 25;
				_cm.CloneCRFSampleID = CloneCRFSampleID;
				_cm.UpdateCRFStatus(_cm);

                _cm.sampleId = Convert.ToInt32(ID);
                _cm.sampleStatusID = 25;
                _cm.updateSampleStatus(_cm);

                BindSampleList();
            }
            if (e.CommandName == "Accept")
            {
                if (Session["EmployeeID"] != null)
                {
                    string EmployeeID = Session["EmployeeID"].ToString();
                    DataTable dtSignatory = _cm.getSignatoryDepartment(EmployeeID);
                    if (dtSignatory.Rows.Count > 0)
                    {
                        Session["InquiryID"] = EnquiryID;
                        Session["CloneCRFSampleID"] = CloneCRFSampleID;

                        int DepartmentID = 0;
                        if (Session["DepartmentID"] != null)
                        {
                            DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());
                        }
                        DataTable dt7 = new DataTable();
                        DataTable dt6 = _cm.GetAllocationDetails(ID);
                       
                        switch (DepartmentID)
                        {  
                            case 1:
                                break;
                            case 2:
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('TestReportPTI.aspx?s=" + _cm.sampleId + "&c=" + CloneCRFSampleID + "&t=" + TestResultID + "' ,'_blank');", true);
                                break;
                            case 3:
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('TestReportMicro.aspx?s=" + _cm.sampleId + "&c=" + CloneCRFSampleID + "&t=" + TestResultID + "' ,'_blank');", true);
                                break;
                            case 4:
                                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('TestReportChemical.aspx?s=" + _cm.sampleId + "&c=" + CloneCRFSampleID + "&t=" + TestResultID + "' ,'_blank');", true);
                                Response.Redirect("TestReportReview.aspx");
                                DataTable dt8 = _cm.NABLOrNonNABL(_cm.sampleId.ToString());
                                if (dt8.Rows.Count == 2)
                                {
                                    for (int i = 1; i <= dt8.Rows.Count; i++)
                                    {
                                        string url = "TestReportChemical.aspx?s=" + _cm.sampleId + "&t=" + TestResultID + "&t1=" + i;
                                        //  Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('" + url + "' ,'_blank');", true);

                                    }
                                }
                                else
                                {
                                    if (dt8.Rows[0]["NABLOrNonNABL"].ToString() == "NABL")
                                    {
                                        string url = "TestReportChemical.aspx?s=" + _cm.sampleId + "&t=" + TestResultID + "&t1=1";
                                        // Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('"+url+"' ,'_blank');", true);
                                    }
                                    else if (dt8.Rows[0]["NABLOrNonNABL"].ToString() == "Non NABL")
                                    {
                                        string url = "TestReportChemical.aspx?s=" + _cm.sampleId + "&t=" + TestResultID + "&t1=2";
                                        //  Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('" + url + "' ,'_blank');", true);

                                    }

                                }
                                break;
                            default:break;
                        }
                        GC.Collect();
                        BindSampleList();
                    }
                }
                
            }
            if (e.CommandName == "SampleManagement")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int inquiryID = (int)this.gvSampleList.DataKeys[rowIndex]["InquiryID"];
                Session["InquiryID"] = inquiryID;
                Response.Redirect("SampleVerification.aspx");
            }
            if (e.CommandName == "EditCRF")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('EditCRFDetails.aspx?E="+ EnquiryID + "' ,'_blank');", true);
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

        protected void lnkEditCRF_Click(object sender, EventArgs e)
        {
           
        }

        protected void btnEditCRF_Click(object sender, EventArgs e)
        {
           
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