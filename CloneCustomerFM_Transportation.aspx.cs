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
    public partial class CloneCustomerFM_Transportation : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    Session["DT"] = null;
                    int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                    //int ReportID = Convert.ToInt32(Request.QueryString["r"].ToString());
                    //Session["ReportID"] = ReportID;

                    DataTable dtInquiryStatus = _cm.GetEnquiryStatus(InquiryId);
                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        int inquiryStatus = Convert.ToInt32(dtInquiryStatus.Rows[0]["inquiryStatusID"].ToString());
                        int ReportID = Convert.ToInt32(dtInquiryStatus.Rows[0]["ReportID"].ToString());
                        ViewState["ReportID"] = ReportID;

                        divCRF.Visible = true;
                        divFail.Visible = false;
                        Session["InquiryID"] = InquiryId;
                        lblInquiry.Text = "ENQ" + InquiryId;
                        hdInquiryId.Value = InquiryId.ToString();
                        DataTable dt = _cm.BindInquiryCustomer(InquiryId);
                        if (dt.Rows.Count > 0)
                        {
                            txtDateOfRequest.Text = dt.Rows[0]["Created"].ToString();
                            txtCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                            Session["CustomerId"] = dt.Rows[0]["CustomerId"].ToString();
                        }
                        int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());
                        ViewState["DepartmentIdGet"] = DepartmentID;
                        string str1 = @"SELECT * from TestParameter WHERE departmentID = '" + DepartmentID + "' ";
                        SqlDataAdapter sda = new SqlDataAdapter(str1, con);
                        con.Open();
                        DataTable dt1 = new DataTable();
                        sda.Fill(dt1);
                        con.Close();

                        if (dt1.Rows.Count > 0)
                        {
                            rptTests.DataSource = dt1;
                            rptTests.DataBind();
                            btnSubmit.Visible = true;
                        }
                        BindContactPerson(InquiryId);
                        BindCustomerCRF(InquiryId);
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void BindCustomerCRF(int inquiryID)
        {
            DataTable dt = new DataTable();
            if (Session["DT"] != null)
            {
                dt = (DataTable)Session["DT"];
                rptTests.DataSource = dt;
                rptTests.DataBind();
                btnSubmit.Visible = true;
            }
            else
            {
                dt = _cm.getCustomerCRFSampleDetails(inquiryID);
                if (dt.Rows.Count > 0)
                {
                    foreach (RepeaterItem item in rptTests.Items)
                    {
                        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        {
                            CheckBox chkTestParam = (CheckBox)item.FindControl("chkTestParam");
                            HiddenField hdnTestID = (HiddenField)item.FindControl("hdnTestID");
                            HiddenField hdnTestName = (HiddenField)item.FindControl("hdnTestName");

                            // Check if this test parameter is in the selected list
                            foreach (DataRow row in dt.Rows)
                            {
                                if (hdnTestID.Value == row["TestParameterID"].ToString())
                                {
                                    chkTestParam.Checked = true;
                                    break;
                                }
                            }
                        }
                    }
                    Session["DT"] = dt;
                    btnSubmit.Visible = true;
                }
                else
                {
                    btnSubmit.Visible = false;
                }
            }
            lblSampleName.Text = dt.Rows[0]["SampleName"].ToString();
            txtSampleName.Text = dt.Rows[0]["SampleName"].ToString();
            lblBatchNo.Text = dt.Rows[0]["BatchNo"].ToString();
            txtBatchNo.Text = dt.Rows[0]["BatchNo"].ToString();
            txtSizeofSample.Text = dt.Rows[0]["SizeOfSample"].ToString();
            lblSampleQty.Text = dt.Rows[0]["SizeOfSample"].ToString();
        }

        private void BindContactPerson(int InquiryId)
        {
            DataTable dt = _cm.BindContactPerson(InquiryId);
            int PersonId = 0;
            if (dt.Rows.Count > 0)
            {
                lblPerson.Text = dt.Rows[0]["CustomerName"].ToString();
                PersonId = Convert.ToInt32(dt.Rows[0]["PersonId"].ToString());
                hdPersonID.Value = PersonId.ToString();
            }
            DataTable dt1 = _cm.BindCustomerContactPerson(PersonId);
            if (dt1.Rows.Count > 0)
            {
                txtMobile.Text = dt1.Rows[0]["Mobile"].ToString();
                txtEmail.Text = dt1.Rows[0]["Email"].ToString();
                txtPhone.Text = dt1.Rows[0]["Phone"].ToString();
                txtDesignation.Text = dt1.Rows[0]["Designation"].ToString();
                txtDepartment.Text = dt1.Rows[0]["Department"].ToString();
                txtAddress.Text = dt1.Rows[0]["AddressLine1"].ToString();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                int CloneCRFSampleID = 0;
                int CustomerCRFSampleID = 0;
                int inquiryID = Convert.ToInt32(Session["InquiryID"]);
                int flag1 = 0;

                // Add checkbox validation flag
                bool atLeastOneChecked = false;

                // Validate Sample Name
                if (txtSampleName.Text == "")
                {
                    txtSampleName.BackColor = System.Drawing.Color.LightPink;
                    txtSampleName.Attributes.Add("style", "outline: 1px solid red");
                    flag1 = 1;
                }
                else
                {
                    txtSampleName.BackColor = System.Drawing.Color.Empty;
                }

                // Check if at least one checkbox is checked
                foreach (RepeaterItem item in rptTests.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        CheckBox chkTestParam = (CheckBox)item.FindControl("chkTestParam");
                        if (chkTestParam.Checked)
                        {
                            atLeastOneChecked = true;
                            break;
                        }
                    }
                }

                if (!atLeastOneChecked)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NotifyCheckbox",
                        "alert('Please select at least one test parameter.');", true);
                    return; // Stop execution
                }

                if (flag1 == 0)
                {
                    DataTable dt1 = _cm.getCustomerCRFSampleDetails(inquiryID);
                    if (dt1.Rows.Count > 0)
                    {
                        if (Session["OrganizationID"] != null)
                        {
                            _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                            CustomerCRFSampleID = Convert.ToInt32(dt1.Rows[0]["CustomerCRFSampleID"].ToString());
                            Session["RequestId"] = Convert.ToInt32(dt1.Rows[0]["CustomerRequestID"].ToString());
                            _cm.inquiryId = inquiryID;
                            _cm.CustomerCRFSampleID = CustomerCRFSampleID;
                            CloneCRFSampleID = _cm.AddCRFClone(_cm);
                            _cm.CloneCRFSampleID = CloneCRFSampleID;

                            if (Session["DT"] != null)
                            {
                                DataTable dtSession = (DataTable)Session["DT"];
                                List<string> checkedTestIDs = new List<string>();
                                foreach (RepeaterItem item in rptTests.Items)
                                {
                                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                                    {
                                        CheckBox chkTestParam = (CheckBox)item.FindControl("chkTestParam");
                                        HiddenField hdnTestID = (HiddenField)item.FindControl("hdnTestID");

                                        if (chkTestParam.Checked)
                                        {
                                            checkedTestIDs.Add(hdnTestID.Value);
                                        }
                                    }
                                }
                                DataRow templateRow = dtSession.Rows[0];
                                foreach (string testParamID in checkedTestIDs)
                                {
                                    string testParamName = "";
                                    string query = "SELECT TestParameter FROM TestParameter WHERE TestParameterID = @TestParameterID";
                                    SqlCommand cmd = new SqlCommand(query, con);
                                    cmd.Parameters.AddWithValue("@TestParameterID", testParamID);

                                    con.Open();
                                    object result = cmd.ExecuteScalar();
                                    if (result != null)
                                    {
                                        testParamName = result.ToString();
                                    }
                                    con.Close();

                                    _cm.customerRequestId = Convert.ToInt32(Session["RequestId"]);
                                    _cm.customerId = Convert.ToInt32(Session["CustomerId"].ToString());
                                    _cm.inquiryId = inquiryID;
                                    _cm.sampleName = txtSampleName.Text;
                                    _cm.SampleType = "";
                                    _cm.batchNo = txtBatchNo.Text;
                                    _cm.testParameterID = testParamID;
                                    _cm.testParameter = testParamName;
                                    _cm.testMethod = templateRow["TestMethod"] != DBNull.Value ? templateRow["TestMethod"].ToString() : "";
                                    _cm.testSubParameterID = templateRow["TestSubParameterID"] != DBNull.Value ? templateRow["TestSubParameterID"].ToString() : "";
                                    _cm.testSubParameter = templateRow["TestSubParameter"] != DBNull.Value ? templateRow["TestSubParameter"].ToString() : "";
                                    _cm.testSubSubParameterID = templateRow["TestSubSubParameterID"] != DBNull.Value ? templateRow["TestSubSubParameterID"].ToString() : "";
                                    _cm.testSubSubParameter = templateRow["TestSubSubParameter"] != DBNull.Value ? templateRow["TestSubSubParameter"].ToString() : "";
                                    _cm.specification = txtSpecification.Text;
                                    _cm.testsRequested = "";
                                    _cm.CASNo = templateRow["CASNo"] != DBNull.Value ? templateRow["CASNo"].ToString() : "";
                                    _cm.MDL = templateRow["MDL"] != DBNull.Value ? templateRow["MDL"].ToString() : "";
                                    _cm.CodeCombination = templateRow["CodeCombination"] != DBNull.Value ? templateRow["CodeCombination"].ToString() : "";
                                    _cm.UOMName = templateRow["UnitOfMeasurement"] != DBNull.Value ? templateRow["UnitOfMeasurement"].ToString() : "";
                                    _cm.sizeOfSample = txtSizeofSample.Text;
                                    _cm.remarks = txtRemarks.Text;
                                    _cm.CRFID = 1;
                                    _cm.CloneCRFSampleID = CloneCRFSampleID;
                                    _cm.RequirementID = templateRow["RequirementID"] != DBNull.Value ? templateRow["RequirementID"].ToString() : "";
                                    _cm.Requirement = templateRow["Requirement"] != DBNull.Value ? templateRow["Requirement"].ToString() : "";
                                    _cm.sampleStatusID = 28;
                                    _cm.ReportID = ViewState["ReportID"].ToString();
                                    _cm.AddCRFSampleDetails_Transport(_cm);
                                }
                                _cm.UpdateCRFSampleDetailsStatus(_cm);
                                _cm.CRFStatusID = 28;
                                _cm.CloneCRFSampleID = CloneCRFSampleID;
                                _cm.UpdateCRFStatus(_cm);
                            }
                        }
                        ClearAll();
                        Response.Redirect(String.Format("CreateRequestCloneFM_Transport.aspx?ID1={0}&ID2={1}", Session["InquiryID"], CloneCRFSampleID), false);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert('Fill Highlighted Fields.')", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify",
                        "alert('Fill highlighted fields');", true);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message.Replace("'", "\\'");
                string script = $"alert('Error: {errorMessage}');";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NotifyError", script, true);
            }
        }

        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            try
            {
                int CloneCRFSampleID = 0;
                int CustomerCRFSampleID = 0;
                int inquiryID = Convert.ToInt32(Session["InquiryID"]);
                int flag1 = 0;

                if (txtSampleName.Text == "")
                {
                    txtSampleName.BackColor = System.Drawing.Color.LightPink;
                    txtSampleName.Attributes.Add("style", "outline: 1px solid red");
                    flag1 = 1;
                }
                else
                {
                    txtSampleName.BackColor = System.Drawing.Color.Empty;
                }

                if (flag1 == 0)
                {
                    DataTable dt1 = _cm.getCustomerCRFSampleDetails(inquiryID);
                    if (dt1.Rows.Count > 0)
                    {
                        if (Session["OrganizationID"] != null)
                        {
                            _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                            CustomerCRFSampleID = Convert.ToInt32(dt1.Rows[0]["CustomerCRFSampleID"].ToString());
                            Session["RequestId"] = Convert.ToInt32(dt1.Rows[0]["CustomerRequestID"].ToString());
                            _cm.inquiryId = inquiryID;
                            _cm.CustomerCRFSampleID = CustomerCRFSampleID;
                            CloneCRFSampleID = _cm.AddCRFClone(_cm);
                            _cm.CloneCRFSampleID = CloneCRFSampleID;

                            if (Session["DT"] != null)
                            {
                                DataTable dtSession = (DataTable)Session["DT"];
                                List<string> checkedTestIDs = new List<string>();
                                foreach (RepeaterItem item in rptTests.Items)
                                {
                                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                                    {
                                        CheckBox chkTestParam = (CheckBox)item.FindControl("chkTestParam");
                                        HiddenField hdnTestID = (HiddenField)item.FindControl("hdnTestID");

                                        if (chkTestParam.Checked)
                                        {
                                            checkedTestIDs.Add(hdnTestID.Value);
                                        }
                                    }
                                }
                                DataRow templateRow = dtSession.Rows[0];
                                foreach (string testParamID in checkedTestIDs)
                                {
                                    string testParamName = "";
                                    string query = "SELECT TestParameter FROM TestParameter WHERE TestParameterID = @TestParameterID";
                                    SqlCommand cmd = new SqlCommand(query, con);
                                    cmd.Parameters.AddWithValue("@TestParameterID", testParamID);

                                    con.Open();
                                    object result = cmd.ExecuteScalar();
                                    if (result != null)
                                    {
                                        testParamName = result.ToString();
                                    }
                                    con.Close();

                                    _cm.customerRequestId = Convert.ToInt32(Session["RequestId"]);
                                    _cm.customerId = Convert.ToInt32(Session["CustomerId"].ToString());
                                    _cm.inquiryId = inquiryID;
                                    _cm.sampleName = txtSampleName.Text;
                                    _cm.SampleType = "";
                                    _cm.batchNo = txtBatchNo.Text;
                                    _cm.testParameterID = testParamID;
                                    _cm.testParameter = testParamName;
                                    _cm.testMethod = templateRow["TestMethod"] != DBNull.Value ? templateRow["TestMethod"].ToString() : "";
                                    _cm.testSubParameterID = templateRow["TestSubParameterID"] != DBNull.Value ? templateRow["TestSubParameterID"].ToString() : "";
                                    _cm.testSubParameter = templateRow["TestSubParameter"] != DBNull.Value ? templateRow["TestSubParameter"].ToString() : "";
                                    _cm.testSubSubParameterID = templateRow["TestSubSubParameterID"] != DBNull.Value ? templateRow["TestSubSubParameterID"].ToString() : "";
                                    _cm.testSubSubParameter = templateRow["TestSubSubParameter"] != DBNull.Value ? templateRow["TestSubSubParameter"].ToString() : "";
                                    _cm.specification = txtSpecification.Text;
                                    _cm.testsRequested = "";
                                    _cm.CASNo = templateRow["CASNo"] != DBNull.Value ? templateRow["CASNo"].ToString() : "";
                                    _cm.MDL = templateRow["MDL"] != DBNull.Value ? templateRow["MDL"].ToString() : "";
                                    _cm.CodeCombination = templateRow["CodeCombination"] != DBNull.Value ? templateRow["CodeCombination"].ToString() : "";
                                    _cm.UOMName = templateRow["UnitOfMeasurement"] != DBNull.Value ? templateRow["UnitOfMeasurement"].ToString() : "";
                                    _cm.sizeOfSample = txtSizeofSample.Text;
                                    _cm.remarks = txtRemarks.Text;
                                    _cm.CRFID = 1;
                                    _cm.CloneCRFSampleID = CloneCRFSampleID;
                                    _cm.RequirementID = templateRow["RequirementID"] != DBNull.Value ? templateRow["RequirementID"].ToString() : "";
                                    _cm.Requirement = templateRow["Requirement"] != DBNull.Value ? templateRow["Requirement"].ToString() : "";
                                    _cm.sampleStatusID = 28;
                                    _cm.ReportID = ViewState["ReportID"].ToString();
                                    _cm.AddCRFSampleDetails_Transport(_cm);
                                }
                                _cm.UpdateCRFSampleDetailsStatus(_cm);
                                _cm.CRFStatusID = 28;
                                _cm.CloneCRFSampleID = CloneCRFSampleID;
                                _cm.UpdateCRFStatus(_cm);
                            }
                        }
                        ClearAll();
                        Response.Redirect(String.Format("CreateRequestCloneFM_Transport.aspx?ID1={0}&ID2={1}", Session["InquiryID"], CloneCRFSampleID), false);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert('Fill Highlighted Fields.')", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Fill highlighted fields');", true);
                }

            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message.Replace("'", "\\'");
                string script = $"alert('Error: {errorMessage}');";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NotifyError", script, true);
                //string msg = ex.Message;
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("CustomerInquiryView.aspx");
        }

        private void ClearAll()
        {
            txtDateOfRequest.Text = "";
            lblInquiry.Text = "";
            txtCustomer.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            txtDesignation.Text = "";
            txtDepartment.Text = "";

            //gvCustomerCRF.DataSource = null;
            //gvCustomerCRF.DataBind();
            //if (gvCustomerCRF.Rows.Count == 0)
            //{
            //    btnSubmit.Visible = false;
            //}
            //else
            //{
            btnSubmit.Visible = true;
            //}
            Session["DT"] = null;
        }


    }
}