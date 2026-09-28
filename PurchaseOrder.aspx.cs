using AjaxControlToolkit;
using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static LIMS.DAL.GLModule;

namespace LIMS
{
    public partial class PurchaseOrder1 : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        string StoreType = "Engineering";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Populate();
                if (Request.QueryString["e"] != null & Request.QueryString["v"] != null)
                {
                    Session["EnquiryID"] = Request.QueryString["e"].ToString();
                    Session["VendorID"] = Request.QueryString["v"].ToString();                    
                }

                if (Session["UserID"] != null)
                {
                    if (Session["EnquiryID"] != null)
                    {
                        string EnquiryID = Session["EnquiryID"].ToString();
                        string VendorID = Session["VendorID"].ToString();
                        LoadVAT(VendorID);
                        GetPODetails(EnquiryID);
                        DataTable dt = _cm.GetUnitFromVendorQuotation(EnquiryID, VendorID);
                        if (dt.Rows.Count > 0)
                        {
                            if (Request.QueryString["ID"] == null)
                            {
                                DataTable dtDiv = _cm.GetPRDepartment(EnquiryID);
                                if(dtDiv.Rows.Count > 0) {
                                    CreatePONumber(StoreType, dtDiv.Rows[0]["DeptCode"].ToString());
                                }
                                
                            }
                            else
                            {
                                if (Session["PurchaseOrderID"] != null)
                                {
                                    hdPurchaseOrderID.Value = Session["PurchaseOrderID"].ToString();
                                    BindPODetails();
                                }
                            }
                        }
                        DateTime Today = DateTime.Now.AddHours(1.5);
                        txtDate.Text = Today.ToString("dd/MMM/yyyy");
                    }
                    else
                    {
                        Response.Redirect("QuoteAnalysis.aspx");
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        private void Populate()
        {
            DataTable dt = _cm.getPurchasePaymentTerms();//_cm.getPaymentTerms(); 4Spe26 Changed based on Christiancus request
            if (dt.Rows.Count > 0)
            {
                ddlPaymentTerms.DataSource = dt;
                ddlPaymentTerms.DataTextField = "PaymentTermName";
                ddlPaymentTerms.DataValueField = "PaymentTermID";
                ddlPaymentTerms.DataBind();
                ddlPaymentTerms.Items.Insert(0, "--Select Payment Terms--");
            }
        }
        private void BindPODetails()
        {
            DataTable dt = new DataTable();
            string PurchaseOrderID = hdPurchaseOrderID.Value;
            dt = _cm.GetPOReportFileName(PurchaseOrderID);
            if (dt.Rows.Count > 0)
            {
                txtPONumber.Text = dt.Rows[0]["PONumber"].ToString();
                txtSpecialInstruction.Text = dt.Rows[0]["SpecialInstructions"].ToString();
                txtDelivery.Text = dt.Rows[0]["Delivery"].ToString();

                ListItem item = ddlPaymentTerms.Items.FindByText(dt.Rows[0]["Payment"].ToString());
                if (item != null)
                {
                    ddlPaymentTerms.ClearSelection();
                    item.Selected = true;
                }
                else
                {
                    ddlPaymentTerms.SelectedIndex = 0; // default item
                }
                txtGuarantee.Text = dt.Rows[0]["Guarantee"].ToString();
                txtRemarks.Text = dt.Rows[0]["Remarks"].ToString();


                btnUpdate.Visible = true;
                BtnSave.Visible = false;
            }
        }
        private void LoadVAT(string VendorID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "Select * from VAT";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlVAT.DataSource = dt;
                        ddlVAT.DataTextField = "VAT";
                        ddlVAT.DataValueField = "Percentage";
                        ddlVAT.DataBind();
                        ddlVAT.Items.Insert(0, new ListItem("--Select VAT--", ""));
                        DataTable dtVen = _cm.GetVendorVAT(VendorID);
                        if (dtVen.Rows.Count > 0)
                        {
                            try
                            {
                                ddlVAT.ClearSelection();
                                ddlVAT.Items.FindByValue(dtVen.Rows[0]["Percentage"].ToString()).Selected = true;
                                ddlVAT.Enabled = false;
                            }
                            catch (Exception ex) { }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", $"alert('Error loading VAT: {ex.Message.Replace("'", "\\'")}');", true);
                    }
                }
            }
        }
        private void CreatePONumber(string StoreType,string Division)
        {
            string POID = "";
            string PP = "";
            
            DataTable dtPO = _cm.GetPurchaseOrder();
            if (dtPO != null)
            {
                string s= dtPO.Rows[0]["PONumber"].ToString().Substring(0, 4);
                POID = (Convert.ToInt32(s) + 1).ToString("D4");
            }
            _cm.POYear = DateTime.Now.ToString("yyyy");
            DataTable dt = _cm.GetPODetails1(StoreType, _cm.POYear);
            if (dt.Rows.Count > 0)
            {
                PP = (Convert.ToInt32(dt.Rows.Count) + 1).ToString("D4");
            }
            else
            {
                PP = "0001";
            }

            // PO(xxxx)-PP(xxxx)/MM/yyyy/Division
            //string PO = POID + "-" + PP + "/" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("yyyy") + "/" + Division;
            // PP(xxxx)-MM-yyyy/PQI-PO-Division Ex. 0001-01-2026/PQI-PO-Division Chenge request from Elan - 01 April 2026 14:45
            string PO = PP + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy") + "/PQI-PO-" + Division;
            
            txtPONumber.Text = PO.ToString();

        }
        private void GetPODetails(string EnquiryID)
       {
            string VendorID = Session["VendorID"].ToString();
            DataTable dtVendorDetails = _cm.GetVendor(VendorID);
            DataTable dt = _cm.GetDetailsForPO(EnquiryID, VendorID);
            DataTable dt1 = _cm.GetContactForPO(EnquiryID, VendorID);
            int rowID = 0;
            if (dt1.Rows.Count > 0)
            {
                txtVendor.Text = dtVendorDetails.Rows[0]["VendorName"].ToString();
                txtContactPerson.Text = dt1.Rows[0]["ContactPerson"].ToString();
                lblContactPerson.Text = dt1.Rows[0]["VendorContactPersonID"].ToString();
                txtEmail.Text = dt1.Rows[0]["ContactEmail"].ToString();
                txtcurrency.Text = dt1.Rows[0]["CurrencyCode"].ToString();
                txtQuoteNo.Text = dt1.Rows[0]["ReportFileName"].ToString();
            }
            bool showVAT = (txtcurrency.Text.ToUpper() == "IDR");
            pnlVATControls.Visible = showVAT;
            if (dt.Rows.Count > 0)
            {

                txtDelivery.Text = dt.Rows[0]["Delivery"].ToString();
                ListItem item = ddlPaymentTerms.Items.FindByText(dt.Rows[0]["Payment"].ToString());

                if (item != null)
                {
                    ddlPaymentTerms.ClearSelection();
                    item.Selected = true;
                }
                else
                {
                    ddlPaymentTerms.SelectedIndex = 0; // default item
                }
                txtGuarantee.Text = dt.Rows[0]["Guarantee"].ToString();
                txtRemarks.Text = dt.Rows[0]["Remarks"].ToString();

                txtDelivery.ReadOnly = false;
                txtGuarantee.ReadOnly = false;
                txtRemarks.ReadOnly = false;

                gvPO.DataSource = dt;
                gvPO.DataBind();
                gvPO.FooterRow.Visible = true;

                decimal count = 0;
                decimal grandTotal = 0;

                foreach (GridViewRow gr in gvPO.Rows)
                {
                    if (gr.RowType == DataControlRowType.DataRow)
                    {
                        Label lblPODetails = (Label)gr.FindControl("lblPODetails");
                        Label lblQuantity = (Label)gr.FindControl("lblQuantity");
                        TextBox txtQty = (TextBox)gr.FindControl("txtQty");
                        TextBox txtTax = (TextBox)gr.FindControl("txtTax");
                        Label lbldiscount = (Label)gr.FindControl("lbldiscount");
                        Label lbldiscountamount = (Label)gr.FindControl("lblDiscountAmount");
                        Label lblTotalAmount = (Label)gr.FindControl("lblTotalAmount");

                        DataRowView rowView = (DataRowView)gr.DataItem;

                        if (rowView != null)
                        {
                            decimal rate = 0, quantity = 0, discountPercent = 0;
                            decimal.TryParse(rowView["Rate"].ToString(), out rate);
                            decimal.TryParse(txtQty.Text, out quantity);

                            // Calculate discount percentage
                            if (lbldiscount != null)
                            {
                                string discountValue = rowView["Discount"].ToString();
                                string numericPart = discountValue.TrimEnd('%');

                                if (decimal.TryParse(numericPart, out discountPercent))
                                {
                                    lbldiscount.Text = _cm.FormatCurrency(discountPercent, txtcurrency.Text);// discountPercent.ToString("0.00");
                                }
                                else
                                {
                                    lbldiscount.Text = _cm.FormatCurrency(0, txtcurrency.Text);// "0.00";
                                }
                            }

                            // Calculate discount amount
                            decimal discountAmount = 0;
                            if (lbldiscountamount != null)
                            {
                                discountAmount = (rate * quantity) * (discountPercent / 100);
                                lbldiscountamount.Text = _cm.FormatCurrency(discountAmount, txtcurrency.Text);// discountAmount.ToString("0.00");
                            }

                            if (lblTotalAmount != null)
                            {
                                decimal totalAmount = (rate * quantity) - discountAmount;
                                lblTotalAmount.Text = _cm.FormatCurrency(totalAmount, txtcurrency.Text);//  totalAmount.ToString("0.00");
                                grandTotal += totalAmount;
                            }
                        }

                        if (lblPODetails.Text.Contains(','))
                        {
                            string[] PODetails = lblPODetails.Text.Split(',');
                            foreach (string PO in PODetails)
                            {
                                if (PO != "")
                                {
                                    DataTable dtPODetails = _cm.GetPurchaseOrderDetail(PO);
                                    if (dtPODetails.Rows.Count > 0)
                                    {
                                        if (dtPODetails.Rows[0]["Quantity"].ToString() != "")
                                        {
                                            count += Convert.ToDecimal(dtPODetails.Rows[0]["Quantity"].ToString());
                                        }
                                    }
                                }
                            }

                            if (count.ToString() == lblQuantity.Text)
                            {
                                txtQty.ReadOnly = true;
                                txtQty.ToolTip = "PO Generated for Requested Quantity";
                                txtQty.ForeColor = Color.Red;
                            }
                        }
                    }
                    rowID++;
                }
                CalculateFooterTotals(showVAT);
            }

        }
        protected void gvPO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblRate = (e.Row.FindControl("lblRate") as Label);
                Label lblQuantity = (e.Row.FindControl("lblQuantity") as Label);
                Label lblAmount = (e.Row.FindControl("lblAmount") as Label);
                Label lblDeliveryDate = (e.Row.FindControl("lblDeliveryDate") as Label);
                TextBox txtDeliveryDate = (e.Row.FindControl("txtDeliveryDate") as TextBox);
                Label lbldiscount = (e.Row.FindControl("lbldiscount") as Label);
                Label lbldiscountamount = (e.Row.FindControl("lblDiscountAmount") as Label);
                Label lblTotalAmount = (e.Row.FindControl("lblTotalAmount") as Label);

