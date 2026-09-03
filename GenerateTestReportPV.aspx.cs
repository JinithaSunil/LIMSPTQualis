using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text.pdf;
using WordInterop = Microsoft.Office.Interop.Word;


namespace LIMS
{
    public partial class GenerateTestReportPV : System.Web.UI.Page
    {

        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        string ID = ""; DataTable dt = new DataTable();
        DAL.CM _cm = new DAL.CM(); string Enquiryid = ""; string ReportID = "";
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
                    if (Session["RoleID"] != null)
                    {
                        int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                        switch (RoleID)
                        {
                            case 2: //Manager
                                divSignatory.Visible = false;
                                divNABL.Visible = false;
                                txtUlrNo.Visible = false;
                                txtIssueDate.Visible = false;
                                break;
                            case 5://Sample Management
                                divSignatory.Visible = true;
                                divNABL.Visible = true;
                                txtUlrNo.Visible = true;
                                txtIssueDate.Visible = true;
                                gvResults.Columns[4].Visible = false;
                                break;
                            default: break;
                        }


                    }
                    string ID = ""; DataTable dt = new DataTable(); string SampleId = ""; string TestResultID = "";
                    if (Request.QueryString["ID"] != null)
                    {
                        if (Request.QueryString["ID"] != null)
                        {
                            ID = Request.QueryString["ID"].ToString();
                            Session["ID"] = ID;
                            ViewState["SampleID"] = ID;

                            dt = _cm.getSampleDetails(Convert.ToInt32(ID));
                            if (dt.Rows.Count > 0)
                            {
                                SampleId = dt.Rows[0]["SampleID"].ToString();
                                Enquiryid = dt.Rows[0]["Enquiryid"].ToString();
                                DataTable dt4 = _cm.GetReportID(Enquiryid);
                                if (dt4.Rows.Count > 0)
                                {
                                    ReportID = dt4.Rows[0]["ReportID"].ToString();
                                    hdReportID.Value = ReportID;
                                }

                                int sampleIDGet = Convert.ToInt32(dt.Rows[0]["ID"].ToString());

                                int checkResult = CheckSampleCounts(sampleIDGet);

                                if (checkResult == 1)
                                {
                                    Console.WriteLine("Counts are equal – test completed.");
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Some test records are pending for testing, so please test them first..'); setTimeout(function() { window.location = 'GenerateReport.aspx'; }, 10);</script>");
                                }


                            }
                            string message = "Fill Highlighted Fields.";
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append("<script type = 'text/javascript'>");
                            sb.Append("window.onload=function(){");
                            sb.Append("alert('");
                            sb.Append(message);
                            sb.Append("')};");
                            sb.Append("</script>");
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
                            Session["SampleId"] = ID;
                            BindDetails(ID);
                            BindResults(ID);
                            //BindResultDropdown(ID);

                        }
                        if (Request.QueryString["TestResultID"] != null)
                        {
                            BindDetails(ID);
                            TestResultID = Request.QueryString["TestResultID"].ToString();
                            Session["TestResultID"] = TestResultID;

                            BindDetails(ID, TestResultID);
                            BindSignatory();
                        }
                    }
                    else
                    {
                        Response.Redirect("Login.aspx");
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }
         private void BindSignatory()
		 {
			 int DepartmentID = Convert.ToInt32(Session["DeptID"].ToString());
			 DataTable dt = _cm.GetAuthorizedSignatory(DepartmentID);
			 ddlSignatory.DataSource = dt;
			 ddlSignatory.DataTextField = "EmployeeName";
			 ddlSignatory.DataValueField = "ID";
			 ddlSignatory.DataBind();
			 ddlSignatory.Items.Insert(0, new ListItem("Select Signatory", ""));
		 }
        public int CheckSampleCounts(int sampleId)
        {
            int result = 0;

            string query = @"WITH FirstCount AS (
                                    SELECT COUNT(*) AS cnt1
                                    FROM SampleDetails sd
                                    INNER JOIN crfclonelist cl ON cl.sampleid = sd.id
                                    INNER JOIN CRFSampleDetails csd ON csd.CloneCRFSampleID = cl.CloneCRFSampleID
                                    WHERE sd.ID = @SampleID
                                ),
                                SecondCount AS (
                                    SELECT COUNT(*) AS cnt2
                                    FROM SampleDetails sd
                                    INNER JOIN crfclonelist cl ON cl.sampleid = sd.id
                                    INNER JOIN CRFSampleDetails csd ON csd.CloneCRFSampleID = cl.CloneCRFSampleID
                                    WHERE sd.ID = @SampleID AND csd.samplestatus = 27
                                )
                                SELECT 
                                    CASE 
                                        WHEN FirstCount.cnt1 = SecondCount.cnt2 THEN 1
                                        ELSE 0
                                    END AS IsCountEqual
                                FROM FirstCount, SecondCount;
                            ";

            using (SqlConnection conn = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", sampleId);
                conn.Open();
                var resultObj = cmd.ExecuteScalar();
                if (resultObj != null)
                {
                    result = Convert.ToInt32(resultObj);
                }
            }

