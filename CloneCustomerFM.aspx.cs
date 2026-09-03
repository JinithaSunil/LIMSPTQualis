using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CloneCustomerFM : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    Session["DT"] = null;
                    int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                    int ReportID = Convert.ToInt32(Request.QueryString["r"].ToString());
                    Session["ReportID"] = ReportID;

                    DataTable dtInquiryStatus = _cm.GetEnquiryStatus(InquiryId);
                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        int inquiryStatus = Convert.ToInt32(dtInquiryStatus.Rows[0]["inquiryStatusID"].ToString());
                       
                        divCRF.Visible = true;
                        divFail.Visible = false;
                        Session["InquiryID"] = InquiryId;
                        lblInquiry.Text = "ENQ" + InquiryId;
                        hdInquiryId.Value = InquiryId.ToString();
                        DataTable dt = _cm.BindInquiryCustomer(InquiryId);
                        int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            BindReportType(DepartmentID);
                            txtDateOfRequest.Text = dt.Rows[0]["Created"].ToString();
                            txtCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                            Session["CustomerId"] = dt.Rows[0]["CustomerId"].ToString();
                            hdCustomerMasterID.Value = dt.Rows[0]["CustomerMasterId"].ToString();
                        }
                        BindContactPerson(InquiryId);
                        BindCustomerCRF(InquiryId);
                        BindRequirement();
                        
                    }
                    if (gvCustomerCRF.Rows.Count == 0)
                    {
                        btnSubmit.Visible = false;
                        btnCancel.Visible = true;
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
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
                ddlReportFormat.Items.FindByValue(lblReportID.Text).Selected = true;
            }
        }
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod]
        public static List<string> GetTestMethods(string prefixText, int count)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandText = "Select * from TestMethod where testmethod like " + " @Search + '%'";
                    com.Parameters.AddWithValue("@Search", prefixText);
                    com.Connection = con;
                    con.Open();
                    List<string> TestMethod = new List<string>();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            TestMethod.Add(sdr["TestMethod"].ToString());
                        }
                    }
                    con.Close();
                    return TestMethod;


                }

            }

        }
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static List<string> GetTestParameters(string prefixText, int count)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlCommand com = new SqlCommand())
                {
                    //com.CommandText = "Select distinct TestParameter from TestParameter where TestParameter like " + " @Search + '%' and isRequirement=0";
                    com.CommandText = "Select distinct TestParameter from TestParameter where TestParameter like " + " @Search + '%' ";
                    com.Parameters.AddWithValue("@Search", prefixText);
                    com.Connection = con;
                    con.Open();
                    List<string> TestParameter = new List<string>();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            TestParameter.Add(sdr["TestParameter"].ToString());
                        }
                    }
                    con.Close();
                    return TestParameter;


                }

            }

        }
     
        private void BindCustomerCRF(int inquiryID)
        {
            DataTable dt = new DataTable();
            if (Session["DT"] != null)
            {
                dt = (DataTable)Session["DT"];
                gvCustomerCRF.DataSource = (DataTable)Session["DT"];
                gvCustomerCRF.DataBind();
            }
            else
            {
                dt = _cm.getCustomerCRFSampleDetails(inquiryID);
                if (dt.Rows.Count > 0)
                {
                    Session["DT"] = dt;
                    gvCustomerCRF.DataSource = (DataTable)Session["DT"];
                    gvCustomerCRF.DataBind();
                }
                else
                {
                    gvCustomerCRF.DataSource = null;
                    gvCustomerCRF.DataBind();
                }
            }
            lblSampleName.Text = dt.Rows[0]["SampleName"].ToString();
           // txtSampleName.Text = dt.Rows[0]["SampleName"].ToString();
            lblBatchNo.Text = dt.Rows[0]["BatchNo"].ToString();
            txtBatchNo.Text = dt.Rows[0]["BatchNo"].ToString();
            txtSizeofSample.Text = dt.Rows[0]["SizeOfSample"].ToString();
        }
        private void BindRequirement()
        {
            DataTable dt = _cm.getRequirement();
            if (dt.Rows.Count > 0)
            {
                ddlRequirement.DataSource = dt;
                ddlRequirement.DataTextField = "Requirement";
                ddlRequirement.DataValueField = "RequirementID";
                ddlRequirement.DataBind();
                ddlRequirement.Items.Insert(0, new ListItem("Select Requirement"));
            }

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
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            AddDetails();
        }
        private void AddDetails()
        {
            int flag = 0;
            if (txtSampleName.Text == "")
            {
                txtSampleName.BackColor = System.Drawing.Color.LightPink;
                txtSampleName.Attributes.Add("style", "outline: 1px solid red");
                flag = 1;
            }
            else
            {
                txtSampleName.BackColor = System.Drawing.Color.Empty;
            }
            if (txtBatchNo.Text == "")
            {
                txtBatchNo.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtBatchNo.BackColor = System.Drawing.Color.Empty;
            }
            if (txtTestParameter.Text == "")
            {
                txtTestParameter.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtTestParameter.BackColor = System.Drawing.Color.Empty;
            }

            if (txtSizeofSample.Text == "")
            {
                txtSizeofSample.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtSizeofSample.BackColor = System.Drawing.Color.Empty;
            }

            if (flag == 0)
            {
                DataTable dt = new DataTable();
                if (Session["DT"] != null)
                {
                    dt = (DataTable)Session["DT"];
                }
                else
                {
                    dt.Columns.Add("SampleName");
                    dt.Columns.Add("BatchNo");
                    dt.Columns.Add("TestMethod");
                    dt.Columns.Add("RequirementID");
                    dt.Columns.Add("Requirement");
                    dt.Columns.Add("TestParameterID");
                    dt.Columns.Add("TestParameter");
                    dt.Columns.Add("TestSubParameterID");
                    dt.Columns.Add("TestSubParameter");
                    dt.Columns.Add("TestSubSubParameterID");
                    dt.Columns.Add("TestSubSubParameter");
                    dt.Columns.Add("Specification");
                    dt.Columns.Add("UnitOfMeasurement");
                    dt.Columns.Add("CASNo");
                    dt.Columns.Add("MDL");
                    dt.Columns.Add("CodeCombination");
                    dt.Columns.Add("SizeofSample");
                    dt.Columns.Add("Remarks");
                }
                //DataTable dtSubParameter = _cm.getTestSubParameter(txtTestParameter.Text);
                lblSampleName.Text = txtSampleName.Text;
                lblBatchNo.Text = txtSizeofSample.Text;
                lblSampleQty.Text = txtSizeofSample.Text;

                if (ddlRequirement.SelectedIndex != 0)
                {
                    if (ddlRequirement.SelectedItem.ToString() != hdRequirement.Value.ToString())
                    {
                        lblRequirement.Text += ddlRequirement.SelectedItem.ToString() + ",";
                        hdRequirement.Value = ddlRequirement.SelectedItem.ToString();
                    }
                }

                DataRow dr = dt.NewRow();
                dr["SampleName"] = txtSampleName.Text.ToString();
                dr["BatchNo"] = txtBatchNo.Text.ToString();
                dr["TestMethod"] = ddlTestMethod.SelectedItem.ToString();
                dr["TestParameterID"] = lblTestParameterID.Text;
                dr["TestParameter"] = txtTestParameter.Text;

                if (ddlTestSubParameter.Items.Count > 0)
                {
                    dr["TestSubParameterID"] = ddlTestSubParameter.SelectedValue.ToString();
                    dr["TestSubParameter"] = ddlTestSubParameter.SelectedItem.ToString();
                }
                else
                {
                    dr["TestSubParameterID"] = 0;
                    dr["TestSubParameter"] = "";
                }

                if (ddlTestSubSubParameter.Items.Count > 0)
                {
                    dr["TestSubSubParameterID"] = ddlTestSubSubParameter.SelectedValue.ToString();
                    dr["TestSubSubParameter"] = ddlTestSubSubParameter.SelectedItem.ToString();
                }
                else
                {
                    dr["TestSubSubParameterID"] = 0;
                    dr["TestSubSubParameter"] = "";
                }

                dr["Specification"] = txtSpecification.Text.ToString();
                dr["UnitOfMeasurement"] = ddlUOM.SelectedItem.ToString();

                if (ddlRequirement.SelectedIndex != 0)
                {
                    dr["Requirement"] = ddlRequirement.SelectedItem.ToString();
                    dr["RequirementID"] = ddlRequirement.SelectedValue.ToString();
                }
                else
                {
                    dr["Requirement"] = "";
                    dr["RequirementID"] = 0;
                }

                dr["LOD"] = txtLOD.Text;
                dr["LOQ"] = txtLOQ.Text;
           
                dr["SizeofSample"] = txtSizeofSample.Text.ToString();
                dr["Remarks"] = txtRemarks.Text.ToString();
                dt.Rows.Add(dr);

                gvCustomerCRF.DataSource = dt;
                gvCustomerCRF.DataBind();

                if (gvCustomerCRF.Rows.Count == 0)
                {
                    btnSubmit.Visible = false;
                }
                else
                {
                    btnSubmit.Visible = true;
                }

                Session["DT"] = dt;
                Clear();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Fill highlighted fields');", true);
            }
        }
   
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int CloneCRFSampleID = 0; int CustomerCRFSampleID = 0;
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
                        DataTable dt = (DataTable)Session["DT"];
                        foreach (DataRow dr in dt.Rows)
                        {
                            _cm.customerRequestId = Convert.ToInt32(Session["RequestId"]);
                            _cm.customerId = Convert.ToInt32(Session["CustomerId"].ToString());
                            _cm.inquiryId = inquiryID;
                            _cm.sampleName = txtSampleName.Text;
                            _cm.SampleType = "";
                            _cm.batchNo = dr["BatchNo"].ToString();
                            _cm.testMethod = dr["TestMethod"].ToString();
                            _cm.testParameterID = dr["TestParameterID"].ToString();
                            _cm.testParameter = dr["TestParameter"].ToString();
                            _cm.testSubParameterID = dr["TestSubParameterID"].ToString();
                            _cm.testSubParameter = dr["TestSubParameter"].ToString();
                            _cm.testSubSubParameterID = dr["TestSubSubParameterID"].ToString();
                            _cm.testSubSubParameter = dr["TestSubSubParameter"].ToString();
                            _cm.specification = dr["Specification"].ToString();
                            _cm.testsRequested = "";
                            _cm.LOD = dr["LOD"].ToString();
                            _cm.LOQ = dr["LOQ"].ToString();
                            _cm.UOMName = dr["UnitOfMeasurement"].ToString();
                            _cm.sizeOfSample = dr["SizeofSample"].ToString();
                            _cm.remarks = dr["Remarks"].ToString();
                            _cm.CRFID = 1;
                            _cm.CloneCRFSampleID = CloneCRFSampleID;
                            _cm.RequirementID = dr["RequirementID"].ToString();
                            _cm.Requirement = dr["Requirement"].ToString();
                            _cm.sampleStatusID = 28;
                            if (dr["RequirementID"].ToString() != "")
                                if (dr["RequirementID"].ToString() != "0")
                                {
                                    DataTable dt2 = _cm.getAdditionalParameterDetails(_cm);
                                    if ((dt2.Rows.Count > 0) && (dt2.Rows[0]["n"].ToString() != ""))
                                    {
                                        int counter = Convert.ToInt32(dt2.Rows[0]["n"].ToString());
                                        for (int i = 0; i < counter; i++)
                                        {
                                            _cm.AddCRFSampleDetails(_cm);
                                        }
                                    }
                                    else
                                    {
                                        _cm.AddCRFSampleDetails(_cm);
                                    }
                                }
                                else
                                {
                                    _cm.AddCRFSampleDetails(_cm);
                                }
                            // _cm.AddCRFSampleDetails(_cm);
                        }
                        _cm.UpdateCRFSampleDetailsStatus(_cm);

                        //change CloneCRF Status to CRF More Info
                        _cm.CRFStatusID = 28;
                        _cm.CloneCRFSampleID = CloneCRFSampleID;
                        _cm.UpdateCRFStatus(_cm);
                    }
                }

                ClearAll();

               // Response.Redirect(String.Format("CreateRequestCloneFM.aspx?ID1={0}&ID2={1}", Session["InquiryID"], CloneCRFSampleID));
                    string url = String.Format("CreateRequestCloneFM.aspx?ID1={0}&ID2={1}", Session["InquiryID"], CloneCRFSampleID);
                    string redirectUrl = "CustomerInquiryView.aspx";
                    string script = $@"alert('Cloned successfully!');
                                               window.open('{url}', '_blank');         
                                               window.location.href = '{redirectUrl}'; 
                                              ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenTabAndRedirect", script, true);
                }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }
            }
              else
              {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Fill highlighted fields');", true);
              }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("CustomerInquiryView.aspx");
        }
        private void Clear()
        {
            //txtSampleName.Text = "";
            //txtBatchNo.Text = "";
            txtTestParameter.Text = "";
            txtSpecification.Text = "";
            txtLOQ.Text = "";
            txtLOD.Text = "";
            ddlUOM.Items.Clear();
            ddlTestMethod.Items.Clear();
            ddlTestSubParameter.Items.Clear();
            ddlTestSubSubParameter.Items.Clear();
            txtRemarks.Text = "";
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

            gvCustomerCRF.DataSource = null;
            gvCustomerCRF.DataBind();
            if (gvCustomerCRF.Rows.Count == 0)
            {
                btnSubmit.Visible = false;
            }
            else
            {
                btnSubmit.Visible = true;
            }
            Session["DT"] = null;
        }

        protected void gvCustomerCRF_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCustomerCRF.EditIndex = -1;
            BindCustomerCRF(Convert.ToInt32(hdInquiryId.Value));
        }

        protected void gvCustomerCRF_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int row = e.RowIndex;
            int r = 0;
            DataTable dt = (DataTable)Session["DT"];
            foreach (DataRow row1 in dt.Rows)
            {
                if (r.ToString() == row.ToString())
                {
                    row1.Delete();
                }
                r++;
            }
            dt.AcceptChanges();
            Session["DT"] = dt;
            gvCustomerCRF.DataSource = dt;
            gvCustomerCRF.DataBind();
        }

        protected void gvCustomerCRF_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCustomerCRF.EditIndex = e.NewEditIndex;
            BindCustomerCRF(Convert.ToInt32(hdInquiryId.Value));
        }

        protected void gvCustomerCRF_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int rowIndex = e.RowIndex;
                GridViewRow row = gvCustomerCRF.Rows[rowIndex];

                string testMethod = ((TextBox)row.Cells[3].Controls[0]).Text; 
                string unitOfMeasurement = ((TextBox)row.Cells[4].Controls[0]).Text; 
                string lod = ((TextBox)row.Cells[5].Controls[0]).Text; 
                string loq = ((TextBox)row.Cells[6].Controls[0]).Text; 
                string specification = ((TextBox)row.Cells[7].Controls[0]).Text; 
                string remarks = ((TextBox)row.Cells[8].Controls[0]).Text; 
                string testParameterID = ((Label)row.FindControl("lblTestParameterID")).Text;
                string testSubParameterID = ((Label)row.FindControl("lblTestSubParameterID")).Text;
                string testSubSubParameterID = ((Label)row.FindControl("lblTestSubSubParameterID")).Text;
                string testParameter = ((Label)row.FindControl("lblTestParameter")).Text;
                string testSubParameter = ((Label)row.FindControl("lblTestSubParameter")).Text;
                string testSubSubParameter = ((Label)row.FindControl("lblTestSubSubParameter")).Text;

                DataTable dt = (DataTable)Session["DT"];

                if (dt != null && rowIndex < dt.Rows.Count)
                {
                    DataRow dr = dt.Rows[rowIndex];

                    dr["TestMethod"] = testMethod;
                    dr["UnitOfMeasurement"] = unitOfMeasurement;
                    dr["LOD"] = lod;
                    dr["LOQ"] = loq;
                    dr["Specification"] = specification;
                    dr["Remarks"] = remarks;
                    dr["TestParameterID"] = testParameterID;
                    dr["TestSubParameterID"] = testSubParameterID;
                    dr["TestSubSubParameterID"] = testSubSubParameterID;
                    dr["TestParameter"] = testParameter;
                    dr["TestSubParameter"] = testSubParameter;
                    dr["TestSubSubParameter"] = testSubSubParameter;

                    Session["DT"] = dt;
                }

                // Exit edit mode and rebind
                gvCustomerCRF.EditIndex = -1;
                BindCustomerCRF(Convert.ToInt32(hdInquiryId.Value));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        protected void txtTestParameter_TextChanged(object sender, EventArgs e)
        {
            txtLOD.Text = ""; txtLOQ.Text = "";
            string param = ""; DataTable dt = new DataTable();
            if (txtTestParameter.Text != "")
            {
                param = txtTestParameter.Text;

                DataTable dtParam = _cm.getParameter_Chemical(param);

                if (dtParam.Rows.Count > 0)
                {
                    lblTestParameterID.Text = dtParam.Rows[0]["TestParameterID"].ToString();
                }
                                
                dt = _cm.getTestSubParameters_New(param);
                if (dt.Rows.Count > 0)
                {
                    ddlTestSubParameter.DataSource = dt;
                    ddlTestSubParameter.DataTextField = "TestSubParameter";
                    ddlTestSubParameter.DataValueField = "TestSubParameterID";
                    ddlTestSubParameter.DataBind();
                    ddlTestSubParameter.Items.Insert(0, "Select Sub Parameter");
                }
                else
                {
                    ddlTestSubParameter.DataSource = null;
                    ddlTestSubParameter.DataBind();

                    dt = _cm.GetMethodOfTestParameter(param);
                    if (dt.Rows.Count > 0)
                    {
                        ddlTestMethod.DataSource = dt;
                        ddlTestMethod.DataTextField = "TestMethod";
                        ddlTestMethod.DataValueField = "TestMethodID";
                        ddlTestMethod.DataBind();
                        ddlTestMethod.Items.Insert(0, "Select Method");

                    }
                }
                //if(lblTestParameterID != null) {
                //    lblTestParameterID.Text = dt.Rows[0]["TestParameterID"].ToString();
                //}

            }

        }
        protected void gvCustomerCRF_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
        protected void ddlTestSubParameter_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ddlTestSubParameter.SelectedIndex != 0)
            {
                string param = ddlTestSubParameter.SelectedItem.ToString();
                //DataTable dt = _cm.getTestSubSubParameters(param);
                DataTable dt = _cm.getTestSubSubParameters_New(param);

                if (dt.Rows.Count > 0)
                {
                    ddlTestSubSubParameter.DataSource = dt;
                    ddlTestSubSubParameter.DataTextField = "TestSubSubParameter";
                    ddlTestSubSubParameter.DataValueField = "TestSubSubParameterID";
                    ddlTestSubSubParameter.DataBind();
                    ddlTestSubSubParameter.Items.Insert(0, "Select Sub Parameter");
                    lblTestSubParameterID.Text = ddlTestSubParameter.SelectedValue.ToString();
                    if ((ddlTestSubSubParameter.SelectedIndex == 0) || (ddlTestSubSubParameter.SelectedIndex == -1))
                    {
                        dt = _cm.GetMethodOfTestSubParameter(txtTestParameter.Text, param);
                        if (dt.Rows.Count > 0)
                        {
                            ddlTestMethod.DataSource = dt;
                            ddlTestMethod.DataTextField = "TestMethod";
                            ddlTestMethod.DataValueField = "TestMethodID";
                            ddlTestMethod.DataBind();
                            ddlTestMethod.Items.Insert(0, "Select Method");
                        }
                    }
                    else
                    {
                        dt = _cm.GetMethodOfTestSubSubParameter(ddlTestSubParameter.SelectedItem.ToString(), ddlTestSubSubParameter.SelectedItem.ToString());
                        if (dt.Rows.Count > 0)
                        {
                            ddlTestMethod.DataSource = dt;
                            ddlTestMethod.DataTextField = "TestMethod";
                            ddlTestMethod.DataValueField = "TestMethodID";
                            ddlTestMethod.DataBind();
                            ddlTestMethod.Items.Insert(0, "Select Method");
                        }
                    }
                }
                else
                {
                    ddlTestSubSubParameter.ClearSelection();
                    ddlTestSubSubParameter.DataSource = dt;
                    ddlTestSubSubParameter.DataBind();

                    dt = _cm.GetMethodOfTestSubParameter(txtTestParameter.Text, param);
                    if (dt.Rows.Count > 0)
                    {
                        ddlTestMethod.DataSource = dt;
                        ddlTestMethod.DataTextField = "TestMethod";
                        ddlTestMethod.DataValueField = "TestMethodID";
                        ddlTestMethod.DataBind();
                        ddlTestMethod.Items.Insert(0, "Select Method");
                    }
                }
            }
        }
        protected void ddlTestSubSubParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTestSubSubParameter.SelectedIndex != 0)
            {
                string param = ddlTestSubSubParameter.SelectedItem.ToString();
                DataTable dt = _cm.GetMethodOfTestSubSubParameter(ddlTestSubParameter.SelectedItem.ToString(), param);
                if (dt.Rows.Count > 0)
                {
                    ddlTestMethod.DataSource = dt;
                    ddlTestMethod.DataTextField = "TestMethod";
                    ddlTestMethod.DataValueField = "TestMethodID";
                    ddlTestMethod.DataBind();
                    ddlTestMethod.Items.Insert(0, "Select Method");
                    lblTestSubParameterID.Text = ddlTestSubParameter.SelectedValue;
                }
            }
            else
            if (ddlTestSubParameter.SelectedIndex != 0)
            {
                string param = ddlTestSubSubParameter.SelectedItem.ToString();
                DataTable dt = _cm.GetMethodOfTestSubParameter(txtTestParameter.Text, param);
                if (dt.Rows.Count > 0)
                {
                    ddlTestMethod.DataSource = dt;
                    ddlTestMethod.DataTextField = "TestMethod";
                    ddlTestMethod.DataValueField = "TestMethodID";
                    ddlTestMethod.DataBind();
                    ddlTestMethod.Items.Insert(0, "Select Method");
                    lblTestSubParameterID.Text = ddlTestSubParameter.SelectedValue;
                }
            }
        }


        protected void ddlTestMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            string param = "";
            string method = ddlTestMethod.SelectedItem.ToString();
            DataTable dt = new DataTable();
            if (ddlTestSubSubParameter.SelectedIndex > 0)
            {
                param = ddlTestSubSubParameter.SelectedItem.ToString();
                dt = _cm.GetUOMOfTestSubSubParameter(param, ddlTestSubParameter.SelectedValue.ToString());
            }
            else
            if (ddlTestSubParameter.SelectedIndex > 0)
            {
                param = ddlTestSubParameter.SelectedItem.ToString();
                dt = _cm.GetUOMOfTestSubParameter(txtTestParameter.Text, param);
            }
            else
            {
                param = txtTestParameter.Text;
                dt = _cm.GetUOMOfTestParameter(param, method);
            }
            if (dt.Rows.Count > 0)
            {
                ddlUOM.DataSource = dt;
                ddlUOM.DataTextField = "UOMName";
                ddlUOM.DataValueField = "UOMID";
                ddlUOM.DataBind();
                ddlUOM.Items.Insert(0, "Select UOM");
            }
            else
            {
                ddlUOM.DataSource = null;
                ddlUOM.DataBind();
            }
        }

        protected void ddlUOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((txtTestParameter.Text != "") && (ddlTestMethod.SelectedIndex != 0) && (ddlUOM.SelectedIndex != 0))
            {
                DataTable dt = new DataTable();
                _cm.testParameter = txtTestParameter.Text;
                _cm.testMethod = ddlTestMethod.SelectedItem.ToString();
                _cm.UOMName = ddlUOM.SelectedItem.ToString();
                if (ddlTestSubSubParameter.SelectedIndex > 0)
                {
                    _cm.testSubSubParameter = ddlTestSubSubParameter.SelectedItem.ToString();
                    dt = _cm.GetTestSubSubParameterDetails(_cm);
                }
                else
                if (ddlTestSubParameter.SelectedIndex > 0)
                {
                    _cm.testSubParameter = ddlTestSubParameter.SelectedItem.ToString();
                    dt = _cm.GetTestSubParameterDetails(_cm);
                }
                else
                {
                    _cm.testSubParameter = "";
                    dt = _cm.GetTestParameterDetails(_cm);
                }
                if (dt.Rows.Count > 0)
                {
                    txtLOD.Text = dt.Rows[0]["LOD"].ToString();
                    txtLOQ.Text = dt.Rows[0]["LOQ"].ToString();
                    txtSpecification.Text = dt.Rows[0]["Specification"].ToString();
                }
            }
        }

    }
}
