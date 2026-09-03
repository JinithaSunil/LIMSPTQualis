using DocumentFormat.OpenXml.Math;
using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static LIMS.DAL.GLModule;

namespace LIMS
{
    public partial class Payments : System.Web.UI.Page
    {
        DAL.GLModule _gl = new GLModule();
        DAL.APModule _ap = new DAL.APModule();
        DAL.Journal _jl = new DAL.Journal();
        DAL.BankingModule _bk = new DAL.BankingModule();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAccounts();
                LoadCOA();
                if (Session["APVariables"] != null)
                {
                    LoadPayeeDetails();
                }
                LoadPayments();
                BindPayee();
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
            dt = _gl.GetLinkedAccounts("Purchase");
            if (dt.Rows.Count > 0)
            {
                ddlAccount.Items.FindByText(dt.Rows[0]["BankAccountID"].ToString()).Selected = true;
                hdAccountCode.Value = ddlAccount.SelectedValue;
                DataTable dtBank = _ap.GetBankFromCode(hdAccountCode.Value);
                if(dtBank.Rows.Count > 0)
                {
                    hdAccountID.Value = dtBank.Rows[0]["BankAccountID"].ToString();
                }
            }
			ddlAccount.Items.Insert(0, "Select Sales Bank A/C");
        }
        private void LoadPayments()
        {
            gvPayments.DataSource = _ap.GetPayments(ddlPayeeType.SelectedItem.Text);
            gvPayments.DataBind();
        }
        private void LoadCOA()
        {
            DataTable dt = _gl.GetCOA("Accounts Payable");
            ddlCOAAccount.DataSource = dt;
            ddlCOAAccount.DataTextField = "AccCode";
            ddlCOAAccount.DataValueField = "AccountCode";
            ddlCOAAccount.DataBind();
            foreach (ListItem item in ddlCOAAccount.Items)
            {
                DataRow row = dt.AsEnumerable()
                                .FirstOrDefault(r => r.Field<string>("AccountCode") == item.Value);

                if (row != null)
                {
                    item.Attributes["AccountID"] = row["AccountID"].ToString();
                }                
            }           
            dt = _gl.GetLinkedAccounts("Purchase");
            if (dt.Rows.Count > 0)
            {
                ddlCOAAccount.Items.FindByText(dt.Rows[0]["PayableAccountID"].ToString()).Selected = true;
                hdCOAAccountID.Value = ddlCOAAccount.SelectedItem.Attributes["AccountID"]; 
            }

            ddlCOA.DataSource = _gl.GetCOA();
            ddlCOA.DataTextField = "AccountName1";
            ddlCOA.DataValueField = "AccountID";
            ddlCOA.DataBind();
            ddlCOA.Items.Insert(0, "Select Account");
        }

