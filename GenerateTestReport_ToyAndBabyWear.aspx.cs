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
    public partial class GenerateTestReport_ToyAndBabyWear : System.Web.UI.Page
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
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
                                break;
                            default: break;
                        }

                        if (RoleID == 2)
                        {
                            ddlResult.Enabled = true;
                        }
                        else
                        {
                            ddlResult.Enabled = false;
                        }
                    }
                    string ID = ""; DataTable dt = new DataTable(); string SampleId = ""; string TestResultID = "";
                    if (Request.QueryString["ID"] != null || Request.QueryString["TestResultID"] != null)
                    {
                        if (Request.QueryString["ID"] != null)
                        {
                            ID = Request.QueryString["ID"].ToString();
                            Session["ID"] = ID;
                            ViewState["SampleID"] = ID;
                            //DataTable dt2 = _cm.GetResultOverallConclusion(ID);

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
                            BindResultDropdown(ID);
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
            string sampleID = ViewState["SampleID"].ToString();
            TestReportNo = _cm.generateReportNo(sampleID);
            txtTestReportNo.Text = TestReportNo;

            DataTable dt4 = _cm.getSampleIDFromSampleUID(Convert.ToInt32(SampleId));
            if (dt4.Rows.Count > 0)
            {
                Session["ID"] = Convert.ToInt32(dt4.Rows[0]["id"].ToString());
            }
            DataTable dt5 = _cm.GetTestResult(Session["ID"].ToString());
            DateTime AnalysisCompletionDate = new DateTime();
            if (dt5.Rows.Count > 0)
            {
                foreach (DataRow dr in dt5.Rows)
                {
                    if (DateTime.Parse((dr["AnalysisCompletionDate"].ToString())) > AnalysisCompletionDate)
                    {
                        AnalysisCompletionDate = DateTime.Parse((dr["AnalysisCompletionDate"].ToString()));
                    }
                    txtAnalysisStartingDate.Text = DateTime.Parse((dr["AnalysisStartingDate"].ToString())).ToString("dd/MM/yyyy");

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
            DataTable dt = _cm.GetTestReportDetails3_ToysAndBabyWear(SampleId);
            int reportID = Convert.ToInt32(ViewState["ReportIdGet"].ToString());
            int RoleId = Convert.ToInt32(Session["RoleID"].ToString());
            //if (reportID == 6417)
            //{
            if (RoleId == 2)
            {
                gvResultsA01Gen.Visible = false;
                gvResults.Visible = true;
                gvResults.DataSource = dt;
                gvResults.DataBind();
            }
            else
            {
                gvResultsA01Gen.Visible = false;
                gvResults.Visible = false;
                gvResultsNoEdit.Visible = true;
                gvResultsNoEdit.DataSource = dt;
                gvResultsNoEdit.DataBind();
            }
            //}
            //else
            //{
            //    gvResultsA01Gen.Visible = true;
            //    gvResults.Visible = false;
            //    gvResultsNoEdit.Visible = false;
            //    gvResultsA01Gen.DataSource = dt;
            //    gvResultsA01Gen.DataBind();
            //}
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
                        _cm.FinalTestResult = ddlResult.SelectedItem.Text;

                        DataTable dt2 = _cm.checkTestResultDetailsExist(_cm.sampleId);
                        if (dt2.Rows.Count > 0)
                        {
                            TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultID"].ToString());
                            _cm.ulrNo = txtUlrNo.Text;
                            _cm.TestReportNo = txtTestReportNo.Text;
                            _cm.UpdateTestResultDetailsChemical(_cm);
                            Session["TestResultId"] = dt2.Rows[0]["TestResultId"].ToString();
                        }
                        else
                        {
                            TestResultId = _cm.AddTestResultDetailsChemical(_cm);
                            Session["TestResultId"] = TestResultId;
                        }

                        DataTable dtClone = _cm.getCloneCRFIDFromSampleID(SampleId);
                        if (dtClone.Rows.Count > 0)
                        {
                            int CloneCRFSampleID = Convert.ToInt32(dtClone.Rows[0]["CloneCRFSampleID"].ToString());
                            _cm.CRFStatusID = 29;
                            _cm.CloneCRFSampleID = CloneCRFSampleID;
                            _cm.UpdateCRFStatus(_cm);
                            _cm.sampleStatusID = 29;
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
                    _cm.ReportType = ddlReportType.SelectedItem.ToString();
                    _cm.issueDate = DateTime.ParseExact(txtIssueDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    _cm.AuthorizedSignatoryID = Convert.ToInt32(ddlSignatory.SelectedValue);
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
                        Response.Redirect("CreateReportFromTemplate_ToyAndBabyWear.aspx?s=" + SampleId + "&c=" + hdReportID.Value);
                    }
                }
            }

        }

        private void BindResultDropdown(string sampleId)
        {
            string query = @" SELECT 
                                    CASE 
                                        WHEN COUNT(DISTINCT t.Conclusion) = 1 THEN MAX(t.Conclusion)
                                        WHEN SUM(CASE WHEN t.Conclusion = 'FAIL' THEN 1 ELSE 0 END) > 0 THEN 'FAIL'
                                        ELSE MAX(t.Conclusion)
                                    END AS [Result]
                                FROM crfsampledetails c
                                LEFT JOIN TestResults t ON c.crfsampleid = t.crfsampleid
                                WHERE t.sampleID = @SampleID";

            using (SqlConnection conn = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", sampleId);
                conn.Open();
                object result = cmd.ExecuteScalar();

                ddlResult.Items.Clear();

                if (result != null)
                {
                    string conclusion = result.ToString().ToUpper();
                    ddlResult.Items.Add(new ListItem("PASS", "PASS"));
                    ddlResult.Items.Add(new ListItem("FAIL", "FAIL"));

                    if (conclusion == "FAIL")
                        ddlResult.SelectedValue = "FAIL";
                    else
                        ddlResult.SelectedValue = "PASS";
                }
                else
                {
                    ddlResult.Items.Add(new ListItem("N/A", "N/A"));
                }
            }
        }

        protected void gvResults_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvResults.EditIndex = e.NewEditIndex;
            BindResults(ViewState["SampleID"].ToString());
        }

        protected void gvResults_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvResults.EditIndex = -1;
            BindResults(ViewState["SampleID"].ToString());
        }

        protected void gvResults_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvResults.Rows[e.RowIndex];
            DropDownList ddlConclusion = (DropDownList)row.FindControl("ddlConclusion");
            string newConclusion = ddlConclusion?.SelectedValue ?? "";
            string testResultId = gvResults.DataKeys[e.RowIndex].Value.ToString();
            UpdateConclusion(testResultId, newConclusion);
            gvResults.EditIndex = -1;
            BindResults(ViewState["SampleID"].ToString());
            BindResultDropdown(ViewState["SampleID"].ToString());
            ScriptManager.RegisterStartupScript(this, GetType(), "updateMsg", "alert('Conclusion updated successfully');", true);
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && gvResults.EditIndex == e.Row.RowIndex)
            {
                DropDownList ddlConclusion = (DropDownList)e.Row.FindControl("ddlConclusion");
                if (ddlConclusion != null)
                {
                    string currentValue = DataBinder.Eval(e.Row.DataItem, "Conclusion").ToString();
                    ListItem li = ddlConclusion.Items.FindByValue(currentValue);
                    if (li != null)
                    {
                        ddlConclusion.ClearSelection();
                        li.Selected = true;
                    }
                }
            }
        }

        public void UpdateConclusion(string testResultId, string conclusion)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "UPDATE TestResults SET Conclusion = @Conclusion,Conclusion_BH = @Conclusion_BH, TestResult=@TestResult WHERE TestResultID = @TestResultID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Conclusion", conclusion);
                if (conclusion == "PASS")
                {
                    cmd.Parameters.AddWithValue("@TestResult", "N.D");
                    cmd.Parameters.AddWithValue("@Conclusion_BH", "LULUS");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@TestResult", "D");
                    cmd.Parameters.AddWithValue("@Conclusion_BH", "TIDAK LULUS");
                }

                cmd.Parameters.AddWithValue("@TestResultID", testResultId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }
}