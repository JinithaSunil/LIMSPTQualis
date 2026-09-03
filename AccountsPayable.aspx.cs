using LIMS.DAL;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class AP : System.Web.UI.Page
    {
        DAL.APModule _ap = new DAL.APModule();
        DAL.CM _cm = new DAL.CM();
        decimal grandTotal = 0;
        decimal subTotal = 0;
        decimal ppnRate = 0.11m; // 11% PPN (change if needed)
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadVendors();
                LoadInvoices();
                BindCurrency();
                Populate();
            }
        }
        private void Populate()
        {
            DataTable dt = _cm.getPaymentTerms();
            if (dt.Rows.Count > 0)
            {
                ddlPaymentTerms.DataSource = dt;
                ddlPaymentTerms.DataTextField = "PaymentTermName";
                ddlPaymentTerms.DataValueField = "PaymentTermID";
                ddlPaymentTerms.DataBind();
                ddlPaymentTerms.Items.Insert(0, "--Select Payment Terms--");
            }
        }
        private void BindCurrency()
        {
            ddlCurrency.DataSource = _cm.getCurrency();
            ddlCurrency.DataTextField = "CurrencyCode";
            ddlCurrency.DataValueField = "CurrencyID"; // or CurrencyCode
            ddlCurrency.DataBind();

            ddlCurrency.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void LoadVendors()
        {
            var vendors = _cm.getPOVendor();

            ddlVendors.DataSource = vendors;
            ddlVendors.DataTextField = "VendorName";
            ddlVendors.DataValueField = "VendorId";
            ddlVendors.DataBind();
            ddlVendors.Items.Insert(0, "Select Vendor");
        }

        private void LoadInvoices()
        {
            if (radFilter.SelectedIndex != -1)
            {
                DataTable dt1 = APModule.GetInvoices(radFilter.SelectedValue);
                gvInvoices.DataSource = dt1;
                gvInvoices.DataBind();
                ViewState["dirState"] = dt1;
                ViewState["sortdr"] = "Asc";
            }
        }

        protected void btnAddInvoice_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (gvGRN.Rows.Count == 0)
            {
                // No rows in GridView
                ScriptManager.RegisterStartupScript(
                    this, GetType(), "msg",
                    "alert('No items found. Please add at least one row.');",
                    true);

                return;
            }
            else
            {
                if (txtVendorTaxInvoiceNumber.Text == "") { flag = 1; txtVendorTaxInvoiceNumber.BackColor = Color.LightPink; } else { txtVendorTaxInvoiceNumber.BackColor = Color.Empty; }
                if (txtInvoiceNumber.Text == "") { flag = 1; txtInvoiceNumber.BackColor = Color.LightPink; } else { txtInvoiceNumber.BackColor = Color.Empty; }
                if (txtInvoiceDate.Text == "") { flag = 1; txtInvoiceDate.BackColor = Color.LightPink; } else { txtInvoiceDate.BackColor = Color.Empty; }
                if (txtDueDate.Text == "") { flag = 1; txtDueDate.BackColor = Color.LightPink; } else { txtDueDate.BackColor = Color.Empty; }
                //Only GR Total to be considered//if (txtAmount.Text == "") { flag = 1; txtAmount.BackColor = Color.LightPink; } else { txtAmount.BackColor = Color.Empty; }
                if (ddlPO.SelectedIndex == 0) { flag = 1; ddlPO.BackColor = Color.LightPink; } else { ddlPO.BackColor = Color.Empty; }
                if (ddlCurrency.SelectedIndex == 0) { flag = 1; ddlCurrency.BackColor = Color.LightPink; } else { ddlCurrency.BackColor = Color.Empty; }
                if (flag == 0)
                {
                    _ap.VendorID = int.Parse(ddlVendors.SelectedValue);
                    _ap.VendorTaxInvoiceNumber = txtVendorTaxInvoiceNumber.Text;
                    _ap.InvoiceNumber = txtInvoiceNumber.Text;
                    _ap.InvoiceDate = DateTime.Parse(txtInvoiceDate.Text);
                    _ap.DueDate = DateTime.Parse(txtDueDate.Text);
                    _ap.CurrencyCode = ddlCurrency.SelectedItem.ToString();
                    _ap.TotalAmount = decimal.Parse(hdTotal.Value); //Only GR Total to be considered. decimal.Parse(txtAmount.Text);
                    _ap.Status = "Pending";
                    _ap.PurchaseOrderID = int.Parse(ddlPO.SelectedValue);
                    _ap.AmountPaid = 0;
                    _ap.Year = _ap.InvoiceDate.ToString("yyyy");
                    hdGRNID.Value = hdGRNID.Value?.TrimEnd(',');
                    _ap.GRNIDs = hdGRNID.Value.ToString();
                    DataTable dtLast = _ap.GetLast("InvoiceAP", "PIYear", _ap.Year);
                    if (dtLast.Rows.Count == 0) { _ap.Counter = 1; }
                    else
                    {
                        _ap.Counter = dtLast.Rows.Count + 1;
                    }
                    _ap.Number = _ap.Counter.ToString("D4") + "-" + _ap.PaymentDate.ToString("MM") + "-" + _ap.Year + "/PQI-PI";
                    int InvoiceID = _ap.AddInvoice(_ap);
                    Clear();
                    LoadInvoices();
                    //string url = $"generatePurchaseInvoice.aspx?i=" + InvoiceID;
                    //Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");

                    string url = "generatePurchaseInvoice.aspx?i=" + InvoiceID;

                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "OpenInvoice",
                        $"window.open('{url}', '_blank');",
                        true
                    );
                    
                }
            }


        }
        protected void Clear()
        {
            ddlVendors.ClearSelection();
            ddlPO.ClearSelection();

            txtAmount.Text = "";
            txtVendorTaxInvoiceNumber.Text = "";
            txtInvoiceNumber.Text = "";
            txtInvoiceDate.Text = "";
            txtDueDate.Text = "";

            Session.Remove("DT");
            gvGRN.DataSource = null;
            gvGRN.DataBind();
        }
        protected void getPOList(string VendorID)
        {
            var invoices = APModule.GetPendingPurchaseInvoices(VendorID);
            if (invoices != null)
            {
                ddlPO.DataSource = invoices;
                ddlPO.DataTextField = "PONumber";
                ddlPO.DataValueField = "PurchaseOrderID";
                ddlPO.DataBind();
                ddlPO.Items.Insert(0, "Select PO");
            }
            else
            {
                ddlPO.DataSource = null;
                ddlPO.DataBind();
            }
        }
        protected void ddlPO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPO.SelectedIndex != 0)
            {
                DataTable dtGRN = _cm.getPOGRN(ddlPO.SelectedValue);
                if (dtGRN.Rows.Count > 0)
                {
                    ddlGRN.DataSource = dtGRN;
                    ddlGRN.DataTextField = "GRNNumber";
                    ddlGRN.DataValueField = "InwardID";
                    ddlGRN.DataBind();
                    ddlGRN.Items.Insert(0, "Select GRN");
                }
                else
                {
                    ddlGRN.DataSource = null;
                    ddlGRN.Items.Clear();
                    ddlGRN.DataBind();
                }
                    DataTable dtCurrency = _cm.getPOCurrency(ddlPO.SelectedValue);
                if (dtCurrency.Rows.Count > 0)
                {
                    ddlCurrency.ClearSelection();
                    ddlCurrency.Items.FindByText(dtCurrency.Rows[0]["CurrencyCode"].ToString()).Selected = true;
                    ddlCurrency.Enabled = false;
                }
                var PO = APModule.GetPO(ddlPO.SelectedValue);
                if (PO != null)
                {
                    ListItem item = ddlPaymentTerms.Items.FindByText(PO.Rows[0]["Payment"].ToString());

                    if (item != null)
                    {
                        ddlPaymentTerms.ClearSelection();
                        item.Selected = true;
                        ddlPaymentTerms.Enabled = false;
                        txtDueDate.Text = Convert.ToDateTime(PO.Rows[0]["DueDate"]).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        ddlPaymentTerms.SelectedIndex = 0; // default item
                    }
                    txtAmount.Text = PO.Rows[0]["GrandTotal"].ToString();
                }

            }
        }

        protected void gvInvoices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string POID = "", vendorID = "", PurchaseOrderID = "";
            if (e.CommandName == "SelectPO")
            {
                // Retrieve the CommandArgument to identify the clicked row
                POID = e.CommandArgument.ToString();
                var PO = APModule.GetPO(POID);
                if (PO != null)
                {
                    vendorID = PO.Rows[0]["VendorID"].ToString();
                    PurchaseOrderID = PO.Rows[0]["PurchaseOrderID"].ToString();
                    string s = vendorID;
                    txtAmount.Text = PO.Rows[0]["GrandTotal"].ToString();
                }
            }
            if (e.CommandName == "Payments")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                POID = gvInvoices.DataKeys[rowIndex].Value.ToString();

                GridViewRow row = gvInvoices.Rows[rowIndex];
                LinkButton lnkPayment = (LinkButton)row.FindControl("lnkPayment");

                var PO = APModule.GetPO(POID);
                if (PO != null)
                {
                    _ap.apVar.vendorID = vendorID;
                    _ap.apVar.PurchaseOrderID = PO.Rows[0]["PurchaseOrderID"].ToString();
                    _ap.apVar.PONumber = PO.Rows[0]["PONumber"].ToString();
                    _ap.apVar.SupplierInvoiceNo = PO.Rows[0]["InvoiceNumber"].ToString();
                    _ap.apVar.PayeeType = "Vendor";
                    vendorID = PO.Rows[0]["VendorID"].ToString();
                    string s = PO.Rows[0]["VendorName"].ToString();
                    _ap.apVar.PayeeName = s;
                    _ap.apVar.PODate = Convert.ToDateTime(PO.Rows[0]["PODate"].ToString());
                    _ap.apVar.POAmount = Convert.ToDecimal(PO.Rows[0]["GrandTotal"].ToString());
                    _ap.apVar.AmountPaid = Convert.ToDecimal(lnkPayment.Text);
                    Session["APVariables"] = _ap.apVar;
                    //Response.Redirect("Payments.aspx");
                    string url = "Payments.aspx";
                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "OpenNewTab",
                        $"window.open('{url}', '_blank');",
                        true
                    );
                }
            }
        }

        protected void gvInvoices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkPayment = (e.Row.FindControl("lnkPayment") as LinkButton);
                int PurchaseOrderID = (int)gvInvoices.DataKeys[e.Row.RowIndex].Values[0];
                DataTable dtPaid = _ap.GetPayments(PurchaseOrderID);
                if (dtPaid.Rows.Count > 0)
                {
                    lnkPayment.Text = dtPaid.Rows[0]["TotalPayments"].ToString();
                }
            }
        }

        protected void gvInvoices_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                gvInvoices.DataSource = dtrslt;
                gvInvoices.DataBind();


            }
        }

        protected void radFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadInvoices();
        }
        public string ToUSD(decimal amount)
        {
            // USD
            CultureInfo usdCulture = new CultureInfo("en-US");
            //return string.Format(usdCulture, "{0:C}", amount);// do not show curreny sybmol - aldiyan 2Feb26
            return amount.ToString("N2", usdCulture);
        }
        public string ToIDR(decimal amount)
        {
            // IDR
            CultureInfo rupiahCulture = new CultureInfo("id-ID");
            //return string.Format(rupiahCulture, "{0:C}", amount); // do not show curreny sybmol - aldiyan 2Feb26
            return amount.ToString("N2", rupiahCulture);
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (ddlGRN.SelectedIndex != 0)
            {
                dt = _cm.GetGRNDetailsForPI(Convert.ToInt32(ddlGRN.SelectedValue));
                DataTable dtSession;

                if (Session["DT"] == null)
                {
                    Session["DT"] = dt;
                }
                else
                {
                    dtSession = (DataTable)Session["DT"];
                    bool grnExists = dtSession.AsEnumerable()
                       .Any(r => r.Field<string>("GRNNumber") == ddlGRN.SelectedItem.ToString());

                    if (!grnExists)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            dtSession.ImportRow(row);
                        }

                        Session["DT"] = dtSession;
                    }
                }
                dt = (DataTable)Session["DT"];
                if (dt.Rows.Count > 0)
                {
                    gvGRN.DataSource = dt;
                    gvGRN.DataBind();
                }
                else
                {
                    gvGRN.DataSource = null;
                    gvGRN.DataBind();
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Session.Remove("DT");
            gvGRN.DataSource = null;
            gvGRN.DataBind();
        }

        protected void gvGRN_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                decimal amount = DataBinder.Eval(e.Row.DataItem, "Amount") == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Amount"));

                subTotal += amount;
                // Get DataKey value
                string id = gvGRN.DataKeys[e.Row.RowIndex].Value.ToString();

                // Append to HiddenField
                if (string.IsNullOrEmpty(hdGRNID.Value))
                {
                    hdGRNID.Value = id;
                }
                else
                {
                    // split existing values
                    var ids = hdGRNID.Value.Split(',');

                    // add only if not already present
                    if (!ids.Contains(id))
                    {
                        hdGRNID.Value += "," + id;
                    }
                }
            }
            // Footer formatting
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                decimal ppn = Math.Round(subTotal * ppnRate, 2);
                decimal total = subTotal + ppn;
                hdTotal.Value = total.ToString();

                // --- MERGE columns 0 to 6 (up to Price-1) ---
                e.Row.Cells[0].ColumnSpan = 8;
                e.Row.Cells[0].Text =
                    "<div style='text-align:right'>Sub Total<br/>PPN<br/><b>Total</b></div>";

                // Hide merged cells
                for (int i = 1; i <= 7; i++)
                {
                    e.Row.Cells[i].Visible = false;
                }

                // Amount column
                e.Row.Cells[8].Text =
                    $"Rp {ToIDR(subTotal)}<br/>" +
                    $"Rp {ToIDR(ppn)}<br/>" +
                    $"<b>Rp {ToIDR(total)}</b>";

                e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Right;
            }
        }

        protected void ddlVendors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((ddlVendors.SelectedIndex != 0)&& (ddlVendors.SelectedIndex != -1))
            {
                getPOList(ddlVendors.SelectedValue);
            }            
        }
    }
}