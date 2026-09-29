using LIMS.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class QuoteAnalysis : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DataTable dtTotal = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    if (Request.QueryString["ID"] != null)
                    {
                        InitializeDataTable();
                        string EnquiryID = Request.QueryString["ID"].ToString();
                        Session["EnquiryID"] = EnquiryID;
                        LoadQuotesWithCurrencyCheck(EnquiryID);
                    }
                    else
                    {
                        InitializeDataTable();
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        private void InitializeDataTable()
        {
            dtTotal.Columns.Clear();
            dtTotal.Columns.Add(new DataColumn("VendorID", typeof(int)));
            dtTotal.Columns.Add(new DataColumn("GrandTotal", typeof(decimal)));
            dtTotal.Columns.Add(new DataColumn("DeliveryPeriod", typeof(string)));
        }
            
        //private void LoadQuotesWithCurrencyCheck(string EnquiryID)
        //{
        //    DataTable dtAllQuotes = _cm.getQuoteVendors(EnquiryID);

        //    if (dtAllQuotes.Rows.Count == 0)
        //    {
        //        // No quotes found
        //        rptCategory.DataSource = null;
        //        rptCategory.DataBind();
        //        return;
        //    }

        //    // Get distinct currencies
        //    var distinctCurrencies = dtAllQuotes.AsEnumerable()
        //        .Select(row => new
        //        {
        //            CurrencyId = Convert.ToInt32(row["QuoteCurrencyId"]),
        //            CurrencyName = GetCurrencyName(Convert.ToInt32(row["QuoteCurrencyId"]))
        //        })
        //        .Distinct()
        //        .ToList();

        //    if (distinctCurrencies.Count > 1)
        //    {
        //        // Multiple currencies - show selection
        //        ShowCurrencySelection(distinctCurrencies);

        //        // Load first currency by default
        //        int defaultCurrency = distinctCurrencies.First().CurrencyId;
        //        Session["SelectedCurrency"] = defaultCurrency;
        //        LoadQuotesByCurrency(EnquiryID, defaultCurrency);
        //    }
        //    else
        //    {
        //        // Single currency - load directly
        //        Session["SelectedCurrency"] = distinctCurrencies.First().CurrencyId;
        //        HideCurrencySelection();
        //        LoadAllQuotes(dtAllQuotes, EnquiryID);
        //    }
        //}
        private void LoadQuotesWithCurrencyCheck(string EnquiryID)
        {
            DataTable dtAllQuotes = _cm.getQuoteVendors(EnquiryID);

            if (dtAllQuotes.Rows.Count == 0)
            {
                // No quotes found
                rptCategory.DataSource = null;
                rptCategory.DataBind();
                return;
            }

            // Get distinct currencies
            var distinctCurrencies = dtAllQuotes.AsEnumerable()
                .Where(row => row["QuoteCurrencyId"] != DBNull.Value &&
                              !string.IsNullOrWhiteSpace(row["QuoteCurrencyId"].ToString()))
                .Select(row => new
                {
                    CurrencyId = Convert.ToInt32(row["QuoteCurrencyId"]),
                    CurrencyName = GetCurrencyName(Convert.ToInt32(row["QuoteCurrencyId"]))
                })
                .Distinct()
                .ToList();

            // Handle case where all QuoteCurrencyId are null
            if (distinctCurrencies.Count == 0)
            {
                // Option 1: Treat as INR by default
                Session["SelectedCurrency"] = 1; // Default INR
                HideCurrencySelection();
                LoadAllQuotes(dtAllQuotes, EnquiryID);
                return;

                // Option 2 (alternative): skip loading
                // rptCategory.DataSource = null;
                // rptCategory.DataBind();
                // return;
            }

            if (distinctCurrencies.Count > 1)
            {
                // Multiple currencies - show selection panel
                ShowCurrencySelection(distinctCurrencies);

                // Load first currency by default
                int defaultCurrency = distinctCurrencies.First().CurrencyId;
                Session["SelectedCurrency"] = defaultCurrency;
                LoadQuotesByCurrency(EnquiryID, defaultCurrency);
            }
            else
            {
                // Single currency - load directly
                Session["SelectedCurrency"] = distinctCurrencies.First().CurrencyId;
                HideCurrencySelection();
                LoadAllQuotes(dtAllQuotes, EnquiryID);
            }
        }

        private string GetCurrencyName(int currencyId)
        {
            try
            {
                DataTable dt = _cm.GetCurrencyDetails(currencyId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string currencyCode = row["CurrencyCode"].ToString();
                    string currencyName = row["CurrencyName"].ToString();

                    return $"{currencyCode} ({currencyName})";
                }

                return $"Currency {currencyId}";
            }
            catch (Exception ex)
            {
                // Log the error
                return $"Error retrieving currency: {currencyId}";
            }
        }
        private void ShowCurrencySelection(dynamic currencies)
        {
            pnlCurrencySelection.Visible = true;
            rblCurrency.Items.Clear();

            foreach (var currency in currencies)
            {
                ListItem item = new ListItem(currency.CurrencyName, currency.CurrencyId.ToString());
                rblCurrency.Items.Add(item);
            }

            if (rblCurrency.Items.Count > 0)
            {
                rblCurrency.SelectedIndex = 0;
            }
        }
        private void HideCurrencySelection()
        {
            pnlCurrencySelection.Visible = false;
        }
        protected void rblCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            string EnquiryID = Session["EnquiryID"].ToString();
            int selectedCurrency = Convert.ToInt32(rblCurrency.SelectedValue);
            Session["SelectedCurrency"] = selectedCurrency;

            LoadQuotesByCurrency(EnquiryID, selectedCurrency);
        }
        private void LoadQuotesByCurrency(string EnquiryID, int currencyId)
        {
            DataTable dtAllQuotes = _cm.getQuoteVendors(EnquiryID);

            // Filter quotes by selected currency
            DataTable dtFiltered = dtAllQuotes.Clone(); // Copy structure

            foreach (DataRow row in dtAllQuotes.Rows)
            {
                if (Convert.ToInt32(row["QuoteCurrencyId"]) == currencyId)
                {
                    dtFiltered.ImportRow(row);
                }
            }

            LoadAllQuotes(dtFiltered, EnquiryID);
        }
        private void LoadAllQuotes(DataTable dt, string EnquiryID)
        {
            rptCategory.DataSource = dt;
            rptCategory.DataBind();

            // Calculate rankings after binding
            CalculateRankings(EnquiryID);

            // Set button visibility
            SetButtonVisibility(EnquiryID);
        }
        private void SetButtonVisibility(string EnquiryID)
        {
            int selectedCurrency = Session["SelectedCurrency"] != null ?
                Convert.ToInt32(Session["SelectedCurrency"]) : 1;

            DataTable dtAllStatuses = _cm.GetVQ(EnquiryID,"");

            bool hasAcceptedQuote = false;
            bool allQuotesReceived = true;

            // Check the overall status across all vendors for this enquiry
            if (dtAllStatuses.Rows.Count > 0)
            {
                foreach (DataRow row in dtAllStatuses.Rows)
                {
                    string statusId = row["StatusID"].ToString();

                    if (statusId == "3")// Quotation accepted
                    {
                        hasAcceptedQuote = true;
                    }
                    if(statusId != "2" && statusId != "7") // Not "Quote Received"
                    {
                        allQuotesReceived = false;
                    }
                }
            }

            // Process each repeater item
            foreach (RepeaterItem item in rptCategory.Items)
            {
                Button btnAccept = (Button)item.FindControl("btnAccept");
                Button btnGeneratePO = (Button)item.FindControl("btnGeneratePO");
                Label lblVendorID = (Label)item.FindControl("lblID");
                Label lblContactPerson = (Label)item.FindControl("lblContactPerson");
                Label lblCurrencyId = (Label)item.FindControl("lblCurrencyId");
                HyperLink hypPO = (HyperLink)item.FindControl("hypPO");

                if (lblVendorID != null && lblCurrencyId != null && btnAccept != null)
                {
                    int itemCurrency = int.TryParse(lblCurrencyId?.Text, out int parsedId) ? parsedId : 0;

                    // Only process items for the selected currency
                    if (itemCurrency == selectedCurrency)
                    {
                        // Get contact person info
                        DataTable dtVC = _cm.GetContactPersonVendor(lblVendorID.Text);
                        if (dtVC.Rows.Count > 0)
                        {
                            lblContactPerson.Text = "(" + dtVC.Rows[0]["ContactPerson"].ToString() +
                                                  ", Mo. :" + dtVC.Rows[0]["ContactMobile"].ToString() + ")";
                        }

                        // Get status info for this specific vendor
                        DataTable dtEn = _cm.GetVQ(EnquiryID,lblVendorID.Text.ToString());
                        if (dtEn.Rows.Count > 0)
                        {
                            string StatusID = dtEn.Rows[0]["StatusID"].ToString();

                            // Reset button states
                            btnAccept.Visible = false;
                            btnGeneratePO.Visible = false;
                            hypPO.Visible = false;
                            btnAccept.CssClass = "btn btn-accept me-2";

                            switch (StatusID)
                            {
                                case "1": // Pending
                                    break;

                                case "2":
                                case "7":
                                    btnAccept.Visible = true;
                                    btnAccept.Enabled = true;
                                    //if (!hasAcceptedQuote && allQuotesReceived)
                                    //{
                                    //    btnAccept.Visible = true;
                                    //    btnAccept.Enabled = true;
                                    //}
                                    //else if (hasAcceptedQuote)
                                    //{
    
                                    //    btnAccept.Visible = false;
                          
                                    //}
                                    //else
                                    //{
                                    //    btnAccept.Visible = true;
                                    //    btnAccept.Enabled = false;
                                    //    btnAccept.ToolTip = "All quotes must be received before acceptance";
                                    //    btnAccept.CssClass += " disabled";
                                    //}
                                    break;

                                case "3": // Accepted
                                    btnGeneratePO.Visible = true;
                                    break;

                                case "9": // PO Generated
                                    hypPO.Visible = true;
                                    break;

                                default:
                                    if (!hasAcceptedQuote && allQuotesReceived)
                                    {
                                        btnAccept.Visible = true;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            btnAccept.Visible = false;
                            btnGeneratePO.Visible = false;
                        }
                    }
                    else
                    {
                        // Hide buttons for non-selected currencies
                        btnAccept.Visible = false;
                        btnGeneratePO.Visible = false;
                        hypPO.Visible = false;
                    }
                }
            }
        }
        protected void OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            string EnquiryID = Session["EnquiryID"].ToString();

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                decimal SubTotal = 0;
                decimal AdditionalCharges = 0;
                decimal VATAmount = 0;
                decimal VAT = 0;
                decimal GrandTotal = 0;

                Label lblID = (Label)e.Item.FindControl("lblID");
                Label lblCurrencyId = (Label)e.Item.FindControl("lblCurrencyId");
                Label lblCurrencyDisplay = (Label)e.Item.FindControl("lblCurrencyDisplay");
                Button btnAccept = (Button)e.Item.FindControl("btnAccept");
                Button btnGeneratePO = (Button)e.Item.FindControl("btnGeneratePO");
                Label lblSubTotal = (Label)e.Item.FindControl("lblSubTotal");
                Label lblAdditionalChargeDescription = (Label)e.Item.FindControl("lblAdditionalChargeDescription");
                Label lblAdditionalCharges = (Label)e.Item.FindControl("lblAdditionalCharges");
                Label lblVATAmount = (Label)e.Item.FindControl("lblVATAmount");
                Label lblGrandTotal = (Label)e.Item.FindControl("lblGrandTotal");
                Label lblPayment = (Label)e.Item.FindControl("lblPayment");
                Label lblRemarks = (Label)e.Item.FindControl("lblRemarks");
                HyperLink hypPO = (HyperLink)e.Item.FindControl("hypPO");
                Repeater rptGallery = (Repeater)e.Item.FindControl("rptGallery");

                if (lblID != null)
                {
                    string vendorId = lblID.Text;

                    int currencyId = int.TryParse(lblCurrencyId?.Text, out int parsedId) ? parsedId : 0;


                    // Display currency symbol
                    if (lblCurrencyDisplay != null)
                    {
                        lblCurrencyDisplay.Text = _cm.GetCurrencySymbol(currencyId);
                    }

                    // Bind quote details
                    DataTable dtQuoteDetails = GetQuoteDetails(Convert.ToInt32(vendorId), EnquiryID);
                    rptGallery.DataSource = dtQuoteDetails;
                    rptGallery.DataBind();

                    if (dtQuoteDetails.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in dtQuoteDetails.Rows)
                        {
                            if (dr1["TotalAfterDiscount"] != DBNull.Value && dr1["TotalAfterDiscount"].ToString() != "")
                            {
                                SubTotal += Convert.ToDecimal(dr1["TotalAfterDiscount"].ToString());
                            }
                            if (dr1["AdditionalCharges"] != DBNull.Value && dr1["AdditionalCharges"].ToString() != "")
                            {
                                AdditionalCharges = Convert.ToDecimal(dr1["AdditionalCharges"].ToString());
                            }

                            // Only calculate VAT if currencyId == 1(IDR)
                            if (currencyId == 1)
                            {
                                if (dr1["VAT"] != DBNull.Value && dr1["VAT"].ToString() != "" && VAT == 0)
                                {
                                    string vatString = dr1["VAT"].ToString().Replace("%", "").Trim();
                                    if (decimal.TryParse(vatString, out decimal parsedVAT))
                                    {
                                        VAT = parsedVAT;
                                    }
                                }
                            }
                        }

                        // VAT & GrandTotal only if INR (currencyId=1)
                        if (currencyId == 1 && VAT > 0)
                        {
                            VATAmount = ((SubTotal+AdditionalCharges) * VAT) / 100;
                            GrandTotal = SubTotal + AdditionalCharges + VATAmount;
                        }
                        else
                        {
                            VATAmount = 0;
                            GrandTotal = SubTotal + AdditionalCharges;
                        }

                        // Set label values with currency formatting
                        int selectedCurrency = Session["SelectedCurrency"] != null ?
                            Convert.ToInt32(Session["SelectedCurrency"]) : 1;
                        string currencySymbol = _cm.GetCurrencySymbol(selectedCurrency);

                        lblSubTotal.Text = _cm.FormatCurrency(SubTotal, currencyId);
                        lblAdditionalChargeDescription.Text= dtQuoteDetails.Rows[0]["AdditionalChargeDescription"].ToString();
                        lblAdditionalCharges.Text = _cm.FormatCurrency(AdditionalCharges, currencyId);
                        lblVATAmount.Text = (currencyId == 1) ? _cm.FormatCurrency(VATAmount, currencyId) : "-";
                        lblGrandTotal.Text = _cm.FormatCurrency(GrandTotal, currencyId);

                        lblPayment.Text = dtQuoteDetails.Rows[0]["PaymentTerms"].ToString();
                        lblRemarks.Text = dtQuoteDetails.Rows[0]["Remarks"].ToString();

                        DataRow dr = dtTotal.NewRow();
                        dtTotal.Rows.Add(dr);
                    }

                    // Set button states based on status
                    DataTable dtStatus = _cm.GetVQDetails(EnquiryID, vendorId);
                    if (dtStatus.Rows.Count > 0)
                    {
                        string StatusID = dtStatus.Rows[0]["StatusID"].ToString();

                        switch (StatusID)
                        {
                            case "1":
                                btnAccept.Visible = false;
                                btnGeneratePO.Visible = false;
                                break;
                            case "2":
                            case "7":
                                btnAccept.Visible = true;
                                btnGeneratePO.Visible = false;
                                break;
                            case "3":
                                btnAccept.Visible = false;
                                btnGeneratePO.Visible = true;
                                break;
                            case "9":
                                btnAccept.Visible = false;
                                btnGeneratePO.Visible = false;
                                hypPO.Visible = true;
                                break;
                            default:
                                btnAccept.Visible = true;
                                btnGeneratePO.Visible = false;
                                break;
                        }
                    }
                }
            }
        }
       
        private void CalculateRankings(string EnquiryID)
        {
            try
            {
                // Create ranking table with currency information
                DataTable dtRanking = new DataTable();
                dtRanking.Columns.Add("VendorID", typeof(int));
                dtRanking.Columns.Add("GrandTotal", typeof(decimal));
                dtRanking.Columns.Add("CurrencyID", typeof(int));

                // Collect data from UI controls
                foreach (RepeaterItem vendorItem in rptCategory.Items)
                {
                    if (vendorItem.ItemType == ListItemType.Item || vendorItem.ItemType == ListItemType.AlternatingItem)
                    {
                        Label lblVendorID = (Label)vendorItem.FindControl("lblID");
                        Label lblCurrencyId = (Label)vendorItem.FindControl("lblCurrencyId");
                        Label lblGrandTotal = (Label)vendorItem.FindControl("lblGrandTotal");

                        if (lblVendorID != null && lblCurrencyId != null && lblGrandTotal != null)
                        {
                            int vendorId = Convert.ToInt32(lblVendorID.Text);
                            int currencyId = Convert.ToInt32(lblCurrencyId.Text);

                            // Extract numeric value from formatted currency
                            decimal grandTotal = _cm.ExtractNumericValueFromCurrency(lblGrandTotal.Text, currencyId);

                            DataRow dr = dtRanking.NewRow();
                            dr["VendorID"] = vendorId;
                            dr["GrandTotal"] = grandTotal;
                            dr["CurrencyID"] = currencyId;
                            dtRanking.Rows.Add(dr);
                        }
                    }
                }

                if (dtRanking.Rows.Count == 0)
                {
                    return; // No data to rank
                }

                // Check for zero total items
                bool hasZeroTotalItems = dtRanking.AsEnumerable().Any(row => Convert.ToDecimal(row["GrandTotal"]) == 0);

                if (hasZeroTotalItems)
                {
                    foreach (RepeaterItem vendorItem in rptCategory.Items)
                    {
                        if (vendorItem.ItemType == ListItemType.Item || vendorItem.ItemType == ListItemType.AlternatingItem)
                        {
                            Label lblRank = (Label)vendorItem.FindControl("lblRank");
                            if (lblRank != null)
                            {
                                lblRank.Text = "Ranking not possible (zero totals)";
                                lblRank.CssClass = "rank-not-available";
                            }
                        }
                    }
                    return;
                }

                // Get the selected currency for filtering
                int selectedCurrency = Session["SelectedCurrency"] != null ?
                    Convert.ToInt32(Session["SelectedCurrency"]) : 1;

                // Filter by selected currency and sort by grand total ascending (lowest first)
                var rankedVendors = dtRanking.AsEnumerable()
                    .Where(row => Convert.ToInt32(row["CurrencyID"]) == selectedCurrency)
                    .OrderBy(row => Convert.ToDecimal(row["GrandTotal"]))
                    .ToList();

                if (rankedVendors.Count == 0)
                {
                    // No vendors for selected currency
                    foreach (RepeaterItem vendorItem in rptCategory.Items)
                    {
                        if (vendorItem.ItemType == ListItemType.Item || vendorItem.ItemType == ListItemType.AlternatingItem)
                        {
                            Label lblRank = (Label)vendorItem.FindControl("lblRank");
                            if (lblRank != null)
                            {
                                lblRank.Text = "No ranking data";
                                lblRank.CssClass = "rank-not-available";
                            }
                        }
                    }
                    return;
                }

                // Assign ranks within the same currency
                int currentRank = 1;
                for (int i = 0; i < rankedVendors.Count; i++)
                {
                    if (i > 0)
                    {
                        decimal prevTotal = Convert.ToDecimal(rankedVendors[i - 1]["GrandTotal"]);
                        decimal currentTotal = Convert.ToDecimal(rankedVendors[i]["GrandTotal"]);

                        if (currentTotal != prevTotal)
                        {
                            currentRank = i + 1;
                        }
                    }

                    int vendorId = Convert.ToInt32(rankedVendors[i]["VendorID"]);

                    // Update rank in UI
                    foreach (RepeaterItem vendorItem in rptCategory.Items)
                    {
                        if (vendorItem.ItemType == ListItemType.Item || vendorItem.ItemType == ListItemType.AlternatingItem)
                        {
                            Label lblVendorID = (Label)vendorItem.FindControl("lblID");
                            Label lblCurrencyId = (Label)vendorItem.FindControl("lblCurrencyId");

                            if (lblVendorID != null && lblCurrencyId != null &&
                                Convert.ToInt32(lblVendorID.Text) == vendorId &&
                                Convert.ToInt32(lblCurrencyId.Text) == selectedCurrency)
                            {
                                Label lblRank = (Label)vendorItem.FindControl("lblRank");
                                if (lblRank != null)
                                {
                                    lblRank.Text = currentRank.ToString();
                                    lblRank.CssClass = (currentRank == 1) ? "rank-first" : "";
                                }
                                break;
                            }
                        }
                    }
                }

                // Clear ranks for vendors with different currencies (not currently displayed)
                foreach (RepeaterItem vendorItem in rptCategory.Items)
                {
                    if (vendorItem.ItemType == ListItemType.Item || vendorItem.ItemType == ListItemType.AlternatingItem)
                    {
                        Label lblCurrencyId = (Label)vendorItem.FindControl("lblCurrencyId");
                        Label lblRank = (Label)vendorItem.FindControl("lblRank");

                        if (lblCurrencyId != null && lblRank != null &&
                            Convert.ToInt32(lblCurrencyId.Text) != selectedCurrency)
                        {
                            lblRank.Text = "";
                            lblRank.CssClass = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error in CalculateRankings: {ex.Message}");

                foreach (RepeaterItem vendorItem in rptCategory.Items)
                {
                    if (vendorItem.ItemType == ListItemType.Item || vendorItem.ItemType == ListItemType.AlternatingItem)
                    {
                        Label lblRank = (Label)vendorItem.FindControl("lblRank");
                        if (lblRank != null)
                        {
                            lblRank.Text = "Error calculating rank";
                            lblRank.CssClass = "rank-error";
                        }
                    }
                }
            }
        }
       
        private DataTable GetQuoteDetails(int VendorID, string EnquiryID)
        {
            return _cm.GetVendorQuoteDetails(VendorID, EnquiryID);
        }
        protected void btnAccept_Click(object sender, EventArgs e)
        {
            Button button = (sender as Button);
            RepeaterItem item = button.NamingContainer as RepeaterItem;
            int index = item.ItemIndex;

            string EnquiryID = Session["EnquiryID"].ToString();
            Label lblID = (Label)rptCategory.Items[index].FindControl("lblID");
            Label lblRank = (Label)rptCategory.Items[index].FindControl("lblRank");
            TextBox txtReason = (TextBox)rptCategory.Items[index].FindControl("txtReason");

            if (lblRank.Text != "Ranking not possible (zero totals)" && lblRank.Text != "Error calculating rank")
            {
                if (lblRank.Text != "1")
                {
                    if (string.IsNullOrEmpty(txtReason.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('Enter reason for Not Choosing #1 ranked quote!');", true);
                        return;
                    }
                }
            }

            _cm.QuotationStatusID = 3;
            _cm.VendorID = Convert.ToInt32(lblID.Text);
            _cm.EnquiryID = Convert.ToInt32(EnquiryID);
            _cm.Reason = txtReason.Text;
            _cm.UpdateVQStatus(_cm);

            // Reload quotes
            LoadQuotesWithCurrencyCheck(EnquiryID);
        }
        protected void btnGeneratePO_Click(object sender, EventArgs e)
        {
            Button button = (sender as Button);
            RepeaterItem item = button.NamingContainer as RepeaterItem;
            int index = item.ItemIndex;

            string EnquiryID = Session["EnquiryID"].ToString();
            Label lblID = (Label)rptCategory.Items[index].FindControl("lblID");

            Session["EnquiryID"] = EnquiryID;
            Session["VendorID"] = lblID.Text;
            Response.Redirect("PurchaseOrder.aspx?e=" + EnquiryID + "&v=" + lblID.Text);
        }
        public DataTable RemoveDuplicateRows(DataTable table, string DistinctColumn)
        {
            try
            {
                System.Collections.ArrayList UniqueRecords = new System.Collections.ArrayList();
                System.Collections.ArrayList DuplicateRecords = new System.Collections.ArrayList();

                foreach (DataRow dRow in table.Rows)
                {
                    if (UniqueRecords.Contains(dRow[DistinctColumn]))
                        DuplicateRecords.Add(dRow);
                    else
                        UniqueRecords.Add(dRow[DistinctColumn]);
                }

                foreach (DataRow dRow in DuplicateRecords)
                {
                    table.Rows.Remove(dRow);
                }

                return table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}