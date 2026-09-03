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
    public partial class CustomerFMN : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    Session["OrganizationID"] = 1;
                    BindServiceRequired();
                    int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                    DataTable dtInquiryStatus = _cm.GetEnquiryStatus(InquiryId);
                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        int inquiryStatus = Convert.ToInt32(dtInquiryStatus.Rows[0]["inquiryStatusID"].ToString());
                        if (inquiryStatus == 5)
                        {
                            int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());

                            if (DepartmentID == 4)
                            {
                                string url = "CustomerFMN_Chemical.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else
                            if (DepartmentID == 4007)
                            {
                                string url = "CustomerFMN_Mechanical.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else
                            if (DepartmentID == 4006)
                            {
                                string url = "CustomerFMN_PV.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else
                            if (DepartmentID == 4008)
                            {
                                string url = "CustomerFMN_HomeAppliances.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 4009)
                            {
                                string url = "CustomerFMN_LED.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
							else if (DepartmentID == 3006)
                            {
                                string url = "CustomerFMN_ElectricalAccessories.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 1)
                            {
                                string url = "CustomerFMN_Transportation.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 3)
                            {
                                string url = "CustomerFMN_ToyAndBabyWear.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 5020)
                            {
                                string url = "CustomerFMN_PhysicalSoftline.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 5015)
                            {
                                string url = "CustomerFMN_LSSM.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else
                            {
                                divCRF.Visible = true;
                                divFail.Visible = false;
                                Session["InquiryID"] = InquiryId;
                                lblInquiry.Text = "ENQ" + InquiryId;
                                hdInquiryId.Value = InquiryId.ToString();
                                DataTable dt = _cm.BindInquiryCustomer(InquiryId);
                                if (dt.Rows.Count > 0)
                                {
                                    lblReportID.Text = dt.Rows[0]["ReportID"].ToString();
                                    BindReportType(DepartmentID);

                                    txtDateOfRequest.Text = dt.Rows[0]["Created"].ToString();
                                    txtCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                                    txtVAT.Text = dt.Rows[0]["VAT"].ToString();
                                    txtAddressLine1.Text = dt.Rows[0]["AddressLine1"].ToString();
                                    txtAddressLine2.Text = dt.Rows[0]["AddressLine2"].ToString() + "," + dt.Rows[0]["State"].ToString();
                                    txtPincode.Text = dt.Rows[0]["Pincode"].ToString();
                                    txtCountry.Text = dt.Rows[0]["Country"].ToString();
                                    Session["CustomerId"] = dt.Rows[0]["CustomerId"].ToString();
                                    hdCustomerMasterID.Value = dt.Rows[0]["CustomerMasterId"].ToString();
                                    BindContactPerson(InquiryId);
                                    BindRequirement();
                                }

                            }
                        }
                        else
                        {
                            divCRF.Visible = false;
                            divFail.Visible = true;
                        }
                    }
                    if (gvCRF.Rows.Count == 0)
                    {
                        btnSubmit.Visible = false;
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        private void BindServiceRequired()
        {
            DataTable dt = _cm.GetBindServiceRequired();
            if (dt.Rows.Count > 0)
            {
                radServiceRequired.DataSource = dt;
                radServiceRequired.DataTextField = "ServiceRequired";
                radServiceRequired.DataValueField = "ID";
                radServiceRequired.DataBind();
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
        [System.Web.Services.WebMethod]
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
        private void BindTestParameter()
        {

        }
        private void BindContactPerson(int InquiryId)
        {
            DataTable dt = _cm.BindContactPerson(InquiryId);
            if (dt.Rows.Count > 0)
            {
                ddlContactPerson.DataSource = dt;
                ddlContactPerson.DataTextField = "CustomerName";
                ddlContactPerson.DataValueField = "PersonId";
                ddlContactPerson.DataBind();
                ddlContactPerson.Items.Insert(0, new ListItem("Select Person"));
            }

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

        protected void ddlContactPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            int PersonId = 0;
            if (ddlContactPerson.SelectedIndex != 0)
            {
                PersonId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                DataTable dt = _cm.BindCustomerContactPerson(PersonId);
                if (dt.Rows.Count > 0)
                {
                    txtMobile.Text = dt.Rows[0]["Mobile"].ToString();
                    txtContactEmail.Text = dt.Rows[0]["Email"].ToString();
                    txtFax.Text = dt.Rows[0]["Fax"].ToString();
                }
            }
        }

        protected void gvCRF_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = (DataTable)Session["DT"];
                if (index >= 0 && index < dt.Rows.Count)
                {
                    dt.Rows.RemoveAt(index);
                    Session["DT"] = dt;

                    gvCRF.DataSource = dt;
                    gvCRF.DataBind();
                }
                if (gvCRF.Rows.Count == 0)
                {
                    btnSubmit.Visible = false;
                }
                else
                {
                    btnSubmit.Visible = true;
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int flag = checkFields();
            if (flag == 0)
            {
                AddDetails();
            }
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
                DataTable dt = createTempTable();
                DataTable dtParam = new DataTable();

                //  DataTable dtSubParameter = _cm.getTestSubParameter(txtTestParameter.Text);
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
                    dtParam = _cm.getRequirementTestParameter(ddlRequirement.SelectedValue.ToString());
                    if (dtParam.Rows.Count > 0) { lblTestParameterID.Text = dtParam.Rows[0]["TestParameterID"].ToString(); }
                }
                else
                {
                    dtParam = _cm.GetTestParameterDetails(txtTestParameter.Text);
                }
                if (dtParam.Rows.Count > 0)
                {
                    if (ddlRequirement.SelectedIndex != 0)
                    {
                        if (dtParam.Rows.Count > 0)
                        {
                            string testparam = "";
                            foreach (DataRow dr in dtParam.Rows)
                            {
                                if (dr["hassubparameter"].ToString() == "1")
                                {
                                    testparam = dr["TestParameter"].ToString();
                                    DataTable dtSubParam = _cm.GetTestSubParameterDetails(testparam, ddlRequirement.SelectedValue);
                                    if (dtSubParam.Rows.Count > 0)
                                    {
                                        try
                                        {
                                            foreach (DataRow drSub in dtSubParam.Rows)
                                            {
                                                DataRow dr1 = dt.NewRow();
                                                dr1["SampleName"] = txtSampleName.Text.ToString();
                                                dr1["BatchNo"] = txtBatchNo.Text.ToString();
                                                dr1["SizeofSample"] = txtSizeofSample.Text.ToString();
                                                dr1["Remarks"] = txtRemarks.Text.ToString();
                                                dr1["Specification"] = drSub["Specification"].ToString();
                                                dr1["TestMethod"] = drSub["Protocol"].ToString();
                                                dr1["UomName"] = drSub["UOMName"].ToString();

                                                dr1["TestParameter"] = drSub["TestParameter"].ToString();
                                                dr1["TestParameterID"] = drSub["TestParameterID"].ToString();
                                                dr1["TestSubParameter"] = drSub["TestSubParameter"].ToString();
                                                dr1["TestSubParameterID"] = drSub["TestSubParameterID"].ToString();

                                                if (ddlRequirement.SelectedIndex != 0)
                                                {
                                                    dr1["Requirement"] = ddlRequirement.SelectedItem.ToString();
                                                    dr1["RequirementID"] = ddlRequirement.SelectedValue.ToString();
                                                }
                                                else
                                                {
                                                    dr1["Requirement"] = "";
                                                    dr1["RequirementID"] = "";
                                                }
                                                dr1["LOD"] = drSub["LOD"].ToString();
                                                dr1["LOQ"] = drSub["LOQ"].ToString();

                                                dt.Rows.Add(dr1);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            string s = ex.Message;
                                        }
                                    }
                                    else
                                    {
                                        DataRow dr1 = dt.NewRow();
                                        dr1["SampleName"] = txtSampleName.Text.ToString();
                                        dr1["BatchNo"] = txtBatchNo.Text.ToString();
                                        dr1["SizeofSample"] = txtSizeofSample.Text.ToString();
                                        dr1["Remarks"] = txtRemarks.Text.ToString();
                                        dr1["Specification"] = dr["Specification"].ToString();
                                        dr1["TestMethod"] = dr["Protocol"].ToString();
                                        dr1["UomName"] = dr["UOMName"].ToString();

                                        dr1["TestParameter"] = dr["TestParameter"].ToString();
                                        dr1["TestParameterID"] = dr["TestParameterID"].ToString();
                                        dr1["TestSubParameter"] = dr["TestSubParameter"].ToString();
                                        dr1["TestSubParameterID"] = dr["TestSubParameterID"].ToString();

                                        if (ddlRequirement.SelectedIndex != 0)
                                        {
                                            dr1["Requirement"] = ddlRequirement.SelectedItem.ToString();
                                            dr1["RequirementID"] = ddlRequirement.SelectedValue.ToString();
                                        }
                                        else
                                        {
                                            dr1["Requirement"] = "";
                                            dr1["RequirementID"] = "";
                                        }
                                        dr1["LOD"] = dr["LOD"].ToString();
                                        dr1["LOQ"] = dr["LOQ"].ToString();

                                        dt.Rows.Add(dr1);
                                    }
                                }
                                else
                                {
                                    try
                                    {

                                        DataRow dr2 = dt.NewRow();
                                        dr2["SampleName"] = txtSampleName.Text.ToString();
                                        dr2["BatchNo"] = txtBatchNo.Text.ToString();
                                        dr2["SizeofSample"] = txtSizeofSample.Text.ToString();
                                        dr2["Remarks"] = txtRemarks.Text.ToString();

                                        dr2["Specification"] = dr["Specification"].ToString();
                                        dr2["TestMethod"] = dr["Protocol"].ToString();
                                        dr2["UomName"] = dr["UOMName"].ToString();
                                        dr2["Requirement"] = ddlRequirement.SelectedItem.ToString();
                                        dr2["RequirementID"] = ddlRequirement.SelectedValue.ToString();

                                        dr2["TestParameter"] = dr["TestParameter"].ToString();
                                        dr2["TestParameterID"] = dr["TestParameterID"].ToString();

                                        dr2["LOD"] = dr["LOD"].ToString();
                                        dr2["LOQ"] = dr["LOQ"].ToString();

                                        dt.Rows.Add(dr2);

                                    }
                                    catch (Exception ex)
                                    {
                                        string s = ex.Message;
                                    }
                                }

                            }
                        }
                    }
                    else
                    if (dtParam.Rows[0]["hasSubParameter"].ToString() == "1")
                    {
                        if ((ddlTestSubParameter.SelectedIndex != 0) && (ddlTestMethod.SelectedIndex != 0) && (ddlUOM.SelectedIndex != 0))
                        {
                            DataRow dr = dt.NewRow();
                            dr["SampleName"] = txtSampleName.Text.ToString();
                            dr["BatchNo"] = txtBatchNo.Text.ToString();
                            dr["SizeofSample"] = txtSizeofSample.Text.ToString();
                            dr["Remarks"] = txtRemarks.Text.ToString();
                            dr["Specification"] = txtSpecification.Text.ToString();
                            dr["TestMethod"] = ddlTestMethod.SelectedItem.ToString();
                            dr["UomName"] = ddlUOM.SelectedItem.ToString();
                            DataTable dtSub = _cm.GetTestSubParameter(txtTestParameter.Text,ddlTestSubParameter.SelectedItem.ToString(), ddlUOM.SelectedValue,ddlTestMethod.SelectedItem.ToString());
                            if (dtSub.Rows.Count > 0)
                            {
                                dr["TestParameter"] = dtSub.Rows[0]["TestParameter"].ToString();
                                dr["TestParameterID"] = dtSub.Rows[0]["TestParameterID"].ToString();
                                dr["TestSubParameter"] = dtSub.Rows[0]["TestSubParameter"].ToString();
                                dr["TestSubParameterID"] = dtSub.Rows[0]["TestSubParameterID"].ToString();
                                dr["TestSubSubParameter"] = "";
                                dr["TestSubSubParameterID"] = "";
                            }


                            if (ddlRequirement.SelectedIndex != 0)
                            {
                                dr["Requirement"] = ddlRequirement.SelectedItem.ToString();
                                dr["RequirementID"] = ddlRequirement.SelectedValue.ToString();
                            }
                            else
                            {
                                dr["Requirement"] = "";
                                dr["RequirementID"] = "";
                            }
                            dr["LOD"] = txtLOD.Text;
                            dr["LOQ"] = txtLOQ.Text;

                            dt.Rows.Add(dr);
                        }
                        else
                        {
                            DataTable dtSubParam = _cm.GetTestSubParameterDetails(txtTestParameter.Text);
                            if (dtSubParam.Rows.Count > 0)
                            {
                                try
                                {

                                    foreach (DataRow drSub in dtSubParam.Rows)
                                    {
                                        DataTable dtSubSubParam = _cm.GetTestSubSubParameterDetails(drSub["TestSubParameter"].ToString());
                                        if (dtSubSubParam.Rows.Count > 0)
                                        {
                                            foreach (DataRow drSubSub in dtSubSubParam.Rows)
                                            {
                                                DataRow dr = dt.NewRow();
                                                dr["SampleName"] = txtSampleName.Text.ToString();
                                                dr["BatchNo"] = txtBatchNo.Text.ToString();
                                                dr["SizeofSample"] = txtSizeofSample.Text.ToString();
                                                dr["Remarks"] = txtRemarks.Text.ToString();
                                                dr["Specification"] = drSubSub["Specification"].ToString();
                                                dr["TestMethod"] = drSubSub["Protocol"].ToString();
                                                dr["UomName"] = drSubSub["UOMName"].ToString();

                                                dr["TestParameter"] = drSubSub["TestParameter"].ToString();
                                                dr["TestParameterID"] = drSubSub["TestParameterID"].ToString();
                                                dr["TestSubParameter"] = drSubSub["TestSubParameter"].ToString();
                                                dr["TestSubParameterID"] = drSubSub["TestSubParameterID"].ToString();
                                                dr["TestSubSubParameter"] = drSubSub["TestSubSubParameter"].ToString();
                                                dr["TestSubSubParameterID"] = drSubSub["TestSubSubParameterID"].ToString();

                                                if (ddlRequirement.SelectedIndex != 0)
                                                {
                                                    dr["Requirement"] = ddlRequirement.SelectedItem.ToString();
                                                    dr["RequirementID"] = ddlRequirement.SelectedValue.ToString();
                                                }
                                                else
                                                {
                                                    dr["Requirement"] = "";
                                                    dr["RequirementID"] = "";
                                                }
                                                dr["LOD"] = drSubSub["LOD"].ToString();
                                                dr["LOQ"] = drSubSub["LOQ"].ToString();

                                                dt.Rows.Add(dr);
                                            }
                                        }
                                        else
                                        {

                                            DataRow dr = dt.NewRow();
                                            dr["SampleName"] = txtSampleName.Text.ToString();
                                            dr["BatchNo"] = txtBatchNo.Text.ToString();
                                            dr["SizeofSample"] = txtSizeofSample.Text.ToString();
                                            dr["Remarks"] = txtRemarks.Text.ToString();
                                            dr["Specification"] = drSub["Specification"].ToString();
                                            dr["TestMethod"] = drSub["Protocol"].ToString();
                                            dr["UomName"] = drSub["UOMName"].ToString();

                                            dr["TestParameter"] = drSub["TestParameter"].ToString();
                                            dr["TestParameterID"] = drSub["TestParameterID"].ToString();
                                            dr["TestSubParameter"] = drSub["TestSubParameter"].ToString();
                                            dr["TestSubParameterID"] = drSub["TestSubParameterID"].ToString();
                                            dr["TestSubSubParameter"] = "";
                                            dr["TestSubSubParameterID"] = "";

                                            if (ddlRequirement.SelectedIndex != 0)
                                            {
                                                dr["Requirement"] = ddlRequirement.SelectedItem.ToString();
                                                dr["RequirementID"] = ddlRequirement.SelectedValue.ToString();
                                            }
                                            else
                                            {
                                                dr["Requirement"] = "";
                                                dr["RequirementID"] = "";
                                            }
                                            dr["LOD"] = drSub["LOD"].ToString();
                                            dr["LOQ"] = drSub["LOQ"].ToString();

                                            dt.Rows.Add(dr);

                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    string s = ex.Message;
                                }
                            }

                        }

                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["SampleName"] = txtSampleName.Text.ToString();
                        dr["BatchNo"] = txtBatchNo.Text.ToString();
                        dr["SizeofSample"] = txtSizeofSample.Text.ToString();
                        dr["Remarks"] = txtRemarks.Text.ToString();
                        dr["Specification"] = txtSpecification.Text.ToString();
                        dr["TestMethod"] = ddlTestMethod.SelectedItem.ToString();
                        dr["TestParameter"] = txtTestParameter.Text;
                        dr["TestParameterID"] = dtParam.Rows[0]["TestParameterID"].ToString();
                        dr["UomName"] = ddlUOM.SelectedItem.ToString();
                        _cm.UOMName = ddlUOM.SelectedItem.ToString();

                        _cm.testParameter = txtTestParameter.Text;
                        if (ddlTestSubParameter.SelectedIndex > 0)
                        {
                            _cm.testSubParameter = ddlTestSubParameter.SelectedItem.ToString();
                        }
                        else
                        {
                            _cm.testSubParameter = "";
                        }
                        if (ddlTestSubSubParameter.SelectedIndex > 0)
                        {
                            _cm.testSubSubParameter = ddlTestSubSubParameter.SelectedItem.ToString();
                        }
                        else
                        {
                            _cm.testSubSubParameter = "";
                        }


                        DataTable dtDetails = new DataTable();
                        if (ddlTestSubSubParameter.SelectedIndex > 0)
                        {
                            dtDetails = _cm.getParameter(_cm, 3);
                            dr["TestSubSubParameterID"] = dtDetails.Rows[0]["TestSubSubParameterID"].ToString();
                            dr["TestSubSubParameter"] = dtDetails.Rows[0]["TestSubSubParameter"].ToString();
                            dr["TestSubParameterID"] = dtDetails.Rows[0]["TestSubParameterID"].ToString();
                            dr["TestSubParameter"] = dtDetails.Rows[0]["TestSubParameter"].ToString();
                            dr["TestParameterID"] = dtDetails.Rows[0]["TestParameterID"].ToString();
                            dr["TestParameter"] = dtDetails.Rows[0]["TestParameter"].ToString();
                        }
                        else
                        if (ddlTestSubParameter.SelectedIndex > 0)
                        {
                            dtDetails = _cm.getParameter(_cm, 2);
                            dr["TestSubSubParameterID"] = "";
                            dr["TestSubSubParameter"] = "";
                            dr["TestSubParameterID"] = dtDetails.Rows[0]["TestSubParameterID"].ToString();
                            dr["TestSubParameter"] = dtDetails.Rows[0]["TestSubParameter"].ToString();
                            dr["TestParameterID"] = dtDetails.Rows[0]["TestParameterID"].ToString();
                            dr["TestParameter"] = dtDetails.Rows[0]["TestParameter"].ToString();
                        }
                        else
                        {
                            dtDetails = _cm.getParameter(_cm, 1);
                            dr["TestSubSubParameterID"] = "";
                            dr["TestSubSubParameter"] = "";
                            dr["TestSubParameterID"] = "";
                            dr["TestSubParameter"] = "";
                            dr["TestParameterID"] = dtDetails.Rows[0]["TestParameterID"].ToString();
                            dr["TestParameter"] = dtDetails.Rows[0]["TestParameter"].ToString();
                        }


                        if (ddlRequirement.SelectedIndex != 0)
                        {
                            dr["Requirement"] = ddlRequirement.SelectedItem.ToString();
                            dr["RequirementID"] = ddlRequirement.SelectedValue.ToString();
                        }
                        else
                        {
                            dr["Requirement"] = "";
                            dr["RequirementID"] = "";
                        }


                        dr["LOD"] = txtLOD.Text;
                        dr["LOQ"] = txtLOQ.Text;
                        dt.Rows.Add(dr);
                    }


                    gvCRF.DataSource = dt;
                    gvCRF.DataBind();

                    if (gvCRF.Rows.Count == 0)
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
        }
        protected DataTable createTempTable()
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
                dt.Columns.Add("UOMName");
                dt.Columns.Add("LOD");
                dt.Columns.Add("LOQ");
                dt.Columns.Add("SizeofSample");
                dt.Columns.Add("Remarks");
            }
            return dt;
        }
        private void Clear()
        {
            ddlRequirement.ClearSelection();
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
        protected int checkFields()
        {
            int flag = 0;
            if (txtDateOfRequest.Text == "")
            {
                txtDateOfRequest.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtDateOfRequest.BackColor = System.Drawing.Color.Empty;
            }
            if (lblInquiry.Text == "")
            {
                lblInquiry.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                lblInquiry.BackColor = System.Drawing.Color.Empty;
            }
            if (ddlContactPerson.SelectedIndex == 0)
            {
                ddlContactPerson.BorderColor = Color.Red; flag = 0;
            }
            else
            {
                ddlContactPerson.BorderColor = Color.Empty;
            }
            if (txtMobile.Text == "")
            {
                txtMobile.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtMobile.BackColor = System.Drawing.Color.Empty;
            }
            if (txtContactEmail.Text == "")
            {
                txtContactEmail.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtContactEmail.BackColor = System.Drawing.Color.Empty;
            }
            if (txtCustomer1.Text == "")
            {
                txtCustomer1.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtCustomer1.BackColor = System.Drawing.Color.Empty;
            }
            if (txtAddressLine11.Text == "")
            {
                txtAddressLine11.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtAddressLine11.BackColor = System.Drawing.Color.Empty;
            }
            if (txtAddressLine21.Text == "")
            {
                txtAddressLine21.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtAddressLine21.BackColor = System.Drawing.Color.Empty;
            }
            if (txtPincode1.Text == "")
            {
                txtPincode1.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPincode1.BackColor = System.Drawing.Color.Empty;
            }
            if (txtCountry1.Text == "")
            {
                txtCountry1.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtCountry1.BackColor = System.Drawing.Color.Empty;
            }
            if (txtContactEmail.Text == "")
            {
                txtContactEmail.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtContactEmail.BackColor = System.Drawing.Color.Empty;
            }
            if (!(System.Text.RegularExpressions.Regex.IsMatch(txtContactEmail.Text, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")))
            {
                txtContactEmail.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtContactEmail.BackColor = System.Drawing.Color.Empty;
            }
            if (radServiceRequired.SelectedIndex == -1)
            {
                radServiceRequired.BackColor = Color.LightPink; flag = 1;
            }
            else
            {
                radServiceRequired.BackColor = Color.Empty;
            }
            if (radSampleReturn.SelectedIndex == -1)
            {
                radSampleReturn.BackColor = Color.LightPink; flag = 1;
            }
            else
            {
                radSampleReturn.BackColor = Color.Empty;
            }
            if (radReportRecipient.SelectedIndex == -1)
            {
                radReportRecipient.BackColor = Color.LightPink; flag = 1;
            }
            else
            {
                radReportRecipient.BackColor = Color.Empty;
            }
            if (radInvoiceRecipient.SelectedIndex == -1)
            {
                radInvoiceRecipient.BackColor = Color.LightPink; flag = 1;
            }
            else
            {
                radInvoiceRecipient.BackColor = Color.Empty;
                if (radInvoiceRecipient.SelectedItem.ToString() != "Applicant")
                {
                    if (txtPayerName.Text == "")
                    {
                        txtPayerName.BorderColor = Color.Red; flag = 1;
                    }
                    else
                    {
                        txtPayerName.BorderColor = Color.Empty;
                    }
                    if (txtPayerContactPerson.Text == "")
                    {
                        txtPayerContactPerson.BorderColor = Color.Red; flag = 1;
                    }
                    else
                    {
                        txtPayerContactPerson.BorderColor = Color.Empty;
                    }
                    if (txtPayerEmail.Text == "")
                    {
                        txtPayerEmail.BorderColor = Color.Red; flag = 1;
                    }
                    else
                    {
                        txtPayerEmail.BorderColor = Color.Empty;
                    }
                    if (txtPayerContactNo.Text == "")
                    {
                        txtPayerContactNo.BorderColor = Color.Red; flag = 1;
                    }
                    else
                    {
                        txtPayerContactNo.BorderColor = Color.Empty;
                    }
                }

            }

            return flag;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int flag = 0;
            flag = checkFields();

            if (flag == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    if (ddlContactPerson.SelectedIndex != 0)
                    {
                        _cm.ReportID = ddlReportFormat.SelectedValue;
                        _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                        _cm.UpdateReportType(_cm);

                        _cm.personId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                        _cm.customerId = Convert.ToInt32(Session["CustomerId"]);

                        int RequestId = 0;
                        _cm.dateOfRequest = Convert.ToDateTime(txtDateOfRequest.Text);

                        _cm.shippingAgency = txtShippingAgency.Text.ToString();
                        _cm.airwayBillNo = txtAirwayBillNo.Text.ToString();
                        _cm.requestTypeId = 1;
                        RequestId = _cm.AddCustomerRequest(_cm);

                        /* Update TAFNo in CustomerRequestDetails*/
                        _cm.GenerateTAFNo(RequestId, "TAF");
                        /* Update TAFNo in CustomerRequestDetails*/

                        _cm.CustomerMasterId = Convert.ToInt32(hdCustomerMasterID.Value);
                        _cm.RequestID = RequestId;
                        _cm.BuyerCustomerName = txtCustomer1.Text;
                        _cm.BuyerAddressLine1 = txtAddressLine11.Text;
                        _cm.BuyerAddressLine2 = txtAddressLine21.Text;
                        _cm.BuyerPincode = txtPincode1.Text;
                        _cm.BuyerCountry = txtCountry1.Text;
                        _cm.ServiceRequired = radServiceRequired.SelectedItem.ToString();
                        _cm.SampleReturn = radSampleReturn.SelectedItem.ToString();
                        _cm.ReportRecipient = radReportRecipient.SelectedItem.ToString();
                        _cm.InvoicePayment = radInvoiceRecipient.SelectedItem.ToString();
                        _cm.PayerName = txtPayerName.Text;
                        _cm.PayerContactPerson = txtPayerContactPerson.Text;
                        _cm.PayerContactNo = txtPayerContactNo.Text;
                        _cm.PayerEmail = txtPayerEmail.Text;
                        _cm.ProductType = txtProductType.Text;
                        _cm.PO = "";
                        _cm.CountryOfOrigin = txtOriginCountry.Text;
                        _cm.CountryOfDestination = txtDesctinationCountry.Text;
                        _cm.ItemStyle = txtItemStyleNo.Text;

                        _cm.AddAdditionalDetails(_cm);

                        Session["RequestId"] = RequestId;

                /* Update TAFNo in CustomerRequestDetails*/
                _cm.GenerateTAFNo(RequestId,"TAF");                
                /* Update TAFNo in CustomerRequestDetails*/
				
                        if (Session["DT"] != null)
                        {
                            if (Session["OrganizationID"] != null)
                            {
                                _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                                DataTable dt = (DataTable)Session["DT"];
                                foreach (DataRow dr in dt.Rows)
                                {
                                    _cm.customerRequestId = Convert.ToInt32(Session["RequestId"]);
                                    _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                                    _cm.sampleName = dr["SampleName"].ToString();
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
                                    _cm.LOD = dr["LOD"].ToString();
                                    _cm.LOQ = dr["LOQ"].ToString();
                                    _cm.UOMName = dr["UomName"].ToString();
                                    _cm.sizeOfSample = dr["SizeofSample"].ToString();
                                    _cm.remarks = dr["Remarks"].ToString();
                                    _cm.CRFID = 1;
                                    _cm.RequirementID = dr["RequirementID"].ToString();
                                    _cm.Requirement = dr["Requirement"].ToString();
                                    _cm.AddCustomerCRFSampleDetails(_cm);

                                }
                            }
                        }

                        //change customer inquiry Status to CRF Received
                        _cm.inquiryStatusId = 6;
                        _cm.UpdateInquiryStatus(_cm);


                        ClearAll();
                        Response.Redirect("CreateRequestCustomerFM.aspx?ID=" + Session["InquiryID"] + "&r=1");
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
                }
            }
        }

        private void ClearAll()
        {
            txtDateOfRequest.Text = "";
            lblInquiry.Text = "";
            txtCustomer.Text = "";
            ddlContactPerson.SelectedIndex = 0;
            txtMobile.Text = "";
            txtContactEmail.Text = "";
            txtAddressLine11.Text = "";

            gvCRF.DataSource = null;
            gvCRF.DataBind();
            if (gvCRF.Rows.Count == 0)
            {
                btnSubmit.Visible = false;
            }
            else
            {
                btnSubmit.Visible = true;
            }
            Session["DT"] = null;
        }

        protected void chkSameAsInvoice_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSameAsInvoice.Checked)
            {
                txtCustomer1.Text = txtCustomer.Text;
                txtAddressLine11.Text = txtAddressLine1.Text;
                txtAddressLine21.Text = txtAddressLine2.Text;
                txtPincode1.Text = txtPincode.Text;
                txtCountry1.Text = txtCountry.Text;
            }
        }
        protected void txtTestParameter_TextChanged(object sender, EventArgs e)
        {
            txtLOD.Text = ""; txtLOQ.Text = "";
            string param = ""; DataTable dt = new DataTable();
            if (txtTestParameter.Text != "")
            {
                param = txtTestParameter.Text;
            }
            //dt = _cm.getTestSubParameters(param);
            dt = _cm.getTestSubParameters_New(param);
            if (dt.Rows.Count > 0)
            {
                ddlTestSubParameter.DataSource = dt;
                ddlTestSubParameter.DataTextField = "TestSubParameter";
                ddlTestSubParameter.DataValueField = "TestSubParameterID";
                ddlTestSubParameter.DataBind();
                ddlTestSubParameter.Items.Insert(0, "Select Sub Parameter");
                dt = _cm.GetMethodOfTestSubParameter(txtTestParameter.Text, param);
                if (dt.Rows.Count > 0)
                {
                    ddlTestMethod.DataSource = dt;
                    ddlTestMethod.DataTextField = "TestMethod";
                    ddlTestMethod.DataValueField = "TestMethodID";
                    ddlTestMethod.DataBind();
                    ddlTestMethod.Items.Insert(0, "Select Method");

                }
                else
                {
                    ddlTestSubParameter.ClearSelection();
                    ddlTestSubParameter.DataSource = null;
                    ddlTestSubParameter.DataBind();

                    ddlTestSubSubParameter.ClearSelection();
                    ddlTestSubSubParameter.DataSource = null;
                    ddlTestSubSubParameter.DataBind();

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
            }
            else
            {
                ddlTestSubParameter.ClearSelection();
                ddlTestSubParameter.DataSource = null;
                ddlTestSubParameter.DataBind();

                dt = _cm.GetMethodOfTestParameter(txtTestParameter.Text);
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
        protected void radInvoiceRecipient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radInvoiceRecipient.SelectedItem.ToString() == "Applicant")
            {
                if (ddlContactPerson.SelectedIndex != 0)
                {
                    txtPayerName.Text = txtCustomer.Text;
                    txtPayerContactPerson.Text = ddlContactPerson.SelectedItem.Text;
                    txtPayerEmail.Text = txtContactEmail.Text;
                    txtPayerContactNo.Text = txtMobile.Text;
                }
                else
                {
                    ddlContactPerson.BorderColor = Color.Red;
                }
            }
            else
            {
                txtPayerName.Text = "";
                txtPayerContactPerson.Text = "";
                txtPayerEmail.Text = "";
                txtPayerContactNo.Text = "";
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
                    txtSpecification.Text = dt.Rows[0]["Specification"].ToString();
                    txtLOD.Text = dt.Rows[0]["LOD"].ToString();
                    txtLOQ.Text = dt.Rows[0]["LOQ"].ToString();
                }
                else
                {
                    _cm.testSubParameter = "";
                    dt = _cm.GetTestParameterDetails(_cm);
                }
            }
        }

        protected void ddlTestSubParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTestSubParameter.SelectedIndex != 0)
            {
                string param = ddlTestSubParameter.SelectedItem.ToString();
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


    }
}