using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerFMN_Transportation : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());
        string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
					BindServiceRequired();
                    DataTable dtInquiryStatus = _cm.GetEnquiryStatus(InquiryId);
                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        int inquiryStatus = Convert.ToInt32(dtInquiryStatus.Rows[0]["inquiryStatusID"].ToString());
                        if (inquiryStatus == 5)
                        {
                            divCRF.Visible = true;
                            divFail.Visible = false;
                            Session["InquiryID"] = InquiryId;
                            lblInquiry.Text = "ENQ" + InquiryId;
                            hdInquiryId.Value = InquiryId.ToString();
                            int ReportID = Convert.ToInt32(dtInquiryStatus.Rows[0]["ReportID"].ToString());
                            ViewState["ReportID"] = ReportID;


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
                            }

                            BindContactPerson(InquiryId);

                            int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());
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

                                Session["DT"] = dt1;
                                btnSubmit.Visible = true;
                            }
                        }
                        else
                        {
                            divCRF.Visible = false;
                            divFail.Visible = true;
                        }
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string batchNo = txtBatchNo.Text;
                string sizeOfSample = txtSizeofSample.Text;
                string sampleName = txtSampleName.Text;
                bool b1 = false;

                // Get all checked test parameters
                List<string> selectedTests = GetSelectedTests();

                if (selectedTests.Count > 0)
                {
                    b1 = true;
                }
                else
                {
                    b1 = false;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Please select at least one Test Parameter/Analysis Test');", true);                    
                }

                if (b1 == true)
                {
                    if (Session["OrganizationID"] != null)
                    {
                        _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());

                        if (ddlContactPerson.SelectedIndex != 0)
                        {
                            int inquiryId = Convert.ToInt32(hdInquiryId.Value);
                            _cm.personId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                            _cm.customerId = Convert.ToInt32(Session["CustomerId"]);
                            int RequestId = 0;
                            _cm.dateOfRequest = Convert.ToDateTime(txtDateOfRequest.Text);
                            _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                            _cm.shippingAgency = txtShippingAgency.Text.ToString();
                            _cm.airwayBillNo = txtAirwayBillNo.Text.ToString();
                            _cm.requestTypeId = 1;
                            RequestId = _cm.AddCustomerRequest(_cm);
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
                            _cm.ProductType = txtProductType.Text;
                            //_cm.PO = txtPO.Text;
                            _cm.CountryOfOrigin = txtOriginCountry.Text;
                            _cm.CountryOfDestination = txtDesctinationCountry.Text;
                            //_cm.ItemStyle = txtItemStyleNo.Text;
                            //_cm.ArticleNumber = txtArticleNo.Text;
                            //_cm.LotNumber = txtLotNo.Text;
                            //_cm.MaterialFiberType = txtMaterialFiberType.Text;
                            //_cm.BrandProduct = txtBrandProduct.Text;
                            _cm.FactoryName = txtFactoryName.Text;
                            _cm.FactoryAddress = txtFactoryAddress.Text;
                            _cm.SupplierName = txtSupplierName.Text;
                            _cm.customerRequestId = Convert.ToInt32(Session["RequestId"]);
                            _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                            _cm.sampleName = txtSampleName.Text;
                            _cm.sizeOfSample = txtSizeofSample.Text;
                            _cm.SampleType = txtProductType.Text;
                            _cm.batchNo = txtBatchNo.Text;
                            _cm.ReportID = ViewState["ReportID"].ToString();

                            int additionalDetailsID = _cm.AddAdditionalDetailsTransport(_cm);
                            if (additionalDetailsID > 0)
                            {
                                foreach (string testId in selectedTests)
                                {
                                    string testName = GetTestParameterName(testId);
                                    _cm.testParameterID = testId;
                                    _cm.testParameter = testName;

                                    _cm.AddCustomerCRFSampleDetails_Transport(_cm);
                                    System.Diagnostics.Debug.WriteLine($"Saving Test: ID={testId}, Name={testName}");
                                }
                                ////change customer inquiry Status to CRF Received
                                _cm.inquiryStatusId = 6;
                                _cm.UpdateInquiryStatus(_cm);

                                Response.Redirect("CreateRequestCustomerFM_Transport.aspx?ID=" + Session["InquiryID"] + "&r=1");
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                                    "<script>alert('An error occurred while adding details!');</script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                                "<script>alert('Please select a contact person!');</script>");
                        }
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                            "<script>alert('Session expired! Please login again.');</script>");
                    }
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        // Method to get all selected test parameters from Repeater
        private List<string> GetSelectedTests()
        {
            List<string> selectedTests = new List<string>();

            foreach (RepeaterItem item in rptTests.Items)
            {
                CheckBox chkTest = (CheckBox)item.FindControl("chkTestParam");
                HiddenField hdnTestID = (HiddenField)item.FindControl("hdnTestID");

                if (chkTest != null && hdnTestID != null && chkTest.Checked)
                {
                    selectedTests.Add(hdnTestID.Value);
                }
            }

            return selectedTests;
        }

        // Method to get test parameter name from database
        private string GetTestParameterName(string testId)
        {
            string testName = string.Empty;
            string query = "SELECT TestParameter FROM TestParameter WHERE TestParameterID = @TestParameterID";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TestParameterID", testId);

                if (con.State != ConnectionState.Open)
                    con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null)
                    testName = result.ToString();

                con.Close();
            }

            return testName;
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


    }
}
