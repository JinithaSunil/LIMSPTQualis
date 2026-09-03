using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GenerateTestReport_Transport : System.Web.UI.Page
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

                                /*int checkResult = CheckSampleCounts(sampleIDGet);
                                if (checkResult == 1)
                                {
                                    Console.WriteLine("Counts are equal – test completed.");
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Some test records are pending for testing, so please test them first..'); setTimeout(function() { window.location = 'GenerateReport.aspx'; }, 10);</script>");
                                }
                                */

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


            DataTable dt5 = _cm.GetTestResult_Transport(Session["ID"].ToString());
            DateTime AnalysisCompletionDate = DateTime.MinValue;

            if (dt5.Rows.Count > 0)
            {
                foreach (DataRow dr in dt5.Rows)
                {
                    string dateString = dr["AnalysisCompletionDate"]?.ToString();
                    DateTime currentDate;

                    if (!string.IsNullOrEmpty(dateString) &&
                        DateTime.TryParse(dateString, out currentDate))
                    {
                        if (currentDate > AnalysisCompletionDate)
                        {
                            AnalysisCompletionDate = currentDate;
                        }

                        txtAnalysisStartingDate.Text = currentDate.ToString("dd/MM/yyyy");
                        txtAnalysisCompleteDate.Text = AnalysisCompletionDate.ToString("dd/MM/yyyy");
                    }
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
            DataTable dt = _cm.GetTestReportDetails_Transport(SampleId);
            int reportID = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["ReportID"]) : 0;

            if (reportID > 0)
            {
                gvResults.DataSource = dt;
                gvResults.DataBind();
            }
        }
        
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
                            _cm.UpdateTestResultDetails_Transport(_cm);
                            //_cm.UpdateTestResultDetails(_cm);
                            Session["TestResultId"] = dt2.Rows[0]["TestResultId"].ToString();
                        }
                        else
                        {
                            TestResultId = _cm.AddTestResultDetails_Tarnsport(_cm);
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
                        Response.Redirect("CreateReportFromTemplate_Transport.aspx?s=" + SampleId + "&c=" + hdReportID.Value);
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


    }
}