            return result;  // Will return 1 or 0
        }
        private void BindDetails(string SampleId)
        {
            DataTable dt1 = _cm.GetTestReportDetails1(SampleId);
            DataTable dt3 = _cm.GetTestReportDetails2(SampleId);
            string LastTestReportNo = ""; string TestReportNo = "";
            if (dt1.Rows.Count > 0)
            {
                txtDiscipline.Text = dt1.Rows[0]["DepartmentName"].ToString();
                txtGroup.Text = dt1.Rows[0]["productgroupname"].ToString();
                txtSampleReceivedDate.Text = DateTime.Parse(dt1.Rows[0]["ReceivedDate"].ToString()).ToString("dd/MM/yyyy");
                txtSampleId.Text = dt1.Rows[0]["SampleIdSeries"].ToString();
                txtSampleRegisterDate.Text = DateTime.Parse(dt1.Rows[0]["Created"].ToString()).ToString("dd/MM/yyyy");
                txtBatchNo.Text = dt1.Rows[0]["BatchNo"].ToString();
                txtNameOfProduct.Text = dt1.Rows[0]["SampleName"].ToString();
                txtSampleQuality.Text = dt1.Rows[0]["SizeOfSample"].ToString();
                txtSampleCondition.Text = "Found Ok";

                ViewState["ReportIdGet"] = dt1.Rows[0]["ReportID"].ToString();
            }
            if (dt3.Rows.Count > 0)
            {
                txtCustomerName.Text = dt3.Rows[0]["CustomerName"].ToString();
                txtContactPerson.Text = dt3.Rows[0]["ContactPerson"].ToString();
                txtContactDetail.Text = dt3.Rows[0]["Mobile"].ToString();
                 ViewState["ReportIdGet"] = dt3.Rows[0]["ReportID"].ToString();
            }

            string sampleID = SampleId;

            TestReportNo = _cm.generateReportNo(sampleID);
            txtTestReportNo.Text = TestReportNo;


            DataTable dt5 = _cm.GetTestResultPv(Session["ID"].ToString());
            DateTime AnalysisCompletionDate = new DateTime();
            if (dt5.Rows.Count > 0)
            {
                foreach (DataRow dr in dt5.Rows)
                {
                    if (DateTime.Parse((dr["AnalysisCompletionDate"].ToString())) > AnalysisCompletionDate)
                    {
                        AnalysisCompletionDate = DateTime.Parse((dr["AnalysisCompletionDate"].ToString()));
                    }
                    txtAnalysisStartingDate.Text = DateTime.Parse((dr["AnalysisCompletionDate"].ToString())).ToString("dd/MM/yyyy");

                    txtAnalysisCompleteDate.Text = AnalysisCompletionDate.ToString("dd/MM/yyyy");
                }
            }
        }
        private void BindDetails(string SampleId, string TestResultID)
        {
            DataTable dt1 = _cm.GetTestReportDetails4(SampleId);
            if (dt1.Rows.Count > 0)
            {
                txtTestReportNo.Text = dt1.Rows[0]["TestReportNo"].ToString();
                txtDateOfMfg.Text = dt1.Rows[0]["DateOfMfg"].ToString();
                txtDateOfExp.Text = dt1.Rows[0]["DateOfExp"].ToString();
                txtTestRequired.Text = dt1.Rows[0]["TestRequired"].ToString();
                txtSampleCondition.Text = dt1.Rows[0]["SampleCondition"].ToString();
                //ViewState["ReportIdGet"] = dt1.Rows[0]["ReportID"].ToString();
            }
        }
        private void BindResults(string SampleId)
        {
            DataTable dt = _cm.GetTestReportDetails_Pv(SampleId);
            int reportID = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["ReportID"]) : 0;

