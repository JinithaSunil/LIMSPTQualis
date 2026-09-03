using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static LIMS.DAL.ARModule;
using static LIMS.DAL.GLModule;
using static LIMS.InvoiceList;

namespace LIMS
{
    public partial class Receipts : System.Web.UI.Page
    {
        DAL.GLModule _gl = new GLModule();
        DAL.APModule _ap = new DAL.APModule();
        DAL.ARModule _ar = new DAL.ARModule();
        DAL.Journal _jl = new DAL.Journal();
        DAL.BankingModule _bk = new DAL.BankingModule();
        DAL.CM _cm = new DAL.CM();
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAccounts();

                LoadCOA();
                if (Session["ARVariables"] != null)
                {
                    LoadPayeeDetails();
                }
                else
                {
                    LoadReceipts();
                }                    
                if (Session["Invoices"] != null)
                {
                    string Invoices = Session["Invoices"].ToString();
                }
            }
        }
        private void LoadAccounts()
        {
            DataTable dt = _gl.GetCOA("Bank");
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
                ddlAccount.Items.FindByText(dt.Rows[0]["BankAccountID"].ToString()).Selected = true;
                hdAccountCode.Value = ddlAccount.SelectedValue;
                DataTable dtBank = _ar.GetBankFromCode(hdAccountCode.Value);
                if(dtBank.Rows.Count > 0)
                {
                    hdAccountID.Value = dtBank.Rows[0]["BankAccountID"].ToString();
                }
            }
            ddlAccount.Items.Insert(0, "Select Sales Bank A/C");
        }
        private void LoadReceipts()
        {
            gvReceipts.DataSource = _ar.GetReceipts();
            gvReceipts.DataBind();
        }
        private void LoadCOA()
        {
            DataTable dt = _gl.GetCOA("Accounts Receivable");
            ddlCOA.DataSource = dt;
            ddlCOA.DataTextField = "AccCode";
            ddlCOA.DataValueField = "AccountCode";
            ddlCOA.DataBind();
            
            foreach (ListItem item in ddlCOA.Items)
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
                ddlCOA.Items.FindByText(dt.Rows[0]["ReceivableAccountID"].ToString()).Selected = true;               
                hdCOAAccountID.Value = ddlCOA.SelectedItem.Attributes["AccountID"]; 
            }
            ddlCOA.Items.Insert(0, "Select Sales Bank A/C");
        }
        private void LoadPayeeDetails()
        {
            _ar.arVar = (ARModule.ARVariables)Session["ARVariables"];
            hdInvoiceARID.Value = _ar.arVar.InvoiceARID;
            hdInvoiceID.Value = _ar.arVar.InvoiceID;
            txtReference.Text = _ar.arVar.Reference;
            ddlPayeeType.SelectedValue = "Customer";
            BindPayee();
            ddlPayee.Items.FindByText(_ar.arVar.PayerName).Selected = true;
            hdPayeeID.Value = ddlPayee.SelectedValue;
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtPOAmount.Text = _ar.arVar.InvoiceAmount.ToString();
            txtAmountPaid.Text = _ar.arVar.AmountPaid.ToString();
            txtAmount.Text = (_ar.arVar.InvoiceAmount - _ar.arVar.AmountPaid).ToString("0.00");
            txtBalance.Text = "0.00";// (_ar.arVar.InvoiceAmount - _ar.arVar.AmountPaid).ToString("0.00");
            DataTable dt = _ar.GetReceipts(hdInvoiceARID.Value);
            if(dt.Rows.Count > 0)
            {
                gvReceipts.DataSource = dt;
                gvReceipts.DataBind();
            }
        }
        protected void Clear()
        {
            ddlAccount.ClearSelection();
            ddlCOA.ClearSelection();
            ddlPayee.ClearSelection();
            ddlPayeeType.ClearSelection();
            txtDate.Text = "";
            txtReference.Text = "";
            txtPOAmount.Text = "";
            txtAmountPaid.Text = "";
            txtAmount.Text = "";
            txtBalance.Text = "";
            txtNotes.Text = "";
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (ddlAccount.SelectedIndex == 0) { ddlAccount.BackColor = Color.LightPink; flag = 1; } else { ddlAccount.BackColor = Color.Empty; }
            if (txtDate.Text == "") { txtDate.BackColor = Color.LightPink; flag = 1; } else { txtDate.BackColor = Color.Empty; }
            if (txtAmount.Text == "") { txtAmount.BackColor = Color.LightPink; flag = 1; } else { txtAmount.BackColor = Color.Empty; }
            if (txtReference.Text == "") { txtReference.BackColor = Color.LightPink; flag = 1; } else { txtReference.BackColor = Color.Empty; }
            if (ddlPayeeType.SelectedIndex == 0) { ddlPayeeType.BackColor = Color.LightPink; flag = 1; } else { ddlPayeeType.BackColor = Color.Empty; }
            if (ddlPayee.SelectedIndex == 0) { ddlPayee.BackColor = Color.LightPink; flag = 1; } else { ddlPayee.BackColor = Color.Empty; }
            if (ddlPayee.SelectedIndex == 0) { ddlPayee.BackColor = Color.LightPink; flag = 1; } else { ddlPayee.BackColor = Color.Empty; }
            if (radPaymentMode.SelectedItem.ToString() == "CHEQUE")
            {
                _ar.Number = "PQI-BM/" + ddlAccount.SelectedItem.ToString();
                if (txtChequeNo.Text == "") { txtChequeNo.BorderColor = Color.Red; flag = 1; } else { txtChequeNo.BorderColor = Color.Empty; }
                if (txtChequeDate.Text == "") { txtChequeDate.BorderColor = Color.Red; flag = 1; } else { txtChequeDate.BorderColor = Color.Empty; }
            }
            else if (radPaymentMode.SelectedItem.ToString() == "BANK")
            {
                _ar.Number = "PQI-BM/" + ddlAccount.SelectedItem.ToString();
                if (txtBankName.Text == "") { txtBankName.BorderColor = Color.Red; flag = 1; } else { txtBankName.BorderColor = Color.Empty; }
                if (txtBankAccount1.Text == "") { txtBankAccount1.BorderColor = Color.Red; flag = 1; } else { txtBankAccount1.BorderColor = Color.Empty; }
                if (txtTransferDate.Text == "") { txtTransferDate.BorderColor = Color.Red; flag = 1; } else { txtTransferDate.BorderColor = Color.Empty; }
            }
            else { _ar.Number = "PQI-CASH-BM"; }
            if (flag == 0)
            {
                //Receipt Entry
                //--------------
                _ar.ReceiptType = "INVOICE";
                _ar.InvoiceARID = hdInvoiceARID.Value;
                _ar.PayerId = Convert.ToInt32(ddlPayee.SelectedValue);
                _ar.ReceiptDate = Convert.ToDateTime(txtDate.Text);
                _ar.ReceiptAmount = Convert.ToDecimal(txtAmount.Text);
                _ar.PaymentMethod = radPaymentMode.SelectedItem.ToString();
                _ar.BankAccountCode = ddlAccount.SelectedValue;
                _ar.ReferenceNo = txtReference.Text;
                _ar.Notes = txtNotes.Text;
                _ar.CreatedOn = DateTime.Now;
                _ar.PayerType = ddlPayeeType.SelectedItem.ToString();
                _ar.PayerName = ddlPayee.SelectedItem.ToString();
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
                List<InvoicePayment> invoices = new List<InvoicePayment>();
                invoices = _ar.GetInvoiceDetails(hdInvoiceID.Value);
                _ar.ReceiptID = _ar.AddReceipt(_ar, invoices);
                //Update Customer Ledger
                _cm.UpdateCustomerLedger(_cm, "Receipt");

                //Journal Entry
                //--------------
                _jl.AccountID = hdCOAAccountID.Value; //ddlCOA.SelectedValue;
                _jl.JournalDate = Convert.ToDateTime(txtDate.Text);
                _jl.ReferenceNo = txtReference.Text;
                _jl.Transaction = "Receipt";
                _jl.Description = ddlPayee.SelectedItem.ToString();
                if (Session["EmployeeName"] != null)
                {
                    _jl.CreatedBy = Session["EmployeeName"].ToString();
                }
                else
                {
                    _jl.CreatedBy = "Accounts User";
                }

                DataTable journalHeader = _jl.JournalHeader();
                journalHeader.Rows.Add(_jl.JournalDate, _jl.ReferenceNo, _jl.Transaction, _jl.Description, _jl.CreatedBy);

                DataTable journalLines = _jl.JournalLines();
                journalLines.Rows.Add("", 0, Convert.ToDecimal(txtAmount.Text), "Accounts Receivable", _jl.AccountID);

                int JournalID = _jl.AddJournalWithGL(journalHeader, journalLines);
                _ar.JournalId = JournalID;
                _ar.UpdateJournalID(_ar); // Update JournalID in Receipts

                //Bank Statement Entry
                //--------------
                DataTable dtBank = _gl.GetBankAndCashAccounts(ddlAccount.SelectedValue);
                if (dtBank != null)
                {
                    if (dtBank.Rows[0]["AccountType"].ToString() == "Bank")
                    {
                        _bk.BankAccountId = hdAccountID.Value;// ddlAccount.SelectedValue;
                        _bk.StatementDate = Convert.ToDateTime(txtDate.Text);

                        DataTable dtBankBalance = _bk.GetBankBalance(_bk);
                        if (dtBankBalance.Rows.Count > 0)
                        {
                            _bk.OpeningBalance = Convert.ToDecimal(dtBankBalance.Rows[0]["OpeningBalance"]);
                            _bk.ClosingBalance = Convert.ToDecimal(dtBankBalance.Rows[0]["ClosingBalance"]);
                        }
                        else
                        {
                            _bk.OpeningBalance = 0;
                            _bk.ClosingBalance = 0;
                        }
                        DataTable bankingHeader = _bk.BankingHeader();
                        bankingHeader.Rows.Add(_bk.BankAccountId, _bk.StatementDate, _bk.ClosingBalance, _bk.ClosingBalance + Convert.ToDecimal(txtAmount.Text), DateTime.Now);

                        DataTable bankingLines = _bk.BankingLines();
                        bankingLines.Rows.Add(Convert.ToDateTime(txtDate.Text), "Receipt from Customer", 0, Convert.ToDecimal(txtAmount.Text), _bk.ClosingBalance + Convert.ToDecimal(txtAmount.Text));
                        int StatementID = _bk.AddBankStatement(bankingHeader, bankingLines);
                        BankingModule.UpdateBalance(
                                int.Parse(hdAccountID.Value),
                                _bk.ClosingBalance
                            );

                        //Bank Reconcilation
                        //--------------
                        DataTable bankRecon = _bk.BankReconciliation();
                        bankRecon.Rows.Add(StatementID, JournalID, Convert.ToDateTime(txtDate.Text), Session["EmployeeName"].ToString(), txtReference.Text);
                        _bk.AddBankReconciliation(bankRecon);
                    }

                }

                
                DAL.GLModule.AccountActivityLog log = new GLModule.AccountActivityLog();
                log.UserId = Convert.ToInt32(Session["UserID"].ToString());
                log.UserName = Session["UserName"].ToString();
                log.ActivityType = "Create";
                log.ModuleName = "Receipt";
                log.ReferenceId = _ar.ReceiptID;
                log.ReferenceNo = txtReference.Text;
                log.AccountId = ddlCOA.SelectedItem.ToString();
                log.Amount = Convert.ToDecimal(txtAmount.Text);
                _gl.LogAccountActivity(log);
                DataTable dtReceipt = _ar.GetReceipts(_ar.ReceiptID);
                if (dtReceipt.Rows.Count > 0)
                {
                    DAL.GLModule.Voucher voucher = new DAL.GLModule.Voucher();
                    voucher.VoucherNo = dtReceipt.Rows[0]["VoucherNumber"].ToString();
                    voucher.VoucherType = "RECEIPT";
                    voucher.VoucherDate = Convert.ToDateTime(dtReceipt.Rows[0]["ReceiptDate"].ToString());
                    voucher.Narration = "Payment From " + dtReceipt.Rows[0]["PayerName"].ToString();
                    voucher.TotalAmount = Convert.ToDecimal(dtReceipt.Rows[0]["ReceiptAmount"].ToString());
                    voucher.FinancialYear = Convert.ToDateTime(dtReceipt.Rows[0]["ReceiptDate"]).ToString("yyyy");
                    voucher.CreatedBy = Convert.ToInt32(Session["UserID"].ToString());
                    voucher.CreatedOn = DateTime.Now;
                    voucher.AccountId = Convert.ToInt32(hdAccountID.Value);
                    voucher.AccountCode = ddlCOA.SelectedItem.ToString();
                    voucher.Debit = Convert.ToDecimal(dtReceipt.Rows[0]["ReceiptAmount"].ToString());
                    voucher.Credit = Convert.ToDecimal(dtReceipt.Rows[0]["ReceiptAmount"].ToString());
                    voucher.BankClearingAccountID = Convert.ToInt32(hdAccountID.Value);// Convert.ToInt32(ddlAccount.SelectedValue);
                    voucher.PaymentMode = radPaymentMode.SelectedItem.ToString();
                    int VoucherID = _gl.SaveVoucher(voucher);
                    _ar.VoucherID = VoucherID;
                    _ar.UpdateVoucherID(_ar); // Update VoucherID in Receipts

                    log = new GLModule.AccountActivityLog();
                    log.UserId = Convert.ToInt32(Session["UserID"].ToString());
                    log.UserName = Session["UserName"].ToString();
                    log.ActivityType = "Create";
                    log.ModuleName = "Voucher";
                    log.ReferenceId = VoucherID;
                    log.ReferenceNo = voucher.VoucherNo;
                    log.AccountId = ddlCOA.SelectedItem.ToString();
                    log.Amount = Convert.ToDecimal(dtReceipt.Rows[0]["ReceiptAmount"].ToString());
                    _gl.LogAccountActivity(log);


                    if (radPaymentMode.SelectedItem.ToString() == "CHEQUE")
                    {
                        DAL.GLModule.Cheque chq = new DAL.GLModule.Cheque();
                        chq.VoucherId = VoucherID;
                        chq.PartyName = ddlPayee.SelectedItem.ToString();
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
                        bnk.PartyName = ddlPayee.SelectedItem.ToString();
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

                Response.Redirect("AccountsReceivable.aspx");
            }

            Clear();
            LoadReceipts();
        }
        protected void ddlPayeeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPayee();
            LoadReceipts();
        }
        protected void BindPayee()
        {
            if (ddlPayeeType.SelectedItem.ToString() == "Customer")
            {
                ddlPayee.DataSource = _ar.GetCustomers();
                ddlPayee.DataTextField = "CompanyName";
                ddlPayee.DataValueField = "CompanyID";
                ddlPayee.DataBind();
                ddlPayee.Items.Insert(0, "Select Payee");
            }
            if (ddlPayeeType.SelectedItem.ToString() == "Vendor")
            {
                ddlPayee.DataSource = _ap.GetVendors();
                ddlPayee.DataTextField = "VendorName";
                ddlPayee.DataValueField = "VendorID";
                ddlPayee.DataBind();
                ddlPayee.Items.Insert(0, "Select Payee");
            }
        }
        protected void txtAmount_TextChanged(object sender, EventArgs e)
        {
            if ((txtAmount.Text != "") && (txtPOAmount.Text != ""))
            {
                txtBalance.Text = (Convert.ToDecimal(txtPOAmount.Text) - (Convert.ToDecimal(txtAmountPaid.Text) + Convert.ToDecimal(txtAmount.Text))).ToString();
            }
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
    }

}
