using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GenerateTestReportMicro : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        string ID = ""; DataTable dt = new DataTable(); string SampleId = ""; string TestResultID = ""; string Enquiryid = ""; string ReportID = ""; string DepartmentId = "";
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
                    }

                    if (Request.QueryString["ID"] != null || Request.QueryString["TestResultID"] != null)
                    {
                        if (Request.QueryString["ID"] != null)
                        {
                            ID = Request.QueryString["ID"].ToString();
                            Session["ID"] = ID;
                            dt = _cm.getSampleDetails(Convert.ToInt32(ID));
                            if (dt.Rows.Count > 0)
                            {
                                SampleId = dt.Rows[0]["SampleID"].ToString();
                                Session["SampleId"] = SampleId;
                                Enquiryid = dt.Rows[0]["Enquiryid"].ToString();
                                hdEnquiryID.Value = Enquiryid;
                                DataTable dt4 = _cm.GetReportID(Enquiryid);
                                if (dt4.Rows.Count > 0)
                                {
                                    DepartmentId = dt4.Rows[0]["DepartmentID"].ToString();
                                    hdDepartmentID.Value = DepartmentId;
                                    ReportID = dt4.Rows[0]["ReportID"].ToString();
                                    hdReportID.Value = ReportID;
                                    BindReportType(Convert.ToInt32(DepartmentId));

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
                        }
                        if (Request.QueryString["TestResultID"] != null)
                        {
                            TestResultID = Request.QueryString["TestResultID"].ToString();
                            Session["TestResultID"] = TestResultID;
                            BindDetails(ID);
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

        private void BindReportType(int DepartmentID)
        {
            DataTable dt = _cm.GetAllReportType(DepartmentID);
            if (dt.Rows.Count > 0)
            {
                ddlReportFormat.DataSource = dt;
                ddlReportFormat.DataTextField = "ReportName";
                ddlReportFormat.DataValueField = "ReportID";
                ddlReportFormat.DataBind();
                ddlReportFormat.Items.Insert(0, new ListItem("Select Report Type", ""));
                ddlReportFormat.Items.FindByValue(hdReportID.Value).Selected = true;
            }
        }
        private void BindResults(string SampleId)
        {
            DataTable dt = _cm.GetTestReportDetails3(SampleId);
            if (dt.Rows.Count > 0)
            {
                lblSampleName.Text = dt.Rows[0]["SampleName"].ToString();
                lblBatchNo.Text = dt.Rows[0]["BatchNo"].ToString();

                gvResults.DataSource = dt;
                gvResults.DataBind();
                DataTable dtResultStatus = _cm.GetCRFListForTestResults(SampleId);
                if (dtResultStatus.Rows.Count == 1)
                {
                    if (dtResultStatus.Rows[0]["SampleStatus"].ToString() == "1006")
                    {
                        btnSubmit.Visible = true;
                        btnSave.Visible = false;
                    }
                    else
                    if (dtResultStatus.Rows[0]["SampleStatus"].ToString() == "27")
                    {
                        btnSave.Visible = false;
                        btnSubmit.Visible = false;
                    }
                    else
                    {
                        btnSave.Visible = true;
                        btnSubmit.Visible = false;
                    }
                }
                else
                {
                    btnSave.Visible = false;
                    btnSubmit.Visible = false;
                }
            }
            else
            {
                gvResults.DataSource = null;
                gvResults.DataBind();
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
        private void BindDetails(string SampleId, string TestResultID)
        {
            DataTable dt1 = _cm.GetTestReportDetails4(SampleId);
            if (dt1.Rows.Count > 0)
            {
                txtUlrNo.Text = dt1.Rows[0]["ULRNo"].ToString();
                txtUlrNo.Text = "LP-708-IDN";
                txtTestReportNo.Text = dt1.Rows[0]["TestReportNo"].ToString();
                txtIssueDate.Text = Convert.ToDateTime(dt1.Rows[0]["IssueDate"].ToString()).ToString("dd/MM/yyyy");
                txtDateOfMfg.Text = dt1.Rows[0]["DateOfMfg"].ToString();
                txtDateOfExp.Text = dt1.Rows[0]["DateOfExp"].ToString();
                txtTestRequired.Text = dt1.Rows[0]["TestRequired"].ToString();
                txtSampleCondition.Text = dt1.Rows[0]["SampleCondition"].ToString();
                txtSampling.Text = dt1.Rows[0]["Sampling"].ToString();
                txtSamplingMethod.Text = dt1.Rows[0]["SamplingMethod"].ToString();
                txtCustomerReference.Text = dt1.Rows[0]["CustomerReference"].ToString();
                txtRemarks.Text = dt1.Rows[0]["Remarks"].ToString();
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
                txtSampleCondition.Text = dt1.Rows[0]["SampleCondition"].ToString();
            }
            if (dt3.Rows.Count > 0)
            {
                txtCustomerName.Text = dt3.Rows[0]["CustomerName"].ToString();
                txtContactPerson.Text = dt3.Rows[0]["ContactPerson"].ToString();
                txtContactDetail.Text = dt3.Rows[0]["Mobile"].ToString();
            }
            string sYear = DateTime.Now.AddHours(12.5).ToString("yy");
            string sMonth = DateTime.Now.AddHours(12.5).ToString("MM");

            txtTestReportNo.Text = _cm.generateReportNo(SampleId);

            DataTable dt4 = _cm.getSampleIDFromSampleUID(Convert.ToInt32(SampleId));
            if (dt4.Rows.Count > 0)
            {
                Session["ID"] = Convert.ToInt32(dt4.Rows[0]["id"].ToString());
            }
            DataTable dt5 = _cm.GetTestResult(Session["ID"].ToString());
            if (dt5.Rows.Count > 0)
            {
                foreach (DataRow dr in dt5.Rows)
                {
                    string ds = dr["AnalysisStartingDate"].ToString();
                    DateTime dt;

                    if (DateTime.TryParseExact(ds, "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        ds = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    if (DateTime.TryParseExact(ds, "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        ds = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    txtAnalysisStartingDate.Text = DateTime.Parse((dr["AnalysisStartingDate"].ToString())).ToString("dd/MM/yyyy");
                    ds = dr["AnalysisCompletionDate"].ToString();
                    if (DateTime.TryParseExact(ds, "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        ds = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    if (DateTime.TryParseExact(ds, "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        ds = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    txtAnalysisCompleteDate.Text = DateTime.Parse((dr["AnalysisCompletionDate"].ToString())).ToString("dd/MM/yyyy");
                }
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int flag = 0;
            int TestResultId = 0;
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
                if (txtCustomerReference.Text == "")
                {
                    txtCustomerReference.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
                else
                {
                    txtCustomerReference.BorderColor = System.Drawing.Color.Empty;
                }
                string SampleId = Session["SampleId"].ToString();
                if (flag == 0)
                {
                    string TestReportNo = txtTestReportNo.Text;
                    string prefix = "";
                    //check TestReportNo exists
                    bool TestReportNoExists = false;
                    {
                        TestReportNoExists = _cm.checkTestReportNoExists(TestReportNo);
                        if (TestReportNoExists)
                        {
                            DataTable dtOrg = _cm.GetOrganizationDetails();
                            if (dtOrg.Rows.Count > 0)
                            {
                                prefix = dtOrg.Rows[0]["PrefixReport"].ToString();
                            }
                            txtTestReportNo.Text = _cm.generateReportNo(SampleId);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Notify", "alert(''Change in TestReport# Allocation!" + TestReportNo + "');", true);
                        }
                    }
                    _cm.sampleId = Convert.ToInt32(SampleId.ToString());
                    _cm.ulrNo = "";
                    _cm.reportNo = txtTestReportNo.Text.ToString();
                    _cm.issueDate = DateTime.Now.AddHours(12.5);// DateTime.ParseExact(DateTime.Now.AddHours(12.5).ToString(), "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    _cm.analysisStartingDate = DateTime.ParseExact(txtAnalysisStartingDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    _cm.analysisCompleteDate = DateTime.ParseExact(txtAnalysisCompleteDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    _cm.nameOfProduct = txtNameOfProduct.Text.ToString();
                    _cm.dateOfMfg = txtDateOfMfg.Text.ToString();
                    _cm.dateOfExp = txtDateOfExp.Text.ToString();
                    _cm.sampleQuality = txtSampleQuality.Text.ToString();
                    _cm.sampleCondition = txtSampleCondition.Text.ToString();
                    _cm.testRequired = txtTestRequired.Text.ToString();
                    _cm.sampling = txtSampling.Text.ToString();
                    _cm.SamplingMethod = txtSamplingMethod.Text.ToString();
                    _cm.CustomerReference = txtCustomerReference.Text;
                    _cm.remarks = txtRemarks.Text;
                    DataTable dt2 = _cm.checkTestResultDetailsExist(_cm.sampleId);
                    if (dt2.Rows.Count > 0)
                    {
                        TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultID"].ToString());
                        _cm.ulrNo = txtUlrNo.Text;
                        _cm.TestReportNo = txtTestReportNo.Text;
                        _cm.UpdateTestResultDetails(_cm);
                        Session["TestResultId"] = dt2.Rows[0]["TestResultId"].ToString();
                    }
                    else
                    {
                        TestResultId = _cm.AddTestResultDetailsMicro(_cm);
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
                    }

                    Response.Redirect("SampleList.aspx");
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
                if (ddlReportType.SelectedIndex == 0)
                {
                    flag = 1;
                    ddlReportType.BorderColor = System.Drawing.Color.Red;
                }
                else
                {
                    ddlReportType.BorderColor = System.Drawing.Color.Empty;
                }
                string SampleId = Session["SampleId"].ToString();
                if (flag == 0)
                {
                    //Session["TestResultID"] = "0";
                    TestResultId = Convert.ToInt32(Session["TestResultID"].ToString());
                    _cm.TestResultID = TestResultId;
                    _cm.ulrNo = txtUlrNo.Text.ToString();
                    _cm.ReportType = ddlReportType.SelectedItem.ToString();
                    _cm.issueDate = DateTime.Now.AddHours(12.5);// DateTime.ParseExact(txtIssueDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    _cm.AuthorizedSignatoryID = Convert.ToInt32(ddlSignatory.SelectedValue);
                    int status = _cm.UpdateTestResultDetailsMicro(_cm);

                    Session["SampleId"] = SampleId;
                    Session["TestResultId"] = TestResultId;
                    // Response.Redirect("TestReportFood.aspx?s=" + SampleId);
                    if (hdReportID.Value != ddlReportFormat.SelectedValue)
                    {
                        _cm.ReportID = ddlReportFormat.SelectedValue;
                        _cm.inquiryId = Convert.ToInt32(hdEnquiryID.Value);
                        _cm.UpdateReportType(_cm);

                    }
                    Response.Redirect("CreateReportFromTemplateFHB.aspx?s=" + SampleId + "&c=" + hdReportID.Value);
                }
            }


            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Fill highlighted fields');", true);
            }
        }

        protected void gvResults_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvResults.EditIndex = e.NewEditIndex;
            string SampleId = Session["SampleId"].ToString();
            this.BindResults(SampleId);
        }

        protected void gvResults_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            
        }

        protected void gvResults_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvResults.EditIndex = -1;
            string SampleId = Session["SampleId"].ToString();
            this.BindResults(SampleId);
        }

        protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Update")
            {
                string SampleId = Session["SampleId"].ToString();
                int index = int.Parse(e.CommandArgument.ToString());
                int CRFSampleId = int.Parse(gvResults.DataKeys[index]["CRFSampleId"].ToString());

                int rowIndex = int.Parse(e.CommandArgument.ToString());
                DropDownList ddlStatus = (DropDownList)gvResults.Rows[rowIndex].Cells[0].FindControl("ddlStatus");
                TextBox txtComments = (TextBox)gvResults.Rows[rowIndex].FindControl("txtComments");

                if (ddlStatus.SelectedIndex != 0)
                {
                    if (txtComments.Text == "") { txtComments.BorderColor = Color.Red; }
                    else
                    {
                        _cm.CRFSampleId = CRFSampleId;
                        txtComments.BorderColor = Color.Empty;
                        _cm.comments = txtComments.Text;
                        _cm.UpdateCRFSampleDetailsComments(_cm);

                        _cm.sampleStatusID = Convert.ToInt32(ddlStatus.SelectedValue);
                        
                        _cm.UpdateCRFSampleDetailsStatus(_cm);


                        DataTable dtResultStatus = _cm.GetCRFListForTestResults(SampleId);
                        if (dtResultStatus.Rows.Count == 1)
                        {
                            if (dtResultStatus.Rows[0]["SampleStatus"].ToString() == "1006")
                            {
                                btnSubmit.Visible = true;
                                btnSave.Visible = false;
                            }
                            else
                            {
                                btnSave.Visible = true;
                                btnSubmit.Visible = false;
                            }
                        }
                        else
                        {
                            btnSave.Visible = true;
                            btnSubmit.Visible = false;
                        }
                        gvResults.EditIndex = -1;
                        this.BindResults(SampleId);
                    }
                }
            
                else
                {
                    txtComments.BorderColor = Color.Empty;
                    _cm.sampleStatusID = Convert.ToInt32(ddlStatus.SelectedValue);
                    _cm.CRFSampleId = CRFSampleId;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    DataTable dtResultStatus = _cm.GetCRFListForTestResults(SampleId);
                    if (dtResultStatus.Rows.Count == 1)
                    {
                        if (dtResultStatus.Rows[0]["SampleStatus"].ToString() == "1006")
                        {
                            btnSubmit.Visible = true;
                        }
                        else
                        {
                            btnSave.Visible = true;
                        }
                    }
                    else
                    {
                        btnSave.Visible = true;
                    }
                    gvResults.EditIndex = -1;
                    this.BindResults(SampleId);
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
                //change CloneCRF status to 'Allocated'
                _cm.CRFStatusID = 19;
                _cm.CloneCRFSampleID = CloneCRFSampleID;
                _cm.UpdateCRFStatus(_cm);

                _cm.sampleStatusID = 19;//Allocated
                _cm.sampleId = Convert.ToInt32(SampleId);
                _cm.updateSampleStatus(_cm);
            }
            Response.Redirect("SampleList.aspx");
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string statusName = DataBinder.Eval(e.Row.DataItem, "StatusName")?.ToString();

                if (!string.Equals(statusName, "Result Accepted", StringComparison.OrdinalIgnoreCase))
                {
                    e.Row.BackColor = System.Drawing.Color.LightPink;
                }
            }
        }
    }
}