            if (reportID > 0)
            {
                gvResults.DataSource = dt;
                gvResults.DataBind();
            }
        }
        // Logic for button visibility:
        // If any ReTest selected -> Show Save button, Hide Submit
        // If all Approve selected -> Show Submit button, Hide Save
        // Otherwise -> Hide both

        protected void ddlAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasReTest = false;
            bool allApproved = true;
            int totalRows = 0;

            foreach (GridViewRow row in gvResults.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    totalRows++;
                    DropDownList ddlAction = (DropDownList)row.FindControl("ddlAction");
                    //CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");

                    if (ddlAction != null)
                    {
                        if (ddlAction.SelectedValue == "ReTest")
                        {
                            hasReTest = true;
                            allApproved = false;
                            // Hide checkbox for ReTest rows
                            //if (chkSelect != null)
                            //    chkSelect.Visible = false;
                        }
                        else if (ddlAction.SelectedValue == "Approve")
                        {
                            //// Show checkbox for Approve rows
                            //if (chkSelect != null)
                            //    chkSelect.Visible = true;
                        }
                        else
                        {
                            // Not selected yet
                            allApproved = false;
                            //    if (chkSelect != null)
                            //        chkSelect.Visible = true;
                            //}
                        }
                    }
                }

                if (hasReTest)
                {
                    btnSave.Visible = true;
                    btnSubmit.Visible = false;
                }
                else if (allApproved && totalRows > 0)
                {
                    btnSave.Visible = false;
                    btnSubmit.Visible = true;
                }
                else
                {
                    btnSave.Visible = false;
                    btnSubmit.Visible = false;
                }
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string SampleId = Session["SampleId"].ToString();
            DataTable dtClone = _cm.getCloneCRFIDFromSampleID(SampleId);

            if (dtClone.Rows.Count > 0)
            {
                int CloneCRFSampleID = Convert.ToInt32(dtClone.Rows[0]["CloneCRFSampleID"].ToString());

                // Process each row individually
                foreach (GridViewRow row in gvResults.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        DropDownList ddlAction = (DropDownList)row.FindControl("ddlAction");
                        if (ddlAction != null && !string.IsNullOrEmpty(ddlAction.SelectedValue))
                        {
                            int statusToSet = 0;
                            string message = "";

                            // Set status based on individual row selection
                            if (ddlAction.SelectedValue == "ReTest")
                            {
                                statusToSet = 1005; // Re-Test
                                message = "Sample marked for re-test by " + Session["EmployeeName"].ToString() + " on " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            }
                            else if (ddlAction.SelectedValue == "Approve")
                            {
                                statusToSet = 1006; // Approve
                                message = "Sample approved by " + Session["EmployeeName"].ToString() + " on " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            }

                            // Update status for this row if action is selected
                            if (statusToSet > 0)
                            {
                                //  Always set CRFCloneList (main CRF) to Allocated (19)
                                _cm.CRFStatusID = 19;
                                _cm.CloneCRFSampleID = CloneCRFSampleID;
                                _cm.UpdateCRFStatus(_cm);

                                //  Always set Sample table to Allocated (19)
                                _cm.sampleStatusID = 19;
                                _cm.sampleId = Convert.ToInt32(SampleId);
                                _cm.updateSampleStatus(_cm);

                                //  Set CRFSampleDetails to actual action status (1005 or 1006)
                                //_cm.CRFSampleId = CloneCRFSampleID;
                                //_cm.sampleStatusID = statusToSet;
                                //_cm.UpdateCRFSampleDetails(_cm);

                                //  Add comment for audit trail
                                _cm.comments = message;
                                _cm.UpdateCRFSampleDetailsComments(_cm);
                            }

                        }
                    }
                }

                // Show success message based on what was processed
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success",
                    "alert('Actions saved successfully!'); window.location.href='SampleList.aspx';", true);
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int flag = 0;
            int TestResultId = 0;
            string SampleId = Session["ID"].ToString();
            int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
            if (RoleID == 1 || RoleID == 2)
            {
                if (txtTestReportNo.Text == "")
                {
                    txtTestReportNo.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
                else
                {
                    txtTestReportNo.BorderColor = System.Drawing.Color.Empty;
                }
                if (txtAnalysisStartingDate.Text == "")
                {
                    txtAnalysisStartingDate.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
                else
                {
                    txtAnalysisStartingDate.BorderColor = System.Drawing.Color.Empty;
                }
                if (txtAnalysisCompleteDate.Text == "")
                {
                    txtAnalysisCompleteDate.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
                else
                {
                    txtAnalysisCompleteDate.BorderColor = System.Drawing.Color.Empty;
                }

                if (txtNameOfProduct.Text == "")
                {
                    txtNameOfProduct.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
                else
                {
                    txtNameOfProduct.BorderColor = System.Drawing.Color.Empty;
                }
                if (txtTestRequired.Text == "")
                {
                    txtTestRequired.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
                else
                {
                    txtTestRequired.BorderColor = System.Drawing.Color.Empty;
                }

                if (flag == 0)
                {
                    if (Session["OrganizationID"] != null)
                    {
                        _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                        string TestReportNo = txtTestReportNo.Text;
                        //check TestReportNo exists
                        bool TestReportNoExists = false;
                        {
                            TestReportNoExists = _cm.checkTestReportNoExists(TestReportNo);
                            string sampleID = ViewState["SampleID"].ToString();
                            TestReportNo = _cm.generateReportNo(sampleID);
                            txtTestReportNo.Text = TestReportNo;
                        }
                        _cm.sampleId = Convert.ToInt32(SampleId.ToString());
                        _cm.ulrNo = "";
                        _cm.reportNo = txtTestReportNo.Text.ToString();
                        _cm.issueDate = DateTime.ParseExact(DateTime.Now.AddHours(12.5).ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        _cm.analysisStartingDate = DateTime.ParseExact(txtAnalysisStartingDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        _cm.analysisCompleteDate = DateTime.ParseExact(txtAnalysisCompleteDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        _cm.nameOfProduct = txtNameOfProduct.Text.ToString();
                        _cm.dateOfMfg = txtDateOfMfg.Text.ToString();
                        _cm.dateOfExp = txtDateOfExp.Text.ToString();
                        _cm.sampleQuality = txtSampleQuality.Text.ToString();
                        _cm.sampleCondition = txtSampleCondition.Text.ToString();
                        _cm.testRequired = txtTestRequired.Text.ToString();

                        DataTable dt2 = _cm.checkTestResultDetailsExist(_cm.sampleId);
                        if (dt2.Rows.Count > 0)
                        {
                            TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultID"].ToString());
                            _cm.ulrNo = txtUlrNo.Text;
                            _cm.TestReportNo = txtTestReportNo.Text;
                            _cm.UpdateTestResultDetailsChemical(_cm);
                            //_cm.UpdateTestResultDetails(_cm);
                            Session["TestResultId"] = dt2.Rows[0]["TestResultId"].ToString();
                        }
                        else
                        {
                            TestResultId = _cm.AddTestResultDetailsPV(_cm);
                            Session["TestResultId"] = TestResultId;
                        }

                        DataTable dtClone = _cm.getCloneCRFIDFromSampleID(SampleId);
                        if (dtClone.Rows.Count > 0)
                        {
                            int CloneCRFSampleID = Convert.ToInt32(dtClone.Rows[0]["CloneCRFSampleID"].ToString());
                            //change CloneCRF status to 'Ready to Print'
                            _cm.CRFStatusID = 29;
                            _cm.CloneCRFSampleID = CloneCRFSampleID;
                            _cm.UpdateCRFStatus(_cm);

                            _cm.sampleStatusID = 29;//Ready to Print
                            _cm.sampleId = Convert.ToInt32(SampleId);
                            _cm.updateSampleStatus(_cm);
                            Response.Redirect("GenerateReport.aspx");
                        }

                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Fill highlighted fields');", true);
                    }
                }
            }
            else if (RoleID == 5)

            {
                //txtUlrNo.Text = "123";
                if (txtUlrNo.Text == "")
                {
                    txtUlrNo.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
                else
                {
                    txtUlrNo.BorderColor = System.Drawing.Color.Empty;
                }
                if (txtIssueDate.Text == "")
                {
                    txtIssueDate.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
                else
                {
                    txtIssueDate.BorderColor = System.Drawing.Color.Empty;
                }
                if (ddlSignatory.SelectedIndex == 0)
                {
                    flag = 1;
                    ddlSignatory.BorderColor = System.Drawing.Color.Red;
                }
                else
                {
                    ddlSignatory.BorderColor = System.Drawing.Color.Empty;
                }
                SampleId = Session["SampleId"].ToString();
                if (flag == 0)
                {
                    TestResultId = Convert.ToInt32(Session["TestResultID"].ToString());
                    _cm.TestResultID = TestResultId;
                    _cm.ulrNo = txtUlrNo.Text.ToString();
                    //_cm.ReportType = "NON NABL";
                    _cm.ReportType = ddlReportType.SelectedItem.ToString();
                    _cm.issueDate = DateTime.ParseExact(txtIssueDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    _cm.AuthorizedSignatoryID = 1001;
                    _cm.remarks = txtRemarks.Text;
                    int status = _cm.UpdateTestResultDetailsMicro(_cm);

                    Session["SampleId"] = SampleId;
                    Session["TestResultId"] = TestResultId;
                    DataTable dtReport = _cm.GetReportFormat(Session["SampleId"].ToString());
                    string ReportName = dtReport.Rows[0]["ReportName"].ToString();
                    if ((ReportName == "NABL") || (ReportName == "NABL_Cert_PDP") || (ReportName == "Non NABL") || (ReportName == "Pesticides"))
                    {
                        Response.Redirect("GenerateReport.aspx?ID=" + SampleId + "&TestResultID=" + TestResultId);
                    }
                    else
                    {
                        Response.Redirect("CreateReportFromTemplate_PV.aspx?s=" + SampleId + "&c=" + hdReportID.Value);
                    }
                }
            }

        }
        protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ExecuteAction")
            {
                try
                {
                    string SampleId = Session["ID"].ToString();
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the dropdown from the row
                    GridViewRow row = gvResults.Rows[rowIndex];
                    DropDownList ddlAction = (DropDownList)row.FindControl("ddlAction");

                    // Validate dropdown selection
                    if (ddlAction.SelectedValue == "0")
                    {
                        ddlAction.BorderColor = System.Drawing.Color.Red;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Alert",
                            "alert('Please select an action!');", true);
                        return;
                    }

                    ddlAction.BorderColor = System.Drawing.Color.Empty;
                    string selectedAction = ddlAction.SelectedValue;

                    DataTable dtClone = _cm.getCloneCRFIDFromSampleID(SampleId);

                    if (dtClone.Rows.Count > 0)
                    {
                        int CloneCRFSampleID = Convert.ToInt32(dtClone.Rows[0]["CloneCRFSampleID"].ToString());

                        if (selectedAction == "Approve")
                        {
                            // Approve Logic
                            _cm.CRFStatusID = 27; // Test Completed status
                            _cm.CloneCRFSampleID = CloneCRFSampleID;
                            _cm.UpdateCRFStatus(_cm);

                            _cm.sampleStatusID = 27; // Test Completed
                            _cm.sampleId = Convert.ToInt32(SampleId);
                            _cm.updateSampleStatus(_cm);

                            // Add approval comment
                            _cm.CRFSampleId = CloneCRFSampleID;
                            _cm.comments = "Test approved by " + Session["EmployeeName"].ToString() + " on " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            _cm.UpdateCRFSampleDetailsComments(_cm);

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success",
                                "alert('Test approved successfully!'); window.location.href='GenerateReport.aspx';", true);
                        }
                        else if (selectedAction == "ReTest")
                        {
                            // Re-Test Logic
                            _cm.CRFStatusID = 1005; // Re-Test status
                            _cm.CloneCRFSampleID = CloneCRFSampleID;
                            _cm.UpdateCRFStatus(_cm);

                            _cm.sampleStatusID = 1005; // Re-Test
                            _cm.sampleId = Convert.ToInt32(SampleId);
                            _cm.updateSampleStatus(_cm);

                            // Add re-test comment
                            _cm.CRFSampleId = CloneCRFSampleID;
                            _cm.comments = "Sample marked for re-test by " + Session["EmployeeName"].ToString() + " on " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            _cm.UpdateCRFSampleDetailsComments(_cm);

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success",
                                "alert('Sample marked for re-testing!'); window.location.href='SampleList.aspx';", true);
                        }

                        // Refresh the grid
                        BindResults(SampleId);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error",
                            "alert('Clone CRF details not found!');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error",
                        "alert('Error: " + ex.Message + "');", true);
                }
            }
        }       
        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    HyperLink hypReport = (HyperLink)e.Row.FindControl("hypPDFReport");
            //    DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");

            //    // Get the data from the DataRow
            //    DataRowView drv = (DataRowView)e.Row.DataItem;
            //    string pdfPath = drv["PDFPath"] != DBNull.Value ? drv["PDFPath"].ToString() : "";

            //    if (!string.IsNullOrEmpty(pdfPath))
            //    {
            //        // Set the PDF URL - remove the leading "~" if present and resolve the URL
            //        if (pdfPath.StartsWith("~"))
            //        {
            //            hypReport.NavigateUrl = ResolveUrl(pdfPath);
            //        }
            //        else if (pdfPath.StartsWith("/"))
            //        {
            //            hypReport.NavigateUrl = pdfPath;
            //        }
            //        else
            //        {
            //            // If path doesn't start with ~ or /, add it
            //            hypReport.NavigateUrl = ResolveUrl("~/" + pdfPath);
            //        }
            //        hypReport.Visible = true;
            //    }
            //    else
            //    {
            //        hypReport.Text = "No Report";
            //        hypReport.Enabled = false;
            //        hypReport.Visible = true;
            //    }

            //    // Store row index in dropdown for later use
            //    ddlStatus.Attributes["data-rowindex"] = e.Row.RowIndex.ToString();
            //}
        }
     
        //private void BindResults(string SampleId, int ReportId)
        //{
        //    DataTable dt = _cm.GetTestReportDetails3_Pv(SampleId, ReportId);
        //    if (dt.Rows.Count > 0)
        //    {
        //        lblSampleName.Text = dt.Rows[0]["SampleName"].ToString();
        //        lblBatchNo.Text = dt.Rows[0]["BatchNo"].ToString();

        //        gvResults.DataSource = dt;
        //        gvResults.DataBind();

        //        DataTable dtResultStatus = _cm.GetCRFListForTestResults(SampleId);
        //        if (dtResultStatus.Rows.Count == 1)
        //        {
        //            if (dtResultStatus.Rows[0]["SampleStatus"].ToString() == "1006")
        //            {
        //                btnSubmit.Visible = true;
        //                btnSave.Visible = false;
        //            }
        //            else if (dtResultStatus.Rows[0]["SampleStatus"].ToString() == "27")
        //            {
        //                btnSave.Visible = false;
        //                btnSubmit.Visible = false;
        //            }
        //            else
        //            {
        //                btnSave.Visible = true;
        //                btnSubmit.Visible = false;
        //            }
        //        }
        //        else
        //        {
        //            btnSave.Visible = true;
        //            btnSubmit.Visible = false;
        //        }
        //    }
        //    else
        //    {
        //        gvResults.DataSource = null;
        //        gvResults.DataBind();
        //    }
        //}


        //private void BindDetailsForGeneraldata(string SampleId)
        //{
        //    DataTable dt1 = _cm.GetTestReportDetails4_Pv(SampleId);
        //    if (dt1.Rows.Count > 0)
        //    {
        //        txtUlrNo.Text = dt1.Rows[0]["ULRNo"].ToString();
        //        txtTestReportNo.Text = dt1.Rows[0]["TestReportNo"].ToString();
        //        txtIssueDate.Text = Convert.ToDateTime(dt1.Rows[0]["IssueDate"].ToString()).ToString("dd/MM/yyyy");
        //        txtDateOfMfg.Text = dt1.Rows[0]["DateOfMfg"].ToString();
        //        txtDateOfExp.Text = dt1.Rows[0]["DateOfExp"].ToString();
        //        txtTestRequired.Text = dt1.Rows[0]["TestRequired"].ToString();
        //        txtSampleCondition.Text = dt1.Rows[0]["SampleCondition"].ToString();
        //        txtSampling.Text = dt1.Rows[0]["Sampling"].ToString();
        //        txtSamplingMethod.Text = dt1.Rows[0]["SamplingMethod"].ToString();
        //        txtCustomerReference.Text = dt1.Rows[0]["CustomerReference"].ToString();
        //        txtRemarks.Text = dt1.Rows[0]["Remarks"].ToString();
        //    }
        //}

        //private void BindDetails(string SampleId, int ReportId)
        //{
        //    DataTable dt1 = _cm.GetTestReportDetails1_Pv(SampleId, ReportId);
        //    DataTable dt3 = _cm.GetTestReportDetails2_Pv(SampleId, ReportId);
        //    string LastTestReportNo = "";
        //    string TestReportNo = "";

        //    if (dt1.Rows.Count > 0)
        //    {
        //        txtDiscipline.Text = dt1.Rows[0]["DepartmentName"].ToString();
        //        txtSampleReceivedDate.Text = DateTime.Parse(dt1.Rows[0]["ReceivedDate"].ToString()).ToString("dd/MM/yyyy");
        //        txtSampleId.Text = dt1.Rows[0]["SampleIdSeries"].ToString();
        //        txtSampleRegisterDate.Text = DateTime.Parse(dt1.Rows[0]["Created"].ToString()).ToString("dd/MM/yyyy");
        //        txtBatchNo.Text = dt1.Rows[0]["BatchNo"].ToString();
        //        txtNameOfProduct.Text = dt1.Rows[0]["SampleName"].ToString();
        //        txtSampleQuality.Text = dt1.Rows[0]["SizeOfSample"].ToString();
        //    }

        //    if (dt3.Rows.Count > 0)
        //    {
        //        txtCustomerName.Text = dt3.Rows[0]["CustomerName"].ToString();
        //        txtContactPerson.Text = dt3.Rows[0]["ContactPerson"].ToString();
        //        txtContactDetail.Text = dt3.Rows[0]["Mobile"].ToString();
        //    }

        //    string sYear = DateTime.Now.AddHours(12.5).ToString("yy");
        //    string sMonth = DateTime.Now.AddHours(12.5).ToString("MM");

        //    DataTable dt2 = _cm.getLastTestReportNo();
        //    if (dt2.Rows.Count > 0)
        //    {
        //        LastTestReportNo = dt2.Rows[0]["TestReportNo"].ToString();
        //        string[] TestReportString = LastTestReportNo.Split('/');
        //        int lastItem = Convert.ToInt32(TestReportString[TestReportString.Length - 1]);
        //        //Check year change. Reset series to '0001'
        //        string midTerm = TestReportString[TestReportString.Length - 2];
        //        if (midTerm.Substring(0, 2) != sYear)
        //        {
        //            TestReportNo = "HSTD/TR/" + sYear + sMonth + "/" + "0001";
        //        }
        //        else
        //        {
        //            int nextItem = lastItem + 1;
        //            TestReportNo = nextItem.ToString().PadLeft(4, '0');
        //            TestReportNo = "HSTD/TR/" + sYear + sMonth + "/" + TestReportNo;
        //        }
        //        txtTestReportNo.Text = TestReportNo;
        //    }
        //    else
        //    {
        //        TestReportNo = "HSTD/TR/" + sYear + sMonth + "/" + "0001";
        //        txtTestReportNo.Text = TestReportNo;
        //    }

        //    DataTable dt4 = _cm.getSampleIDFromSampleUID(Convert.ToInt32(SampleId));
        //    if (dt4.Rows.Count > 0)
        //    {
        //        Session["ID"] = Convert.ToInt32(dt4.Rows[0]["id"].ToString());
        //    }

        //    DataTable dt5 = _cm.GetTestResult(Session["ID"].ToString());
        //    if (dt5.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in dt5.Rows)
        //        {
        //            string ds = dr["AnalysisStartingDate"].ToString();
        //            DateTime dt;

        //            if (DateTime.TryParseExact(ds, "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt))
        //            {
        //                ds = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        //            }
        //            if (DateTime.TryParseExact(ds, "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt))
        //            {
        //                ds = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        //            }
        //            txtAnalysisStartingDate.Text = DateTime.Parse((dr["AnalysisStartingDate"].ToString())).ToString("dd/MM/yyyy");

        //            ds = dr["AnalysisCompletionDate"].ToString();
        //            if (DateTime.TryParseExact(ds, "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt))
        //            {
        //                ds = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        //            }
        //            if (DateTime.TryParseExact(ds, "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt))
        //            {
        //                ds = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        //            }
        //            txtAnalysisCompleteDate.Text = DateTime.Parse((dr["AnalysisCompletionDate"].ToString())).ToString("dd/MM/yyyy");
        //        }
        //    }
        //}
        // protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //if (e.CommandName == "UpdateStatus")
        //{
        //    string SampleId = Session["SampleId"].ToString();
        //    int ReportId = Convert.ToInt32(Session["ReportID"]);
        //    int rowIndex = int.Parse(e.CommandArgument.ToString());

        //    // Get controls from the row
        //    DropDownList ddlStatus = (DropDownList)gvResults.Rows[rowIndex].FindControl("ddlAction");
        //    TextBox txtComments = (TextBox)gvResults.Rows[rowIndex].FindControl("txtComments");

        //    // Get CRFSampleId from DataKeys
        //    int CRFSampleId = int.Parse(gvResults.DataKeys[rowIndex]["CRFSampleId"].ToString());

        //    // Validate dropdown selection
        //    if (ddlStatus.SelectedIndex == 0)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Alert",
        //            "alert('Please select an action!');", true);
        //        return;
        //    }

        //    int selectedStatus = Convert.ToInt32(ddlStatus.SelectedValue);
        //    string comments = txtComments.Text.Trim();

        //    // Validate comments for Re-Test
        //    if (selectedStatus == 1005 && string.IsNullOrEmpty(comments))
        //    {
        //        txtComments.BorderColor = Color.Red;
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Alert",
        //            "alert('Comments are required for Re-Test!');", true);
        //        return;
        //    }

        //    txtComments.BorderColor = Color.Empty;

        //    try
        //    {
        //        // Update status
        //        _cm.CRFSampleId = CRFSampleId;
        //        _cm.sampleStatusID = selectedStatus;
        //        _cm.UpdateCRFSampleDetailsStatus(_cm);

        //        // Update comments if provided
        //        if (!string.IsNullOrEmpty(comments))
        //        {
        //            _cm.comments = comments;
        //            _cm.UpdateCRFSampleDetailsComments(_cm);
        //        }

        //        // Check if all samples are accepted (1006)
        //        DataTable dtResultStatus = _cm.GetCRFListForTestResults(SampleId);

        //        if (dtResultStatus.Rows.Count > 0)
        //        {
        //            bool allAccepted = true;
        //            foreach (DataRow row in dtResultStatus.Rows)
        //            {
        //                if (row["SampleStatus"].ToString() != "1006")
        //                {
        //                    allAccepted = false;
        //                    break;
        //                }
        //            }

        //            if (allAccepted)
        //            {
        //                btnSubmit.Visible = true;
        //                btnSave.Visible = false;
        //            }
        //            else
        //            {
        //                btnSave.Visible = true;
        //                btnSubmit.Visible = false;
        //            }
        //        }

        //        // Show success message
        //        string statusText = selectedStatus == 1006 ? "Result Accepted" : "Re-Test";
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success",
        //            "alert('Status updated to " + statusText + " successfully!');", true);

        //        // Refresh the grid
        //        //BindResults(SampleId, ReportId);
        //        BindResults(SampleId);
        //    }
        //    catch (Exception ex)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error",
        //            "alert('Error: " + ex.Message + "');", true);
        //    }
        //}

        //// Keep your existing "Update" command if needed
        //if (e.CommandName == "Update")
        //{
        //    string SampleId = Session["SampleId"].ToString();
        //    int ReportId = Convert.ToInt32(Session["ReportID"]);
        //    int index = int.Parse(e.CommandArgument.ToString());
        //    int CRFSampleId = int.Parse(gvResults.DataKeys[index]["CRFSampleId"].ToString());
        //    int rowIndex = int.Parse(e.CommandArgument.ToString());
        //    DropDownList ddlStatus = (DropDownList)gvResults.Rows[rowIndex].Cells[0].FindControl("ddlStatus");
        //    TextBox txtComments = (TextBox)gvResults.Rows[rowIndex].FindControl("txtComments");

        //    if (ddlStatus.SelectedIndex != 0)
        //    {
        //        if (txtComments.Text == "")
        //        {
        //            txtComments.BorderColor = Color.Red;
        //        }
        //        else
        //        {
        //            _cm.CRFSampleId = CRFSampleId;
        //            txtComments.BorderColor = Color.Empty;
        //            _cm.comments = txtComments.Text;
        //            _cm.UpdateCRFSampleDetailsComments(_cm);
        //            _cm.sampleStatusID = Convert.ToInt32(ddlStatus.SelectedValue);

        //            _cm.UpdateCRFSampleDetailsStatus(_cm);
        //            DataTable dtResultStatus = _cm.GetCRFListForTestResults(SampleId);

        //            if (dtResultStatus.Rows.Count == 1)
        //            {
        //                if (dtResultStatus.Rows[0]["SampleStatus"].ToString() == "1006")
        //                {
        //                    btnSubmit.Visible = true;
        //                    btnSave.Visible = false;
        //                }
        //                else
        //                {
        //                    btnSave.Visible = true;
        //                    btnSubmit.Visible = false;
        //                }
        //            }
        //            else
        //            {
        //                btnSave.Visible = true;
        //                btnSubmit.Visible = false;
        //            }

        //            gvResults.EditIndex = -1;
        //           // this.BindResults(SampleId, ReportId);
        //            this.BindResults(SampleId);
        //        }
        //    }
        //    else
        //    {
        //        txtComments.BorderColor = Color.Empty;
        //        _cm.sampleStatusID = Convert.ToInt32(ddlStatus.SelectedValue);
        //        _cm.CRFSampleId = CRFSampleId;
        //        _cm.UpdateCRFSampleDetailsStatus(_cm);
        //        DataTable dtResultStatus = _cm.GetCRFListForTestResults(SampleId);

        //        if (dtResultStatus.Rows.Count == 1)
        //        {
        //            if (dtResultStatus.Rows[0]["SampleStatus"].ToString() == "1006")
        //            {
        //                btnSubmit.Visible = true;
        //            }
        //            else
        //            {
        //                btnSave.Visible = true;
        //            }
        //        }
        //        else
        //        {
        //            btnSave.Visible = true;
        //        }

        //        gvResults.EditIndex = -1;
        //        //this.BindResults(SampleId, ReportId);
        //        this.BindResults(SampleId);
        //    }
        //}
        //}
        //protected void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    int flag = 0;
        //    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());

        //    if (RoleID == 1 || RoleID == 2)
        //    {
        //        string SampleId = Session["SampleId"].ToString();
        //        int ReportID = Convert.ToInt32(Session["ReportID"]);

        //        if (flag == 0)
        //        {
        //            string TestReportNo = txtTestReportNo.Text;
        //            string prefix = "";
        //            //check TestReportNo exists
        //            bool TestReportNoExists = false;
        //            {
        //                TestReportNoExists = _cm.checkTestReportNoExists(TestReportNo);
        //                if (TestReportNoExists)
        //                {
        //                    DataTable dtOrg = _cm.GetOrganizationDetails();
        //                    if (dtOrg.Rows.Count > 0)
        //                    {
        //                        prefix = dtOrg.Rows[0]["PrefixReport"].ToString();
        //                    }
        //                    string[] TestReportString = txtTestReportNo.Text.Split('/');
        //                    int lastItem = Convert.ToInt32(TestReportString[TestReportString.Length - 1]);
        //                    int nextItem = lastItem + 1;
        //                    string sYear = DateTime.Now.AddHours(12.5).ToString("yy");
        //                    string sMonth = DateTime.Now.AddHours(12.5).ToString("MM");

        //                    //Check year change. Reset series to '0001'
        //                    string midTerm = TestReportString[TestReportString.Length - 2];
        //                    if (midTerm.Substring(0, 2) != sYear)
        //                    {
        //                        TestReportNo = prefix + sYear + sMonth + "0001";
        //                    }
        //                    else
        //                    {
        //                        nextItem = lastItem + 1;
        //                        TestReportNo = nextItem.ToString().PadLeft(4, '0');
        //                        TestReportNo = prefix + sYear + sMonth + TestReportNo;
        //                    }

        //                    txtTestReportNo.Text = TestReportNo;
        //                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Notify", "alert('Change in TestReport# Allocation! " + TestReportNo + "');", true);
        //                }
        //            }

        //            _cm.sampleId = Convert.ToInt32(SampleId);
        //            _cm.ReportID = ReportID.ToString(); // Convert to string if _cm.ReportID is string type
        //            _cm.ulrNo = "";
        //            _cm.reportNo = txtTestReportNo.Text.ToString();
        //            _cm.issueDate = DateTime.Now.AddHours(12.5);
        //            _cm.analysisStartingDate = DateTime.ParseExact(txtAnalysisStartingDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //            _cm.analysisCompleteDate = DateTime.ParseExact(txtAnalysisCompleteDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //            _cm.nameOfProduct = txtNameOfProduct.Text.ToString();
        //            _cm.dateOfMfg = txtDateOfMfg.Text.ToString();
        //            _cm.dateOfExp = txtDateOfExp.Text.ToString();
        //            _cm.sampleQuality = txtSampleQuality.Text.ToString();
        //            _cm.sampleCondition = txtSampleCondition.Text.ToString();
        //            _cm.testRequired = txtTestRequired.Text.ToString();
        //            _cm.sampling = txtSampling.Text.ToString();
        //            _cm.SamplingMethod = txtSamplingMethod.Text.ToString();
        //            _cm.CustomerReference = txtCustomerReference.Text;
        //            _cm.remarks = txtRemarks.Text;

        //            // Check if test result details exist for this SampleId and ReportID
        //            DataTable dt2 = _cm.checkTestResultDetailsExist_Pv(Convert.ToInt32(SampleId), ReportID);
        //            if (dt2.Rows.Count > 0)
        //            {
        //                _cm.ulrNo = txtUlrNo.Text;
        //                _cm.TestReportNo = txtTestReportNo.Text;
        //                _cm.UpdateTestResultDetails(_cm);
        //            }
        //            else
        //            {
        //                _cm.AddTestResultDetailsMicro(_cm);
        //            }

        //            DataTable dtClone = _cm.getCloneCRFIDFromSampleID(SampleId);
        //            if (dtClone.Rows.Count > 0)
        //            {
        //                int CloneCRFSampleID = Convert.ToInt32(dtClone.Rows[0]["CloneCRFSampleID"].ToString());
        //                //change CloneCRF status to 'Ready to Print'
        //                _cm.CRFStatusID = 29;
        //                _cm.CloneCRFSampleID = CloneCRFSampleID;
        //                _cm.UpdateCRFStatus(_cm);

        //                _cm.sampleStatusID = 29;//Ready to Print
        //                _cm.sampleId = Convert.ToInt32(SampleId);
        //                _cm.updateSampleStatus(_cm);
        //            }

        //            Response.Redirect("SampleList.aspx");
        //        }
        //    }
        //    else if (RoleID == 5)
        //    {
        //        string SampleId = Session["SampleId"].ToString();
        //        int ReportID = Convert.ToInt32(Session["ReportID"]);

        //        if (flag == 0)
        //        {
        //            _cm.ReportID = ReportID.ToString(); // Convert to string if _cm.ReportID is string type
        //            _cm.ulrNo = txtUlrNo.Text.ToString();
        //            _cm.ReportType = ddlReportType.SelectedItem.ToString();
        //            _cm.issueDate = DateTime.Now.AddHours(12.5);
        //            _cm.AuthorizedSignatoryID = Convert.ToInt32(ddlSignatory.SelectedValue);
        //            int status = _cm.UpdateTestResultDetailsMicro(_cm);

        //            Session["SampleId"] = SampleId;
        //            Session["ReportID"] = ReportID;

        //            // Pass both SampleId and ReportID in query string
        //            //Response.Redirect("CreateReportFromTemplate.aspx?s=" + SampleId + "&r=" + ReportID);
        //        }
        //    }
        //    else
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Fill highlighted fields');", true);
        //    }
        //}
    }
}