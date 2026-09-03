using System;
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
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.VariantTypes;
using LIMS.DAL;
using static LIMS.DAL.GLModule;

namespace LIMS
{
    public partial class InvoiceGenerate : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.GLModule _gl = new DAL.GLModule();

        decimal subtotal = 0;
        decimal dppRate = 11 / 12;
        decimal vatRate = 0.11m; // 11%
        decimal amountPaid = 100; // example
        private decimal Discount
        {
            get => Session["Discount"] == null ? 0 : (decimal)Session["Discount"];
            set => Session["Discount"] = value;
        }

        private decimal DownPayment
        {
            get => Session["DownPayment"] == null ? 0 : (decimal)Session["DownPayment"];
            set => Session["DownPayment"] = value;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if ((Request.QueryString["ID"] != null) && (Request.QueryString["c"] != null))
                {
                    BindTaxes();
                    BindValues();
                    BindInvoice();
                    BindDates();
                }
            }
        }
        protected void BindTaxes()
        {
            DataTable dt = _cm.GetOrganizationDetails();
            if (dt.Rows.Count > 0)
            {
                vatRate = Convert.ToDecimal(dt.Rows[0]["tax(%)"].ToString());
                string dbValue = dt.Rows[0]["DPPPercentage"].ToString().Trim();

                if (dbValue.Contains("/"))
                {
                    string[] parts = dbValue.Split('/');

                    decimal numerator = Convert.ToDecimal(parts[0]);
                    decimal denominator = Convert.ToDecimal(parts[1]);

                    dppRate = numerator / denominator;   // 0.9166667
                }
                else
                {
                    // Handles values like: 12, 12.5, 0.92
                    dppRate = Convert.ToDecimal(dbValue);
                }
            }
        }
        protected void BindDates()
        {
            txtInvoiceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DataTable dtpaymentTerm = new DataTable();
            int DayCount = 0;
            if (ddlPaymentTerm.SelectedIndex != 0)
            {
                dtpaymentTerm = _cm.getPaymentTerms(ddlPaymentTerm.SelectedValue);
                if (dtpaymentTerm.Rows.Count > 0)
                {
                    DayCount = Convert.ToInt32(dtpaymentTerm.Rows[0]["DayCount"].ToString());
                    txtDueDate.Text = (Convert.ToDateTime(txtInvoiceDate.Text).AddDays(DayCount)).ToString("yyyy-MM-dd");
                }
            }


        }

        protected void BindInvoice()
        {
           
            subtotal = 0; // 🔹 important reset
            int QuotationID = Convert.ToInt32(Request.QueryString["ID"].ToString());
            hdQuotationID.Value = QuotationID.ToString();
            int RevisionCount = Convert.ToInt32(Request.QueryString["c"].ToString());
            string CurrencyName = "";
            string Remarks = "";
            decimal SubTotal = 0;
            decimal DownPayment = 0;
            decimal Discount = 0;
            decimal Totalbeforetax = 0;
            decimal VAT = 0;
            decimal Grandtotal = 0;
            
            BindTaxes();
            
            DataTable dtQM = _cm.getquotationmaster(Convert.ToInt32(QuotationID));
            if (dtQM.Rows.Count > 0)
            {
                lblInvoiceNo.Text = GenerateInvoiceNumber();
                hdCustomerMasterID.Value = dtQM.Rows[0]["CustomerMasterID"].ToString();
                lblCompanyName.Text = dtQM.Rows[0]["CompanyName"].ToString();
                lblAddressLine1.Text = dtQM.Rows[0]["AddressLine1"].ToString();
                lblAddressLine2.Text = dtQM.Rows[0]["AddressLine2"] + " " +
                            dtQM.Rows[0]["AddressLine3"] + " <br/>" +
                            dtQM.Rows[0]["District"] + " " +
                            dtQM.Rows[0]["State"] + " " +
                            dtQM.Rows[0]["Pincode"] + " " +
                            dtQM.Rows[0]["Country"].ToString();
                lblNPWP.Text = dtQM.Rows[0]["NPWP"].ToString();
                ddlPaymentTerm.Items.FindByValue(dtQM.Rows[0]["PaymentTermID"].ToString()).Selected = true;
                ddlCurrency.Items.FindByValue(dtQM.Rows[0]["CurrencyID"].ToString()).Selected = true;
                CurrencyName = dtQM.Rows[0]["CurrencyName"].ToString();
                Remarks = dtQM.Rows[0]["Remarks"].ToString();

                Discount = Convert.ToDecimal(dtQM.Rows[0]["Discount"].ToString());
                VAT = Convert.ToDecimal(dtQM.Rows[0]["VATAmount"].ToString());
                Grandtotal = Convert.ToDecimal(dtQM.Rows[0]["GrandTotal"].ToString());
                Totalbeforetax = Grandtotal - VAT;
                SubTotal = Totalbeforetax + Discount;
                txtPIC.Text = dtQM.Rows[0]["EmployeeName"].ToString();
                lblPONumber.Text = dtQM.Rows[0]["PONumber"].ToString();


                DataTable dtQD = _cm.getquotationdetailsNew(QuotationID);
                if (dtQD.Rows.Count > 0)
                {
                    DataTable dt = new DataTable();
                    if (Session["GridData"] == null)
                    {
                        dt.Columns.Add("QuotationDetailID", typeof(int));
                        dt.Columns.Add("RequestNo", typeof(string));
                        dt.Columns.Add("WorkOrderNumber", typeof(string));
                        dt.Columns.Add("TestItems", typeof(string));
                        dt.Columns.Add("sampleDescription", typeof(string));
                        dt.Columns.Add("TestItemDetails", typeof(string));
                        dt.Columns.Add("Quantity", typeof(int));
                        dt.Columns.Add("Price", typeof(decimal));
                        dt.Columns.Add("TotalAmount", typeof(decimal));
                        dt.Columns.Add("ItemType", typeof(string));
                        dt.Columns.Add("AccountCode", typeof(string));
                        dt.Columns.Add("DepartmentID", typeof(string));

                        foreach (DataRow row in dtQD.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            dr["QuotationDetailID"] = row["QuotationDetailID"].ToString();
                            dr["RequestNo"] = row["RequestNo"].ToString();
                            dr["WorkOrderNumber"] = row["WorkOrderNumber"].ToString();
                            dr["TestItems"] = row["TestItems"].ToString();
                            dr["sampleDescription"] = row["sampleDescription"].ToString();
                            dr["TestItemDetails"] = row["TestItems"].ToString();
                            dr["Quantity"] = Convert.ToInt32(Convert.ToDecimal(row["Quantity"].ToString()));
                            dr["Price"] = Convert.ToDecimal(row["Price"].ToString());
                            dr["TotalAmount"] = Convert.ToDecimal(row["TotalAmount"].ToString());
                            dr["ItemType"] = row["ItemType"].ToString();
                            dr["AccountCode"] = "";
                            dr["DepartmentID"] = row["DepartmentID"].ToString();
                            dt.Rows.Add(dr);
                        }
                        Session["GridData"] = dt;
                    }
                    if (Session["GridData"] != null)
                    {
                        dt = (DataTable)Session["GridData"];
                    }

                    gvItems.DataSource = dt;
                    gvItems.DataBind();
                }

            }
        }
        private string GenerateInvoiceNumber()
        {
            string newInvoiceNumber = "INV-00001"; // Default invoice number in case no invoices exist

            // Get the last InvoiceNumber from the database
            string query = @"SELECT COALESCE(MAX(CAST(SUBSTRING(InvoiceNumber, PATINDEX('%[0-9]%', InvoiceNumber), LEN(InvoiceNumber)) AS INT)), 0) AS LastInvoiceNumber 
                             FROM invoices WHERE InvoiceNumber LIKE 'INV%'";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();

                // Check if result is not DBNull.Value and handle the result
                if (result != DBNull.Value)
                {
                    int lastInvoiceNumber = Convert.ToInt32(result);
                    newInvoiceNumber = "INV-" + (lastInvoiceNumber + 1).ToString("D5"); // Increment last invoice number
                }
            }

            return newInvoiceNumber;
        }
        protected void BindValues()
        {
            DataTable dt = _cm.GetTransactionCode();
            if (dt.Rows.Count > 0)
            {
                ddlTransactionCode.DataSource = dt;
                ddlTransactionCode.DataTextField = "TransactionCode";
                ddlTransactionCode.DataValueField = "ID";
                ddlTransactionCode.DataBind();
                ddlTransactionCode.Items.Insert(0, "Select");
            }
            dt = _cm.getCurrency();
            if (dt.Rows.Count > 0)
            {
                ddlCurrency.DataSource = dt;
                ddlCurrency.DataTextField = "CurrencyCode";
                ddlCurrency.DataValueField = "CurrencyID";
                ddlCurrency.DataBind();
                ddlCurrency.Items.Insert(0, "--Select Currency--");
            }
            dt = _cm.getPaymentTerms();
            if (dt.Rows.Count > 0)
            {
                ddlPaymentTerm.DataSource = dt;
                ddlPaymentTerm.DataTextField = "PaymentTermName";
                ddlPaymentTerm.DataValueField = "PaymentTermID";
                ddlPaymentTerm.DataBind();
                ddlPaymentTerm.Items.Insert(0, "--Select Payment Terms--");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (gvItems.FooterRow == null)
                return;

            Label lblSubTotal = (Label)gvItems.FooterRow.FindControl("lblSubtotal");
            TextBox txtDiscount = (TextBox)gvItems.FooterRow.FindControl("txtDiscount");
            Label lblDPP = (Label)gvItems.FooterRow.FindControl("lblDPP");
            Label lblVAT = (Label)gvItems.FooterRow.FindControl("lblVat");
            Label lblGrandtotal = (Label)gvItems.FooterRow.FindControl("lblGrandTotal");
            TextBox txtDownPayment = (TextBox)gvItems.FooterRow.FindControl("txtDownPayment");
            Label lblBalanceDue = (Label)gvItems.FooterRow.FindControl("lblBalance");

            if (lblInvoiceNo.Text == "") { flag = 1; lblInvoiceNo.BackColor = Color.LightPink; } else { lblInvoiceNo.BackColor = Color.Empty; }
            if (txtInvoiceDate.Text == "") { flag = 1; txtInvoiceDate.BorderColor = Color.Red; } else { txtInvoiceDate.BorderColor = Color.Empty; }
            if (txtDueDate.Text == "") { flag = 1; txtDueDate.BorderColor = Color.Red; } else { txtDueDate.BorderColor = Color.Empty; }
            if (ddlCurrency.SelectedIndex == 0) { flag = 1; ddlCurrency.BackColor = Color.LightPink; } else { ddlCurrency.BackColor = Color.Empty; }
            if (ddlPaymentTerm.SelectedIndex == 0) { flag = 1; ddlPaymentTerm.BackColor = Color.LightPink; } else { ddlPaymentTerm.BackColor = Color.Empty; }
            if (ddlTransactionCode.SelectedIndex == 0) { flag = 1; ddlTransactionCode.BackColor = Color.LightPink; } else { ddlTransactionCode.BackColor = Color.Empty; }
            if (ddlBuyerType.SelectedIndex == 0) { flag = 1; ddlBuyerType.BackColor = Color.LightPink; } else { ddlBuyerType.BackColor = Color.Empty; }
            if (gvItems.Rows.Count == 0) { flag = 2; }
            if (flag == 0)
            {
                _cm.companyId = Convert.ToInt32(hdCustomerMasterID.Value);
                _cm.InvoiceDate = Convert.ToDateTime(txtInvoiceDate.Text);
                _cm.DueDate = Convert.ToDateTime(txtDueDate.Text);

                _cm.PaidAmount = 0;
                _cm.InvoiceStatus = 1;
                _cm.QuotationID = Convert.ToInt32(hdQuotationID.Value);
                _cm.InvoiceNumber = lblInvoiceNo.Text;
                _cm.PaymentTermID = Convert.ToInt32(ddlPaymentTerm.SelectedValue);
                _cm.CurrencyID = Convert.ToInt32(ddlCurrency.SelectedValue);
                _cm.WONumber = "";
                _cm.TransactionCodeID = Convert.ToInt32(ddlPaymentTerm.SelectedValue.ToString());
                _cm.PIC = txtPIC.Text;
                _cm.PONumber = lblPONumber.Text;
                _cm.BuyerIDType = ddlBuyerType.SelectedItem.ToString();
                switch (ddlCurrency.SelectedItem.ToString())
                {
                    case "IDR":
                        CultureInfo rupiahCulture = new CultureInfo("id-ID");
                        var nf = rupiahCulture.NumberFormat;
                        _cm.SubTotal = Convert.ToDecimal(lblSubTotal.Text.Replace(nf.CurrencySymbol, "")
                                                   .Replace(nf.CurrencyGroupSeparator, "")
                                                   .Trim());
                        _cm.DiscountAmount = Convert.ToDecimal(txtDiscount.Text.Replace(nf.CurrencySymbol, "")
                                                                         .Replace(nf.CurrencyGroupSeparator, "")
                                                                         .Trim());
                        _cm.DPPAmount = Convert.ToDecimal(lblDPP.Text.Replace(nf.CurrencySymbol, "")
                                                                        .Replace(nf.CurrencyGroupSeparator, "")
                                                                        .Trim());
                        _cm.VATAmount = Convert.ToDecimal(lblVAT.Text.Replace(nf.CurrencySymbol, "")
                                                                         .Replace(nf.CurrencyGroupSeparator, "")
                                                                         .Trim());
                        _cm.TotalAmount = Convert.ToDecimal(lblGrandtotal.Text.Replace(nf.CurrencySymbol, "")
                                                                       .Replace(nf.CurrencyGroupSeparator, "")
                                                                       .Trim());
                        _cm.DownPayment = Convert.ToDecimal(txtDownPayment.Text.Replace(nf.CurrencySymbol, "")
                                                                         .Replace(nf.CurrencyGroupSeparator, "")
                                                                         .Trim());
                        _cm.BalanceDue = Convert.ToDecimal(lblBalanceDue.Text.Replace(nf.CurrencySymbol, "")
                                                                         .Replace(nf.CurrencyGroupSeparator, "")
                                                                         .Trim());
                        break;
                    case "US Dollar":
                        CultureInfo usdCulture = new CultureInfo("en-US");
                        var nf1 = usdCulture.NumberFormat;
                        _cm.SubTotal = Convert.ToDecimal(lblSubTotal.Text.Replace(nf1.CurrencySymbol, "")
                                                                         .Replace(nf1.CurrencyGroupSeparator, "")
                                                                         .Trim());
                        _cm.DiscountAmount = Convert.ToDecimal(txtDiscount.Text.Replace(nf1.CurrencySymbol, "")
                                                                         .Replace(nf1.CurrencyGroupSeparator, "")
                                                                         .Trim());                        
                        _cm.DPPAmount = Convert.ToDecimal(lblDPP.Text.Replace(nf1.CurrencySymbol, "")
                                                                        .Replace(nf1.CurrencyGroupSeparator, "")
                                                                        .Trim());
                        _cm.VATAmount = Convert.ToDecimal(lblVAT.Text.Replace(nf1.CurrencySymbol, "")
                                                                         .Replace(nf1.CurrencyGroupSeparator, "")
                                                                         .Trim());
                        _cm.TotalAmount = Convert.ToDecimal(lblGrandtotal.Text.Replace(nf1.CurrencySymbol, "")
                                                                       .Replace(nf1.CurrencyGroupSeparator, "")
                                                                       .Trim());
                        _cm.DownPayment = Convert.ToDecimal(txtDownPayment.Text.Replace(nf1.CurrencySymbol, "")
                                                                         .Replace(nf1.CurrencyGroupSeparator, "")
                                                                         .Trim());
                        _cm.BalanceDue = Convert.ToDecimal(lblBalanceDue.Text.Replace(nf1.CurrencySymbol, "")
                                                                         .Replace(nf1.CurrencyGroupSeparator, "")
                                                                         .Trim());
                       
                        break;
                    default:
                        break;
                }

                DataTable dt = Session["GridData"] as DataTable;
                _cm.InvoiceID = _cm.AddInvoice(_cm, dt);
                UpdateInvoice(_cm.QuotationID);

                //Update Customer Ledger
                _cm.UpdateCustomerLedger(_cm, "Invoice");

                DAL.GLModule.AccountActivityLog log = new GLModule.AccountActivityLog();
                log.UserId = Convert.ToInt32(Session["UserID"].ToString());
                log.UserName = Session["UserName"].ToString();
                log.ActivityType = "Create";
                log.ModuleName = "Invoice";
                log.ReferenceId = _cm.InvoiceID;
                log.ReferenceNo = lblInvoiceNo.Text + " | " + lblCompanyName.Text;
                log.AccountId = "-";
                log.Amount = Convert.ToDecimal(_cm.TotalAmount);
                _gl.LogAccountActivity(log);

                Response.Redirect("GenerateInvoice.aspx?ID=" + _cm.InvoiceID);
            }
            else if (flag == 1)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }
            else if (flag == 2)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Invoice Items Missing.)", true);
            }
        }
        private void UpdateInvoice(object quotationMasterID)
        {
            // Get the current date (you can format it as needed)
            string invoiceDate = DateTime.Now.ToString("yyyy-MM-dd");

            string updateQuery = @"UPDATE QuotationMaster SET InvoiceNumber = @InvoiceNumber, InvoiceDate = @InvoiceDate, 
                                    QuotationStatusId = @QuotationStatusId, UpdatedDate = @UpdatedDate
                                   WHERE QuotationMasterID = @QuotationMasterID";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@InvoiceNumber", lblInvoiceNo.Text);
                cmd.Parameters.AddWithValue("@InvoiceDate",txtInvoiceDate.Text);
                cmd.Parameters.AddWithValue("@QuotationStatusId", 15);
                cmd.Parameters.AddWithValue("@UpdatedDate",DateTime.Now);
                cmd.Parameters.AddWithValue("@QuotationMasterID", quotationMasterID);

                cmd.ExecuteNonQuery();
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {

        }

        protected void gvItems_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvItems.EditIndex = e.NewEditIndex;
            BindInvoice();
        }

        protected void gvItems_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            DataTable dt = Session["GridData"] as DataTable;
            int id = Convert.ToInt32(gvItems.DataKeys[e.RowIndex].Value);

            GridViewRow row = gvItems.Rows[e.RowIndex];
            DropDownList ddlCOA = (DropDownList)row.FindControl("ddlCOA");

            //ddlCOA.ClearSelection();

            DataRow dr = dt.Select("QuotationDetailID=" + id)[0];
            dr["AccountCode"] = ddlCOA.SelectedItem.Text;

            Session["GridData"] = dt;

            gvItems.EditIndex = -1;
            BindInvoice();
        }

        protected void gvItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItems.EditIndex = -1;
            BindInvoice();
        }
        private CultureInfo GetCurrencyCulture(string currency)
        {
            switch (currency)
            {
                case "IDR":
                    return new CultureInfo("id-ID");
                case "USD":
                    return new CultureInfo("en-US");
                case "EUR":
                    return new CultureInfo("fr-FR");
                case "INR":
                default:
                    return new CultureInfo("en-IN");
            }
        }
        protected void gvItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField hdItemType = (HiddenField)e.Row.FindControl("hdItemType");
                HiddenField hdDepartmentID = (HiddenField)e.Row.FindControl("hdDepartmentID");
                HiddenField hdquotationdetailID = (HiddenField)e.Row.FindControl("hdquotationdetailID");
                Label lblAccountCode = (Label)e.Row.FindControl("lblAccountCode");
                Label lblAccountName = (Label)e.Row.FindControl("lblAccountName");

                if ((hdquotationdetailID!=null)&&(hdItemType != null)&& (hdDepartmentID != null))
                {
                    DataTable dt = _cm.GetAccountMapping(hdquotationdetailID.Value, hdDepartmentID.Value, hdItemType.Value);
                    if (dt.Rows.Count > 0)
                    {
                        lblAccountCode.Text = dt.Rows[0]["AccountCode"].ToString();
                        lblAccountName.Text = dt.Rows[0]["AccountName"].ToString();
                    }
                }
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                decimal discount = Discount;
                decimal downPayment = DownPayment;

                decimal taxable = Math.Max(0, subtotal - discount);
                decimal dpp = taxable * dppRate;
                decimal vat = taxable * vatRate/100;
                decimal grandTotal = dpp + vat;
                decimal balanceDue = Math.Max(0, grandTotal - downPayment);

                CultureInfo culture = GetCurrencyCulture(ddlCurrency.SelectedItem.ToString());
                culture.NumberFormat.CurrencySymbol = "";

                ((Label)e.Row.FindControl("lblSubtotal")).Text =
                    string.Format(culture, "{0:N0}", subtotal);

                ((TextBox)e.Row.FindControl("txtDiscount")).Text =
                    string.Format(culture, "{0:N0}", discount);

                ((Label)e.Row.FindControl("lblDPP")).Text =
                    string.Format(culture, "{0:N0}", dpp);

                ((Label)e.Row.FindControl("lblVat")).Text =
                  string.Format(culture, "{0:N0}", vat);

                ((Label)e.Row.FindControl("lblGrandTotal")).Text =
                    string.Format(culture, "{0:N0}", grandTotal);

                ((TextBox)e.Row.FindControl("txtDownPayment")).Text =
                    string.Format(culture, "{0:N0}", downPayment);

                ((Label)e.Row.FindControl("lblBalance")).Text =
                    string.Format(culture, "{0:N0}", balanceDue);
                // Hide first 5 footer cells
                for (int i = 0; i < 7; i++)
                {
                    e.Row.Cells[i].Visible = false;
                }

                // Footer starts in 6th cell
                e.Row.Cells[7].ColumnSpan = e.Row.Cells.Count - 1;
                e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                
                Session["Subtotal"] = subtotal;
                Session["Discount"] = discount;
                Session["DPP"] = dpp;
                Session["VAT"] = vat;
                Session["GrandTotal"] = grandTotal;
                Session["BalanceDue"] = balanceDue;
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblTotalAmount = (Label)e.Row.FindControl("lblTotalAmount");
                subtotal += Convert.ToDecimal(lblTotalAmount.Text);
            }

            if (e.Row.RowType == DataControlRowType.DataRow &&
                e.Row.RowState.HasFlag(DataControlRowState.Edit))
            {
                HiddenField hdAccountCode = (HiddenField)e.Row.FindControl("hdAccountCode");
                DropDownList ddlCOA = (DropDownList)e.Row.FindControl("ddlCOA");

                if (ddlCOA != null)
                {
                    ddlCOA.DataSource = _gl.GetCOA();
                    ddlCOA.DataTextField = "AccountName1";
                    ddlCOA.DataValueField = "AccountID";
                    ddlCOA.DataBind();
                    ddlCOA.Items.Insert(0, "Select Account");
                    if (hdAccountCode.Value != "")
                    {
                        ddlCOA.Items.FindByText(hdAccountCode.Value).Selected=true;
                    }
                }
            }
        }

        protected void ddlPaymentTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDates();
        }

        protected void txtInvoiceDate_TextChanged(object sender, EventArgs e)
        {
            BindDates();
        }

        protected void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            TextBox txtQuantity = (TextBox)sender;
            GridViewRow row = (GridViewRow)txtQuantity.NamingContainer;

            int rowIndex = row.RowIndex;

            // Example: get ID
            int id = Convert.ToInt32(gvItems.DataKeys[rowIndex].Value);

            int qty = int.Parse(txtQuantity.Text);
            Label lblPrice = (Label)row.FindControl("lblPrice");
            Label lblTotalAmount = (Label)row.FindControl("lblTotalAmount");

            decimal Price = Convert.ToDecimal(lblPrice.Text);
            decimal TotalAmount = qty * Price;
            lblTotalAmount.Text = TotalAmount.ToString();

            // 🔹 Update Session DataTable
            DataTable dt = Session["GridData"] as DataTable;
            dt.Rows[rowIndex]["Quantity"] = qty;
            dt.Rows[rowIndex]["Price"] = Price;
            dt.Rows[rowIndex]["TotalAmount"] = TotalAmount;
            Session["GridData"] = dt;

            BindInvoice();
        }

        protected void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            decimal discount;
            decimal.TryParse(txt.Text, out discount);

            Discount = discount;

            BindInvoice(); // 🔹 recalculates footer
        }
        protected void txtDownPayment_TextChanged(object sender, EventArgs e)
        {
            decimal.TryParse(((TextBox)sender).Text, out decimal val);
            DownPayment = val;
            BindInvoice();
        }
    }
}