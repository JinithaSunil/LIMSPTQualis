using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerFMN_Chemical : System.Web.UI.Page
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
                            BindRequirement();
                            string str1 = @"SELECT 
                                        t.TestParameterID,t.TestParameter,ts.TestSubParameterID, 
                                        ts.TestSubParameter, sgt.RequirementId,sgt.SubGroupName, 
                                        tss.TestSubSubParameterID,tss.TestSubSubParameter, 
                                        t.UOMName, ts.CASNo, ts.MDL, ts.Specification, ts.protocol AS TestMethod 
                                    FROM 
                                        (SELECT * FROM ProductSubGroup p
                                        CROSS APPLY STRING_SPLIT(TestParameterId, ',')) x
                                    INNER JOIN CustomerInquiry ci ON ci.productCategoryID = x.subgroupid
                                    INNER JOIN TestParameter t ON t.TestParameterID = x.value
                                    INNER JOIN (SELECT * FROM ProductSubGroup p2
                                               CROSS APPLY STRING_SPLIT(TestSubParameterId, ',')) y 
                                               ON y.SubGroupID = ci.productCategoryID
                                    INNER JOIN TestSubParameter ts ON ts.TestSubParameterID = y.value AND ts.TestParameterID = t.TestParameterID
                                    LEFT JOIN TestSubSubParameter tss ON tss.testsubparameterid = ts.TestSubParameterID
                                    LEFT JOIN ProductSubGroup sgt ON sgt.SubGroupID = ci.productCategoryID
                                    WHERE ci.InquiryId= '" + InquiryId + "' ";

                            SqlDataAdapter sda = new SqlDataAdapter(str1, con);
                            con.Open();
                            DataTable dt1 = new DataTable();
                            sda.Fill(dt1);

                            if (dt1.Rows.Count > 0)
                            {
                                txtSampleName.Text = dt1.Rows[0]["SubGroupName"].ToString();
                                ddlRequirement.SelectedValue = dt1.Rows[0]["RequirementId"].ToString();
                                Session["DT"] = dt1;
                                gvCRF.DataSource = dt1;
                                gvCRF.DataBind();
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
            if (ddlTestMethod.SelectedIndex == 0)
            {
                flag = 0;
                ddlTestMethod.BackColor = Color.LightPink;
            }
            if (ddlTestMethod.Items.Count == 0)
            {
                flag = 2;
            }
            if (ddlUOM.SelectedIndex == 0)
            {
                flag = 0;
                ddlUOM.BackColor = Color.LightPink;
            }
            if (ddlUOM.Items.Count == 0)
            {
                flag = 2;
            }
            if (flag == 0)
            {
                DataTable dt;
                if (Session["DT"] != null)
                {
                    dt = (DataTable)Session["DT"];

                    // Ensure all required columns exist
                    string[] requiredColumns = {
                "SampleName", "BatchNo", "TestMethod", "RequirementID", "Requirement",
                "TestParameterID", "TestParameter", "TestSubParameterID", "TestSubParameter",
                "TestSubSubParameterID", "TestSubSubParameter", "Specification", "UOMName",
                "CASNo", "MDL", "SizeofSample", "Remarks"
            };

                    foreach (string colName in requiredColumns)
                    {
                        if (!dt.Columns.Contains(colName))
                        {
                            dt.Columns.Add(colName, typeof(string));
                        }
                    }
                }
                else
                {
                    dt = new DataTable();

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
                    dt.Columns.Add("CASNo");
                    dt.Columns.Add("MDL");
                    dt.Columns.Add("SizeofSample");
                    dt.Columns.Add("Remarks");
                }

                lblSampleName.Text = txtSampleName.Text;
                lblBatchNo.Text = txtBatchNo.Text;
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
                dr["SampleName"] = txtSampleName.Text;
                dr["BatchNo"] = txtBatchNo.Text;
                dr["SizeofSample"] = txtSizeofSample.Text;
                dr["Remarks"] = txtRemarks.Text;
                if (ddlTestMethod.SelectedItem != null)
                    dr["TestMethod"] = ddlTestMethod.SelectedItem.Text;
                else
                    dr["TestMethod"] = "";
                dr["TestParameterID"] = lblTestParameterID.Text;
                dr["TestParameter"] = txtTestParameter.Text;

                if (ddlTestSubSubParameter.SelectedIndex > 0)
                {
                    dr["TestSubSubParameterID"] = ddlTestSubSubParameter.SelectedValue.ToString();
                    dr["TestSubSubParameter"] = ddlTestSubSubParameter.SelectedItem.ToString();
                }
                else
                {
                    dr["TestSubSubParameterID"] = DBNull.Value;
                    dr["TestSubSubParameter"] = "";
                }

                if (ddlTestSubParameter.SelectedIndex > 0)
                {
                    dr["TestSubParameterID"] = ddlTestSubParameter.SelectedValue.ToString();
                    dr["TestSubParameter"] = ddlTestSubParameter.SelectedItem.ToString();
                }
                else
                {
                    dr["TestSubParameterID"] = "";
                    dr["TestSubParameter"] = "";
                }

                dr["Specification"] = txtSpecification.Text;
                if (ddlUOM.SelectedIndex > 0)
                {
                    dr["UOMName"] = ddlUOM.SelectedValue.ToString();
                }
                else
                {
                    dr["UOMName"] = "";
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

                dr["CASNo"] = txtCasNumber.Text;
                dr["MDL"] = txtMDL.Text;
                dt.Rows.Add(dr);

                gvCRF.DataSource = dt;
                gvCRF.DataBind();
                btnSubmit.Visible = gvCRF.Rows.Count > 0;
                Session["DT"] = dt;
                Clear();
            }
            else if (flag == 2)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('No Test Method or UOM found for Test Parameter');", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Fill highlighted fields');", true);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string batchNo = txtBatchNo.Text;
                string sizeOfSample = txtSizeofSample.Text;
                string sampleName = txtSampleName.Text;

                if (string.IsNullOrWhiteSpace(batchNo) || string.IsNullOrWhiteSpace(sizeOfSample) || string.IsNullOrWhiteSpace(sampleName))
                {

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Sample Name , Batch Number and Size of Sample can't be blank!');</script>");
                }
                else
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
                                _cm.PO = txtPO.Text;
                                _cm.CountryOfOrigin = txtOriginCountry.Text;
                                _cm.CountryOfDestination = txtDesctinationCountry.Text;
                                _cm.ItemStyle = txtItemStyleNo.Text;

                                _cm.ArticleNumber = txtArticleNo.Text;
                                _cm.LotNumber = txtLotNo.Text;
                                _cm.MaterialFiberType = txtMaterialFiberType.Text;
                                _cm.BrandProduct = txtBrandProduct.Text;
                                _cm.FactoryName = txtFactoryName.Text;
                                _cm.FactoryAddress = txtFactoryAddress.Text;
                                //_cm.BuyersName = txtBuyersNameDivision.Text;
                                _cm.SupplierName = txtSupplierName.Text;

                                int additionalDetailsID = _cm.AddAdditionalDetailsChemical(_cm);
                                if (additionalDetailsID > 0)
                                {
                                if (Session["DT"] != null)
                                {
                                    if (Session["OrganizationID"] != null)
                                    {
                                        _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                                        DataTable dt = (DataTable)Session["DT"];

                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            if (!dr.Table.Columns.Contains("SampleName"))
                                            {
                                                dr.Table.Columns.Add("SampleName", typeof(string)); // Add the column if it doesn't exist
                                            }
                                            dr["SampleName"] = txtSampleName.Text.ToString();


                                            if (!dr.Table.Columns.Contains("BatchNo"))
                                            {
                                                dr.Table.Columns.Add("BatchNo", typeof(string)); // Add the column if it doesn't exist
                                            }
                                            dr["BatchNo"] = txtBatchNo.Text.ToString();

                                            if (!dr.Table.Columns.Contains("SizeofSample"))
                                            {
                                                dr.Table.Columns.Add("SizeofSample", typeof(string)); // Add the column if it doesn't exist
                                            }
                                            dr["SizeofSample"] = txtSizeofSample.Text.ToString();

                                            if (!dr.Table.Columns.Contains("Remarks"))
                                            {
                                                dr.Table.Columns.Add("Remarks", typeof(string)); // Add the column if it doesn't exist
                                            }
                                            dr["Remarks"] = txtRemarks.Text.ToString();

                                            if (!dr.Table.Columns.Contains("Requirement"))
                                            {
                                                dr.Table.Columns.Add("Requirement", typeof(string));
                                            }
                                            if (!dr.Table.Columns.Contains("RequirementID"))
                                            {
                                                dr.Table.Columns.Add("RequirementID", typeof(string));
                                            }

                                            string selectedValue = ddlRequirement.SelectedValue.ToString();

                                            if (int.TryParse(selectedValue, out int requirementId))
                                            {
                                                dr["RequirementID"] = requirementId;
                                            }
                                            else
                                            {
                                                dr["RequirementID"] = DBNull.Value;  // or some other default value
                                            }

                                            dr["Requirement"] = ddlRequirement.SelectedItem.ToString();


                                            _cm.customerRequestId = Convert.ToInt32(Session["RequestId"]);
                                            _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                                            _cm.sampleName = dr["SampleName"] != DBNull.Value ? dr["SampleName"].ToString() : txtSampleName.Text;
                                            _cm.sizeOfSample = dr["SizeofSample"] != DBNull.Value ? dr["SizeofSample"].ToString() : txtSampleName.Text;
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
                                            _cm.CASNo = dr["CASNo"].ToString();
                                            _cm.MDL = dr["MDL"].ToString();
                                            _cm.UOMName = dr["UomName"].ToString();
                                            _cm.remarks = dr["Remarks"].ToString();
                                            _cm.CRFID = 4; /* FOR CHEMICAL*/
                                            _cm.RequirementID = dr["RequirementID"].ToString();
                                            _cm.Requirement = dr["Requirement"].ToString();

                                            _cm.AddCustomerCRFSampleDetails_Chemical(_cm);

                                        }
                                    }
                                }
                                //change customer inquiry Status to CRF Received
                                _cm.inquiryStatusId = 6;
                                _cm.UpdateInquiryStatus(_cm);

                                ClearAll();
                                Response.Redirect("CreateRequestCustomerFM_Chemical.aspx?ID=" + Session["InquiryID"] + "&r=1");
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('An error occurred while adding details!');", true);
                                    // Show error message in alert box
                                    string errorMessage = "An error occurred while adding details.";
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + errorMessage.Replace("'", "\\'") + "'); </script>");
                                }
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
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
                int index = Convert.ToInt32(e.CommandArgument);
                DataTable dt = Session["DT"] as DataTable;

                if (dt != null && index >= 0 && index < dt.Rows.Count)
                {
                    dt.Rows[index].Delete();
                    dt.AcceptChanges();  // Ensure the changes are committed
                    Session["DT"] = dt;  // Update the session with modified DataTable
                    gvCRF.DataSource = dt;
                    gvCRF.DataBind();
                }
                btnSubmit.Visible = gvCRF.Rows.Count > 0;
            }
        }
        
        private void Clear()
        {
            // txtTestParameter.Text = "";
            //txtSpecification.Text = "";
            //txtLOQ.Text = "";
            //txtLOD.Text = "";
            //ddlUOM.Items.Clear();
            //ddlTestMethod.Items.Clear();
            //ddlTestSubParameter.Items.Clear();
            //ddlTestSubSubParameter.Items.Clear();
            //txtRemarks.Text = "";
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
            //txtCasNumber.Text = ""; txtMDL.Text = ""; txtCodeCombination.Text = "";
            string param = ""; DataTable dt = new DataTable();
            if (txtTestParameter.Text != "")
            {
                param = txtTestParameter.Text;
                //DataTable dtParam = _cm.getParameter(param);
                DataTable dtParam = _cm.getParameter_Chemical(param);
                if (dtParam.Rows.Count > 0)
                {
                    lblTestParameterID.Text = dtParam.Rows[0]["TestParameterID"].ToString();
                }
            }

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
                ddlTestSubParameter.Items.Clear();
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
                    txtCasNumber.Text = dt.Rows[0]["CASNo"].ToString();
                    txtMDL.Text = dt.Rows[0]["MDL"].ToString();
                    //txtCodeCombination.Text = dt.Rows[0]["CodeCombination"].ToString();
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
                    ddlTestSubSubParameter.Items.Clear();
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