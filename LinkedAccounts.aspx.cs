using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class LinkedAccounts : System.Web.UI.Page
    {
        DAL.GLModule _gl = new DAL.GLModule();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindAccounts();
                BindValues();
            }
        }
        protected void BindAccounts()
        {
            DataTable dt = _gl.GetCOA("Accounts Receivable");
            if (dt.Rows.Count > 0)
            {
                ddlSalesReceivable.DataSource = dt;
                ddlSalesReceivable.DataTextField = "AccCode";
                ddlSalesReceivable.DataValueField = "AccountCode";
                ddlSalesReceivable.DataBind();
                ddlSalesReceivable.Items.Insert(0, "Select Dr. A/C");
            }
            else
            {
                ddlSalesReceivable.DataSource = null;
                ddlSalesReceivable.DataBind();
            }
            dt = _gl.GetCOA("Accounts Payable");
            if (dt.Rows.Count > 0)
            {
                ddlPurchasePayable.DataSource = dt;
                ddlPurchasePayable.DataTextField = "AccCode";
                ddlPurchasePayable.DataValueField = "AccountCode";
                ddlPurchasePayable.DataBind();
                ddlPurchasePayable.Items.Insert(0, "Select Cr. A/C");

                ddlItemReceipt.DataSource = dt;
                ddlItemReceipt.DataTextField = "AccCode";
                ddlItemReceipt.DataValueField = "AccountCode";
                ddlItemReceipt.DataBind();
                ddlItemReceipt.Items.Insert(0, "Select Liability Cr. A/C");
            }
            else
            {
                ddlPurchasePayable.DataSource = null;
                ddlPurchasePayable.DataBind();

                ddlItemReceipt.DataSource = null;
                ddlItemReceipt.DataBind();
            }
            dt = _gl.GetCOA("Bank");
            if (dt.Rows.Count > 0)
            {
                ddlSalesBank.DataSource = dt;
                ddlSalesBank.DataTextField = "AccCode";
                ddlSalesBank.DataValueField = "AccountCode";
                ddlSalesBank.DataBind();
                ddlSalesBank.Items.Insert(0, "Select Sales Bank A/C");

                ddlPurchaseBank.DataSource = dt;
                ddlPurchaseBank.DataTextField = "AccCode";
                ddlPurchaseBank.DataValueField = "AccountCode";
                ddlPurchaseBank.DataBind();
                ddlPurchaseBank.Items.Insert(0, "Select Payment Bank A/C");

                ddlSupplierDeposit.DataSource = dt;
                ddlSupplierDeposit.DataTextField = "AccCode";
                ddlSupplierDeposit.DataValueField = "AccountCode";
                ddlSupplierDeposit.DataBind();
                ddlSupplierDeposit.Items.Insert(0, "Select Supplier Deposit Bank A/C");

            }
            else
            {
                ddlSalesBank.DataSource = null;
                ddlSalesBank.DataBind();

                ddlPurchaseBank.DataSource = null;
                ddlPurchaseBank.DataBind();

                ddlSupplierDeposit.DataSource = null;
                ddlSupplierDeposit.DataBind();

            }
            dt = _gl.GetCOA("Credit Card");
            if (dt.Rows.Count > 0)
            {
                ddlCustomerDeposit.DataSource = dt;
                ddlCustomerDeposit.DataTextField = "AccCode";
                ddlCustomerDeposit.DataValueField = "AccountCode";
                ddlCustomerDeposit.DataBind();
                ddlCustomerDeposit.Items.Insert(0, "Select Customer Deposit Bank A/C");
            }
            else
            {
                ddlCustomerDeposit.DataSource = null;
                ddlCustomerDeposit.DataBind();
            }
        }

        protected void BindValues()
        {
            DataTable dt = _gl.GetLinkedAccounts("Sales");
            if (dt.Rows.Count > 0)
            {
                ddlSalesReceivable.Items.FindByText(dt.Rows[0]["ReceivableAccountID"].ToString()).Selected = true;
                ddlSalesBank.Items.FindByText(dt.Rows[0]["BankAccountID"].ToString()).Selected = true;
                ddlCustomerDeposit.Items.FindByText(dt.Rows[0]["DepositAccountID"].ToString()).Selected = true;

                if (Convert.ToBoolean(dt.Rows[0]["TrackDeposits"])) { chkSalesDeposit.Checked = true; } else { chkSalesFreight.Checked = false; }
                if (Convert.ToBoolean(dt.Rows[0]["ChargeFreight"])) { chkSalesFreight.Checked = true; } else { chkSalesFreight.Checked = false; }
                if (Convert.ToBoolean(dt.Rows[0]["EarlyPaymentDiscount"])) { chkSalesDiscount.Checked = true; } else { chkSalesFreight.Checked = false; }
                if (Convert.ToBoolean(dt.Rows[0]["LatePaymentCharges"])) { chkSalesLateFee.Checked = true; } else { chkSalesFreight.Checked = false; }
            }
            dt = _gl.GetLinkedAccounts("Purchase");
            if (dt.Rows.Count > 0)
            {
                ddlPurchasePayable.Items.FindByText(dt.Rows[0]["PayableAccountID"].ToString()).Selected = true;
                ddlPurchaseBank.Items.FindByText(dt.Rows[0]["BankAccountID"].ToString()).Selected = true;
                ddlItemReceipt.Items.FindByText(dt.Rows[0]["PayableAccountID"].ToString()).Selected = true;
                ddlSupplierDeposit.Items.FindByText(dt.Rows[0]["DepositAccountID"].ToString()).Selected = true;

                if (Convert.ToBoolean(dt.Rows[0]["TrackDeposits"])) { chkItemReceipt.Checked = true; } else { chkSalesFreight.Checked = false; }
                if (Convert.ToBoolean(dt.Rows[0]["ChargeFreight"])) { chkPurchaseFreight.Checked = true; } else { chkSalesFreight.Checked = false; }
                if (Convert.ToBoolean(dt.Rows[0]["EarlyPaymentDiscount"])) { chkPurchaseDiscount.Checked = true; } else { chkSalesFreight.Checked = false; }
                if (Convert.ToBoolean(dt.Rows[0]["LatePaymentCharges"])) { chkPurchaseLateFee.Checked = true; } else { chkSalesFreight.Checked = false; }
            }
        }
        protected void chkSalesDeposit_CheckedChanged(object sender, EventArgs e)
        {
            ddlCustomerDeposit.Enabled = chkSalesDeposit.Checked;
        }

        protected void chkItemReceipt_CheckedChanged(object sender, EventArgs e)
        {
            ddlItemReceipt.Enabled = chkItemReceipt.Checked;
        }

        protected void chkSupplierDeposit_CheckedChanged(object sender, EventArgs e)
        {
            ddlSupplierDeposit.Enabled = chkSupplierDeposit.Checked;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            DAL.GLModule.LinkedAccount LA = new DAL.GLModule.LinkedAccount();
            LA.Module = "Sales";
            LA.ReceivableAccountId = ddlSalesReceivable.SelectedItem.ToString();
            LA.PayableAccountId = "";
            LA.BankAccountId = ddlSalesBank.SelectedItem.ToString();
            LA.DepositAccountId = ddlCustomerDeposit.SelectedItem.ToString();
            LA.ItemReceipt = 0;
            if (chkSalesDeposit.Checked) { LA.TrackDeposits = 1; } else { LA.TrackDeposits = 0; }
            if (chkSalesFreight.Checked) { LA.ChargeFreight = 1; } else { LA.ChargeFreight = 0; }
            if (chkSalesDiscount.Checked) { LA.EarlyPaymentDiscount = 1; } else { LA.EarlyPaymentDiscount = 0; }
            if (chkSalesLateFee.Checked) { LA.LatePaymentCharges = 1; } else { LA.LatePaymentCharges = 0; }
            _gl.UpdateLinkedAccount(LA);

            LA.Module = "Purchase";
            LA.ReceivableAccountId = "";
            LA.PayableAccountId = ddlPurchasePayable.SelectedItem.ToString();
            LA.BankAccountId = ddlPurchaseBank.SelectedItem.ToString();
            LA.DepositAccountId = ddlSupplierDeposit.SelectedItem.ToString();
            if (chkItemReceipt.Checked) { LA.ItemReceipt = 1; } else { LA.ItemReceipt = 0; }
            if (chkSupplierDeposit.Checked) { LA.TrackDeposits = 1; } else { LA.TrackDeposits = 0; }
            if (chkPurchaseFreight.Checked) { LA.ChargeFreight = 1; } else { LA.ChargeFreight = 0; }
            if (chkPurchaseDiscount.Checked) { LA.EarlyPaymentDiscount = 1; } else { LA.EarlyPaymentDiscount = 0; }
            if (chkPurchaseDiscount.Checked) { LA.LatePaymentCharges = 1; } else { LA.LatePaymentCharges = 0; }
            _gl.UpdateLinkedAccount(LA);

            BindValues();
        }

    }
}