        private void LoadPayeeDetails()
        {
            _ap.apVar = (APModule.APVariables)Session["APVariables"];
            hdPOID.Value = _ap.apVar.PurchaseOrderID;
            txtReference.Text = "Accounts Payable — " + _ap.apVar.PayeeName + " - " + _ap.apVar.PONumber + " — " + _ap.apVar.PODate.ToString("yyyy-MM-dd");
            ddlPayeeType.SelectedValue = "Vendor";
            BindPayee();
            ddlPayee.ClearSelection();
            ddlPayee.Items.FindByText(_ap.apVar.PayeeName).Selected = true;
            hdPayeeID.Value = ddlPayee.SelectedValue;
            ddlCOA.ClearSelection();
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            lblPONumber.Text = _ap.apVar.PONumber;
            lblSupplierInvoiceNo.Text = _ap.apVar.SupplierInvoiceNo;
            txtPOAmount.Text = _ap.apVar.POAmount.ToString();
            txtAmountPaid.Text = _ap.apVar.AmountPaid.ToString();
            txtAmount.Text = (_ap.apVar.POAmount - _ap.apVar.AmountPaid).ToString("0.00");
            txtBalance.Text = "0.0";
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
                if (txtChequeNo.Text == "") { txtChequeNo.BorderColor = Color.Red; flag = 1; } else { txtChequeNo.BorderColor = Color.Empty; }
                if (txtChequeDate.Text == "") { txtChequeDate.BorderColor = Color.Red; flag = 1; } else { txtChequeDate.BorderColor = Color.Empty; }
            }
            if (flag == 0)
            {  //Payment Entry
                //--------------

                _ap.PayeeId = Convert.ToInt32(ddlPayee.SelectedValue);
                _ap.PaymentDate = Convert.ToDateTime(txtDate.Text);
                _ap.TotalAmount = Convert.ToDecimal(txtPOAmount.Text);
                _ap.PaymentAmount = Convert.ToDecimal(txtAmount.Text);
                _ap.PrevPaymentAmount = Convert.ToDecimal(txtAmountPaid.Text);
                _ap.BalanceAmount = Convert.ToDecimal(txtBalance.Text);
                _ap.PaymentMethod = hdPaymentMode.Value;
                _ap.ReferenceNo = txtReference.Text;
                _ap.Notes = txtNotes.Text;
                _ap.CreatedOn = DateTime.Now;
                _ap.PayeeType = ddlPayeeType.SelectedItem.ToString();
                _ap.PayeeName = ddlPayee.SelectedItem.ToString();
                _ap.PaymentMode = radPaymentMode.SelectedItem.ToString();
                if (lblPONumber.Text.Length > 0) { _ap.PONumber = lblPONumber.Text; } else { _ap.PONumber = ""; }
                if (lblSupplierInvoiceNo.Text.Length > 0) { _ap.SupplierInvoiceNo = lblSupplierInvoiceNo.Text; } else { _ap.SupplierInvoiceNo = ""; }
                if (hdPOID.Value == "") { _ap.PurchaseOrderID = 0; }
                else { _ap.PurchaseOrderID = Convert.ToInt32(hdPOID.Value); }
                _ap.Year = _ap.PaymentDate.ToString("yyyy");
                DataTable dtLast = _ap.GetLast("Payments", "VoucherYear", _ap.Year);
                if (dtLast.Rows.Count == 0) { _ap.Counter = 1; }
                else
                {
                    _ap.Counter = dtLast.Rows.Count + 1;
                }
                _ap.Number = _ap.Counter.ToString("D4") + "-" + _ap.PaymentDate.ToString("MM") + "-" + _ap.Year + "/" + _ap.Number;

                DataTable dt = new DataTable();
                dt.Columns.Add("COAAccountCode", typeof(string));
                dt.Columns.Add("BankAccountCode", typeof(string));
                dt.Columns.Add("Amount", typeof(decimal));
                dt.Columns.Add("PaymentRequisitionDetailsID", typeof(int));

                DataRow dr = dt.NewRow();
                dr["COAAccountCode"] = ddlCOA.SelectedItem;
                dr["BankAccountCode"] = ddlAccount.SelectedItem;
                dr["Amount"] = _ap.PaymentAmount;
                dr["PaymentRequisitionDetailsID"] = 0;
                dt.Rows.Add(dr);

                _ap.PaymentID = _ap.AddPayment(_ap, dt);
                DAL.GLModule.AccountActivityLog log = new GLModule.AccountActivityLog();
                log.UserId = Convert.ToInt32(Session["UserID"].ToString());
                log.UserName = Session["UserName"].ToString();
                log.ActivityType = "Create";
                log.ModuleName = "Payment";
                log.ReferenceId = _ap.PaymentID;
                log.ReferenceNo = txtReference.Text;
                log.AccountId = ddlCOA.SelectedItem.ToString();
                log.Amount = Convert.ToDecimal(txtAmount.Text);
                _gl.LogAccountActivity(log);

                //Journal Entry
                //--------------
                _jl.AccountID = hdCOAAccountID.Value; //ddlCOA.SelectedValue;
                _jl.JournalDate = Convert.ToDateTime(txtDate.Text);
                _jl.ReferenceNo = txtReference.Text;
                _jl.Transaction = "Payment";
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

                journalLines.Rows.Add("", Convert.ToDecimal(txtAmount.Text), 0, "Accounts Payable", _jl.AccountID);
                int JournalID = 0;
                try
                {
                    JournalID = _jl.AddJournalWithGL(journalHeader, journalLines);
                    _ap.JournalId = JournalID;
                    _ap.UpdateJournalID(_ap); // Update JournalID in Receipts
                }
                catch (SqlException ex)
                {
                    foreach (SqlError err in ex.Errors)
                    {
                        Console.WriteLine(err.Message);
                        Console.WriteLine("Procedure: " + err.Procedure);
                        Console.WriteLine("Line: " + err.LineNumber);
                    }
                    throw;
                }
                //Bank Statement Entry
                //--------------
                DataTable dtBank = _gl.GetBankAndCashAccounts(ddlAccount.SelectedValue);
                if (dtBank != null)
                {
                    if (dtBank.Rows[0]["AccountType"].ToString() == "Bank")
                    {
                        hdPaymentMode.Value = "Bank";
                        _ap.Number = "PQI-BK/" + ddlAccount.SelectedItem.ToString();
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
                        bankingHeader.Rows.Add(_bk.BankAccountId, _bk.StatementDate, _bk.ClosingBalance, _bk.ClosingBalance - Convert.ToDecimal(txtAmount.Text), DateTime.Now);

                        DataTable bankingLines = _bk.BankingLines();
                        bankingLines.Rows.Add(Convert.ToDateTime(txtDate.Text), "Payment to Vendor", Convert.ToDecimal(txtAmount.Text), 0, _bk.ClosingBalance - Convert.ToDecimal(txtAmount.Text));
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
                    else { _ap.Number = "PQI-CASH-BK"; hdPaymentMode.Value = "Cash"; }
                }

              


                if (txtAmountPaid.Text != "")
                {
                    //Update amount paid in Invoices and InvoiceAR tables
                    _ap.AmountPaid = Convert.ToDecimal(txtAmountPaid.Text) + Convert.ToDecimal(txtAmount.Text);
                    _ap.UpdatePOAmountPaid(_ap);
                    DataTable dtPayment = _ap.GetPOPayments(_ap.PaymentID);
                    if (dtPayment.Rows.Count > 0)
                    {
                        DAL.GLModule.Voucher voucher = new DAL.GLModule.Voucher();
                        voucher.VoucherNo = dtPayment.Rows[0]["VoucherNumber"].ToString();
                        voucher.VoucherType = "PAYMENT";
                        voucher.VoucherDate = Convert.ToDateTime(dtPayment.Rows[0]["PaymentDate"].ToString());
                        voucher.Narration = "Payment To " + dtPayment.Rows[0]["PayeeName"].ToString();
                        voucher.TotalAmount = Convert.ToDecimal(dtPayment.Rows[0]["PaymentAmount"].ToString());
                        voucher.FinancialYear = Convert.ToDateTime(dtPayment.Rows[0]["PaymentDate"]).ToString("yyyy");
                        voucher.CreatedBy = Convert.ToInt32(Session["UserID"].ToString());
                        voucher.CreatedOn = DateTime.Now;
                        voucher.AccountId = Convert.ToInt32(ddlCOA.SelectedValue);
                        voucher.AccountCode = ddlCOA.SelectedItem.ToString();
                        voucher.Debit = Convert.ToDecimal(dtPayment.Rows[0]["PaymentAmount"].ToString());
                        voucher.Credit = Convert.ToDecimal(dtPayment.Rows[0]["PaymentAmount"].ToString());
                        voucher.BankClearingAccountID = Convert.ToInt32(ddlAccount.SelectedValue);
                        voucher.PaymentMode = radPaymentMode.SelectedItem.ToString();
                        int VoucherID = _gl.SaveVoucher(voucher);
                        _ap.VoucherID = VoucherID;
                        _ap.UpdateVoucherID(_ap); // Update VoucherID in Payments

                        log = new GLModule.AccountActivityLog();
                        log.UserId = Convert.ToInt32(Session["UserID"].ToString());
                        log.UserName = Session["UserName"].ToString();
                        log.ActivityType = "Create";
                        log.ModuleName = "Voucher";
                        log.ReferenceId = VoucherID;
                        log.ReferenceNo = voucher.VoucherNo;
                        log.AccountId = ddlCOA.SelectedItem.ToString();
                        log.Amount = Convert.ToDecimal(dtPayment.Rows[0]["PaymentAmount"].ToString());
                        _gl.LogAccountActivity(log);

                        DAL.GLModule.Cheque chq = new DAL.GLModule.Cheque();
                        chq.VoucherId = VoucherID;
                        chq.PartyName = ddlPayee.SelectedItem.ToString();
                        chq.ChequeNo = txtChequeNo.Text;
                        chq.ChequeDate = Convert.ToDateTime(txtChequeDate.Text);
                        chq.BankAccountID = txtBankAccount.Text;
                        chq.Amount = Convert.ToDecimal(txtAmount.Text);
                        chq.Status = "Issued";
                        int ChequeID = _gl.AddChequeDetails(chq);

                        log = new GLModule.AccountActivityLog();
                        log.UserId = Convert.ToInt32(Session["UserID"].ToString());
                        log.UserName = Session["UserName"].ToString();
                        log.ActivityType = "Create";
                        log.ModuleName = "Cheque Outward";
                        log.ReferenceId = ChequeID;
                        log.ReferenceNo = txtChequeNo.Text + " | " + ddlPayee.SelectedItem.ToString();
                        log.AccountId = txtBankAccount.Text;
                        log.Amount = Convert.ToDecimal(chq.Amount);
                        _gl.LogAccountActivity(log);
                    }
                    string url = "generateVoucher.aspx?o=1&x=" + _ap.PaymentID.ToString();
                    Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                    url = "AccountsPayable.aspx";
                    Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                }
                Clear();
            }
            LoadPayments();
        }
       
        protected void ddlPayeeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPayee();
            LoadPayments();
        }
        protected void BindPayee()
        {
            if (ddlPayeeType.SelectedItem.ToString() == "Vendor")
            {
                ddlPayee.DataSource = _ap.GetPayee("Vendor");
                ddlPayee.DataTextField = "VendorName";
                ddlPayee.DataValueField = "VendorID";
                ddlPayee.DataBind();
                ddlPayee.Items.Insert(0, "Select Payee");
            }

            if (ddlPayeeType.SelectedItem.ToString() == "Employee")
            {
                ddlPayee.DataSource = _ap.GetPayee("Employee");
                ddlPayee.DataTextField = "EmployeeName";
                ddlPayee.DataValueField = "EmployeeID";
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

        protected void ddlCOAAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCOAAccount.SelectedIndex != 0)
            {
                ddlCOA.ClearSelection();
                ddlCOA.Items.FindByValue(ddlCOAAccount.SelectedValue).Selected = true;
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
            }
        }
    }

}
