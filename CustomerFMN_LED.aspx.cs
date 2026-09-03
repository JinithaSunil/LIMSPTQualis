using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerFMN_LED : System.Web.UI.Page
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

                    Session["OrganizationID"] = 1;
                    int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                    DataTable dtInquiryStatus = _cm.GetEnquiryStatus(InquiryId);
                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        int inquiryStatus = Convert.ToInt32(dtInquiryStatus.Rows[0]["inquiryStatusID"].ToString());
                        if (inquiryStatus == 5)
                        {
                            int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());

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
                                txtVAT.Text = dt.Rows[0]["VAT"].ToString();
                                txtAddressLine1.Text = dt.Rows[0]["AddressLine1"].ToString();
                                txtAddressLine2.Text = dt.Rows[0]["AddressLine2"].ToString() + "," + dt.Rows[0]["State"].ToString();
                                txtPincode.Text = dt.Rows[0]["Pincode"].ToString();
                                txtCountry.Text = dt.Rows[0]["Country"].ToString();
                                Session["CustomerId"] = dt.Rows[0]["CustomerId"].ToString();
                                hdCustomerMasterID.Value = dt.Rows[0]["CustomerMasterId"].ToString();
                                BindContactPerson(InquiryId);
                                BindSampleRadioList();
                            }
                        }
                        else
                        {
                            divCRF.Visible = false;
                            divFail.Visible = true;
                        }
                    }
                    if (gvTests.Rows.Count == 0)
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

        protected void gvCRF_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = (DataTable)Session["DT"];
                dt.Rows[index].Delete();
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
            }
        }

        private void BindSampleRadioList()
        {
            if (Request.QueryString["ID"] != null)
            {
                string EnquiryID = Request.QueryString["ID"].ToString();
                DataTable dt = _cm.getStandardAndReportsDataNew(EnquiryID);
                int j = 0;
                if (dt.Rows.Count > 0)
                {
                    gvTests.DataSource = dt;
                    gvTests.DataBind();
                    Session["DT"] = dt;
                }
            }
        }


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

        protected DataTable createTempTable()
        {
            DataTable dt = new DataTable();
            if (Session["DT"] != null)
            {
                dt = (DataTable)Session["DT"];
            }
            else
            {
                dt.Columns.Add("StandardID");
                dt.Columns.Add("StandardName");
                dt.Columns.Add("SampleName");
                dt.Columns.Add("BatchNo");
                dt.Columns.Add("SizeofSample");
                dt.Columns.Add("ReportID");
                dt.Columns.Add("ReportName");
                dt.Columns.Add("TestMethod");
                dt.Columns.Add("Remarks");
            }
            return dt;
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
            if (servicerequiredby.Text == "")
            {
                servicerequiredby.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                servicerequiredby.BackColor = System.Drawing.Color.Empty;
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


            if (txtProductType.Text == "")
            {
                txtProductType.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtProductType.BackColor = System.Drawing.Color.Empty;
            }

            if (txtbrand.Text == "")
            {
                txtbrand.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtbrand.BackColor = System.Drawing.Color.Empty;
            }

            if (txtProductColor.Text == "")
            {
                txtProductColor.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtProductColor.BackColor = System.Drawing.Color.Empty;
            }

            if (txtProductSerialNo.Text == "")
            {
                txtProductSerialNo.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtProductSerialNo.BackColor = System.Drawing.Color.Empty;
            }

            if (txtProductSpecification.Text == "")
            {
                txtProductSpecification.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtProductSpecification.BackColor = System.Drawing.Color.Empty;
            }

            if (txtProductName.Text == "")
            {
                txtProductName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtProductName.BackColor = System.Drawing.Color.Empty;
            }

            if (txtQuantityofsample.Text == "")
            {
                txtQuantityofsample.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtQuantityofsample.BackColor = System.Drawing.Color.Empty;
            }

            if (txtPayerName.Text == "")
            {
                txtPayerName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPayerName.BackColor = System.Drawing.Color.Empty;
            }

            if (txtPayerEmail.Text == "")
            {
                txtPayerEmail.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPayerEmail.BackColor = System.Drawing.Color.Empty;
            }

            if (txtPayerContactNo.Text == "")
            {
                txtPayerContactNo.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPayerContactNo.BackColor = System.Drawing.Color.Empty;
            }

            if (txtPayerContactPerson.Text == "")
            {
                txtPayerContactPerson.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPayerContactPerson.BackColor = System.Drawing.Color.Empty;
            }

            return flag;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int flag = 0;
            flag = checkFields();
            int SampleResultId = 0;
            int CloneCRFSampleID = 0;

            if (flag == 0)
            {

                _cm.OrganizationID = 1;

                if (ddlContactPerson.SelectedIndex != 0)
                {
                    _cm.personId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                    _cm.customerId = Convert.ToInt32(Session["CustomerId"]);
                    _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                    _cm.dateOfRequest = Convert.ToDateTime(txtDateOfRequest.Text);

                    _cm.shippingAgency = txtShippingAgency.Text;
                    _cm.airwayBillNo = txtAirwayBillNo.Text;
                    _cm.requestTypeId = 1;

                    int RequestId = _cm.AddCustomerRequest(_cm);

                    _cm.CustomerMasterId = Convert.ToInt32(hdCustomerMasterID.Value);
                    _cm.RequestID = RequestId;
                    Session["RequestId"] = RequestId;

                /* Update TAFNo in CustomerRequestDetails*/
                _cm.GenerateTAFNo(RequestId,"TAF");                
                /* Update TAFNo in CustomerRequestDetails*/
				
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

                    _cm.ServiceRequiredBy = Convert.ToDateTime(servicerequiredby.Text);
                    _cm.ProductType = txtProductType.Text;

                    _cm.PO = txtPO.Text;
                    _cm.ItemStyle = txtItemStyleNo.Text;

                    _cm.CountryOfOrigin = txtOriginCountry.Text;
                    _cm.CountryOfDestination = txtDesctinationCountry.Text;

                    _cm.Quantityofproduct = txtQuantityofsample.Text;
                    _cm.ArticleNumber = txtArticleNo.Text;
                    _cm.LotNumber = txtLotNo.Text;
                    _cm.FactoryName = txtFactoryName.Text;
                    _cm.FactoryAddress = txtFactoryAddress.Text;
                    _cm.SupplierName = txtSupplierName.Text;

                    _cm.productName = txtProductName.Text;
                    _cm.BrandProduct = txtbrand.Text;
                    _cm.Color = txtProductColor.Text;
                    _cm.ProductSpecification = txtProductSpecification.Text;
                    _cm.ProductSerialNo = txtProductSerialNo.Text;



                    int additionalDetailsID = _cm.AddAdditionalDetailsLED(_cm);
                    if (additionalDetailsID > 0)
                    {

                        Session["RequestId"] = RequestId;

                        // Create a new DataTable to store selected tests
                        DataTable selectedTests = new DataTable();
                        selectedTests.Columns.Add("StandardID", typeof(string));
                        selectedTests.Columns.Add("StandardName", typeof(string));

                        selectedTests.Columns.Add("ReportID", typeof(string));
                        selectedTests.Columns.Add("ReportName", typeof(string));
                        selectedTests.Columns.Add("TestMethod", typeof(string));
                        selectedTests.Columns.Add("Remarks", typeof(string));
                        selectedTests.Columns.Add("Part", typeof(string));
                        selectedTests.Columns.Add("Requirement", typeof(string));


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
                                dr["StandardName"] = txtStandardName.Text;
                                dr["StandardID"] = lblStandardID.Text;
                                selectedTests.Rows.Add(dr);
                            }
                        }

                        Session["DT"] = selectedTests;
                        if (Session["DT"] != null)
                        {
                            int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                            DataTable dtInquiryStatus = _cm.GetEnquiryStatus(InquiryId);
                            int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());
                            DataTable dt = (DataTable)Session["DT"];

                            foreach (DataRow dr in dt.Rows)
                            {
                                _cm.customerRequestId = RequestId;
                                _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                                _cm.sampleName = txtProductName.Text;
                                _cm.SampleType = txtProductType.Text;
                                _cm.batchNo = txtbrand.Text;
                                _cm.testMethod = dt.Columns.Contains("TestMethod") ? dr["TestMethod"].ToString() : "";
                                _cm.ReportID = dr["ReportID"].ToString();
                                _cm.ReportName = dr["ReportName"].ToString();
                                _cm.sizeOfSample = txtQuantityofsample.Text;
                                _cm.voltage = txtProductType.Text;
                                _cm.remarks = dt.Columns.Contains("Remarks") ? dr["Remarks"].ToString() : "";
                                _cm.part = dt.Columns.Contains("Part") ? dr["Part"].ToString() : "";
                                _cm.requirement = dt.Columns.Contains("Requirement") ? dr["Requirement"].ToString() : "";
                                _cm.departmentID = DepartmentID;                                
                                _cm.CloneCRFSampleID = CloneCRFSampleID;

                                _cm.TestStandardID = dr["StandardID"].ToString();
                                _cm.TestStandardName = dr["StandardName"].ToString();

                                _cm.AddCustomerCRFSampleDetailsLED(_cm);
                                _cm.inquiryStatusId = 6;
                                _cm.UpdateInquiryStatus(_cm);

                            }
                        }
                    }

                    ClearAll();
                    Response.Redirect("CreateRequestCustomerFM_LED.aspx?ID=" + Session["InquiryID"] + "&r=1");

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert('Please select a contact person.')", true);
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

        protected void gvTests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow row = (GridViewRow)(((Control)e.CommandSource).NamingContainer);
            int ReportID = Convert.ToInt32(e.CommandArgument);
            CheckBox cbox = (CheckBox)row.Cells[1].Controls[0];
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

        public class CustomerTestInfo
        {
            public int TestId { get; set; }
            public string Remarks { get; set; }
            public string Part { get; set; }
            public string Requirement { get; set; }
        }

        private List<CustomerTestInfo> _crfCustomerTestInfo = null;

        private List<CustomerTestInfo> GetCRFCustomerTestInfo(int customerID)
        {
            List<CustomerTestInfo> testInfoList = new List<CustomerTestInfo>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                conn.Open();

                string query = "SELECT TestList, Remarks, Part, Requirement FROM CustomerInquiry WHERE InquiryId = @CustomerID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Skip NULL TestList values
                            if (!reader.IsDBNull(reader.GetOrdinal("TestList")))
                            {
                                string testListValue = reader["TestList"].ToString();
                                string remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? "" : reader["Remarks"].ToString();
                                string part = reader.IsDBNull(reader.GetOrdinal("Part")) ? "" : reader["Part"].ToString();
                                string requirement = reader.IsDBNull(reader.GetOrdinal("Requirement")) ? "" : reader["Requirement"].ToString();

                                // If the value contains commas, it's a comma-separated list
                                if (testListValue.Contains(","))
                                {
                                    string[] testIds = testListValue.Split(',');

                                    // Split the other fields if they also contain commas
                                    string[] remarksList = remarks.Contains(",") ? remarks.Split(',') : new string[] { remarks };
                                    string[] partsList = part.Contains(",") ? part.Split(',') : new string[] { part };
                                    string[] requirementsList = requirement.Contains(",") ? requirement.Split(',') : new string[] { requirement };

                                    for (int i = 0; i < testIds.Length; i++)
                                    {
                                        if (int.TryParse(testIds[i].Trim(), out int id))
                                        {
                                            string testRemark = (i < remarksList.Length) ? remarksList[i].Trim() : "";
                                            string testPart = (i < partsList.Length) ? partsList[i].Trim() : "";
                                            string testRequirement = (i < requirementsList.Length) ? requirementsList[i].Trim() : "";

                                            testInfoList.Add(new CustomerTestInfo
                                            {
                                                TestId = id,
                                                Remarks = testRemark,
                                                Part = testPart,
                                                Requirement = testRequirement
                                            });
                                        }
                                    }
                                }
                                // Otherwise, try to parse it as a single integer
                                else if (int.TryParse(testListValue, out int id))
                                {
                                    testInfoList.Add(new CustomerTestInfo
                                    {
                                        TestId = id,
                                        Remarks = remarks,
                                        Part = part,
                                        Requirement = requirement
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return testInfoList;
        }

        private int GetCustomerIDFromUrl()
        {
            // Extract customer ID from the URL
            if (Request.QueryString["ID"] != null)
            {
                return Convert.ToInt32(Request.QueryString["ID"]);
            }
            return 0; // Default or error value
        }

        protected void ddlservicerequired_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected value directly from the RadioButtonList
            string selectedService = radServiceRequired.SelectedValue;
            BindDeliveryDate(selectedService);
        }

        private void BindDeliveryDate(string service)
        {
            int days = 0;

            // Determine days based on selected service
            switch (service)
            {
                case "Regular":
                    days = 8;
                    break;
                case "Express":
                    days = 5;
                    break;
                case "Urgent":
                    days = 3;
                    break;
            }

            // Initial approximate delivery date
            DateTime ApproxDeliveryDate = DateTime.Now.AddDays(days);
            string currentYear = DateTime.Now.Year.ToString();

            // Get holiday data
            var holidayData = _cm.getHolidayDetails(currentYear);

            // Loop to check both holidays and weekends
            bool isHolidayOrWeekend = true;
            while (isHolidayOrWeekend)
            {
                isHolidayOrWeekend = false;
                // Check if ApproxDeliveryDate is a holiday
                foreach (DataRow holiday in holidayData.Rows)
                {
                    DateTime holidayDate = Convert.ToDateTime(holiday["Date"]);
                    // If ApproxDeliveryDate matches a holiday, add 1 more day
                    if (ApproxDeliveryDate.Date == holidayDate.Date)
                    {
                        ApproxDeliveryDate = ApproxDeliveryDate.AddDays(1);
                        isHolidayOrWeekend = true;  // Flag to check again after adjustment
                        break;  // Exit loop to recheck after adjusting
                    }
                }

                // Check if ApproxDeliveryDate is on a weekend (Saturday or Sunday)
                if (ApproxDeliveryDate.DayOfWeek == DayOfWeek.Saturday || ApproxDeliveryDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    ApproxDeliveryDate = ApproxDeliveryDate.AddDays(1);  // Add 1 more day to skip the weekend
                    isHolidayOrWeekend = true;  // Flag to check again after adjustment
                }
            }

            // Now, set the adjusted delivery date in the Textbox
            // Change format to yyyy-MM-dd for HTML5 date input compatibility
            servicerequiredby.Text = ApproxDeliveryDate.ToString("yyyy-MM-dd");
        }

        public void SaveSampleImage(int sampleId, string fileName, string createdDate, int enquiryId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO sampleimages (SampleID, filename, createdDate, EnquiryID) VALUES (@SampleID, @filename, @createdDate, @EnquiryID)", con))
                {
                    cmd.Parameters.AddWithValue("@SampleID", sampleId);
                    cmd.Parameters.AddWithValue("@filename", fileName);
                    cmd.Parameters.AddWithValue("@createdDate", createdDate);
                    cmd.Parameters.AddWithValue("@EnquiryID", enquiryId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void gvTests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkTest = (e.Row.FindControl("chkTest") as CheckBox);
                chkTest.Checked = true;
            }
        }


    }
}