using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace LIMS
{
    public partial class QuotationCalibEditPage : System.Web.UI.Page
    {
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");


        [Serializable]
        public class QuotationDetail
        {
            public string SampleDescription { get; set; }
            public string ProductName { get; set; }

            public string TestItems { get; set; }
            public decimal Quantity { get; set; }
            public string UOM { get; set; }
            public decimal Price { get; set; }
            public decimal VATAmount { get; set; }
            public decimal VATPercentage { get; set; }
            public decimal TotalAmount { get; set; }
        }


        public class ProductInfo
        {
            public string SubGroupName { get; set; }
            public string ProductGroupID { get; set; }
            public string GroupName { get; set; }
        }


        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnClear.Enabled = false;
                btnSubmit.Enabled = true;

                if (Request.QueryString["QuotationMasterID"] != null)
                {
                    string quotationMasterID = Request.QueryString["QuotationMasterID"];
                    lblQuotationMasterID.Text = quotationMasterID;
                    lblQuotationMasterID.Visible = false;
                    ViewState["QuotationDetails"] = new List<QuotationDetail>();
                    txtDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    Populate();
                    GetQuotationData();
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                    Response.Redirect("QuotationList.aspx");
                }

            }
        }

        private void GetQuotationData()
        {
            string QuotationNo = "";
            string Date = "";
            string CompanyName = "";
            string ContactPerson = "";
            string Address = "";
            string Dept = "";
            string UOMName = "";
            string FromTelephone = "";
            string FromEmail = "";
            string To = "";
            string ToTelephone = "";
            string ToEmail = "";
            string ProductName = "";
            string PaymentTerm = "";
            string CurrencyName = "";
            string Remarks = "";
            decimal Quantity = 0;
            decimal Price = 0;
            decimal TotalAmount = 0;
            decimal SubTotal = 0;
            decimal Discount = 0;
            decimal Totalbeforetax = 0;
            decimal VAT = 0;
            decimal Grandtotal = 0;
            string TestItems = "";
            string SampleDescription = "";


            int QuotationID = Convert.ToInt32(lblQuotationMasterID.Text);


            System.Data.DataTable dtQM = _cm.getquotationmaster(QuotationID);
            if (dtQM.Rows.Count > 0)
            {
                Date = dtQM.Rows[0]["QuotationDate"].ToString();
                QuotationNo = dtQM.Rows[0]["QuotationNumber"].ToString();
                CompanyName = dtQM.Rows[0]["CompanyName"].ToString();
                Address = dtQM.Rows[0]["AddressLine1"] + ", " +
                            dtQM.Rows[0]["AddressLine2"] + ", " +
                            dtQM.Rows[0]["AddressLine3"] + ", " +
                            dtQM.Rows[0]["District"] + " ," +
                            dtQM.Rows[0]["State"] + "," +
                            dtQM.Rows[0]["Pincode"] + " - " +
                            dtQM.Rows[0]["Country"].ToString();
                To = dtQM.Rows[0]["Title"] + " " + dtQM.Rows[0]["FirstName"] + "  " + dtQM.Rows[0]["LastName"].ToString();
                ToTelephone = dtQM.Rows[0]["Phone"].ToString();
                ToEmail = dtQM.Rows[0]["Email"].ToString();
                PaymentTerm = dtQM.Rows[0]["PaymentTermName"].ToString();
                //int paymentTermId = Convert.ToInt32(dtQM.Rows[0]["PaymentTermId"].ToString());
                CurrencyName = dtQM.Rows[0]["CurrencyName"].ToString();
                Remarks = dtQM.Rows[0]["Remarks"].ToString();

                lblRevisionCount.Text = dtQM.Rows[0]["Revisioncount"].ToString();
                lblRevisionCount.Visible = false;

                txtValidity.Text = dtQM.Rows[0]["QuotationValidityInDays"].ToString();
                txtDate.Text = dtQM.Rows[0]["QuotationDate"].ToString();

                Discount = Convert.ToDecimal(dtQM.Rows[0]["Discount"].ToString());
                VAT = Convert.ToDecimal(dtQM.Rows[0]["VATAmount"].ToString());
                Grandtotal = Convert.ToDecimal(dtQM.Rows[0]["GrandTotal"].ToString());

                SubTotal = ((Grandtotal - VAT) + (Discount));
                Totalbeforetax = SubTotal - Discount;
                Grandtotal = Totalbeforetax + VAT;

                lblTotalBeforeTax.Text = Totalbeforetax.ToString();
                lblGrandTotal.Text = Grandtotal.ToString();
                lblVATAmount.Text = VAT.ToString();

                txtDiscount.Text = Discount.ToString();
                ddlVAT.SelectedValue = dtQM.Rows[0]["VATId"].ToString();
                ViewState["ddlVATId"] = dtQM.Rows[0]["VATId"].ToString();

                //ddlPaymentTerms.SelectedItem.Text = PaymentTerm;
                ddlPaymentTerms.SelectedValue = dtQM.Rows[0]["PaymentTermId"].ToString();
                txtRemarks.Text = Remarks;

                string customerMasterId = dtQM.Rows[0]["CustomerMasterId"].ToString();
                ddlCustomer.DataSource = _cm.getCustomers();
                ddlCustomer.DataBind();
                ddlCustomer.SelectedValue = customerMasterId;
                ddlCustomer_SelectedIndexChanged(ddlCustomer, EventArgs.Empty);
            }


            System.Data.DataTable dtQD = _cm.getquotationdetailsNew(QuotationID);
            if (dtQD.Rows.Count > 0)
            {
                int QuotationDetailsId = Convert.ToInt32(dtQD.Rows[0]["QuotationDetailId"].ToString());
                TestItems = dtQD.Rows[0]["TestItems"].ToString();
                ProductName = dtQD.Rows[0]["TestItemDetails"].ToString();
                SampleDescription = dtQD.Rows[0]["SampleDescription"].ToString();
                Quantity = Convert.ToDecimal(dtQD.Rows[0]["Quantity"].ToString());
                Price = Convert.ToDecimal(dtQD.Rows[0]["Price"].ToString());
                int uomId = Convert.ToInt32(dtQD.Rows[0]["UOMId"].ToString());
                UOMName = dtQD.Rows[0]["UOMName"].ToString();

                gvQuotationDetails.DataSource = dtQD;
                gvQuotationDetails.DataBind();
            }
        }

        protected void gvQuotationDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the TotalAmount value for the current row
                decimal totalAmount = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "TotalAmount"));

                // Store the grand total in the ViewState or Session if required
                if (ViewState["GrandTotal"] == null)
                {
                    ViewState["GrandTotal"] = 0m;
                }

                decimal grandTotal = Convert.ToDecimal(ViewState["GrandTotal"]);
                grandTotal += totalAmount;
                ViewState["GrandTotal"] = grandTotal;

                lblSubTotal.Text = ViewState["GrandTotal"].ToString();

                CalculateGrandTotal(sender, e);
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                // Display the grand total in the footer row
                Label lblGrandTotal = (Label)e.Row.FindControl("lblGrandTotal");
                if (lblGrandTotal != null)
                {
                    lblGrandTotal.Text = Convert.ToDecimal(ViewState["GrandTotal"]).ToString("N2");
                }
            }
        }

        protected void gvQuotationDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvQuotationDetails.Rows[rowIndex];
                var quotationDetailsId = gvQuotationDetails.DataKeys[rowIndex]["QuotationDetailId"];
                DeleteQuotationDetail(quotationDetailsId);
                ClearProductFields();
                Response.Redirect(Request.Url.ToString(), true);
            }
        }


        private void Populate()
        {
            System.Data.DataTable dt = _cm.getCustomers();
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
                                        FROM LocationDetails 
                                        WHERE CustomerMasterId = @CustomerMasterId";
                    cmd.Parameters.AddWithValue("@CustomerMasterId", ddlCustomer.SelectedItem.Value);

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

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            int? productId = null;
            decimal vatPercentage = 0;
            if (ddlVAT.SelectedIndex > 0)
            {
                vatPercentage = Convert.ToDecimal(ddlVAT.SelectedValue);
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            decimal quantity = Convert.ToDecimal(txtQuantity.Text);
            decimal price = Convert.ToDecimal(txtPrice.Text);
            decimal baseAmount = quantity * price;
            decimal vatAmount = baseAmount * (vatPercentage / 100);
            decimal totalAmount = baseAmount;
            DateTime currentDateTime = DateTime.Now;

            using (SqlCommand productLookupCmd = new SqlCommand("SELECT SubGroupID FROM ProductSubGroup WHERE SubGroupName = @ProductName", _sCon))
            {
                productLookupCmd.Parameters.AddWithValue("@ProductName", txtTestItem.Text);
                _sCon.Open();
                object result = productLookupCmd.ExecuteScalar();
                _sCon.Close();
                if (result != null && result != DBNull.Value)
                {
                    productId = Convert.ToInt32(result);
                }
            }

            int revisioncountNew = Convert.ToInt32(lblRevisionCount.Text) + 1;

            string str = @"INSERT INTO QuotationDetails 
                           (QuotationMasterID, UserId, ProductId, UOMId, Quantity, Price, TotalAmount, IsDelete, CreatedDate, TestItems, TestItemDetails,SampleDescription,revisioncount) 
                           VALUES 
                           (@QuotationMasterID, @UserId, @ProductId, @UOMId, @Quantity, @Price, @TotalAmount, @IsDelete, @CreatedDate, @TestItems,@TestItemDetails,@SampleDescription, @revisioncount)";

            using (SqlCommand detailCmd = new SqlCommand(str, _sCon))
            {
                // Adding parameters to the command
                detailCmd.Parameters.AddWithValue("@QuotationMasterID", lblQuotationMasterID.Text); // Assuming this is a string
                detailCmd.Parameters.AddWithValue("@UserId", userId); // Assuming userId is a valid int or similar
                detailCmd.Parameters.AddWithValue("@ProductId", (object)productId ?? DBNull.Value); // Handle null ProductId
                detailCmd.Parameters.AddWithValue("@UOMId", "1"); // Assuming 1032 is a valid UOM ID
                detailCmd.Parameters.AddWithValue("@Quantity", quantity); // Assuming quantity is a decimal or float
                detailCmd.Parameters.AddWithValue("@Price", price); // Assuming price is a decimal or float
                detailCmd.Parameters.AddWithValue("@TotalAmount", totalAmount); // Assuming totalAmount is a decimal or float
                detailCmd.Parameters.AddWithValue("@IsDelete", false); // Assuming false is the correct value for IsDelete
                detailCmd.Parameters.AddWithValue("@CreatedDate", currentDateTime); // Assuming currentDateTime is a DateTime object
                detailCmd.Parameters.AddWithValue("@TestItems", txtTestItem.Text); // Handle null TestItems
                detailCmd.Parameters.AddWithValue("@TestItemDetails", txtTestItemsData.Text);
                detailCmd.Parameters.AddWithValue("@SampleDescription", txtSampleDescription.Text);
                detailCmd.Parameters.AddWithValue("@revisioncount", revisioncountNew); // Assuming revisioncount is 1 or any other valid value

                _sCon.Open();
                int rowsAffecteds = detailCmd.ExecuteNonQuery();
                _sCon.Close();

                if (rowsAffecteds > 0)
                {
                    //try
                    //{
                    //    btnSubmit_Click(sender, e);
                    //}
                    //catch (Exception ex)
                    //{
                    //    string msg = ex.Message;
                    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                    //}
                }

            }
            ClearProductFields();
            Response.Redirect(Request.Url.ToString(), true);
        }

        private void DeleteQuotationDetail(object quotationDetailId)
        {
            // Define your SQL DELETE query
            string query = "DELETE FROM QuotationDetails WHERE QuotationDetailId = @QuotationDetailId";

            // Create a connection and command object
            using (SqlConnection conn = new SqlConnection(_sCon.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add the parameter to the command
                    cmd.Parameters.AddWithValue("@QuotationDetailId", quotationDetailId);

                    try
                    {
                        // Open the connection
                        conn.Open();

                        // Execute the DELETE command
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    try
                    {
                        int quotationMasterId = Convert.ToInt32(lblQuotationMasterID.Text);

                        using (SqlCommand cmd = new SqlCommand(@"UPDATE QuotationMaster SET 
                                                                  QuotationValidityInDays=@QuotationValidityInDays, PaymentTermId=@PaymentTermId,
                                                                  Remarks=@Remarks, QuotationStatusId=@QuotationStatusId, Discount=@Discount,
                                                                  GrandTotal=@GrandTotal, VATId=@VATId, VatAmount=@VatAmount, UpdatedDate=@UpdatedDate,
                                                                  UserId = @UserId, Revisioncount = @revisioncount
                                                                  WHERE QuotationMasterId = @QuotationMasterId", conn))
                        {
                            int userId = Convert.ToInt32(Session["UserId"]);
                            //int vatIdValue = ddlVAT.SelectedValue == "0" ? 0 : 1;
                            string vatIdValue = ddlVAT.SelectedItem.Value;

                            if (vatIdValue == "--Select VAT--")
                            {
                                vatIdValue = "0";
                            }
                            else
                            {
                                vatIdValue = "1";
                            }

                            decimal vatAmount = 0;
                            decimal.TryParse(lblVATAmount.Text, out vatAmount);
                            int paymentTermsId = Convert.ToInt32(ddlPaymentTerms.SelectedValue);
                            int revisiondata = 0;

                            if (lblRevisionCount.Text == "")
                            {
                                revisiondata = 0 + 1;
                            }
                            else
                            {
                                revisiondata = Convert.ToInt32(lblRevisionCount.Text) + 1;
                            }

                            cmd.Parameters.AddWithValue("@QuotationMasterId", quotationMasterId);
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@QuotationValidityInDays", Convert.ToInt32(txtValidity.Text));
                            cmd.Parameters.AddWithValue("@PaymentTermId", paymentTermsId);
                            cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);
                            cmd.Parameters.AddWithValue("@QuotationStatusId", 7);
                            cmd.Parameters.AddWithValue("@UpdatedDate", date + " " + time);
                            cmd.Parameters.AddWithValue("@Discount", Convert.ToDecimal(txtDiscount.Text));
                            cmd.Parameters.AddWithValue("@GrandTotal", Convert.ToDecimal(lblGrandTotal.Text.Replace(",", "")));
                            cmd.Parameters.AddWithValue("@VATId", vatIdValue);
                            cmd.Parameters.AddWithValue("@VatAmount", vatAmount);
                            cmd.Parameters.AddWithValue("@revisioncount", revisiondata);

                            conn.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();
                            conn.Close();

                            if (rowsAffected > 0)
                            {
                                //ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccess", "alert('Quotation and details saved successfully!');", true);
                                //string url = $"GenerateQuotation.aspx?ID={quotationMasterId}&r={revisiondata}";
                                //Response.Redirect(url);

                                string url = $"GenerateQuotationCalib.aspx?ID={quotationMasterId}&r={revisiondata}";
                                string redirectUrl = "QuotationList.aspx";
                                string script = $@"alert('Quotation and details saved successfully');
                                                   window.open('{url}', '_blank');
                                                   window.location.href = '{redirectUrl}';
                                                  ";
                                ScriptManager.RegisterStartupScript(this, GetType(), "OpenTabAndRedirect", script, true);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "ShowError", "alert('No data was updated. Please check the input values.');", true);
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }

                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowError", $"alert('Error saving quotation: {ex.Message}');", true);
            }
        }

        //  GenerateQuotationNumber //
        private string GenerateQuotationNumber()
        {

            string prefix = "QQ";
            string dateString = DateTime.Now.ToString("yyyyMM");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ISNULL(MAX(CAST(RIGHT(QuotationNumber, 4) AS INT)), 0) + 1 " +
                    "FROM QuotationMaster " +
                    "WHERE QuotationNumber LIKE @Prefix + @DateString + '-%'", conn))
                {
                    cmd.Parameters.AddWithValue("@Prefix", prefix);
                    cmd.Parameters.AddWithValue("@DateString", dateString);

                    int sequence = Convert.ToInt32(cmd.ExecuteScalar());
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
                ViewState["hdnVATId"] = hdnVATId.Value;
            }
            else
            {
                hdnVATId.Value = ddlVAT.SelectedValue;
            }
            CalculateGrandTotal(sender, e);
        }

        protected void CalculateGrandTotal(object sender, EventArgs e)
        {
            try
            {
                decimal subTotal = Convert.ToDecimal(ViewState["GrandTotal"].ToString());
                lblSubTotal.Text = subTotal.ToString("N2");
                decimal discount = 0;
                if (!string.IsNullOrEmpty(txtDiscount.Text))
                {
                    decimal.TryParse(txtDiscount.Text.Trim(), out discount);
                }
                decimal totalBeforeTax = subTotal - discount;
                lblTotalBeforeTax.Text = totalBeforeTax.ToString("N2");
                string ddlvatt = ddlVAT.SelectedItem.Value;

                if (ddlvatt == "--Select VAT--")
                {
                    ddlvatt = "0";
                }
                else
                {
                    ddlvatt = "1";
                }
                decimal vatAmount = 0;

                if (!string.IsNullOrEmpty(ddlvatt))
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT Percentage FROM VAT WHERE VATId = @VATId", conn))
                        {
                            cmd.Parameters.AddWithValue("@VATId", ddlvatt);
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
                else
                {

                }
                lblVATAmount.Text = vatAmount.ToString("N2");
                decimal grandTotal = totalBeforeTax + vatAmount;
                lblGrandTotal.Text = grandTotal.ToString("N2");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", $"alert('Error calculating totals: {ex.Message}');", true);
            }
        }



    }
}