                lblDeliveryDate.Visible = false;
                txtDeliveryDate.Visible = true;

                string currencyCode = txtcurrency.Text;

                DataRowView rowView = (DataRowView)e.Row.DataItem;

                if (rowView != null)
                {
                    decimal rate = 0, quantity = 0, discountPercent = 0;

                    decimal.TryParse(rowView["Rate"].ToString(), out rate);
                    decimal.TryParse(rowView["EnqQty"].ToString(), out quantity);

                    decimal amount = rate * quantity;
                    if (lblRate != null)
                    {
                        // lblAmount.Text = Math.Round(amount, 2).ToString("0.00");
                        lblRate.Text = _cm.FormatCurrency(rate, currencyCode);
                    }
                    if (lblAmount != null)
                    {
                        // lblAmount.Text = Math.Round(amount, 2).ToString("0.00");
                        lblAmount.Text = _cm.FormatCurrency(amount, currencyCode);
                    }

                    string discountValue = rowView["Discount"].ToString();
                    if (!string.IsNullOrEmpty(discountValue))
                    {
                        string numericPart = discountValue.TrimEnd('%');
                        if (decimal.TryParse(numericPart, out discountPercent))
                        {
                            if (lbldiscount != null)
                            {
                                //lbldiscount.Text = discountPercent.ToString("0.00") + "%";
                                //lbldiscount.Text = discountPercent.ToString("0.00");
                                lbldiscount.Text = _cm.FormatCurrency(discountPercent, currencyCode);
                            }

                            decimal discountAmount = amount * (discountPercent / 100);
                            if (lbldiscountamount != null)
                            {
                                //lbldiscountamount.Text = discountAmount.ToString("0.00");
                                lbldiscountamount.Text = _cm.FormatCurrency(discountAmount, currencyCode);
                            }

                            decimal totalAmount = amount - discountAmount;
                            if (lblTotalAmount != null)
                            {
                                //lblTotalAmount.Text = totalAmount.ToString("0.00");
                                lblTotalAmount.Text = _cm.FormatCurrency(totalAmount, currencyCode);
                            }
                        }
                    }
                    else
                    {
                        if (lbldiscount != null) lbldiscount.Text = _cm.FormatCurrency(0, currencyCode) +"%";//"0.00%";
                        if (lbldiscountamount != null) lbldiscountamount.Text = _cm.FormatCurrency(0, currencyCode); //"0.00";
                        if (lblTotalAmount != null) lblTotalAmount.Text = _cm.FormatCurrency(0, currencyCode); //amount.ToString("0.00");
                    }
                }
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                // Find the footer cells for Dis.% and Dis. (Amt.)
                // Adjust indexes according to your GridView columns.
                e.Row.Cells[8].ColumnSpan = 2;

