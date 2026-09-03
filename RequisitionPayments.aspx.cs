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

namespace LIMS
{
    public partial class RequisitionPayments : System.Web.UI.Page
    {
        DAL.GLModule _gl = new GLModule();
        DAL.APModule _ap = new DAL.APModule();
        DAL.Journal _jl = new DAL.Journal();
        DAL.BankingModule _bk = new DAL.BankingModule();
        DAL.CM _cm = new DAL.CM();
        decimal totalAmount = 0;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["prVariables"] != null)
                {
                    BindRequisition();
                    LoadPayeeDetails();
                    ToggleSaveButton();
                }
                else
                {
                    LoadPayments();
                }
            }
        }
        private void BindRequisition()
        {
            _ap.prVar = (APModule.PRVariables)Session["PRVariables"];
            hdPaymentRequisitionID.Value = _ap.prVar.PaymentRequisitionID;
            int PaymentRequisitionID = 0;
            if (_ap.prVar.PaymentRequisitionID != "")
            {
                PaymentRequisitionID = Convert.ToInt32(_ap.prVar.PaymentRequisitionID);
                DataTable dt = _cm.GetPaymentRequisitionDetails(PaymentRequisitionID);
                if (dt.Rows.Count > 0)
                {
                    gvPaymentRequisition.DataSource = dt;
                    gvPaymentRequisition.DataBind();
                }
            }
            LoadPaymentRequisitionItems(PaymentRequisitionID);
        }
        private void LoadPayments()
        {
            if (Request.QueryString["a"] != null)
            {
                gvPayments.DataSource = _ap.GetPayments(ddlPayeeType.SelectedItem.Text);
                gvPayments.DataBind();
            }
            if (Request.QueryString["e"] != null)
            {
                int EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                gvPayments.DataSource = _ap.GetPayments(EmployeeID, 1);
                gvPayments.DataBind();
            }

        }
        private void ToggleSaveButton()
        {
            btnSave.Visible = AllRowsHaveRequiredValues();
        }
        private void LoadPayeeDetails()
        {
            _ap.prVar = (APModule.PRVariables)Session["PRVariables"];
            hdPaymentRequisitionID.Value = _ap.prVar.PaymentRequisitionID;
            txtReference.Text = "Accounts Payable — " + _ap.prVar.PayeeName + " - " + _ap.prVar.PaymentRequisitionNumber + " — " + _ap.prVar.RequisitionDate.ToString("yyyy-MM-dd");
            ddlPayeeType.SelectedValue = "Employee";
            BindPayee();
            ddlPayee.ClearSelection();
            ddlPayee.Items.FindByText(_ap.prVar.PayeeName).Selected = true;
            hdPayeeID.Value = ddlPayee.SelectedValue;
            DataTable dtPR = _ap.GetEmployeePaymentRequisition(hdPayeeID.Value);
            if (dtPR.Rows.Count > 0)
            {
                ddlPaymentRequisition.DataSource = dtPR;
                ddlPaymentRequisition.DataTextField = "RequisitionNumber";
                ddlPaymentRequisition.DataValueField = "PaymentRequisitionID";
                ddlPaymentRequisition.DataBind();
                ddlPaymentRequisition.Items.Insert(0, "Select Payment Requisition");
            }
            ddlPaymentRequisition.ClearSelection();
            ddlPaymentRequisition.Items.FindByText(_ap.prVar.PaymentRequisitionNumber).Selected = true;
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlPaymentRequisition.Items.FindByText(_ap.prVar.PaymentRequisitionNumber);
            txtTotalAmount.Text = _ap.prVar.TotalAmount.ToString();
            txtAmountPaid.Text = _ap.prVar.PrevAmountPaid.ToString();
            txtAmount.Text = (_ap.prVar.TotalAmount - _ap.prVar.PrevAmountPaid).ToString("0.00");
            txtBalance.Text = (_ap.prVar.TotalAmount - _ap.prVar.PrevAmountPaid).ToString("0.00");
            BindDetails();
        }

        protected void BindDetails()
        {
            DataTable dtPRDetails = _ap.GetRequisitionPayment(Convert.ToInt32(hdPaymentRequisitionID.Value));
            if (dtPRDetails.Columns.Count == 0)
            {
                dtPRDetails.Columns.Add("COAAccountIDDr", typeof(int));
                dtPRDetails.Columns.Add("COAAccountCode", typeof(string));
                dtPRDetails.Columns.Add("COAAccountNameDr", typeof(string));
                dtPRDetails.Columns.Add("COAAccountIDCr", typeof(int));
                dtPRDetails.Columns.Add("COAAccountCodeCr", typeof(string));
                dtPRDetails.Columns.Add("COAAccountNameCr", typeof(string));
                dtPRDetails.Columns.Add("BankAccountCode", typeof(string));
                dtPRDetails.Columns.Add("Amount", typeof(decimal));
                dtPRDetails.Columns.Add("ID", typeof(int));
                dtPRDetails.Columns.Add("PaymentDescription", typeof(string));
            }
            Session["GridData"] = dtPRDetails;
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtDate.Text == "") { txtDate.BackColor = Color.LightPink; flag = 1; } else { txtDate.BackColor = Color.Empty; }
            if (txtAmount.Text == "") { txtAmount.BackColor = Color.LightPink; flag = 1; } else { txtAmount.BackColor = Color.Empty; }
            if (txtReference.Text == "") { txtReference.BackColor = Color.LightPink; flag = 1; } else { txtReference.BackColor = Color.Empty; }
            //if (ddlPayeeType.SelectedIndex == 0) { ddlPayeeType.BackColor = Color.LightPink; flag = 1; } else { ddlPayeeType.BackColor = Color.Empty; }
            if (ddlPayee.SelectedIndex == 0) { ddlPayee.BackColor = Color.LightPink; flag = 1; } else { ddlPayee.BackColor = Color.Empty; }
            if (ddlPayee.SelectedIndex == 0) { ddlPayee.BackColor = Color.LightPink; flag = 1; } else { ddlPayee.BackColor = Color.Empty; }
            if (flag == 0)
            {
                DataTable dt = Session["GridData"] as DataTable;

                if (dt.Rows.Count > 0)
                {
                    _ap.UpdatePaymentRequisitionPayment(dt);
                }
                //Journal Entry
                //--------------
                //_jl.AccountID = hdCOAAccountCode.Value;
                //_jl.JournalDate = Convert.ToDateTime(txtDate.Text);
                //_jl.ReferenceNo = txtReference.Text;
                //_jl.Transaction = "Payment";
                //_jl.Description = ddlPayee.SelectedItem.ToString();
                //if (Session["EmployeeName"] != null)
                //{
                //    _jl.CreatedBy = Session["EmployeeName"].ToString();
                //}
                //else
                //{
                //    _jl.CreatedBy = "Accounts User";
                //}

                //DataTable journalHeader = _jl.JournalHeader();
                //journalHeader.Rows.Add(_jl.JournalDate, _jl.ReferenceNo, _jl.Transaction, _jl.Description, _jl.CreatedBy);

                //DataTable journalLines = _jl.JournalLines();

                ////journalLines.Rows.Add("", Convert.ToDecimal(txtAmount.Text), 0, "Accounts Payable", _jl.AccountID);
                //decimal amount = Convert.ToDecimal(txtAmountPaid.Text);
                //journalLines.Rows.Add(
                //        "",
                //        amount,
                //        0,
                //        "Payment Requisition",
                //        dt.Rows[0]["COAAccountIDDr"].ToString()
                //    );

                //journalLines.Rows.Add(
                //    "",
                //    0,
                //    amount,
                //    "Payment Requisition",
                //    dt.Rows[0]["COAAccountIDCr"].ToString()
                //);

                //int JournalID = _jl.AddJournalWithGL(journalHeader, journalLines);

                ////Bank Statement Entry
                ////--------------
                //DataTable dtBank = _gl.GetBankAndCashAccounts(dt.Rows[0]["BankAccountID"].ToString());//jini
                //if (dtBank != null)
                //{
                //    if (dtBank.Rows[0]["AccountType"].ToString() == "Bank")
                //    {
                //        _ap.Number = "PQI-BK/" + dt.Rows[0]["BankAccountCode"].ToString(); //+ ddlAccount.SelectedItem.ToString();
                //        _bk.BankAccountId = dt.Rows[0]["BankAccountID"].ToString(); //ddlAccount.SelectedValue;
                //        _bk.StatementDate = Convert.ToDateTime(txtDate.Text);

                //        DataTable dtBankBalance = _bk.GetBankBalance(_bk);
                //        if (dtBankBalance.Rows.Count > 0)
                //        {
                //            _bk.OpeningBalance = Convert.ToDecimal(dtBankBalance.Rows[0]["OpeningBalance"]);
                //            _bk.ClosingBalance = Convert.ToDecimal(dtBankBalance.Rows[0]["ClosingBalance"]);
                //        }
                //        else
                //        {
                //            _bk.OpeningBalance = 0;
                //            _bk.ClosingBalance = 0;
                //        }
                //        DataTable bankingHeader = _bk.BankingHeader();
                //        bankingHeader.Rows.Add(_bk.BankAccountId, _bk.StatementDate, _bk.ClosingBalance, _bk.ClosingBalance - Convert.ToDecimal(txtAmount.Text), DateTime.Now);

                //        DataTable bankingLines = _bk.BankingLines();
                //        bankingLines.Rows.Add(Convert.ToDateTime(txtDate.Text), "Payment to Vendor", Convert.ToDecimal(txtAmount.Text), 0, _bk.ClosingBalance - Convert.ToDecimal(txtAmount.Text));
                //        int StatementID = _bk.AddBankStatement(bankingHeader, bankingLines);

                //        //Bank Reconcilation
                //        //--------------
                //        DataTable bankRecon = _bk.BankReconciliation();
                //        bankRecon.Rows.Add(StatementID, JournalID, Convert.ToDateTime(txtDate.Text), Session["EmployeeName"].ToString(), txtReference.Text);
                //        _bk.AddBankReconciliation(bankRecon);

                //    }
                //    else { _ap.Number = "PQI-CASH-BK"; }
                //}

                //Payment Entry
                //--------------

                _ap.PayeeId = Convert.ToInt32(ddlPayee.SelectedValue);
                _ap.PaymentDate = Convert.ToDateTime(txtDate.Text);
                _ap.PaymentAmount = Convert.ToDecimal(txtAmount.Text);
                _ap.PrevPaymentAmount = Convert.ToDecimal(txtAmountPaid.Text);
                _ap.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text);
                _ap.PaymentMethod = "";
                _ap.ReferenceNo = txtReference.Text;
                _ap.Notes = txtNotes.Text;
                _ap.JournalId = 0;// JournalID;
                _ap.CreatedOn = DateTime.Now;
                _ap.PayeeType = ddlPayeeType.SelectedItem.ToString();
                _ap.PayeeName = ddlPayee.SelectedItem.ToString();
                if (ddlPaymentRequisition.SelectedIndex != 0) { _ap.PONumber = ddlPaymentRequisition.SelectedItem.Text; } else { _ap.PONumber = ""; }
                _ap.SupplierInvoiceNo = "";
                if (ddlPaymentRequisition.SelectedIndex != 0)
                {
                    _ap.PurchaseOrderID = Convert.ToInt32(ddlPaymentRequisition.SelectedValue.ToString());
                    _ap.prVar.PaymentRequisitionID = ddlPaymentRequisition.SelectedValue.ToString();
                }
                else { _ap.PurchaseOrderID = 0; }
                _ap.Year = _ap.PaymentDate.ToString("yyyy");
                DataTable dtLast = _ap.GetLast("Payments", "VoucherYear", _ap.Year);
                if (dtLast.Rows.Count == 0) { _ap.Counter = 1; }
                else
                {
                    _ap.Counter = dtLast.Rows.Count + 1;
                }
                _ap.Number = _ap.Counter.ToString("D4") + "-" + _ap.PaymentDate.ToString("MM") + "-" + _ap.Year + "/" + _ap.Number;



                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }

                else
                {
                    int paymentID = _ap.AddPayment(_ap, dt);
                    if (txtAmountPaid.Text != "")
                    {
                        //Update amount paid in Invoices and InvoiceAR tables
                        _ap.prVar.AmountPaid = Convert.ToDecimal(txtAmountPaid.Text) + Convert.ToDecimal(txtAmount.Text);
                        _ap.UpdatePRAmountPaid(_ap);

                        Response.Redirect("generateVoucher.aspx?o=2&x=" + paymentID.ToString());
                    }
                }
            }
        }
        protected void ddlPayeeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPayee();
        }
        protected void BindPayee()
        {
            if (ddlPayeeType.SelectedItem.ToString() == "Employee")
            {
                ddlPayee.DataSource = _ap.GetEmployees();
                ddlPayee.DataTextField = "EmployeeName";
                ddlPayee.DataValueField = "ID";
                ddlPayee.DataBind();
                ddlPayee.Items.Insert(0, "Select Payee");
            }
        }

        private bool AllRowsHaveRequiredValues()
        {
            DataTable dt = Session["GridData"] as DataTable;

            if (dt == null || dt.Rows.Count == 0)
                return false;

            decimal diff = 0;
            if (txtDifference.Text != "")
            {
                diff = Convert.ToDecimal(txtDifference.Text);
            }
            return true;
        }

        protected void gvPaymentRequisition_RowDataBound(
                        object sender,
                        GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            HiddenField hf =
                e.Row.FindControl(
                    "hfPaymentRequisitionDetailsID")
                as HiddenField;

            if (hf == null)
                return;

            long detailsID;

            if (!long.TryParse(
                hf.Value,
                out detailsID))
                return;

            GridView gvJournal =
                e.Row.FindControl("gvLineJournal")
                as GridView;

            Panel pnlJournal =
                e.Row.FindControl("pnlLineJournal")
                as Panel;

            if (gvJournal == null)
                return;

            object openID =
                Session["OpenJournalDetailsID"];

            if (openID == null)
                return;

            long openDetailsID;

            if (!long.TryParse(
                openID.ToString(),
                out openDetailsID))
                return;

            if (detailsID != openDetailsID)
                return;

            DataTable dtJournal =
                Session["Journal_" + detailsID]
                as DataTable;

            if (dtJournal == null)
                return;

            gvJournal.DataSource = dtJournal;
            gvJournal.DataBind();

            if (pnlJournal != null)
                pnlJournal.Visible = true;
        }
        protected void gvPayment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Calculate total
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblSrNo = (Label)e.Row.FindControl("lblSrNo");
                lblSrNo.Text = (e.Row.RowIndex + 1).ToString();
                totalAmount += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Amount"));
            }

            // Show total in footer
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[5].Text = "Total Amount";
                e.Row.Cells[5].Font.Bold = true;

                e.Row.Cells[6].Text = totalAmount.ToString("N2");
                e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[6].Font.Bold = true;

                txtAmount.Text = totalAmount.ToString();
            }
        }

        protected void gvPayment_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = Session["GridData"] as DataTable;

            if (dt != null)
            {
                // Delete the row using the GridView row index
                dt.Rows.RemoveAt(e.RowIndex);

                // Accept changes (optional)
                dt.AcceptChanges();

                // Save back to Session
                Session["GridData"] = dt;
            }
        }

        protected void txtAmount_TextChanged(object sender, EventArgs e)
        {

        }

        protected void ddlPaymentRequisition_SelectedIndexChanged(object sender, EventArgs e)
        {
            int paymentRequisitionID = Convert.ToInt32(ddlPaymentRequisition.SelectedValue);

            // LoadPaymentInformation(paymentRequisitionID);

            LoadPaymentRequisitionItems(paymentRequisitionID);
        }
        //    private void LoadPaymentInformation(int paymentRequisitionID)
        //    {
        //        string sql = @"
        //SELECT
        //    prm.PaymentRequisitionID,
        //    prm.PaymentRequisitionNo,
        //    prm.PayeeType,
        //    prm.PayeeID,
        //    p.PayeeName,
        //    prm.TotalAmount,
        //    ISNULL(prm.AmountPaid,0) AS AmountPaid,
        //    (prm.TotalAmount - ISNULL(prm.AmountPaid,0)) AS BalanceAmount,
        //    prm.ReferenceNo,
        //    prm.Notes
        //FROM PaymentRequisitionMaster prm
        //INNER JOIN PayeeMaster p
        //    ON prm.PayeeID = p.PayeeID
        //WHERE prm.PaymentRequisitionID=@PaymentRequisitionID";

        //        DataTable dt = 

        //        if (dt.Rows.Count == 0)
        //            return;

        //        DataRow dr = dt.Rows[0];

        //        ddlPayeeType.SelectedValue = dr["PayeeType"].ToString();

        //        hdPayeeID.Value = dr["PayeeID"].ToString();

        //        ddlPayee.SelectedValue = dr["PayeeID"].ToString();

        //        txtReference.Text = dr["ReferenceNo"].ToString();

        //        txtTotalAmount.Text = Convert.ToDecimal(dr["TotalAmount"]).ToString("N2");

        //        txtAmountPaid.Text = Convert.ToDecimal(dr["AmountPaid"]).ToString("N2");

        //        txtBalance.Text = Convert.ToDecimal(dr["BalanceAmount"]).ToString("N2");

        //        // Default payment amount = remaining balance
        //        txtAmount.Text = txtBalance.Text;

        //        txtDifference.Text = "0.00";

        //        txtNotes.Text = dr["Notes"].ToString();
        //    }
        private void LoadPaymentRequisitionItems(int paymentRequisitionID)
        {
            DataTable dt = _cm.GetPaymentRequisitionDetails(paymentRequisitionID);

            gvPaymentRequisition.DataSource = dt;
            gvPaymentRequisition.DataBind();
        }


        private void GenerateJournal(
    int templateID,
    DataRow requisitionLine,
    DataTable dtJournal)
        {
            // Your requisition line appears to use TotalPrice
            decimal amount = GetDecimal(requisitionLine, "TotalPrice");

            decimal bankCharge =
                GetDecimal(requisitionLine, "BankCharge");

            decimal withholdingTax =
                GetDecimal(requisitionLine, "WithholdingTax");

            int HWHT = withholdingTax > 0 ? 1 : 0;

            decimal netAmount =
                amount + bankCharge - withholdingTax;

            System.Diagnostics.Debug.WriteLine(
                $"TemplateID={templateID}, " +
                $"Amount={amount}, " +
                $"BankCharge={bankCharge}, " +
                $"WHT={withholdingTax}, " +
                $"HWHT={HWHT}");

            // Get template lines
            DataTable dtTemplate =
                _cm.GetJournalTemplateDetails(templateID);

            if (dtTemplate == null || dtTemplate.Rows.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"NO TEMPLATE DETAILS FOUND FOR TemplateID={templateID}");

                return;
            }

            System.Diagnostics.Debug.WriteLine(
                $"Template rows = {dtTemplate.Rows.Count}");

            int requisitionLineID =
                Convert.ToInt32(
                    requisitionLine["PaymentRequisitionDetailsID"]);

            foreach (DataRow templateRow in dtTemplate.Rows)
            {
                string entryType =
                    templateRow["EntryType"].ToString().Trim();

                int accountID =
                    Convert.ToInt32(templateRow["AccountID"]);

                int sequenceNo =
                    Convert.ToInt32(templateRow["SequenceNo"]);

                string amountType =
                    templateRow["AmountTypeName"]
                        .ToString()
                        .Trim()
                        .ToUpper();

                int templateHWHT = 0;

                if (dtTemplate.Columns.Contains("HWHT") &&
                    templateRow["HWHT"] != DBNull.Value)
                {
                    templateHWHT =
                        Convert.ToInt32(templateRow["HWHT"]);
                }

                // Skip WHT lines when there is no withholding tax
                if (templateHWHT == 1 && HWHT == 0)
                    continue;

                decimal journalAmount = 0;

                switch (amountType)
                {
                    case "AMOUNT":
                    case "TOTALAMOUNT":
                    case "GROSSAMOUNT":
                        journalAmount = amount;
                        break;

                    case "BANKCHARGE":
                        journalAmount = bankCharge;
                        break;

                    case "WITHHOLDINGTAX":
                    case "WHT":
                    case "TAX":
                        journalAmount = withholdingTax;
                        break;

                    case "NETAMOUNT":
                    case "NETPAYMENT":
                        journalAmount = netAmount;
                        break;
                }

                System.Diagnostics.Debug.WriteLine(
                    $"Seq={sequenceNo}, " +
                    $"DrCr={entryType}, " +
                    $"AccountID={accountID}, " +
                    $"AmountType={amountType}, " +
                    $"Amount={journalAmount}");

                // Don't add zero-value lines
                if (journalAmount <= 0)
                    continue;

                DataTable dtAccount =
                    _cm.GetCOAAccountDetails(accountID);

                string accountCode = "";
                string accountName = "";

                if (dtAccount != null &&
                    dtAccount.Rows.Count > 0)
                {
                    accountCode =
                        dtAccount.Rows[0]["AccountCode"].ToString();

                    accountName =
                        dtAccount.Rows[0]["AccountName"].ToString();
                }

                DataRow journalRow =
                    dtJournal.NewRow();

                journalRow["PaymentRequisitionDetailsID"] =
                    requisitionLineID;

                journalRow["SequenceNo"] =
                    sequenceNo;

                journalRow["EntryType"] =
                    entryType;

                journalRow["AccountID"] =
                    accountID;

                journalRow["AccountCode"] =
                    accountCode;

                journalRow["AccountName"] =
                    accountName;

                journalRow["Description"] =
                    dtTemplate.Columns.Contains("Description") &&
                    templateRow["Description"] != DBNull.Value
                        ? templateRow["Description"].ToString()
                        : "";

                if (entryType.Equals(
                    "Dr",
                    StringComparison.OrdinalIgnoreCase))
                {
                    journalRow["Debit"] = journalAmount;
                    journalRow["Credit"] = 0m;
                }
                else if (entryType.Equals(
                    "Cr",
                    StringComparison.OrdinalIgnoreCase))
                {
                    journalRow["Debit"] = 0m;
                    journalRow["Credit"] = journalAmount;
                }
                else
                {
                    continue;
                }

                dtJournal.Rows.Add(journalRow);
            }
        }
        private decimal GetDecimal(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return 0;

            if (row[columnName] == DBNull.Value)
                return 0;

            decimal value;

            if (decimal.TryParse(
                row[columnName].ToString(),
                out value))
            {
                return value;
            }

            return 0;
        }

        private DataTable CreateJournalTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JournalID", typeof(int));
            dt.Columns.Add("SequenceNo", typeof(int));

            dt.Columns.Add("EntryType", typeof(string));      // D / C

            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("AccountCode", typeof(string));
            dt.Columns.Add("AccountName", typeof(string));

            dt.Columns.Add("Description", typeof(string));

            dt.Columns.Add("AmountType", typeof(string));     // PAYMENT,BANKCHARGE,WHT,BALANCE
            dt.Columns.Add("Amount", typeof(decimal));

            dt.Columns.Add("BankID", typeof(int));
            dt.Columns.Add("BankName", typeof(string));

            dt.Columns.Add("IsBankAccount", typeof(bool));
            dt.Columns.Add("IsEditable", typeof(bool));

            dt.Columns.Add("CostCenterID", typeof(int));
            dt.Columns.Add("ProjectID", typeof(int));

            return dt;
        }
        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {

        }
        protected void gvPaymentRequisition_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            long detailsID;

            if (!long.TryParse(e.CommandArgument.ToString(),out detailsID))
            {
                return;
            }

            if (e.CommandName.Equals("Journal", StringComparison.OrdinalIgnoreCase))
            {
                int requisitionLineID;

                if (!int.TryParse(e.CommandArgument.ToString(),out requisitionLineID))
                {
                    return;
                }

                // Find the row that was clicked
                GridViewRow row =
                    (GridViewRow)((Control)e.CommandSource).NamingContainer;

                // Find controls inside that row
                Panel pnlLineJournal =
                    (Panel)row.FindControl("pnlLineJournal");

                GridView gvLineJournal =
                    (GridView)row.FindControl("gvLineJournal");

                Label lblLineDebit =
                    (Label)row.FindControl("lblLineDebit");

                Label lblLineCredit =
                    (Label)row.FindControl("lblLineCredit");

                Label lblLineDifference =
                    (Label)row.FindControl("lblLineDifference");

                if (pnlLineJournal == null ||
                    gvLineJournal == null)
                    return;

                // Get requisition line
                DataTable dtLine =
                    _cm.GetPaymentRequisitionLine(requisitionLineID);

                if (dtLine == null ||
                    dtLine.Rows.Count == 0)
                    return;

                DataRow dr = dtLine.Rows[0];

                // Withholding Tax
                decimal withholdingTax =
                    GetDecimal(dr, "WithholdingTax");

                int hasWithholdingTax =
                    withholdingTax > 0 ? 1 : 0;

                // Payee Type
                string payeeType = "Vendor";//dr["PayeeType"].ToString();

                // Get accounting template
                int templateID =
                    _cm.GetTemplateID(
                        "Payment",
                        payeeType,
                        hasWithholdingTax);

                if (templateID <= 0)
                {
                    lblLineDifference.Text =
                        "Template not found";

                    pnlLineJournal.Visible = true;
                    return;
                }

                // Create journal table
                DataTable dtJournal =
                    CreateJournalTable();

                // Generate journal entries
                GenerateJournal(
                    templateID,
                    dr,
                    dtJournal);

                Session["OpenJournalDetailsID"] = requisitionLineID;
                Session["Journal_" + requisitionLineID] = dtJournal;

                // Bind journal for THIS requisition line
                gvLineJournal.DataSource = dtJournal;
                gvLineJournal.DataBind();

                // Calculate totals
                decimal debit = 0;
                decimal credit = 0;

                foreach (DataRow jr in dtJournal.Rows)
                {
                    debit += GetDecimal(jr, "Debit");
                    credit += GetDecimal(jr, "Credit");
                }

                decimal difference = debit - credit;

                lblLineDebit.Text =
                    debit.ToString("N2");

                lblLineCredit.Text =
                    credit.ToString("N2");

                lblLineDifference.Text =
                    difference.ToString("N2");

                lblLineDifference.ForeColor =
                    difference == 0
                        ? System.Drawing.Color.Green
                        : System.Drawing.Color.Red;

               

                // Change button text
                LinkButton lnkJournal =
                    (LinkButton)row.FindControl("lnkJournal");

                // Show journal
                if (pnlLineJournal.Visible)
                {
                    pnlLineJournal.Visible = false;
                    lnkJournal.Text = "View Journal";
                }
                else
                {
                    pnlLineJournal.Visible = true;
                    lnkJournal.Text = "Hide Journal";
                }
                //if (lnkJournal != null)
                //    lnkJournal.Text = "Hide Journal";
            }
            if (e.CommandName == "AddJournal")
            {
                AddJournalLine(detailsID);
            }
           
        }
        protected void gvLineJournal_RowCommand(
                        object sender,
                        GridViewCommandEventArgs e)
        {
            try
            {
                // Get the clicked journal row
                LinkButton btn =
                    e.CommandSource as LinkButton;

                if (btn == null)
                    return;

                GridViewRow journalRow =
                    btn.NamingContainer as GridViewRow;

                if (journalRow == null)
                    return;

                // Get the nested GridView
                GridView gvJournal =
                    sender as GridView;

                if (gvJournal == null)
                    return;

                // Get the parent Payment Requisition row
                GridViewRow paymentRow =
                    gvJournal.NamingContainer as GridViewRow;

                if (paymentRow == null)
                    return;
                // Get PaymentRequisitionDetailsID
                HiddenField hfDetailsID =
                    paymentRow.FindControl(
                        "hfPaymentRequisitionDetailsID")
                    as HiddenField;

                if (hfDetailsID == null)
                {
                    lblMessage.Text =
                        "Payment Requisition Details ID not found.";

                    lblMessage.Visible = true;
                    return;
                }

                long detailsID;

                if (!long.TryParse(
                    hfDetailsID.Value,
                    out detailsID))
                {
                    // Save the complete journal for this requisition
                    SaveJournal(detailsID);
                }
                if (e.CommandName == "SaveJournal")
                {
                    long journalLineID;

                    if (!long.TryParse(
                        Convert.ToString(e.CommandArgument),
                        out journalLineID))
                    {
                        lblMessage.Text = "Invalid Journal Line ID.";
                        lblMessage.Visible = true;
                        return;
                    }

                    // Save the complete journal for this requisition
                    SaveJournal(detailsID);
                }
                else if (e.CommandName == "DeleteJournal")
                {
                    long journalLineID;

                    if (!long.TryParse(
                        Convert.ToString(e.CommandArgument),
                        out journalLineID))
                    {
                        return;
                    }

                    _cm.DeleteJournalLine(journalLineID);
                  
                    BindLineJournal(
                        gvJournal,
                        detailsID);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Journal operation failed: " +
                    ex.Message;

                lblMessage.Visible = true;
            }
        }
        private void BindLineJournal(GridView gvLineJournal,long detailsID)
        {
            DataTable dt = _cm.GetJournalLines(detailsID);

            gvLineJournal.DataSource = dt;
            gvLineJournal.DataBind();
        }
        protected void gvLineJournal_RowDataBound(
                object sender,
                GridViewRowEventArgs e)
                    {
                        if (e.Row.RowType != DataControlRowType.DataRow)
                            return;

                        DropDownList ddl =
                            (DropDownList)e.Row.FindControl("ddlAccount");

                        if (ddl == null)
                            return;

                        BindCOA(ddl);

                        DataRowView row =
                            (DataRowView)e.Row.DataItem;

                        if (row.DataView.Table.Columns.Contains("AccountID"))
                        {
                            string accountID =
                                row["AccountID"].ToString();

                            if (ddl.Items.FindByValue(accountID) != null)
                            {
                                ddl.SelectedValue = accountID;
                            }
                        }
                    }
        private void BindCOA(DropDownList ddl)
        {
            DataTable dt = _gl.GetCOA("Accounts Payable");

            ddl.Items.Clear();
            ddl.Items.Add(
                new ListItem("-- Select Account --", "")
            );

            foreach (DataRow row in dt.Rows)
            {
                string accountID = row["AccountID"].ToString();

                string text =
                    row["AccountCode"].ToString()
                    + " - "
                    + row["AccountName"].ToString();

                ddl.Items.Add(
                    new ListItem(text, accountID)
                );
            }
        }

        private void AddJournalLine(long detailsID)
        {
            try
            {
                // Get JournalID from JournalHeader
                long journalID = _cm.GetJournalIDForPayment(detailsID);

                if (journalID <= 0)
                {
                    string createdBy = Session["UserName"] != null
                            ? Session["UserName"].ToString()
                            : "System";
                    journalID = _cm.CreatePaymentJournalHeader(detailsID, createdBy);

                    if (journalID <= 0)
                    {
                        lblMessage.Text = "Unable to create Journal Header.";
                        lblMessage.Visible = true;
                        lblMessage.ForeColor = Color.Red;
                        return;
                    }
                }

                // Get next sequence for this journal
                int nextSequence = _cm.GetNextJournalSequence(journalID);

                // Insert blank journal line
                long journalLineID = _cm.AddJournalLine(
                    journalID,
                    detailsID,
                    nextSequence);

                if (journalLineID <= 0)
                {
                    lblMessage.Text = "Unable to add journal line.";
                    lblMessage.Visible = true;
                    return;
                }

                // Rebind payment requisition GridView
                BindRequisition();

                // Find the newly created parent row
                foreach (GridViewRow row in gvPaymentRequisition.Rows)
                {
                    HiddenField hf =
                        row.FindControl("hfPaymentRequisitionDetailsID")
                        as HiddenField;

                    if (hf == null)
                        continue;

                    long rowDetailsID;

                    if (!long.TryParse(hf.Value, out rowDetailsID))
                        continue;

                    if (rowDetailsID != detailsID)
                        continue;

                    // Open journal panel
                    Panel pnl =
                        row.FindControl("pnlLineJournal")
                        as Panel;

                    if (pnl != null)
                        pnl.Visible = true;

                    // Get nested journal GridView
                    GridView gvJournal =
                        row.FindControl("gvLineJournal")
                        as GridView;

                    if (gvJournal != null)
                    {
                        BindLineJournal(
                            gvJournal,
                            detailsID);
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Unable to add journal entry: " + ex.Message;

                lblMessage.Visible = true;
            }
        }

        private void SaveJournal(long detailsID)
        {
            try
            {
                GridViewRow parentRow = null;

                // Find the payment requisition row
                foreach (GridViewRow row in gvPaymentRequisition.Rows)
                {
                    HiddenField hfDetailsID =
                        row.FindControl("hfPaymentRequisitionDetailsID")
                        as HiddenField;

                    if (hfDetailsID == null)
                        continue;

                    long rowDetailsID;

                    if (!long.TryParse(
                        hfDetailsID.Value,
                        out rowDetailsID))
                        continue;

                    if (rowDetailsID == detailsID)
                    {
                        parentRow = row;
                        break;
                    }
                }

                if (parentRow == null)
                {
                    ShowMessage(
                        "Payment requisition line not found.",
                        "danger");

                    return;
                }


                // Find nested journal GridView
                GridView gvJournal =
                    parentRow.FindControl("gvLineJournal")
                    as GridView;

                if (gvJournal == null)
                {
                    ShowMessage(
                        "Journal grid not found.",
                        "danger");

                    return;
                }


                decimal totalDebit = 0;
                decimal totalCredit = 0;

                foreach (GridViewRow row in gvJournal.Rows)
                {
                    TextBox txtDebit =
                        row.FindControl("txtDebit") as TextBox;

                    TextBox txtCredit =
                        row.FindControl("txtCredit") as TextBox;

                    decimal debit = 0;
                    decimal credit = 0;

                    if (txtDebit != null)
                    {
                        decimal.TryParse(
                            txtDebit.Text,
                            out debit);
                    }

                    if (txtCredit != null)
                    {
                        decimal.TryParse(
                            txtCredit.Text,
                            out credit);
                    }

                    totalDebit += debit;
                    totalCredit += credit;
                }

                decimal payAmount = 0;

                decimal.TryParse(
                    txtAmount.Text,
                    out payAmount);

                decimal difference = payAmount - totalDebit;

                if (Math.Abs(difference) > 0.01m)
                {
                    ShowMessage(
                        "Total Debit must equal Pay Amount. " +
                        "Pay Amount = " + payAmount.ToString("N2") +
                        ", Total Debit = " + totalDebit.ToString("N2") +
                        ", Difference = " + difference.ToString("N2"),
                        "danger");

                    return;
                }
                // Must have at least one journal line
                if (gvJournal.Rows.Count == 0)
                {
                    ShowMessage(
                        "Please add at least one journal entry.",
                        "danger");

                    return;
                }

/*
                // Journal must balance
                difference =
                    Math.Abs(totalDebit - totalCredit);


                if (difference > 0.01m)
                {
                    ShowMessage(
                        "Journal is not balanced. " +
                        "Debit = " +
                        totalDebit.ToString("N2") +
                        ", Credit = " +
                        totalCredit.ToString("N2") +
                        ", Difference = " +
                        difference.ToString("N2"),
                        "danger");

                    return;
                }
*/

                // Save all journal lines
                foreach (GridViewRow row in gvJournal.Rows)
                {
                    DropDownList ddlAccount =
                        row.FindControl("ddlAccount")
                        as DropDownList;

                    TextBox txtDescription =
                        row.FindControl("txtJournalDescription")
                        as TextBox;

                    TextBox txtDebit =
                        row.FindControl("txtDebit")
                        as TextBox;

                    TextBox txtCredit =
                        row.FindControl("txtCredit")
                        as TextBox;

                    HiddenField hfJournalLineID =
                        row.FindControl("hfJournalLineID")
                        as HiddenField;


                    long journalLineID;

                    if (!long.TryParse(
                        hfJournalLineID.Value,
                        out journalLineID))
                    {
                        continue;
                    }


                    long accountID =
                        Convert.ToInt64(
                            ddlAccount.SelectedValue);


                    decimal debit = 0;
                    decimal credit = 0;


                    decimal.TryParse(
                        txtDebit.Text,
                        out debit);

                    decimal.TryParse(
                        txtCredit.Text,
                        out credit);


                    string description =
                        txtDescription != null
                            ? txtDescription.Text.Trim()
                            : "";


                    _cm.UpdateJournalLine(
                        journalLineID,
                        accountID,
                        description,
                        debit,
                        credit);
                }


                // Update journal totals
                _cm.UpdateJournalTotals(
                    detailsID,
                    totalDebit,
                    totalCredit);


                // Successfully saved
                ShowMessage(
                    "Journal saved successfully.",
                    "success");


                // Refresh the journal
                BindLineJournal(
                    gvJournal,
                    detailsID);
            }
            catch (Exception ex)
            {
                ShowMessage(
                    "Error saving journal: " + ex.Message,
                    "danger");
            }
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;

            if (type == "success")
            {
                lblMessage.CssClass = "alert alert-success";
            }
            else if (type == "danger")
            {
                lblMessage.CssClass = "alert alert-danger";
            }
            else if (type == "warning")
            {
                lblMessage.CssClass = "alert alert-warning";
            }
            else
            {
                lblMessage.CssClass = "alert alert-info";
            }
        }
        protected void btnSaveJournal_Click(object sender, EventArgs e)
        {
            long detailsID = 0;

            Button btn = sender as Button;

            if (btn == null)
                return;

            GridViewRow paymentRow =
                btn.NamingContainer as GridViewRow;

            if (paymentRow == null)
                return;

            HiddenField hf =
                paymentRow.FindControl(
                    "hfPaymentRequisitionDetailsID")
                as HiddenField;

            if (hf == null ||
                !long.TryParse(hf.Value, out detailsID))
            {
                ShowMessage(
                    "Payment Requisition Details ID not found.",
                    "danger");

                return;
            }

            SaveJournal(detailsID);
        }
        protected void lnkSaveJournal_Click(
     object sender,
     EventArgs e)
        {
            try
            {
                LinkButton btn =
                    sender as LinkButton;

                if (btn == null)
                    return;

                long detailsID;

                if (!long.TryParse(
                    btn.CommandArgument,
                    out detailsID))
                {
                    ShowMessage(
                        "Invalid Payment Requisition Details ID.",
                        "danger");

                    return;
                }

                SaveJournal(detailsID);
            }
            catch (Exception ex)
            {
                ShowMessage(
                    "Unable to save journal: " + ex.Message,
                    "danger");
            }
        }

        protected void btnSave_Click1(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtDate.Text == "") { txtDate.BackColor = Color.LightPink; flag = 1; } else { txtDate.BackColor = Color.Empty; }
            if (txtAmount.Text == "") { txtAmount.BackColor = Color.LightPink; flag = 1; } else { txtAmount.BackColor = Color.Empty; }
            if (txtReference.Text == "") { txtReference.BackColor = Color.LightPink; flag = 1; } else { txtReference.BackColor = Color.Empty; }
            //if (ddlPayeeType.SelectedIndex == 0) { ddlPayeeType.BackColor = Color.LightPink; flag = 1; } else { ddlPayeeType.BackColor = Color.Empty; }
            if (ddlPayee.SelectedIndex == 0) { ddlPayee.BackColor = Color.LightPink; flag = 1; } else { ddlPayee.BackColor = Color.Empty; }
            if (ddlPayee.SelectedIndex == 0) { ddlPayee.BackColor = Color.LightPink; flag = 1; } else { ddlPayee.BackColor = Color.Empty; }
            if (flag == 0)
            {
                DataTable dt = Session["GridData"] as DataTable;

                if (dt.Rows.Count > 0)
                {
                    _ap.UpdatePaymentRequisitionPayment(dt);
                }
                //Journal Entry
                //--------------
                //_jl.AccountID = hdCOAAccountCode.Value;
                //_jl.JournalDate = Convert.ToDateTime(txtDate.Text);
                //_jl.ReferenceNo = txtReference.Text;
                //_jl.Transaction = "Payment";
                //_jl.Description = ddlPayee.SelectedItem.ToString();
                //if (Session["EmployeeName"] != null)
                //{
                //    _jl.CreatedBy = Session["EmployeeName"].ToString();
                //}
                //else
                //{
                //    _jl.CreatedBy = "Accounts User";
                //}

                //DataTable journalHeader = _jl.JournalHeader();
                //journalHeader.Rows.Add(_jl.JournalDate, _jl.ReferenceNo, _jl.Transaction, _jl.Description, _jl.CreatedBy);

                //DataTable journalLines = _jl.JournalLines();

                ////journalLines.Rows.Add("", Convert.ToDecimal(txtAmount.Text), 0, "Accounts Payable", _jl.AccountID);
                //decimal amount = Convert.ToDecimal(txtAmountPaid.Text);
                //journalLines.Rows.Add(
                //        "",
                //        amount,
                //        0,
                //        "Payment Requisition",
                //        dt.Rows[0]["COAAccountIDDr"].ToString()
                //    );

                //journalLines.Rows.Add(
                //    "",
                //    0,
                //    amount,
                //    "Payment Requisition",
                //    dt.Rows[0]["COAAccountIDCr"].ToString()
                //);

                //int JournalID = _jl.AddJournalWithGL(journalHeader, journalLines);

                ////Bank Statement Entry
                ////--------------
                //DataTable dtBank = _gl.GetBankAndCashAccounts(dt.Rows[0]["BankAccountID"].ToString());//jini
                //if (dtBank != null)
                //{
                //    if (dtBank.Rows[0]["AccountType"].ToString() == "Bank")
                //    {
                //        _ap.Number = "PQI-BK/" + dt.Rows[0]["BankAccountCode"].ToString(); //+ ddlAccount.SelectedItem.ToString();
                //        _bk.BankAccountId = dt.Rows[0]["BankAccountID"].ToString(); //ddlAccount.SelectedValue;
                //        _bk.StatementDate = Convert.ToDateTime(txtDate.Text);

                //        DataTable dtBankBalance = _bk.GetBankBalance(_bk);
                //        if (dtBankBalance.Rows.Count > 0)
                //        {
                //            _bk.OpeningBalance = Convert.ToDecimal(dtBankBalance.Rows[0]["OpeningBalance"]);
                //            _bk.ClosingBalance = Convert.ToDecimal(dtBankBalance.Rows[0]["ClosingBalance"]);
                //        }
                //        else
                //        {
                //            _bk.OpeningBalance = 0;
                //            _bk.ClosingBalance = 0;
                //        }
                //        DataTable bankingHeader = _bk.BankingHeader();
                //        bankingHeader.Rows.Add(_bk.BankAccountId, _bk.StatementDate, _bk.ClosingBalance, _bk.ClosingBalance - Convert.ToDecimal(txtAmount.Text), DateTime.Now);

                //        DataTable bankingLines = _bk.BankingLines();
                //        bankingLines.Rows.Add(Convert.ToDateTime(txtDate.Text), "Payment to Vendor", Convert.ToDecimal(txtAmount.Text), 0, _bk.ClosingBalance - Convert.ToDecimal(txtAmount.Text));
                //        int StatementID = _bk.AddBankStatement(bankingHeader, bankingLines);

                //        //Bank Reconcilation
                //        //--------------
                //        DataTable bankRecon = _bk.BankReconciliation();
                //        bankRecon.Rows.Add(StatementID, JournalID, Convert.ToDateTime(txtDate.Text), Session["EmployeeName"].ToString(), txtReference.Text);
                //        _bk.AddBankReconciliation(bankRecon);

                //    }
                //    else { _ap.Number = "PQI-CASH-BK"; }
                //}

                //Payment Entry
                //--------------

                _ap.PayeeId = Convert.ToInt32(ddlPayee.SelectedValue);
                _ap.PaymentDate = Convert.ToDateTime(txtDate.Text);
                _ap.PaymentAmount = Convert.ToDecimal(txtAmount.Text);
                _ap.PrevPaymentAmount = Convert.ToDecimal(txtAmountPaid.Text);
                _ap.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text);
                _ap.PaymentMethod = "";
                _ap.ReferenceNo = txtReference.Text;
                _ap.Notes = txtNotes.Text;
                _ap.JournalId = 0;// JournalID;
                _ap.CreatedOn = DateTime.Now;
                _ap.PayeeType = ddlPayeeType.SelectedItem.ToString();
                _ap.PayeeName = ddlPayee.SelectedItem.ToString();
                if (ddlPaymentRequisition.SelectedIndex != 0) { _ap.PONumber = ddlPaymentRequisition.SelectedItem.Text; } else { _ap.PONumber = ""; }
                _ap.SupplierInvoiceNo = "";
                if (ddlPaymentRequisition.SelectedIndex != 0)
                {
                    _ap.PurchaseOrderID = Convert.ToInt32(ddlPaymentRequisition.SelectedValue.ToString());
                    _ap.prVar.PaymentRequisitionID = ddlPaymentRequisition.SelectedValue.ToString();
                }
                else { _ap.PurchaseOrderID = 0; }
                _ap.Year = _ap.PaymentDate.ToString("yyyy");
                DataTable dtLast = _ap.GetLast("Payments", "VoucherYear", _ap.Year);
                if (dtLast.Rows.Count == 0) { _ap.Counter = 1; }
                else
                {
                    _ap.Counter = dtLast.Rows.Count + 1;
                }
                _ap.Number = _ap.Counter.ToString("D4") + "-" + _ap.PaymentDate.ToString("MM") + "-" + _ap.Year + "/" + _ap.Number;



                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }

                else
                {
                    int paymentID = _ap.AddPayment(_ap, dt);
                    if (txtAmountPaid.Text != "")
                    {
                        //Update amount paid in Invoices and InvoiceAR tables
                        _ap.prVar.AmountPaid = Convert.ToDecimal(txtAmountPaid.Text) + Convert.ToDecimal(txtAmount.Text);
                        _ap.UpdatePRAmountPaid(_ap);

                        Response.Redirect("generateVoucher.aspx?o=2&x=" + paymentID.ToString());
                    }
                }
            }
        }
    }

}
