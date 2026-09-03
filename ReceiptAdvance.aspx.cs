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
using ClosedXML.Excel;
using static LIMS.DAL.GLModule;
using static LIMS.DAL.ARModule;

namespace LIMS
{
    public partial class ReceiptAdvance : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.GLModule _gl = new GLModule();
        DAL.APModule _ap = new DAL.APModule();
        DAL.ARModule _ar = new DAL.ARModule();
        DAL.Journal _jl = new DAL.Journal();
        DAL.BankingModule _bk = new DAL.BankingModule();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadCustomers();
                LoadAccounts();
            }
        }
        private void LoadAccounts()
        {
            DataTable dt = _gl.GetCOA("Credit Card");
            ddlAccount.DataSource = dt;
            ddlAccount.DataTextField = "AccCode";
            ddlAccount.DataValueField = "AccountCode";
            ddlAccount.DataBind();

            foreach (ListItem item in ddlAccount.Items)
            {
                DataRow row = dt.AsEnumerable()
                                .FirstOrDefault(r => r.Field<string>("AccountCode") == item.Value);

                if (row != null)
                {
                    item.Attributes["AccountID"] = row["AccountID"].ToString();
                }
            }
            dt = _gl.GetLinkedAccounts("Sales");
            if (dt.Rows.Count > 0)
            {
                ddlAccount.Items.FindByText(dt.Rows[0]["DepositAccountID"].ToString()).Selected = true;
                BindBankAccount();
            }
            ddlAccount.Items.Insert(0, "Select Sales Bank A/C");
        }
        private void LoadCustomers()
        {
            var customers = ARModule.getPendingARCustomers();

            ddlCustomers.DataSource = customers;
            ddlCustomers.DataTextField = "CompanyName";
            ddlCustomers.DataValueField = "CustomerMasterId";
            ddlCustomers.DataBind();
            ddlCustomers.Items.Insert(0, "Select Customer");
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
            getQuotationList();
        }
        protected void getQuotationList()
        {
            if (ddlCustomers.SelectedIndex != -1)
            {
                var quotations = ARModule.GetPendingQuotations(ddlCustomers.SelectedValue);
                if (quotations != null)
                {
                    ddlQuotation.DataSource = quotations;
                    ddlQuotation.DataTextField = "QuotationNumber";
                    ddlQuotation.DataValueField = "QuotationMasterID";
                    ddlQuotation.DataBind();
                    ddlQuotation.Items.Insert(0, "Select Quotation");
                }
                else
                {
                    ddlQuotation.DataSource = null;
                    ddlQuotation.DataBind();
                }
            }
        }
        protected void Clear()
        {
            ddlCustomers.ClearSelection();
            ddlQuotation.ClearSelection();
            txtReference.Text = "";
            txtAmount.Text = "";
            txtAdvanceAmount.Text = "";
        }
        protected void ddlQuotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            decimal totalAmount = 0; decimal AmountPaid = 0;

            if (ddlQuotation.SelectedIndex != 0)
            {
                DataTable dt = _ar.GetQuotationDetails(Convert.ToInt32(ddlQuotation.SelectedValue));
                if (dt.Rows.Count > 0)
                {
                    totalAmount = Convert.ToDecimal(dt.Rows[0]["GrandTotal"].ToString());
                    _ar.arVar.PayerName = ddlCustomers.SelectedItem.ToString();
                    _ar.arVar.QuotationNumber = ddlQuotation.SelectedItem.ToString();
                    txtReference.Text = "Advance Received — " + _ar.arVar.PayerName + " - " + _ar.arVar.QuotationNumber;
                }
            }

            //hdInvoiceNumber.Value = InvoiceNo;
            hdAmountPaid.Value = AmountPaid.ToString();
            txtAmount.Text = totalAmount.ToString();
            txtAdvanceAmount.Text= totalAmount.ToString();
        }
        protected void radPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radPaymentMode.SelectedIndex != -1)
            {
                if (radPaymentMode.SelectedItem.ToString() == "CHEQUE")
                {
                    tblCheque.Visible = true;
                }
                else
                {
                    tblCheque.Visible = false;
                }
                if (radPaymentMode.SelectedItem.ToString() == "BANK")
                {
                    tblBank.Visible = true;
                }
                else
                {
                    tblBank.Visible = false;
                }
            }
        }


        protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindBankAccount();
        }
        protected void BindBankAccount()
        {
            if (ddlAccount.SelectedIndex != 0)
            {
                hdAccountCode.Value = ddlAccount.SelectedValue;
                DataTable dtBank = _ar.GetBankFromCode(hdAccountCode.Value);
                if (dtBank.Rows.Count > 0)
                {
                    hdAccountID.Value = dtBank.Rows[0]["BankAccountID"].ToString();
                }
            }
        }
        protected void btnSaveAdvance_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (ddlCustomers.SelectedIndex == 0) { flag = 1; ddlCustomers.BackColor = Color.LightPink; } else { ddlCustomers.BackColor = Color.Empty; }
            if (ddlQuotation.SelectedIndex == 0) { flag = 1; ddlQuotation.BackColor = Color.LightPink; } else { ddlQuotation.BackColor = Color.Empty; }
            if (ddlAccount.SelectedIndex == 0) { flag = 1; ddlAccount.BackColor = Color.LightPink; } else { ddlAccount.BackColor = Color.Empty; }
            if (txtAmount.Text == "") { flag = 1; txtAmount.BorderColor = Color.Red; } else { txtAmount.BorderColor = Color.Empty; }
            if (flag == 0)
            {
                hdAccountCode.Value = ddlAccount.SelectedValue;
                DataTable dtBank = _ar.GetBankFromCode(hdAccountCode.Value);
                if (dtBank.Rows.Count > 0)
                {
                    hdAccountID.Value = dtBank.Rows[0]["BankAccountID"].ToString();
                }
                int customerId = int.Parse(ddlCustomers.SelectedValue);
                int quotationId = ddlQuotation.SelectedIndex > 0
                                    ? int.Parse(ddlQuotation.SelectedValue)
                                    : 0;

                decimal amount = decimal.Parse(txtAmount.Text);

                int bankAccountId = Convert.ToInt32(hdAccountID.Value);

                // 1. Insert Advance Record

                _ar.CustomerID = customerId;
                _ar.QuotationID = quotationId.ToString();
                _ar.TotalAmount = amount;
                int AdvanceID = _gl.InsertCustomerAdvance(_ar);

                // 2. Insert Receipt
                _ar.ReceiptType = "ADVANCE";
                _ar.InvoiceARID = "";
                _ar.PayerId = Convert.ToInt32(ddlCustomers.SelectedValue);
                _ar.ReceiptDate = Convert.ToDateTime(txtDate.Text);
                _ar.ReceiptAmount = Convert.ToDecimal(txtAdvanceAmount.Text);
                _ar.PaymentMethod = radPaymentMode.SelectedItem.ToString();
                _ar.BankAccountCode = ddlAccount.SelectedValue;
                _ar.ReferenceNo = txtReference.Text;
                _ar.Notes = txtNotes.Text;
                _ar.CreatedOn = DateTime.Now;
                _ar.PayerType = "Customer";
                _ar.PayerName = ddlCustomers.SelectedItem.ToString();
                if (hdInvoiceID.Value == "") { _ar.InvoiceID = "0"; }
                else { _ar.InvoiceID = hdInvoiceID.Value; }
                _ar.Year = _ar.ReceiptDate.ToString("yyyy");
                DataTable dtLast = _ar.GetLast("Receipts", "VoucherYear", _ar.Year);
                if (dtLast.Rows.Count == 0) { _ar.Counter = 1; }
                else
                {
                    _ar.Counter = dtLast.Rows.Count + 1;
                }
                _ar.Number = _ar.Counter.ToString("D4") + "-" + _ar.ReceiptDate.ToString("MM") + "-" + _ar.Year + "/" + _ar.Number;
                List<AdvancePayment> quotation = new List<AdvancePayment>();
                quotation = _ar.GetQuotationDetails(ddlQuotation.SelectedValue);
                _ar.ReceiptID = _ar.AddAdvanceReceipt(_ar, quotation);

                // 3. Create Voucher                
                DAL.GLModule.AccountActivityLog log = new GLModule.AccountActivityLog();
                log.UserId = Convert.ToInt32(Session["UserID"].ToString());
                log.UserName = Session["UserName"].ToString();
                log.ActivityType = "Create";
                log.ModuleName = "Receipt";
                log.ReferenceId = _ar.ReceiptID;
                log.ReferenceNo = txtReference.Text;
                log.AccountId = ddlAccount.SelectedItem.ToString();
                log.Amount = Convert.ToDecimal(txtAmount.Text);
                _gl.LogAccountActivity(log);
                DataTable dtReceipt = _ar.GetReceipts(_ar.ReceiptID);
                int VoucherID = 0;
                if (dtReceipt.Rows.Count > 0)
                {
                    DAL.GLModule.Voucher voucher = new DAL.GLModule.Voucher();
                    voucher.VoucherNo = dtReceipt.Rows[0]["VoucherNumber"].ToString();
                    voucher.VoucherType = "RECEIPT";
                    voucher.VoucherDate = Convert.ToDateTime(dtReceipt.Rows[0]["ReceiptDate"].ToString());
                    voucher.Narration = "Advance Payment From " + dtReceipt.Rows[0]["PayerName"].ToString();
                    voucher.TotalAmount = Convert.ToDecimal(dtReceipt.Rows[0]["ReceiptAmount"].ToString());
                    voucher.FinancialYear = Convert.ToDateTime(dtReceipt.Rows[0]["ReceiptDate"]).ToString("yyyy");
                    voucher.CreatedBy = Convert.ToInt32(Session["UserID"].ToString());
                    voucher.CreatedOn = DateTime.Now;
                    voucher.AccountId = Convert.ToInt32(hdAccountID.Value);
                    voucher.Debit = Convert.ToDecimal(dtReceipt.Rows[0]["ReceiptAmount"].ToString());
                    voucher.Credit = Convert.ToDecimal(dtReceipt.Rows[0]["ReceiptAmount"].ToString());
                    voucher.BankClearingAccountID = Convert.ToInt32(hdAccountID.Value);// Convert.ToInt32(ddlAccount.SelectedValue);
                    voucher.PaymentMode = radPaymentMode.SelectedItem.ToString();
                    VoucherID = _gl.SaveVoucher(voucher);
                    _ar.VoucherID = VoucherID;
                    _ar.UpdateVoucherID(_ar); // Update VoucherID in Receipts
                }
                if (radPaymentMode.SelectedItem.ToString() == "CHEQUE")
                {
                    DAL.GLModule.Cheque chq = new DAL.GLModule.Cheque();
                    chq.VoucherId = VoucherID;
                    chq.PartyName = ddlCustomers.SelectedItem.ToString();
                    chq.ChequeNo = txtChequeNo.Text;
                    chq.ChequeDate = Convert.ToDateTime(txtChequeDate.Text);
                    chq.BankAccountID = txtBankAccount.Text;
                    chq.Amount = Convert.ToDecimal(txtAmount.Text);
                    chq.Status = "Received";
                    int ChequeID = _gl.AddChequeDetails(chq);

                    log = new GLModule.AccountActivityLog();
                    log.UserId = Convert.ToInt32(Session["UserID"].ToString());
                    log.UserName = Session["UserName"].ToString();
                    log.ActivityType = "Create";
                    log.ModuleName = "Cheque Inward";
                    log.ReferenceId = ChequeID;
                    log.ReferenceNo = txtChequeNo.Text;
                    log.AccountId = txtBankAccount.Text;
                    log.Amount = Convert.ToDecimal(chq.Amount);
                    _gl.LogAccountActivity(log);
                }

                if (radPaymentMode.SelectedItem.ToString() == "BANK")
                {
                    DAL.GLModule.BankTransfer bnk = new DAL.GLModule.BankTransfer();
                    bnk.VoucherId = VoucherID;
                    bnk.PartyName = ddlCustomers.SelectedItem.ToString();
                    bnk.BankName = txtBankName.Text;
                    bnk.TransferDate = Convert.ToDateTime(txtTransferDate.Text);
                    bnk.BankAccountID = txtBankAccount1.Text;
                    bnk.Amount = Convert.ToDecimal(txtAmount.Text);
                    bnk.Status = "Received";
                    int TransferID = _gl.AddBankTransfer(bnk);

                    _ar.arVar = (ARModule.ARVariables)Session["ARVariables"];
                    log = new GLModule.AccountActivityLog();
                    log.UserId = Convert.ToInt32(Session["UserID"].ToString());
                    log.UserName = Session["UserName"].ToString();
                    log.ActivityType = "Create";
                    log.ModuleName = "Bank Transfer Inward";
                    log.ReferenceId = TransferID;
                    log.ReferenceNo = txtBankName.Text + " | " + _ar.arVar.InvoiceNumber;
                    log.AccountId = txtBankAccount1.Text;
                    log.Amount = Convert.ToDecimal(bnk.Amount);
                    _gl.LogAccountActivity(log);
                }
            }
            Clear();
        }
    }
}