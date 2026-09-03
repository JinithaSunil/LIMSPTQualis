using LIMS.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Math;

namespace LIMS
{
    public partial class AccountsReceivable : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.ARModule _ar = new DAL.ARModule();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCustomers();
                LoadInvoices();
            }
        }
        private void LoadCustomers()
        {
            var customers = ARModule.getARCustomers(radFilter.SelectedValue);

            ddlCustomers.DataSource = customers;
            ddlCustomers.DataTextField = "CompanyName";
            ddlCustomers.DataValueField = "CustomerMasterId";
            ddlCustomers.DataBind();
            ddlCustomers.Items.Insert(0, "Select Customer");
        }
        private void LoadInvoices()
        {
            if (radFilter.SelectedIndex != -1)
            {
                DataTable dt1 = ARModule.GetInvoices(radFilter.SelectedValue);
                gvInvoices.DataSource = dt1;
                gvInvoices.DataBind();
                ViewState["dirState"] = dt1;
                ViewState["sortdr"] = "Asc";
            }
        }

        public class InvoiceView
        {
            public int InvoiceId { get; set; }
            public string CustomerName { get; set; }
            public DateTime InvoiceDate { get; set; }
            public DateTime DueDate { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal AmountPaid { get; set; }
            public decimal Balance => TotalAmount - AmountPaid;
            public string Status { get; set; }
        }

        protected void ddlCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            getInvoiceList();
        }
        protected void getInvoiceList()
        {
            if (ddlCustomers.SelectedIndex != -1)
            {
                var invoices = ARModule.GetPendingInvoices(ddlCustomers.SelectedValue);
                if (invoices != null)
                {
                    chkInvoice.DataSource = invoices;
                    chkInvoice.DataTextField = "InvoiceNumber";
                    chkInvoice.DataValueField = "InvoiceID";
                    chkInvoice.DataBind();
                }
                else
                {
                    chkInvoice.DataSource = null;
                    chkInvoice.DataBind();
                }
            }
        }
        protected void btnAddInvoice_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (ddlCustomers.SelectedIndex == 0) { flag = 1; ddlCustomers.BackColor = Color.LightPink; } else { ddlCustomers.BackColor = Color.Empty; }
            if (chkInvoice.SelectedIndex == -1) { flag = 1; chkInvoice.BackColor = Color.LightPink; } else { chkInvoice.BackColor = Color.Empty; }
            if (flag == 0)
            {
                _ar.CustomerID = int.Parse(ddlCustomers.SelectedValue);
                _ar.CurrencyCode = ddlCurrency.SelectedValue;
                _ar.TotalAmount = decimal.Parse(txtAmount.Text);
                _ar.Status = "Pending";
                string value = "";
                string text = "";
                foreach (ListItem item in chkInvoice.Items)
                {
                    if (item.Selected)
                    {
                        text += item.Text;     // display text
                        value += item.Value +",";   // value

                    }
                }
                if (value.Length > 0)
                {
                    value = value.Remove(value.Length - 1);
                }
                _ar.InvoiceID = value;
                _ar.AmountPaid = 0;
                _ar.InvoiceARID = _ar.AddInvoice(_ar);
                
                Clear();
            }
            LoadInvoices();
        }
        protected void Clear()
        {
            ddlCustomers.ClearSelection();
            chkInvoice.ClearSelection();
            txtAmount.Text = "";
        }
        protected void radFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadInvoices();
        }

        protected void gvInvoices_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string InvoiceARID = "", customerID = "";

            if (e.CommandName == "Payments")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvInvoices.Rows[rowIndex];
                InvoiceARID = gvInvoices.DataKeys[rowIndex].Value.ToString();
                Label lblInvoiceNumber = (Label)row.FindControl("lblInvoiceNumber");
                LinkButton lnkPayment = (LinkButton)row.FindControl("lnkPayment");

                var Inv = _ar.GetARInvoiceDetails(Convert.ToInt32(InvoiceARID));
                if (Inv != null)
                {
                    _ar.arVar.InvoiceARID = InvoiceARID;
                    _ar.arVar.customerID = Inv.Rows[0]["customerID"].ToString();
                    _ar.arVar.InvoiceID = Inv.Rows[0]["InvoiceID"].ToString();
                    _ar.arVar.InvoiceNumber = lblInvoiceNumber.Text;
                    _ar.arVar.PayeeType = "Customer";
                    ddlCustomers.SelectedValue = _ar.arVar.customerID;
                    string s = ddlCustomers.SelectedItem.ToString();
                    _ar.arVar.PayerName = s;
                    _ar.arVar.InvoiceAmount = Convert.ToDecimal(Inv.Rows[0]["TotalAmount"].ToString());
                    _ar.arVar.AmountPaid = Convert.ToDecimal(lnkPayment.Text);
                    _ar.arVar.Reference = "Accounts Receivable — " + _ar.arVar.PayerName + " - " + _ar.arVar.InvoiceNumber;
                    Session["ARVariables"] = _ar.arVar;
                    //Response.Redirect("Receipts.aspx");
                    string url = "Receipts.aspx";
                    Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                }
            }
        }

        protected void gvInvoices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblInvoiceNumber = (e.Row.FindControl("lblInvoiceNumber") as Label);
                LinkButton lnkPayment = (e.Row.FindControl("lnkPayment") as LinkButton);
                int InvoiceARId = (int)gvInvoices.DataKeys[e.Row.RowIndex].Values[0];
                DataTable dtInvoices = new DataTable();
                if (radFilter.SelectedValue.ToString() != "3")
                {
                    dtInvoices = _ar.GetARInvoices(InvoiceARId);
                    if (dtInvoices.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtInvoices.Rows)
                        {
                            lblInvoiceNumber.Text += dr["InvoiceNumber"].ToString() + ",";
                        }
                        lblInvoiceNumber.Text = lblInvoiceNumber.Text.Substring(0, lblInvoiceNumber.Text.Length - 1);
                    }
                    DataTable dtPaid = _ar.GetARReceipts(InvoiceARId);

                    if (dtPaid.Rows.Count > 0)
                    {
                        lnkPayment.Text = dtPaid.Rows[0]["TotalReceipt"].ToString();
                    }
                }
                else
                {
                    dtInvoices = _ar.GetInvoiceDetails(InvoiceARId);
                    if (dtInvoices.Rows.Count > 0)
                    {
                        decimal AmountPaid = 0;
                        foreach (DataRow dr in dtInvoices.Rows)
                        {
                            lblInvoiceNumber.Text += dr["InvoiceNumber"].ToString() + ",";
                            AmountPaid += Convert.ToDecimal(dr["AmountPaid"]);
                        }
                        lblInvoiceNumber.Text = lblInvoiceNumber.Text.Substring(0, lblInvoiceNumber.Text.Length - 1);
                        hdAmountPaid.Value = AmountPaid.ToString();
                    }
                    lnkPayment.Visible = false;
                }
               
            }
        }

        protected void btnReceipt_Click(object sender, EventArgs e)
        {
            List<string> selectedIDs = new List<string>();
            string InvoiceID = "";
            foreach (GridViewRow row in gvInvoices.Rows)
            {

                Label lblInvoiceID = row.FindControl("lblInvoiceID") as Label;
                Label lblCustomerID = row.FindControl("lblCustomerID") as Label;
                Label lblInvoiceNumber = row.FindControl("lblInvoiceNumber") as Label;
                Label lblTotalAmount = row.FindControl("lblTotalAmount") as Label;
                Label lblInvoiceDate = row.FindControl("lblInvoiceDate") as Label;
                LinkButton lnkPayment = (LinkButton)row.FindControl("lnkPayment");

                _ar.arVar.customerID = lblCustomerID.Text;
                Session["Invoices"] = lblInvoiceID.Text + ",";
                _ar.arVar.InvoiceNumber = lblInvoiceNumber.Text + ",";
                _ar.arVar.PayeeType = "Customer";
                ddlCustomers.Items.FindByValue(lblCustomerID.Text).Selected = true;
                string s = ddlCustomers.SelectedItem.ToString();
                _ar.arVar.PayerName = s;
                _ar.arVar.InvoiceAmount += Convert.ToDecimal(lblTotalAmount.Text);
                _ar.arVar.AmountPaid = Convert.ToDecimal(lnkPayment.Text);

            }

            Session["ARVariables"] = _ar.arVar;
            Response.Redirect("Receipts.aspx");
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

        protected void chkInvoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            decimal totalAmount = 0; string InvoiceNo = "";decimal AmountPaid = 0;
            foreach (ListItem item in chkInvoice.Items)
            {
                if (item.Selected)
                {
                    string text = item.Text;     // display text
                    string value = item.Value;   // value
                    DataTable dt = _ar.GetInvoiceDetails(Convert.ToInt32(value));
                    if (dt.Rows.Count > 0)
                    {
                        InvoiceNo += text + ",";
                        totalAmount += Convert.ToDecimal(dt.Rows[0]["TotalAmount"].ToString());
                        AmountPaid += Convert.ToDecimal(dt.Rows[0]["AmountPAid"].ToString());
                    }
                }
            }
            if (InvoiceNo.Length > 0)
            {
                // Assign the new string back to the variable
                InvoiceNo = InvoiceNo.Remove(InvoiceNo.Length - 1);
            }
            hdInvoiceNumber.Value = InvoiceNo;
            hdAmountPaid.Value = AmountPaid.ToString();
            txtAmount.Text = totalAmount.ToString();
        }
    }
}