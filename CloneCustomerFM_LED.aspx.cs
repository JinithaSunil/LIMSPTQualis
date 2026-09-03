using DocumentFormat.OpenXml.Bibliography;
using Org.BouncyCastle.Asn1.Ocsp;
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
    public partial class CloneCustomerFM_LED : System.Web.UI.Page
    {

        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
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
                        //  if (inquiryStatus == 6)
                        // {
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

        private void BindCustomerCRF(int inquiryID)
        {
            DataTable dt = new DataTable();
            if (Session["DT"] != null)
            {
                dt = (DataTable)Session["DT"];
                gvTests.DataSource = (DataTable)Session["DT"];
                gvTests.DataBind();
            }
            else
            {
                dt = _cm.getCustomerCRFSampleDetails(inquiryID);
                if (dt.Rows.Count > 0)
                {
                    Session["DT"] = dt;
                    gvTests.DataSource = (DataTable)Session["DT"];
                    gvTests.DataBind();
                }
                else
                {
                    gvTests.DataSource = null;
                    gvTests.DataBind();
                }
            }
            lblSampleName.Text = dt.Rows[0]["SampleName"].ToString();
            //txtSampleName.Text = dt.Rows[0]["SampleName"].ToString();
            lblBatchNo.Text = dt.Rows[0]["BatchNo"].ToString();
            txtBatchNo.Text = dt.Rows[0]["BatchNo"].ToString();
            txtSizeofSample.Text = dt.Rows[0]["SizeOfSample"].ToString();
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
                    dt.Columns.Add("TestStandardID");
                    dt.Columns.Add("TestStandard");
                }

                lblSampleName.Text = txtSampleName.Text;
                lblBatchNo.Text = txtSizeofSample.Text;
                lblSampleQty.Text = txtSizeofSample.Text;

                DataRow dr = dt.NewRow();
                dr["SampleName"] = txtSampleName.Text.ToString();
                dr["BatchNo"] = txtBatchNo.Text.ToString();
                dr["Specification"] = txtSpecification.Text.ToString();
                dr["SizeofSample"] = txtSizeofSample.Text.ToString();
                dr["Remarks"] = txtRemarks.Text.ToString();
                dt.Rows.Add(dr);

                gvTests.DataSource = dt;
                gvTests.DataBind();

                if (gvTests.Rows.Count == 0)
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

        private void Clear()
        {
            txtSpecification.Text = "";
            txtRemarks.Text = "";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                int CloneCRFSampleID = 0;
                int CustomerCRFSampleID = 0;
                int inquiryID = Convert.ToInt32(Session["InquiryID"]);
                int flag1 = 0;

                // Validation
                if (string.IsNullOrWhiteSpace(txtSampleName.Text))
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

                    if (dt1.Rows.Count > 0 && Session["OrganizationID"] != null)
                    {
                        _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());

                        CustomerCRFSampleID = Convert.ToInt32(dt1.Rows[0]["CustomerCRFSampleID"].ToString());
                        Session["RequestId"] = Convert.ToInt32(dt1.Rows[0]["CustomerRequestID"].ToString());

                        _cm.inquiryId = inquiryID;
                        DataTable dtInquiryStatus = _cm.GetEnquiryStatus(inquiryID);
                        int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());

                        _cm.CustomerCRFSampleID = CustomerCRFSampleID;
                        CloneCRFSampleID = _cm.AddCRFClone(_cm);
                        _cm.CloneCRFSampleID = CloneCRFSampleID;

                        DataTable selectedTests = new DataTable();
                        selectedTests.Columns.Add("ReportID", typeof(string));
                        selectedTests.Columns.Add("ReportName", typeof(string));
                        selectedTests.Columns.Add("TestMethod", typeof(string));
                        selectedTests.Columns.Add("Remarks", typeof(string));
                        selectedTests.Columns.Add("Part", typeof(string));
                        selectedTests.Columns.Add("Requirement", typeof(string));
                        selectedTests.Columns.Add("TestStandardID", typeof(string));
                        selectedTests.Columns.Add("TestStandard", typeof(string));

                        // Loop through GridView to get selected tests
                        foreach (GridViewRow row in gvTests.Rows)
                        {
                            CheckBox chkTest = (CheckBox)row.FindControl("chkTest");

                            if (chkTest != null && chkTest.Checked)
                            {
                                Label lblReportID = (Label)row.FindControl("lblReportID");
                                TextBox txtMethod = (TextBox)row.FindControl("txtMethod");
                                TextBox txtRemarks = (TextBox)row.FindControl("txtRemarks");
                                TextBox txtPart = (TextBox)row.FindControl("txtPart");
                                TextBox txtRequirement = (TextBox)row.FindControl("txtRequirement");
                                TextBox txtStandardName = (TextBox)row.FindControl("txtStandardName");
                                Label lblStandardID = (Label)row.FindControl("lblStandardID");

                                // Add this test to our DataTable
                                DataRow dr = selectedTests.NewRow();
                                dr["ReportID"] = lblReportID.Text;
                                dr["ReportName"] = chkTest.Text;
                                dr["TestMethod"] = txtMethod.Text;
                                dr["Remarks"] = txtRemarks.Text;
                                dr["Part"] = txtPart.Text;
                                dr["Requirement"] = txtRequirement.Text;
                                dr["TestStandard"] = txtStandardName.Text;
                                dr["TestStandardID"] = lblStandardID.Text;
                                selectedTests.Rows.Add(dr);
                            }
                        }

                        Session["DT"] = selectedTests;

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
                                _cm.batchNo = txtBatchNo.Text;
                                _cm.testMethod = dt.Columns.Contains("TestMethod") ? dr["TestMethod"].ToString() : "";
                                _cm.specification = txtSpecification.Text;
                                _cm.testsRequested = "";
                                //_cm.UOMName = dr["UnitOfMeasurement"].ToString();
                                _cm.sizeOfSample = txtSizeofSample.Text;
                                _cm.remarks = dt.Columns.Contains("Remarks") ? dr["Remarks"].ToString() : "";
                                _cm.CRFID = 4;
                                _cm.CloneCRFSampleID = CloneCRFSampleID;
                                //_cm.RequirementID = dr["RequirementID"].ToString();
                                _cm.Requirement = dt.Columns.Contains("Requirement") ? dr["Requirement"].ToString() : "";
                                _cm.sampleStatusID = 28;
                                _cm.departmentID = DepartmentID;

                                
                                
                                _cm.sampleName = txtSampleName.Text;
                                //_cm.SampleType = txtProductType.Text;
                                //_cm.batchNo = txtbrand.Text;
                                _cm.testMethod = dt.Columns.Contains("TestMethod") ? dr["TestMethod"].ToString() : "";
                                _cm.ReportID = dr["ReportID"].ToString();
                                _cm.ReportName = dr["ReportName"].ToString();
                                _cm.sizeOfSample = txtSizeofSample.Text;
                                
                                _cm.remarks = dt.Columns.Contains("Remarks") ? dr["Remarks"].ToString() : "";
                                _cm.part = dt.Columns.Contains("Part") ? dr["Part"].ToString() : "";
                                _cm.requirement = dt.Columns.Contains("Requirement") ? dr["Requirement"].ToString() : "";

                                _cm.TestStandardID = dr["TestStandardID"].ToString();
                                _cm.TestStandardName = dr["TestStandard"].ToString();

                                _cm.AddCRFSampleDetails_LED(_cm);
                            }
                        }

                        

                        _cm.UpdateCRFSampleDetailsStatus(_cm);
                        _cm.CRFStatusID = 28;
                        _cm.CloneCRFSampleID = CloneCRFSampleID;
                        _cm.UpdateCRFStatus(_cm);

                        ClearAll();
                        Response.Redirect(String.Format("CreateRequestCloneFM_LED.aspx?ID1={0}&ID2={1}",Session["InquiryID"], CloneCRFSampleID), false);
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
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
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

            gvTests.DataSource = null;
            gvTests.DataBind();
            if (gvTests.Rows.Count == 0)
            {
                btnSubmit.Visible = false;
            }
            else
            {
                btnSubmit.Visible = true;
            }
            Session["DT"] = null;
        }


        protected void gvTests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow row = (GridViewRow)(((Control)e.CommandSource).NamingContainer);
            int ReportID = Convert.ToInt32(e.CommandArgument);
            CheckBox cbox = (CheckBox)row.Cells[1].Controls[0];
        }


        protected void gvTests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkTest = (e.Row.FindControl("chkTest") as CheckBox);
                chkTest.Checked = true;
            }
        }

        protected void chkTest_CheckedChanged(object sender, EventArgs e)
        {
            GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
            int index = row.RowIndex;
            Label lblReportID = (Label)gvTests.Rows[index].FindControl("lblReportID");
            TextBox txtCount = (TextBox)gvTests.Rows[index].FindControl("txtCount");
            CheckBox cb1 = (CheckBox)gvTests.Rows[index].FindControl("chkTest");
            if (!cb1.Checked) { cb1.BorderColor = Color.Red; }
        }



    }
}