                // Hide the second cell
                e.Row.Cells[9].Visible = false;
            }
        }
        
        protected void ddlVAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool showVAT = (txtcurrency.Text.ToUpper() == "IDR");
            CalculateFooterTotals(showVAT);
        }
        private void CalculateFooterTotals(bool calculateVAT)
        {
            decimal subTotal = 0;
            string additionalChargeDescription = "";
            decimal additionalCharges = 0;
            decimal vatTotal = 0;
            decimal grandTotal = 0;

            foreach (GridViewRow row in gvPO.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    Label lblAdditionalChargeDescription = (Label)row.FindControl("lblAdditionalChargeDescription");
                    Label lblAdditionalChargesValue = (Label)row.FindControl("lblAdditionalChargesValue");
                    Label lblTotalAmount = (Label)row.FindControl("lblTotalAmount");

                    if (lblTotalAmount != null && lblTotalAmount.Text != "")
                    {
                        decimal rowTotal = _cm.ToAnyCurrencyToDecimal(lblTotalAmount.Text, txtcurrency.Text);
                        subTotal += rowTotal;
                    }
                    if (lblAdditionalChargeDescription != null) { additionalChargeDescription = lblAdditionalChargeDescription.Text; }
                    if (lblAdditionalChargesValue != null && lblAdditionalChargesValue.Text!="") { additionalCharges = Convert.ToDecimal(lblAdditionalChargesValue.Text); }
                }
            }

            if (calculateVAT && ddlVAT.SelectedValue != "")
            {
                decimal vatPercent = Convert.ToDecimal(ddlVAT.SelectedValue);
                vatTotal = ((subTotal+additionalCharges) * vatPercent / 100);
            }

            grandTotal = subTotal + additionalCharges + vatTotal;

            if (gvPO.FooterRow != null)
            {
                Label lblSubtotal = (Label)gvPO.FooterRow.FindControl("lblSubtotal");
                Label lblAdditionalCharges1 = (Label)gvPO.FooterRow.FindControl("lblAdditionalCharges1");
                Label lblAdditionalCharges = (Label)gvPO.FooterRow.FindControl("lblAdditionalCharges");
                Label lblVAT = (Label)gvPO.FooterRow.FindControl("lblVAT");
                Label lblVATpercentage = (Label)gvPO.FooterRow.FindControl("lblVATpercentage");
                Label lblGrandTotal = (Label)gvPO.FooterRow.FindControl("lblGrandTotal");
                Panel pnlVATFooter = (Panel)gvPO.FooterRow.FindControl("pnlVATFooter");
                Panel pnlVATFooterValues = (Panel)gvPO.FooterRow.FindControl("pnlVATFooterValues");
                Label lblVATPercentText = (Label)gvPO.FooterRow.FindControl("lblVATPercentText");

                if (lblSubtotal != null) 
                    lblSubtotal.Text = _cm.FormatCurrency(subTotal, txtcurrency.Text);// subTotal.ToString("0.00");

                if (pnlVATFooter != null) pnlVATFooter.Visible = calculateVAT;
                if (pnlVATFooterValues != null) pnlVATFooterValues.Visible = calculateVAT;

                if (calculateVAT)
                {
                    if (lblVATPercentText != null)
                    {
                        lblVATPercentText.Text = "VAT (" + ddlVAT.SelectedItem.Text + ")";
                    }

                    if (lblVATpercentage != null)
                    {
                        lblVATpercentage.Text = ddlVAT.SelectedItem.Text;
                    }

                    if (lblVAT != null)
                    {
                        lblVAT.Text = _cm.FormatCurrency(vatTotal, txtcurrency.Text);// vatTotal.ToString("0.00");
                    }
                }
                lblAdditionalCharges1.Text = additionalChargeDescription;
                lblAdditionalCharges.Text = _cm.FormatCurrency(additionalCharges, txtcurrency.Text);
                if (lblGrandTotal != null) lblGrandTotal.Text = _cm.FormatCurrency(grandTotal, txtcurrency.Text);// grandTotal.ToString("0.00");
                DateTime Today = DateTime.Now.AddHours(1.5);
                txtDate.Text = Today.ToString("dd/MMM/yyyy");
            }
        }
        protected void txtQty_TextChanged(object sender, EventArgs e)
        {
            TextBox txtQty = (TextBox)sender;
            GridViewRow row = (GridViewRow)txtQty.NamingContainer;

            Label lblRate = (Label)row.FindControl("lblRate");
            Label lblAmount = (Label)row.FindControl("lblAmount");
            Label lbldiscount = (Label)row.FindControl("lbldiscount");
            Label lbldiscountamount = (Label)row.FindControl("lblDiscountAmount");
            Label lblTotalAmount = (Label)row.FindControl("lblTotalAmount");
            Label lblQuantity = (Label)row.FindControl("lblQuantity");

            if (lblQuantity != null && lblQuantity.Text != "" && txtQty.Text != "")
            {
                decimal enqQty;
                decimal enteredQty;

                if (!decimal.TryParse(lblQuantity.Text, out enqQty))
                {
                    ShowAlert("Invalid Enquiry Quantity format");
                    txtQty.Text = "";
                    return;
                }

                if (!decimal.TryParse(txtQty.Text, out enteredQty))
                {
                    ShowAlert("Please enter a valid quantity");
                    txtQty.Text = "";
                    return;
                }

                if (enteredQty > enqQty)
                {
                    ShowAlert($"Quantity cannot exceed Enquiry Quantity ({enqQty})");
                    txtQty.Text = enqQty.ToString();
                    enteredQty = enqQty;
                }

                if (lblRate != null && lblRate.Text != "")
                {
                    decimal rate;
                    if (!decimal.TryParse(lblRate.Text, out rate))
                    {
                        ShowAlert("Invalid rate format");
                        return;
                    }

                    decimal amount = rate * enteredQty;
                    lblAmount.Text = _cm.FormatCurrency(amount, txtcurrency.Text);// amount.ToString("0.00");

                    decimal discountPercent = 0;
                    if (lbldiscount != null && lbldiscount.Text != "")
                    {
                        //string discountText = lbldiscount.Text.Replace("%", "").Trim();
                        string discountText = lbldiscount.Text.Trim();
                        decimal.TryParse(discountText, out discountPercent);
                    }

                    decimal discountAmount = amount * (discountPercent / 100);
                    if (lbldiscountamount != null)
                    {
                        lbldiscountamount.Text = _cm.FormatCurrency(discountAmount, txtcurrency.Text);// discountAmount.ToString("0.00");
                    }

                    decimal totalAmount = amount - discountAmount;
                    if (lblTotalAmount != null)
                    {
                        lblTotalAmount.Text = _cm.FormatCurrency(totalAmount, txtcurrency.Text);//  totalAmount.ToString("0.00");
                    }

                    bool showVAT = (txtcurrency.Text.ToUpper() == "IDR");
                    pnlVATControls.Visible = showVAT;

                    CalculateFooterTotals(showVAT);

                    DateTime Today = DateTime.Now.AddHours(1.5);
                    txtDate.Text = Today.ToString("dd/MMM/yyyy");
                }
            }
            else if (txtQty.Text != "")
            {
                ShowAlert("Enquiry Quantity not available");
                txtQty.Text = "";
            }
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }
        protected void BtnSave_Click(object sender, EventArgs e)
        {

            DateTime Today = DateTime.Now.AddHours(1.5);
            txtDate.Text = Today.ToString("dd/MMM/yyyy");
            int flag = 0;

            if (string.IsNullOrEmpty(txtPONumber.Text.Trim()))
            {
                txtPONumber.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtPONumber.BorderColor = Color.Empty;
            }

            if (pnlVATControls.Visible)
            {
                if (ddlVAT.SelectedIndex == 0 || string.IsNullOrEmpty(ddlVAT.SelectedValue))
                {
                    ddlVAT.BorderColor = Color.Red;
                    flag = 1;
                }
                else
                {
                    ddlVAT.BorderColor = Color.Empty;
                }
            }

            if (gvPO.Rows.Count > 0)
            {
                for (int i = 0; i < gvPO.Rows.Count; i++)
                {
                    Label lblDeliveryDate = (Label)gvPO.Rows[i].FindControl("lblDeliveryDate");
                    TextBox txtDeliveryDate = (TextBox)gvPO.Rows[i].FindControl("txtDeliveryDate");
                    TextBox txtQty = (TextBox)gvPO.Rows[i].FindControl("txtQty");
                    
                    // Check delivery date from the visible control
                    string deliveryDateText = "";
                    if (txtDeliveryDate != null && txtDeliveryDate.Visible)
                    {
                        deliveryDateText = txtDeliveryDate.Text.Trim();
                    }
                    else if (lblDeliveryDate != null)
                    {
                        deliveryDateText = lblDeliveryDate.Text.Trim();
                    }

                    if (string.IsNullOrEmpty(deliveryDateText))
                    {
                        if (lblDeliveryDate != null) lblDeliveryDate.BorderColor = Color.Red;
                        if (txtDeliveryDate != null) txtDeliveryDate.BorderColor = Color.Red;
                        flag = 1;
                    }
                    else
                    {
                        if (lblDeliveryDate != null) lblDeliveryDate.BorderColor = Color.Empty;
                        if (txtDeliveryDate != null) txtDeliveryDate.BorderColor = Color.Empty;
                    }

                    if (string.IsNullOrEmpty(txtQty.Text.Trim()) || Convert.ToDecimal(txtQty.Text) == 0)
                    {
                        txtQty.BorderColor = Color.Red;
                        flag = 1;
                    }
                    else
                    {
                        txtQty.BorderColor = Color.Empty;
                    }
                }
            }
            else
            {
                flag = 1;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorMessage",
                    "alert('No items available in the grid to save!');", true);
                return;
            }

            if (flag == 0)
            {
                try
                {
                    _cm.PONumber = txtPONumber.Text.Trim();
                    txtDate.Enabled = true;
                    _cm.PODate = DateTime.Now.AddHours(1.5);
                    _cm.SpecialInstruction = txtSpecialInstruction.Text.Trim();
                    _cm.VendorID = Convert.ToInt32(Session["VendorID"].ToString());
                    _cm.CreatedDate = DateTime.Now.AddHours(1.5);

                    Label lblAdditionalCharges1 = (Label)gvPO.FooterRow.FindControl("lblAdditionalCharges1");
                    Label lblAdditionalCharges = (Label)gvPO.FooterRow.FindControl("lblAdditionalCharges");
                    Label lblGrandTotal = (Label)gvPO.FooterRow.FindControl("lblGrandTotal");
                    Label lblVAT = (Label)gvPO.FooterRow.FindControl("lblVAT");
                    if (lblAdditionalCharges1 != null && !string.IsNullOrEmpty(lblAdditionalCharges1.Text))
                    {
                        _cm.AdditionalChargeDescription = lblAdditionalCharges1.Text;
                    }
                    else
                    {
                        _cm.AdditionalChargeDescription = "";
                    }
                    if (lblAdditionalCharges != null && !string.IsNullOrEmpty(lblAdditionalCharges.Text))
                    {
                        _cm.AdditionalCharges = _cm.ToAnyCurrencyToDecimal(lblAdditionalCharges.Text, txtcurrency.Text);
                    }
                    else
                    {
                        _cm.AdditionalCharges = _cm.ToAnyCurrencyToDecimal("0.0", txtcurrency.Text);
                    }
                    if (lblGrandTotal != null && !string.IsNullOrEmpty(lblGrandTotal.Text))
                    {
                        _cm.GrandTotal = _cm.ToAnyCurrencyToDecimal(lblGrandTotal.Text, txtcurrency.Text);
                    }
                    else
                    {
                        _cm.GrandTotal = _cm.ToAnyCurrencyToDecimal("0.0",txtcurrency.Text);
                    }

                    if (lblVAT != null)
                    {
                        if(lblVAT.Text != "")
                        {
                            _cm.VAT = _cm.ToAnyCurrencyToDecimal(lblVAT.Text, txtcurrency.Text).ToString();
                        }
                        else
                        {
                            _cm.VAT = "0";
                        }
                    }
                    else
                    {
                        _cm.VAT = "0";
                    }


                    _cm.Delivery = txtDelivery.Text.Trim();
                    _cm.Payment = ddlPaymentTerms.SelectedItem.ToString();
                    _cm.Guarantee = txtGuarantee.Text.Trim();
                    _cm.Remarks = txtRemarks.Text.Trim();

                    if (!string.IsNullOrEmpty(lblContactPerson.Text))
                        _cm.ContactPersonID = Convert.ToInt32(lblContactPerson.Text);
                    else
                        _cm.ContactPersonID = 0;

                    DataTable dtPOYear = _cm.GetPOYear();
                    if (dtPOYear.Rows.Count > 0)
                    {
                        _cm.POYear = dtPOYear.Rows[0]["POYear"].ToString();
                        DataTable dtPO = _cm.GetPODetails1(StoreType, _cm.POYear);

                        if (dtPO.Rows.Count > 0 && !string.IsNullOrEmpty(dtPO.Rows[0]["POSeries"].ToString()))
                        {
                            _cm.POSeries = Convert.ToInt32(dtPO.Rows[0]["POSeries"]) + 1;
                        }
                        else
                        {
                            _cm.POSeries = 1;
                        }
                    }

                    string EnquiryID = Session["EnquiryID"].ToString();
                    DataTable dtEnquiry = _cm.CheckEnquiry(EnquiryID);

                    if (dtEnquiry.Rows.Count > 0 && dtEnquiry.Columns.Contains("ProjectID"))
                    {
                        object projectIdObj = dtEnquiry.Rows[0]["ProjectID"];
                        if (projectIdObj != DBNull.Value && !string.IsNullOrEmpty(projectIdObj.ToString().Trim()))
                        {
                            if (int.TryParse(projectIdObj.ToString().Trim(), out int projectId))
                            {
                                _cm.ProjectID = projectId;
                            }
                            else
                            {
                                _cm.ProjectID = 0;
                            }
                        }
                        else
                        {
                            _cm.ProjectID = 0;
                        }
                    }
                    else
                    {
                        _cm.ProjectID = 0;
                    }

                    _cm.StoreType = StoreType;

                    if (Session["UnitID"] != null)
                    {
                        _cm.UnitID = Convert.ToInt32(Session["UnitID"].ToString());
                    }
                    else
                    {
                        _cm.UnitID = 1;
                    }

                    _cm.PurchaseOrderID = _cm.AddPurchaseOrder(_cm);

                    if (_cm.PurchaseOrderID <= 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "errorMessage",
                            "alert('Error saving Purchase Order. Please try again.');", true);
                        return;
                    }

                    _cm.EnquiryID = Convert.ToInt32(EnquiryID);
                    _cm.EnquiryStatusID = 0;
                    _cm.UpdateEnquiry(_cm);

                    if (Session["UserID"] != null && Session["EmployeeID"] != null)
                    {
                        _cm.StatusID = 1;
                        _cm.UserID = Convert.ToInt32(Session["UserID"].ToString());
                        _cm.EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                        _cm.Remarks = "PO Generated";
                        int POHistoryID = _cm.AddPOHistory(_cm);
                    }

                    Session["PurchaseOrderID"] = _cm.PurchaseOrderID;

                    bool hasValidItems = false;
                    for (int i = 0; i < gvPO.Rows.Count; i++)
                    {
                        Label lblItemID = (Label)gvPO.Rows[i].FindControl("lblItemID");
                        Label lblItemCode = (Label)gvPO.Rows[i].FindControl("lblItemCode");
                        Label lblItemSpecification = (Label)gvPO.Rows[i].FindControl("lblItemSpecification");
                        Label lblVendorQuotationID = (Label)gvPO.Rows[i].FindControl("lblVendorQuotationID");
                        Label lblVendorQuotationDetailsID = (Label)gvPO.Rows[i].FindControl("lblVendorQuotationDetailsID");
                        Label lblEnquiryDetailsID = (Label)gvPO.Rows[i].FindControl("lblEnquiryDetailsID");
                        Label lblDeliveryDate = (Label)gvPO.Rows[i].FindControl("lblDeliveryDate");
                        TextBox txtDeliveryDate = (TextBox)gvPO.Rows[i].FindControl("txtDeliveryDate");
                        Label lblRate = (Label)gvPO.Rows[i].FindControl("lblRate");
                        Label lblAmount = (Label)gvPO.Rows[i].FindControl("lblAmount");
                        Label lblDiscount = (Label)gvPO.Rows[i].FindControl("lbldiscount");
                        Label lblDiscountAmount = (Label)gvPO.Rows[i].FindControl("lblDiscountAmount");
                        TextBox txtTax = (TextBox)gvPO.Rows[i].FindControl("txtTax");
                        TextBox txtQty = (TextBox)gvPO.Rows[i].FindControl("txtQty");
                        Label lblTotalAmount = (Label)gvPO.Rows[i].FindControl("lblTotalAmount");
                        DropDownList ddlVAT = (DropDownList)gvPO.Rows[i].FindControl("ddlVAT");
                        HiddenField hdRemainingQty = (HiddenField)gvPO.Rows[i].FindControl("hdRemainingQty");

                        if (lblVendorQuotationDetailsID == null || string.IsNullOrEmpty(lblVendorQuotationDetailsID.Text))
                        {
                            continue;
                        }

                        decimal quantity = 0; decimal remainingQty = 0;
                        if (txtQty != null && !string.IsNullOrEmpty(txtQty.Text.Trim()) && decimal.TryParse(txtQty.Text.Trim(), out quantity) && quantity > 0)
                        {
                            if (hdRemainingQty != null && !string.IsNullOrEmpty(hdRemainingQty.Value.Trim()) && decimal.TryParse(hdRemainingQty.Value.Trim(), out remainingQty))
                            {
                                if((remainingQty - quantity) == 0)
                                {
                                    _cm.QuotationStatusID = 9;//QuotationStatus-PO Generated
                                }
                                else
                                {
                                    _cm.QuotationStatusID = 1017;//QuotationStatus-Partial PO Generated
                                }
                            }
                            hasValidItems = true;

                            _cm.ItemID = Convert.ToInt32(lblItemID.Text);
                            _cm.ItemCode = lblItemCode.Text;
                            _cm.VendorQuotationID = Convert.ToInt32(lblVendorQuotationID.Text);
                            _cm.VendorQuotationDetailsID = Convert.ToInt32(lblVendorQuotationDetailsID.Text);

                            if (lblDiscount != null && !string.IsNullOrEmpty(lblDiscount.Text))
                            {
                                _cm.Discount = _cm.ToAnyCurrencyToDecimal(lblDiscount.Text,txtcurrency.Text).ToString();
                            }
                            else
                            {
                                _cm.Discount = "";
                            }

                            if (lblDiscountAmount != null && !string.IsNullOrEmpty(lblDiscountAmount.Text))
                            {
                                _cm.DiscountAmount = _cm.ToAnyCurrencyToDecimal(lblDiscountAmount.Text,txtcurrency.Text);
                            }
                            else
                            {
                                _cm.DiscountAmount = 0;
                            }

                            if (txtTax != null && !string.IsNullOrEmpty(txtTax.Text.Trim()))
                            {
                                _cm.Tax = _cm.ToAnyCurrencyToDecimal(txtTax.Text.Trim(),txtcurrency.Text);
                            }
                            else
                            {
                                _cm.Tax = 0;
                            }

                            if (lblVAT != null && !string.IsNullOrEmpty(lblVAT.Text))
                            {
                                _cm.TaxAmount = _cm.ToAnyCurrencyToDecimal(lblVAT.Text,txtcurrency.Text);
                            }
                            else
                            {
                                decimal amount = _cm.ToAnyCurrencyToDecimal(lblAmount.Text,txtcurrency.Text);
                                _cm.TaxAmount = Math.Round((amount * _cm.Tax) / 100, 2);
                            }

                            if (lblTotalAmount != null && !string.IsNullOrEmpty(lblTotalAmount.Text))
                            {
                                _cm.TotalAmount = _cm.ToAnyCurrencyToDecimal(lblTotalAmount.Text,txtcurrency.Text);
                            }
                            else
                            {
                                decimal amount = _cm.ToAnyCurrencyToDecimal(lblAmount.Text,txtcurrency.Text);
                                _cm.TotalAmount = amount + _cm.TaxAmount - _cm.DiscountAmount;
                            }

                            if (txtDeliveryDate != null && txtDeliveryDate.Visible && !string.IsNullOrEmpty(txtDeliveryDate.Text.Trim()))
                            {
                                _cm.DeliveryDate = Convert.ToDateTime(txtDeliveryDate.Text);
                            }
                            else if (lblDeliveryDate != null && !string.IsNullOrEmpty(lblDeliveryDate.Text))
                            {
                                _cm.DeliveryDate = Convert.ToDateTime(lblDeliveryDate.Text);
                            }
                            else
                            {
                                _cm.DeliveryDate = DateTime.Now.AddHours(1.5).AddDays(30);
                            }

                            _cm.Quantity = quantity;
                            _cm.Rate = _cm.ToAnyCurrencyToDecimal(lblRate.Text,txtcurrency.Text);
                            _cm.Amount = _cm.ToAnyCurrencyToDecimal(lblAmount.Text,txtcurrency.Text);

                            if (ddlVAT != null && !string.IsNullOrEmpty(ddlVAT.SelectedValue))
                            {
                                _cm.VATID = Convert.ToInt32(ddlVAT.SelectedValue);
                            }
                            else
                            {
                                _cm.VATID = 1;
                            }
                            if (lblItemSpecification != null && !string.IsNullOrEmpty(lblItemSpecification.Text))
                            {
                                _cm.ItemSpecification = lblItemSpecification.Text;
                            }
                            else
                            {
                                _cm.ItemSpecification = "";
                            }
                            _cm.StatusID = 1;
                            _cm.AllocatedQty = 0;
                            _cm.ReceivedQty = 0;
                            _cm.EnquiryDetailsID = Convert.ToInt32(lblEnquiryDetailsID.Text);
                            _cm.CreatedDate = DateTime.Now.AddHours(1.5);

                            int poDetailsID = _cm.AddPurchaseOrderDetails(_cm);

                            if (poDetailsID > 0)
                            {
                                _cm.PODetailsID = poDetailsID;

                                DataTable dtEnqDetails = _cm.GetEnquiry(lblEnquiryDetailsID.Text);
                                if (dtEnqDetails.Rows.Count > 0)
                                {
                                    string PODetailsID = dtEnqDetails.Rows[0]["PODetailsID"].ToString();
                                    _cm.PODetails = PODetailsID + ",";
                                    _cm.EnquiryDetailsID = Convert.ToInt32(dtEnqDetails.Rows[0]["EnquiryDetailsID"]);
                                    _cm.UpdatePOInEnquiryDetails(_cm);
                                }

                                _cm.UpdatePOIDInPR(_cm);
                                
                                _cm.UpdateVendorQuotationDetailsStatus(_cm);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorMessage",
                                    $"alert('Error saving item details for row {i + 1}. Please try again.');", true);
                                return;
                            }
                        }
                        else
                        {
                            if (lblVendorQuotationDetailsID != null && !string.IsNullOrEmpty(lblVendorQuotationDetailsID.Text))
                            {
                                _cm.VendorQuotationDetailsID = Convert.ToInt32(lblVendorQuotationDetailsID.Text);
                                _cm.QuotationStatusID = 2;
                                _cm.UpdateVendorQuotationDetailsStatus(_cm);
                            }
                        }
                    }

                    if (!hasValidItems)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "errorMessage",
                            "alert('No valid items found to save. Please check quantities.');", true);
                        return;
                    }

                    string PurchaseOrderID = _cm.PurchaseOrderID.ToString();
                    DataTable dt = _cm.GetPRDetailsonProject(PurchaseOrderID);
                    if (dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0]["ProjectID"].ToString()) && dt.Rows[0]["ProjectID"].ToString() != "0")
                    {
                        _cm.ProjectID = Convert.ToInt32(dt.Rows[0]["ProjectID"].ToString());
                        _cm.PurchaseRequisitionID = Convert.ToInt32(dt.Rows[0]["PurchaseRequisitionID"].ToString());
                        _cm.AddProjectPRDetails(_cm);
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "successMessage",
                        "alert('Purchase Order saved successfully!'); window.location='GeneratePO.aspx';", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "errorMessage",
                        $"alert('Error saving Purchase Order: {ex.Message}');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "validationMessage",
                    "alert('Please fill all required fields.');", true);
            }
        }
        protected void VATCalculation(DropDownList ddlVAT, int rowID)
        {
            decimal TotalAmount = 0; decimal DiscountAmount = 0; decimal Tax = 0; decimal TaxAmt = 0; decimal Amount = 0;

            string VAT = ddlVAT.SelectedValue.ToString();
            DataTable dt = _cm.GetVATDetails(VAT);
            if (dt.Rows.Count > 0)
            {
                Tax = Convert.ToDecimal(dt.Rows[0]["Percentage"].ToString());
            }

            Label lblDiscountAmount = (Label)gvPO.Rows[rowID].FindControl("lblDiscountAmount");
            Label lblTotalAmount = (Label)gvPO.Rows[rowID].FindControl("lblTotalAmount");
            Label lblAmount = (Label)gvPO.Rows[rowID].FindControl("lblAmount");
            Label lblTaxAmount = (Label)gvPO.Rows[rowID].FindControl("lblTaxAmount");

            Label lblVAT = (Label)gvPO.FooterRow.FindControl("lblVAT");
            Label lblVAT1 = (Label)gvPO.FooterRow.FindControl("lblVAT1");
            Label lblGrandTotal = (Label)gvPO.FooterRow.FindControl("lblGrandTotal");

            if (lblAmount.Text != "")
            {
                Amount = Convert.ToDecimal(lblAmount.Text.ToString());
            }
            if (lblDiscountAmount.Text != "")
            {
                DiscountAmount = Convert.ToDecimal(lblDiscountAmount.Text.ToString());
            }

            decimal DisAmt = Amount - DiscountAmount;

            TaxAmt = (DisAmt * Tax) / 100;
            lblTaxAmount.Text = TaxAmt.ToString();

            lblVAT1.Text = ddlVAT.SelectedItem.ToString();
            lblVAT.Text = Math.Round(TaxAmt, 2).ToString();

            TotalAmount = Amount + TaxAmt - DiscountAmount;
            lblTotalAmount.Text = Math.Round(TotalAmount, 2).ToString();
            DateTime Today = DateTime.Now.AddHours(1.5);
            txtDate.Text = Today.ToString("dd/MMM/yyyy");

            decimal GrandTotal = 0; decimal TaxAmount = 0;

            for (int i = 0; i < gvPO.Rows.Count; i++)
            {

                Label lblTotalAmount1 = (Label)gvPO.Rows[i].FindControl("lblTotalAmount");
                Label lblTaxAmount1 = (Label)gvPO.Rows[i].FindControl("lblTaxAmount");

                if (lblTotalAmount1.Text != "" && lblTaxAmount1.Text != "")
                {
                    GrandTotal += Convert.ToDecimal(lblTotalAmount1.Text.ToString());


                    TaxAmount += Convert.ToDecimal(lblTaxAmount1.Text.ToString());
                    lblVAT.Text = Math.Round(TaxAmount, 2).ToString();
                }
            }
            lblGrandTotal.Text = Math.Round(GrandTotal, 2).ToString();
        }
        public DataTable RemoveDuplicateRows(DataTable table, string DistinctColumn)
        {
            try
            {
                ArrayList UniqueRecords = new ArrayList();
                ArrayList DuplicateRecords = new ArrayList();
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
        protected void gvPO_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvPO.Rows[e.RowIndex];
            int BatchID = int.Parse(gvPO.DataKeys[e.RowIndex]["BatchID"].ToString());
        }
        protected void gvPO_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                TextBox txtDeliveryDate = (TextBox)gvPO.Rows[rowIndex].FindControl("txtDeliveryDate");

            }
        }
        protected void gvPO_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPO.EditIndex = -1;
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            _cm.PurchaseOrderID = Convert.ToInt32(hdPurchaseOrderID.Value);
            _cm.SpecialInstruction = txtSpecialInstruction.Text;
            _cm.Delivery = txtDelivery.Text;

            _cm.Payment = ddlPaymentTerms.SelectedItem.ToString();
            _cm.Guarantee = txtGuarantee.Text;
            _cm.Remarks = txtRemarks.Text;

            _cm.AmendPO(_cm);
            _cm.StatusID = 1;
            _cm.Reason = "";
            _cm.UpdatePurchaseOrderStatus(_cm);
            _cm.UpdatePOApproval(_cm);
            Session["PurchaseOrderID"] = _cm.PurchaseOrderID;
            Response.Redirect("GeneratePO.aspx");
        }
    }
}