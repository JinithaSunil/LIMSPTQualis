using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class QuotationCalib : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["QuotationDetails"] = new List<QuotationDetail>();
                txtDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                Populate();
                BindGrid();

                if (Request.QueryString["CustomerMasterID"] != null)
                {
                    string customerMasterId = Request.QueryString["CustomerMasterID"].ToString();
                    string inquiryId = Request.QueryString["id"].ToString();
                    ViewState["InquiryId"] = inquiryId;

                    DataTable dtCustomerdeptId = _cm.getCustomersDepartment(customerMasterId);
                    int customerDepartmentID = Convert.ToInt32(dtCustomerdeptId.Rows[0]["departmentid"].ToString());
                    ViewState["CustomerDepartmentID"] = dtCustomerdeptId.Rows[0]["departmentid"].ToString();

                    int enquiryId = Convert.ToInt32(inquiryId);
                    bool dataExists = false;
                    DataTable dtInquiryStatus = _cm.GetEnquiryStatus(enquiryId);
                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        if (dtInquiryStatus.Rows[0]["Instruments"].ToString() != "")
                        {
                            string[] s1 = dtInquiryStatus.Rows[0]["Instruments"].ToString().Split('|');
                            foreach (string s2 in s1)
                            {
                                string[] inst = s2.Split(':');
                                hdInstruments.Value += inst[2] + ",";
                            }
                        }
                        if (hdInstruments.Value.Length > 0)
                        {
                            hdInstruments.Value = hdInstruments.Value.Remove(hdInstruments.Value.Length - 1);
                            BindInstruments();
                        }
                    }

                        string query = "SELECT COUNT(EnquiryId) FROM QuotationMaster WHERE EnquiryId = @EnquiryId";

                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@EnquiryId", enquiryId);

                            conn.Open();
                            int count = Convert.ToInt32(cmd.ExecuteScalar());
                            dataExists = count > 0;
                        }
                    }

                    if (dataExists)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Quotation already generated, so Edit Quotation'); setTimeout(function() { window.location = 'QuotationCalibEditPage.aspx'; }, 10);</script>");
                    }

                    lblCustomerMasterId.Text = customerMasterId;
                    lblCustomerMasterId.Visible = false;
                    ddlCustomer.DataSource = _cm.getCustomers();
                    ddlCustomer.DataBind();
                    ddlCustomer.SelectedValue = customerMasterId;
                    ddlCustomer_SelectedIndexChanged(ddlCustomer, EventArgs.Empty);
                    ddlCustomer.Enabled = false;
                    BindInstruments();
                }
            }
        }

        private void BindQuotationGridforMech(DataTable dataTable)
        {
            gvQuotationDetails2.DataSource = dataTable;
            gvQuotationDetails2.DataBind();
            gvQuotationDetails2.Visible = true;

            gvQuotationDetails1.DataSource = null;
            gvQuotationDetails1.DataBind();
            gvQuotationDetails1.Visible = false;
        }

        private void BindQuotationGrid(DataTable dataTable)
        {
            gvQuotationDetails1.DataSource = dataTable;
            gvQuotationDetails1.DataBind();
            gvQuotationDetails1.Visible = true;

            gvQuotationDetails2.DataSource = null;
            gvQuotationDetails2.DataBind();
            gvQuotationDetails2.Visible = false;
        }



        private void Populate()
        {
            DataTable dt = _cm.getUOM();
           
            dt = _cm.getCustomers();
            if (dt.Rows.Count > 0)
            {
                ddlCustomer.DataSource = dt;
                ddlCustomer.DataTextField = "CompanyName";
                ddlCustomer.DataValueField = "CustomerMasterId";
                ddlCustomer.DataBind();
                ddlCustomer.Items.Insert(0, "--Select Customer--");
            }
            dt = _cm.getVAT();
            if (dt.Rows.Count > 0)
            {
                ddlVAT.DataSource = dt;
                ddlVAT.DataTextField = "VATCode";
                ddlVAT.DataValueField = "VATId";
                ddlVAT.DataBind();
                ddlVAT.Items.Insert(0, "--Select VAT--");
            }
            dt = _cm.getCurrency();
            if (dt.Rows.Count > 0)
            {
                ddlCurrency.DataSource = dt;
                ddlCurrency.DataTextField = "CurrencyName";
                ddlCurrency.DataValueField = "CurrencyID";
                ddlCurrency.DataBind();
                ddlCurrency.Items.Insert(0, "--Select Currency--");
            }
            dt = _cm.getPaymentTerms();
            if (dt.Rows.Count > 0)
            {
                ddlPaymentTerms.DataSource = dt;
                ddlPaymentTerms.DataTextField = "PaymentTermName";
                ddlPaymentTerms.DataValueField = "PaymentTermID";
                ddlPaymentTerms.DataBind();
                ddlPaymentTerms.Items.Insert(0, "--Select Payment Terms--");
            }
        }

        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod]
        public static List<string> GetProducts(string prefixText)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandText = @"SELECT ps.SubGroupName, ps.ProductGroupID, pg.ProductGroupName 
                               FROM [LIMSPTQualis].[dbo].[ProductSubGroup] ps
                               INNER JOIN [LIMSPTQualis].[dbo].[ProductGroup] pg 
                               ON ps.ProductGroupID = pg.ProductGroupID
                               WHERE ps.SubGroupName LIKE @Prefix + '%'
                               ORDER BY ps.SubGroupName";
                    com.Parameters.AddWithValue("@Prefix", prefixText);
                    com.Connection = con;
                    con.Open();
                    List<string> products = new List<string>();
                    using (SqlDataReader dr = com.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            products.Add(dr["SubGroupName"].ToString());

                        }
                    }
                    con.Close();
                    return products;
                }

            }

        }

        // ProductInfo class
        public class ProductInfo
        {
            public string SubGroupName { get; set; }
            public string ProductGroupID { get; set; }
            public string GroupName { get; set; }
            public bool IsBold { get; set; } = false;
            public bool IsMainParameter { get; set; } = false;
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCustomer.SelectedValue != "")
            {
                string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"SELECT AddressLine1, AddressLine2,AddressLine3, City, State, Country, Pincode 
                                        FROM LocationDetails WHERE CustomerMasterId = @CustomerMasterId";

                    if (Request.QueryString["CustomerMasterID"] != null)
                    {
                        cmd.Parameters.AddWithValue("@CustomerMasterId", lblCustomerMasterId.Text);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CustomerMasterId", ddlCustomer.SelectedItem.Value);
                    }
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Concatenate all address parts
                            string address = reader["AddressLine1"].ToString();
                            address += !string.IsNullOrEmpty(reader["AddressLine2"].ToString()) ? ", " + reader["AddressLine2"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["AddressLine3"].ToString()) ? ", " + reader["AddressLine3"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["City"].ToString()) ? ", " + reader["City"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["State"].ToString()) ? ", " + reader["State"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["Country"].ToString()) ? ", " + reader["Country"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["Pincode"].ToString()) ? "-" + reader["Pincode"].ToString() : "";

                            txtBillingAddress.Text = address;
                            txtShippingAddress.Text = address;
                        }
                    }
                    conn.Close();
                }
            }
            else
            {
                // Clear billing address if no company is selected
                txtBillingAddress.Text = string.Empty;
                txtShippingAddress.Text = string.Empty;
            }
        }

        protected void CalculateTotal(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQuantity.Text) &&
                !string.IsNullOrEmpty(txtPrice.Text) &&
                !string.IsNullOrEmpty(ddlVAT.SelectedValue))
            {
                decimal quantity = 0;
                decimal price = 0;
                decimal VAT = 0;
                decimal discount = 0;

                if (txtQuantity.Text != "") { quantity = Convert.ToDecimal(txtQuantity.Text); }
                if (txtPrice.Text != "") { price = Convert.ToDecimal(txtPrice.Text); }
                if (ddlVAT.SelectedIndex != 0) { VAT = Convert.ToDecimal(ddlVAT.SelectedValue); }
                if (txtDiscount.Text != "") { discount = Convert.ToDecimal(txtDiscount.Text); }

                decimal baseAmount = (quantity * price);

                decimal totalAmount = baseAmount;

                txtTotalAmount.Text = totalAmount.ToString("N2");
            }
        }

        public static List<ProductInfo> GetProductHierarchy(string productName)
        {
            List<ProductInfo> hierarchy = new List<ProductInfo>();

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandText = @"SELECT DISTINCT 
                                            pg.ProductGroupName, 
                                            psg.SubGroupName, 
                                            pssg.SubSubGroupName, 
                                            r.Requirement, 
                                            tp.TestParameter,
                                            tsp.TestSubParameter,
                                            tssp.TestSubSubParameter
                                        FROM ProductGroup pg 
                                        INNER JOIN ProductSubGroup psg ON psg.ProductGroupID = pg.ProductGroupID
                                        LEFT JOIN ProductSubSubGroup pssg ON pssg.SubGroupID = psg.SubGroupID
                                        INNER JOIN Requirement r ON r.DepartmentID = psg.DepartmentID 
                                        CROSS APPLY STRING_SPLIT(psg.TestParameterId, ',') AS splitParam
                                        INNER JOIN TestParameter tp ON tp.TestParameterID = TRY_CAST(splitParam.value AS INT)
                                        INNER JOIN TestSubParameter tsp ON tsp.TestParameterID = tp.TestParameterID
                                        INNER JOIN TestSubSubParameter tssp ON tssp.TestSubParameterID = tsp.TestSubParameterID
                                        WHERE psg.SubGroupName = @ProductName
                                        ORDER BY tp.TestParameter, tsp.TestSubParameter, tssp.TestSubSubParameter";

                    com.Parameters.AddWithValue("@ProductName", productName);
                    com.Connection = con;
                    con.Open();

                    // Data structures to hold grouped data
                    string currentProductGroup = "";
                    string currentRequirement = "";

                    // Dictionary to group test parameters and their sub-parameters
                    Dictionary<string, Dictionary<string, List<string>>> testHierarchy =
                        new Dictionary<string, Dictionary<string, List<string>>>();

                    using (SqlDataReader dr = com.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // Get current row data
                            if (string.IsNullOrEmpty(currentProductGroup))
                                currentProductGroup = dr["ProductGroupName"].ToString();

                            if (string.IsNullOrEmpty(currentRequirement))
                                currentRequirement = dr["Requirement"].ToString();

                            string testParameter = dr["TestParameter"]?.ToString();
                            string testSubParameter = dr["TestSubParameter"]?.ToString();
                            string testSubSubParameter = dr["TestSubSubParameter"]?.ToString();

                            // Group the hierarchy data
                            if (!string.IsNullOrEmpty(testParameter))
                            {
                                if (!testHierarchy.ContainsKey(testParameter))
                                {
                                    testHierarchy[testParameter] = new Dictionary<string, List<string>>();
                                }

                                if (!string.IsNullOrEmpty(testSubParameter))
                                {
                                    if (!testHierarchy[testParameter].ContainsKey(testSubParameter))
                                    {
                                        testHierarchy[testParameter][testSubParameter] = new List<string>();
                                    }

                                    if (!string.IsNullOrEmpty(testSubSubParameter))
                                    {
                                        if (!testHierarchy[testParameter][testSubParameter].Contains(testSubSubParameter))
                                        {
                                            testHierarchy[testParameter][testSubParameter].Add(testSubSubParameter);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Build the hierarchical HTML structure
                    if (testHierarchy.Count > 0)
                    {
                        string hierarchyText = "";

                        // Add Product Group
                        if (!string.IsNullOrEmpty(currentProductGroup))
                        {
                            hierarchyText += $"<h5>{currentProductGroup}</h5><br/>";
                        }

                        // Add Requirement
                        if (!string.IsNullOrEmpty(currentRequirement))
                        {
                            hierarchyText += $"<h5>{currentRequirement}</h5><br/>";
                        }

                        // Build test hierarchy
                        foreach (var testParam in testHierarchy)
                        {

                            hierarchyText += $"<strong>{testParam.Key}</strong>";

                            foreach (var testSubParam in testParam.Value)
                            {

                                hierarchyText += $"<h6>{testSubParam.Key}</h6>";

                                // Add all Test Sub Sub Parameters (h5 equivalent)
                                if (testSubParam.Value.Count > 0)
                                {
                                    string subSubParams = string.Join(", ", testSubParam.Value);
                                    hierarchyText += $"<strong>{subSubParams}</strong>";
                                }
                            }
                        }

                        hierarchy.Add(new ProductInfo
                        {
                            SubGroupName = hierarchyText,
                            ProductGroupID = "3",
                            GroupName = "ActualItem"
                        });
                    }

                    con.Close();
                }
            }

            return hierarchy;
        }

        // Add this RowDataBound event handler

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            if (ddlInstruments.SelectedIndex == 0)
            {
                ddlInstruments.BackColor = Color.LightPink;
                return;
            }
            else
            {
                txtTestItem.Text = ddlInstruments.SelectedItem.ToString();
            }
                
            if (string.IsNullOrEmpty(txtTestItem.Text))
            {// Show error message
                return;
            }

            List<QuotationDetail> details = ViewState["QuotationDetails"] as List<QuotationDetail> ?? new List<QuotationDetail>();

            decimal vatPercentage = 0;
            if (ddlVAT.SelectedIndex > 0)
            {
                vatPercentage = Convert.ToDecimal(ddlVAT.SelectedValue);
            }

            decimal quantity = Convert.ToDecimal(txtQuantity.Text);
            decimal price = Convert.ToDecimal(txtPrice.Text);
            decimal baseAmount = quantity * price;
            decimal vatAmount = baseAmount * (vatPercentage / 100);
            decimal totalAmount = baseAmount;
            //decimal totalAmount = baseAmount + vatAmount;//

            var detail = new QuotationDetail
            {
                SampleDescription = txtSampleDescription.Text,
                TestItems = txtTestItemsData.Text,
                ProductName = txtTestItem.Text,
                Quantity = quantity,
                UOM = "1",
                Price = price,
                VATPercentage = vatPercentage,
                VATAmount = vatAmount,
                TotalAmount = totalAmount
            };

            details.Add(detail);
            ViewState["QuotationDetails"] = details;
            BindGrid();
            ClearProductFields();
            CalculateGrandTotal(sender, e);
        }



        protected void gvQuotation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as QuotationDetail;
                if (item != null)
                {
                    var productNameCell = e.Row.Cells[0];

                    switch (item.GroupName)
                    {
                        case "ProductGroup":
                            productNameCell.Text = $"<strong>{item.ProductName}</strong>";
                            e.Row.CssClass = "product-group-row";
                            break;

                        case "Requirement":
                            productNameCell.Text = $"<em>{item.ProductName}</em>";
                            e.Row.CssClass = "requirement-row";
                            break;

                        case "ActualItem":
                            productNameCell.Text = item.ProductName;
                            e.Row.CssClass = "actual-item-row";
                            break;

                        default:
                            productNameCell.Text = item.ProductName;
                            break;
                    }

                    if (item.IsRequirement)
                    {
                        e.Row.Cells[1].Text = ""; // Quantity
                        e.Row.Cells[2].Text = ""; // UOM  
                        e.Row.Cells[3].Text = ""; // Price
                        e.Row.Cells[4].Text = ""; // Total Amount
                        e.Row.Cells[5].Visible = false; // Remove button
                    }
                }
            }
        }

        // Keep existing BindGrid method as is
        private void BindGrid()
        {
            List<QuotationDetail> details = ViewState["QuotationDetails"] as List<QuotationDetail> ?? new List<QuotationDetail>();
            gvQuotationDetails.DataSource = details;
            gvQuotationDetails.DataBind();
        }

        // Keep existing remove method as is
        protected void gvQuotationDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                List<QuotationDetail> details = ViewState["QuotationDetails"] as List<QuotationDetail>;
                if (details != null && rowIndex < details.Count)
                {
                    details.RemoveAt(rowIndex);
                    ViewState["QuotationDetails"] = details;
                    BindGrid();
                    CalculateGrandTotal(sender, e);
                }
            }
        }
        // Updated save method - saves all items

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Save Quotation Master (keep existing code as is)
                            int quotationMasterId;
                            using (SqlCommand cmd = new SqlCommand(@"INSERT INTO QuotationMaster (UserId, QuotationNumber, QuotationValidityInDays, 
                                                                        CompanyId, PaymentTermId, Remarks, QuotationDate, QuotationStatusId, 
                                                                        IsMailSent, Counter, Year, CreatedDate, CustomerMasterID, Discount,
                                                                        GrandTotal,CurrencyID, VATId, VatAmount,Revisioncount,EnquiryId) 
                                                                        VALUES 
                                                                        (@UserId, @QuotationNumber, @QuotationValidityInDays, @CompanyId,
                                                                        @PaymentTermId, @Remarks, @QuotationDate, @QuotationStatusId,
                                                                        @IsMailSent, @Counter, @Year, @CreatedDate, @CustomerMasterID, @Discount, 
                                                                        @GrandTotal, @CurrencyID, @VATId, @VatAmount, @Revisioncount, @EnquiryId); 
                                                                        SELECT SCOPE_IDENTITY();", conn, transaction))
                            {
                                string quotationNumber = GenerateQuotationNumber();
                                int userId = Convert.ToInt32(Session["UserId"]);
                                int counter = Convert.ToInt32(quotationNumber.Substring(quotationNumber.Length - 4));
                                int currentYear = DateTime.Now.Year;

                                object vatIdValue = DBNull.Value;
                                if (!string.IsNullOrEmpty(hdnVATId.Value))
                                {
                                    vatIdValue = Convert.ToInt32(hdnVATId.Value);
                                }
                                decimal vatAmount = 0;
                                decimal.TryParse(lblVATAmount.Text, out vatAmount);
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                cmd.Parameters.AddWithValue("@QuotationNumber", quotationNumber);
                                cmd.Parameters.AddWithValue("@QuotationValidityInDays", Convert.ToInt32(txtValidity.Text));
                                cmd.Parameters.AddWithValue("@CompanyId", ddlCustomer.SelectedValue);
                                cmd.Parameters.AddWithValue("@PaymentTermId", ddlPaymentTerms.SelectedValue);
                                cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);
                                cmd.Parameters.AddWithValue("@QuotationDate", txtDate.Text);
                                cmd.Parameters.AddWithValue("@QuotationStatusId", 1);
                                cmd.Parameters.AddWithValue("@IsMailSent", false);
                                cmd.Parameters.AddWithValue("@Counter", counter);
                                cmd.Parameters.AddWithValue("@Year", currentYear);
                                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                                cmd.Parameters.AddWithValue("@CustomerMasterID", ddlCustomer.SelectedValue);
                                cmd.Parameters.AddWithValue("@Discount", Convert.ToDecimal(txtDiscount.Text));
                                cmd.Parameters.AddWithValue("@GrandTotal", Convert.ToDecimal(lblGrandTotal.Text.Replace(",", "")));
                                cmd.Parameters.AddWithValue("@CurrencyID", ddlCurrency.SelectedValue);
                                cmd.Parameters.AddWithValue("@VATId", vatIdValue);
                                cmd.Parameters.AddWithValue("@VatAmount", vatAmount);
                                cmd.Parameters.AddWithValue("@Revisioncount", 0);
                                cmd.Parameters.AddWithValue("@EnquiryId", Convert.ToInt32(ViewState["InquiryId"].ToString()));

                                quotationMasterId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Save ALL quotation details (including parameters and sub-parameters)
                            List<QuotationDetail> details = ViewState["QuotationDetails"] as List<QuotationDetail>;

                            foreach (var detail in details)
                            {
                                using (SqlCommand detailCmd = new SqlCommand(@"INSERT INTO QuotationDetails 
                                                                      (QuotationMasterID, UserId, ProductId, UOMId,
                                                                      Quantity, Price, TotalAmount, IsDelete, CreatedDate, 
                                                                      TestItems, TestItemDetails, SampleDescription, revisioncount) 
                                                                      VALUES 
                                                                      (@QuotationMasterID, @UserId, @ProductId, @UOMId,
                                                                      @Quantity, @Price, @TotalAmount, @IsDelete, @CreatedDate, 
                                                                      @TestItems, @TestItemDetails, @SampleDescription, @revisioncount)", conn, transaction))
                                {
                                    int userId = Convert.ToInt32(Session["UserId"]);
                                    DateTime currentDateTime = DateTime.Now;

                                    detailCmd.Parameters.AddWithValue("@QuotationMasterID", quotationMasterId);
                                    detailCmd.Parameters.AddWithValue("@UserId", userId);
                                    detailCmd.Parameters.AddWithValue("@ProductId", DBNull.Value);
                                    detailCmd.Parameters.AddWithValue("@UOMId", 1032);
                                    detailCmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                    detailCmd.Parameters.AddWithValue("@Price", detail.Price);
                                    detailCmd.Parameters.AddWithValue("@TotalAmount", detail.TotalAmount);
                                    detailCmd.Parameters.AddWithValue("@IsDelete", false);
                                    detailCmd.Parameters.AddWithValue("@CreatedDate", currentDateTime);
                                    detailCmd.Parameters.AddWithValue("@TestItems", detail.ProductName.Trim());
                                    detailCmd.Parameters.AddWithValue("@TestItemDetails", detail.TestItems);
                                    detailCmd.Parameters.AddWithValue("@SampleDescription", detail.SampleDescription);
                                    detailCmd.Parameters.AddWithValue("@revisioncount", 1);

                                    detailCmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            ClearForm();
                            //Update QuotationId in CalibrationRequest
                            _cm.QuotationID = quotationMasterId;
                            _cm.EnquiryID = Convert.ToInt32(Request.QueryString["id"].ToString());
                            _cm.UpdateQuotationCalibrationRequest(_cm);

                            string url = $"GenerateQuotationCalib.aspx?ID={quotationMasterId}&r=1";
                            string redirectUrl = "QuotationList.aspx";
                            string script = $@"alert('Quotation saved successfully!');
                                               window.open('{url}', '_blank');         
                                               window.location.href = '{redirectUrl}'; 
                                              ";
                            ScriptManager.RegisterStartupScript(this, GetType(), "OpenTabAndRedirect", script, true);

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                    $"alert('Error: {ex.Message}');", true);
            }
        }

        private string GenerateQuotationNumber()
        {
            string prefix = "QQ";
            string dateString = DateTime.Now.ToString("yyyyMM");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ISNULL(MAX(CAST(SUBSTRING(QuotationNumber, LEN(@Prefix) + LEN(@DateString) + 2, 4) AS INT)), 0) + 1 " +
                    "FROM QuotationMaster " +
                    "WHERE QuotationNumber LIKE @Prefix + @DateString + '%'", conn))
                {
                    cmd.Parameters.AddWithValue("@Prefix", prefix);
                    cmd.Parameters.AddWithValue("@DateString", dateString);

                    // Get the next sequence number
                    int sequence = Convert.ToInt32(cmd.ExecuteScalar());

                    // Return the formatted quotation number with sequence padded to 4 digits
                    return $"{prefix}{dateString}{sequence:D4}";
                }
            }
        }

        private void ClearProductFields()
        {
            txtTestItem.Text = null;
            txtQuantity.Text = "0";
            txtPrice.Text = "0";
            txtDiscount.Text = "0";
            ddlVAT.SelectedIndex = 0;
            txtTotalAmount.Text = "0";
        }

        private void ClearForm()
        {
            // Clear all fields
            ddlCustomer.SelectedIndex = 0;
            txtValidity.Text = "";

            txtBillingAddress.Text = "";
            txtShippingAddress.Text = "";
            txtTestItem.Text = "";
            txtQuantity.Text = "";
            txtPrice.Text = "";
            ddlVAT.SelectedIndex = 0;
            txtDiscount.Text = "";
            txtTotalAmount.Text = "";
            ddlPaymentTerms.SelectedIndex = 0;
            ddlCurrency.SelectedIndex = 0;
            txtRemarks.Text = "";
            lblGrandTotal.Text = "";
            lblVATAmount.Text = "";
            lblTotalBeforeTax.Text = "";
            lblSubTotal.Text = "";
            // Clear the GridView data
            gvQuotationDetails.DataSource = null;
            gvQuotationDetails.DataBind();

            // Enable the disabled fields
            ddlCustomer.Enabled = true;
            txtValidity.Enabled = true;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            // Clear all fields
            ddlCustomer.SelectedIndex = 0;
            txtValidity.Text = "";
            txtBillingAddress.Text = "";
            txtShippingAddress.Text = "";
            txtTestItem.Text = "";
            txtQuantity.Text = "";
            txtPrice.Text = "";
            ddlVAT.SelectedIndex = 0;
            txtDiscount.Text = "";
            txtTotalAmount.Text = "";
            ddlPaymentTerms.SelectedIndex = 0;
            ddlCurrency.SelectedIndex = 0;
            txtRemarks.Text = "";
            lblGrandTotal.Text = "";
            lblVATAmount.Text = "";
            lblTotalBeforeTax.Text = "";
            lblSubTotal.Text = "";
            // Clear the GridView data
            gvQuotationDetails.DataSource = null;
            gvQuotationDetails.DataBind();

            // Enable the disabled fields
            ddlCustomer.Enabled = true;
            txtValidity.Enabled = true;
        }

        protected void CalculateGrandTotal(object sender, EventArgs e)
        {
            try
            {
                List<QuotationDetail> details = ViewState["QuotationDetails"] as List<QuotationDetail> ?? new List<QuotationDetail>();

                // Calculate Sub Total (before VAT)
                decimal subTotal = details.Sum(d => d.Quantity * d.Price);
                lblSubTotal.Text = subTotal.ToString("N2");

                // Get discount value
                decimal discount = 0;
                if (!string.IsNullOrEmpty(txtDiscount.Text))
                {
                    decimal.TryParse(txtDiscount.Text.Trim(), out discount);
                }

                // Calculate total before tax
                decimal totalBeforeTax = subTotal - discount;
                //lblTotalBeforeTax.Text = totalBeforeTax.ToString("N2");
                lblTotalBeforeTax.Text = totalBeforeTax.ToString();

                // Calculate VAT amount
                decimal vatAmount = 0;
                if (!string.IsNullOrEmpty(hdnVATId.Value))
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT Percentage FROM VAT WHERE VATId = @VATId", conn))
                        {
                            cmd.Parameters.AddWithValue("@VATId", hdnVATId.Value);
                            conn.Open();
                            object result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                decimal vatPercentage = Convert.ToDecimal(result);
                                vatAmount = totalBeforeTax * (vatPercentage / 100);
                            }
                        }
                    }
                }
                lblVATAmount.Text = vatAmount.ToString("N2");


                // Calculate grand total
                decimal grandTotal = totalBeforeTax + vatAmount;
                lblGrandTotal.Text = grandTotal.ToString("N2");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"alert('Error calculating totals: {ex.Message}');", true);
            }
        }

        protected void ddlCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCurrency.SelectedIndex > 0)
            {
                string currencyCode = GetCurrencyCode(ddlCurrency.SelectedValue);
                lblPrice.Text = $"Price ({currencyCode}):";
                lblTotalAmount.Text = $"Total Amount ({currencyCode}):";
            }
            else
            {
                lblPrice.Text = "Price (IDR):";
                lblTotalAmount.Text = "Total Amount (IDR):";
            }
        }

        private string GetCurrencyCode(string currencyId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string sQuery = "SELECT CurrencyCode FROM Currency WHERE CurrencyID = @CurrencyID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CurrencyID", currencyId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "IDR";
                }
            }
        }

        protected void ddlVAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlVAT.SelectedIndex > 0)
            {
                hdnVATId.Value = ddlVAT.SelectedValue;
            }
            else
            {
                hdnVATId.Value = "";
            }
            CalculateGrandTotal(sender, e);
        }
        protected void BindInstruments()
        {
            DataTable dt = new DataTable();
            if (hdInstruments.Value.Length > 0)
            {
                dt = _cm.getCalibInstruments(hdInstruments.Value);
                if (dt.Rows.Count > 0)
                {
                    ddlInstruments.DataSource = dt;
                    ddlInstruments.DataTextField = "InstrumentName";
                    ddlInstruments.DataValueField = "InstrumentID";
                    ddlInstruments.DataBind();
                    ddlInstruments.Items.Insert(0, "Select Instrument");
                }
            }
        }

    }

}