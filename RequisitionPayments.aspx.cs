using DocumentFormat.OpenXml.Math;
using LIMS.DAL;
using PuppeteerSharp.Mobile;
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
                Session.Remove("DT");

                if (Session["prVariables"] != null)
                {
                    //BindRequisition();
                    LoadPayeeDetails();
                    ToggleSaveButton();
                }
                else
                {
                    LoadPayments();
                }
            }
        }
        protected void LoadBanks(DropDownList ddlBank)
        {
            DataTable dt = _cm.GetCOAAccountDetails("Bank");
            if (dt.Rows.Count > 0)
            {
                ddlBank.DataSource = dt;
                ddlBank.DataTextField = "AccountName";
                ddlBank.DataValueField = "AccountID";
                ddlBank.DataBind();
                ddlBank.Items.Insert(0, "Select Bank");
            }
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
            BindPaymentRequisitions();
            ddlPaymentRequisition.ClearSelection();
            ddlPaymentRequisition.Items.FindByText(_ap.prVar.PaymentRequisitionNumber).Selected = true;
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlPaymentRequisition.Items.FindByText(_ap.prVar.PaymentRequisitionNumber);
            ddlPaymentRequisition.Enabled = false;
            //txtTotalAmount.Text = _ap.prVar.TotalAmount.ToString();
            //txtAmountPaid.Text = _ap.prVar.PrevAmountPaid.ToString();
            //txtPayAmount.Text = (_ap.prVar.TotalAmount - _ap.prVar.PrevAmountPaid).ToString("0.00");
            //txtBalance.Text = (_ap.prVar.TotalAmount - _ap.prVar.PrevAmountPaid).ToString("0.00");
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

        protected void BindPaymentRequisitions()
        {
            ViewState.Clear();
            DataTable dtPR = _ap.GetEmployeePaymentRequisition(hdPayeeID.Value);
            if (dtPR.Rows.Count > 0)
            {
                ddlPaymentRequisition.DataSource = dtPR;
                ddlPaymentRequisition.DataTextField = "RequisitionNumber";
                ddlPaymentRequisition.DataValueField = "PaymentRequisitionID";
                ddlPaymentRequisition.DataBind();
                ddlPaymentRequisition.Items.Insert(0, "Select Payment Requisition");
                ViewState["PaymentRequisition"] = dtPR;
            }
            else
            {
                ddlPaymentRequisition.DataSource = null;
                ddlPaymentRequisition.DataBind();
            }
        }
        private bool AllRowsHaveRequiredValues()
        {
            DataTable dt = Session["GridData"] as DataTable;

            if (dt == null || dt.Rows.Count == 0)
                return false;

            decimal diff = 0;
            //if (txtDifference.Text != "")
            //{
            //    diff = Convert.ToDecimal(txtDifference.Text);
            //}
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

        protected void txtPayAmount_TextChanged(object sender, EventArgs e)
        {
            CalculatePaymentAmounts();
        }

        protected void ddlPaymentRequisition_SelectedIndexChanged(object sender, EventArgs e)
        {
            int paymentRequisitionID = Convert.ToInt32(ddlPaymentRequisition.SelectedValue);

            // LoadPaymentInformation(paymentRequisitionID);

            //LoadPaymentRequisitionItems(paymentRequisitionID);
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
        //        txtPayAmount.Text = txtBalance.Text;

        //        txtDifference.Text = "0.00";

        //        txtNotes.Text = dr["Notes"].ToString();
        //    }
        //private void LoadPaymentRequisitionItems(int paymentRequisitionID)
        //{
        //    DataTable dt = _cm.GetPaymentRequisitionDetails(paymentRequisitionID);

        //    gvPaymentRequisition.DataSource = dt;
        //    gvPaymentRequisition.DataBind();
        //}


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
        protected void gvPaymentRequisition_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            long detailsID;

            if (!long.TryParse(e.CommandArgument.ToString(), out detailsID))
            {
                return;
            }

            if (e.CommandName.Equals("Journal", StringComparison.OrdinalIgnoreCase))
            {
                int requisitionLineID;

                if (!int.TryParse(e.CommandArgument.ToString(), out requisitionLineID))
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
                DataTable dtJournal = CreateJournalTable();

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
                // AddJournalLine(detailsID);
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
                    //SaveJournal(detailsID);
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
                    // SaveJournal(detailsID);
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
        private void BindLineJournal(GridView gvLineJournal, long detailsID)
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

            //BindCOA(ddl);

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
        private void BindCOA(DropDownList ddl, string AccountType)
        {
            DataTable dt = _gl.GetCOA(AccountType);

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

            //SaveJournal(detailsID);
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

                //SaveJournal(detailsID);
            }
            catch (Exception ex)
            {
                ShowMessage(
                    "Unable to save journal: " + ex.Message,
                    "danger");
            }
        }
        private DataTable CreatePaymentJournalTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("COAAccountCode", typeof(string));
            dt.Columns.Add("COAAccountIDDr", typeof(long));
            dt.Columns.Add("BankAccountCode", typeof(string));
            dt.Columns.Add("COAAccountNameDr", typeof(string));
            dt.Columns.Add("AmountDr", typeof(decimal));

            dt.Columns.Add("COAAccountIDCr", typeof(long));
            dt.Columns.Add("COAAccountCodeCr", typeof(string));
            dt.Columns.Add("COAAccountNameCr", typeof(string));
            dt.Columns.Add("AmountCr", typeof(decimal));

            return dt;
        }
        protected void btnSave_Click1(object sender, EventArgs e)
        {
            int flag = 0; DataTable dtPaymentJournal = new DataTable();
            if (txtDate.Text == "") { txtDate.BackColor = Color.LightPink; flag = 1; } else { txtDate.BackColor = Color.Empty; }
            if (txtReference.Text == "") { txtReference.BackColor = Color.LightPink; flag = 1; } else { txtReference.BackColor = Color.Empty; }
            if (ddlPayee.SelectedIndex == 0) { ddlPayee.BackColor = Color.LightPink; flag = 1; } else { ddlPayee.BackColor = Color.Empty; }
            if (ddlPayee.SelectedIndex == 0) { ddlPayee.BackColor = Color.LightPink; flag = 1; } else { ddlPayee.BackColor = Color.Empty; }
            if (flag == 0)
            {
                //Payment Entry
                //--------------
                try
                {
                    dtPaymentJournal = CreatePaymentJournalTable();
                    foreach (GridViewRow row in gvPaymentRequisitions.Rows)
                    {
                        if (row.RowType != DataControlRowType.DataRow)
                            continue;

                        // -------------------------------------------------
                        // Get controls
                        // -------------------------------------------------

                        TextBox txtDebitPayAmount =
                            row.FindControl("txtDebitPayAmount")
                            as TextBox;

                        DropDownList ddlDebitCOA =
                            row.FindControl("ddlDebitCOA")
                            as DropDownList;

                        TextBox txtCreditPayAmount =
                            row.FindControl("txtCreditPayAmount")
                            as TextBox;

                        DropDownList ddlCreditCOA =
                            row.FindControl("ddlCreditCOA")
                            as DropDownList;

                        DropDownList ddlBank =
                            row.FindControl("ddlBank")
                            as DropDownList;

                        CheckBox chkPR =
                            row.FindControl("chkPR")
                            as CheckBox;

                        HiddenField hfPRID =
                            row.FindControl("hfPaymentRequisitionID")
                            as HiddenField;

                        // -------------------------------------------------
                        // Process only selected PR
                        // -------------------------------------------------

                        if (chkPR == null || !chkPR.Checked)
                            continue;

                        long paymentRequisitionID = 0;

                        if (hfPRID != null)
                        {
                            long.TryParse(
                                hfPRID.Value,
                                out paymentRequisitionID);
                        }

                        // -----------------------------
                        // Debit
                        // -----------------------------

                        decimal amountDr = 0;

                        decimal.TryParse(
                            txtDebitPayAmount?.Text,
                            out amountDr);

                        long coaAccountIDDr = 0;

                        long.TryParse(
                            ddlDebitCOA?.SelectedValue,
                            out coaAccountIDDr);


                        string coaAccountCodeDr = "";

                        string coaAccountNameDr = "";

                        if (ddlDebitCOA != null &&
                            ddlDebitCOA.SelectedIndex > 0)
                        {
                            ListItem item =
                                ddlDebitCOA.SelectedItem;

                            coaAccountCodeDr =
                                item.Text.Contains(" - ")
                                    ? item.Text.Split(
                                        new[] { " - " },
                                        StringSplitOptions.None)[0]
                                    : item.Text;

                            coaAccountNameDr =
                                item.Text.Contains(" - ")
                                    ? item.Text.Substring(
                                        item.Text.IndexOf(" - ") + 3)
                                    : item.Text;
                        }


                        // -----------------------------
                        // Credit
                        // -----------------------------

                        decimal amountCr = 0;

                        decimal.TryParse(
                            txtCreditPayAmount?.Text,
                            out amountCr);

                        long coaAccountIDCr = 0;

                        long.TryParse(
                            ddlCreditCOA?.SelectedValue,
                            out coaAccountIDCr);


                        string coaAccountCodeCr = "";

                        string coaAccountNameCr = "";

                        if (ddlCreditCOA != null &&
                            ddlCreditCOA.SelectedIndex > 0)
                        {
                            ListItem item =
                                ddlCreditCOA.SelectedItem;

                            coaAccountCodeCr =
                                item.Text.Contains(" - ")
                                    ? item.Text.Split(
                                        new[] { " - " },
                                        StringSplitOptions.None)[0]
                                    : item.Text;

                            coaAccountNameCr =
                                item.Text.Contains(" - ")
                                    ? item.Text.Substring(
                                        item.Text.IndexOf(" - ") + 3)
                                    : item.Text;
                        }


                        // -----------------------------
                        // Bank
                        // -----------------------------

                        string bankAccountCode = "";

                        if (ddlBank != null &&
                            ddlBank.SelectedIndex > 0)
                        {
                            bankAccountCode =
                                ddlBank.SelectedItem.Text;
                        }


                        // -----------------------------
                        // Add row
                        // -----------------------------

                        DataRow dr =
                            dtPaymentJournal.NewRow();

                        dr["COAAccountCode"] =
                            coaAccountCodeDr;

                        dr["COAAccountIDDr"] =
                            coaAccountIDDr;

                        dr["BankAccountCode"] =
                            bankAccountCode;

                        dr["COAAccountNameDr"] =
                            coaAccountNameDr;

                        dr["AmountDr"] =
                            amountDr;

                        dr["COAAccountIDCr"] =
                            coaAccountIDCr;

                        dr["COAAccountCodeCr"] =
                            coaAccountCodeCr;

                        dr["COAAccountNameCr"] =
                            coaAccountNameCr;

                        dr["AmountCr"] =
                            amountCr;

                        dtPaymentJournal.Rows.Add(dr);
                    }

                }

                catch (Exception ex)
                {
                    ShowMessage(
                           "Unable to prepare payment journal: " +
                           ex.Message,
                           "danger");
                }

                if (dtPaymentJournal == null || dtPaymentJournal.Rows.Count == 0)
                {
                    return;
                }

                else
                {
                    _ap.PayeeId = Convert.ToInt32(ddlPayee.SelectedValue);
                    _ap.PaymentDate = Convert.ToDateTime(txtDate.Text);
                    _ap.PaymentAmount = Convert.ToDecimal(txtTotalAmount.Text);
                    _ap.PrevPaymentAmount = Convert.ToDecimal(txtAmountPaid.Text);
                    _ap.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text);
                    _ap.PaymentMethod = "";
                    _ap.ReferenceNo = txtReference.Text;
                    _ap.Notes = "";
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


                    int paymentID = _ap.AddPayment(_ap, dtPaymentJournal);
                    if (txtAmountPaid.Text != "")
                    {
                        //Update amount paid in Invoices and InvoiceAR tables
                        _ap.prVar.AmountPaid = Convert.ToDecimal(txtAmountPaid.Text) + Convert.ToDecimal(txtTotalAmount.Text);
                        _ap.UpdatePRAmountPaid(_ap);

                        Response.Redirect("generateVoucher.aspx?o=2&x=" + paymentID.ToString());
                    }
                }
            }
        }

        protected void radOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radOption.SelectedItem.ToString() == "Single") { ddlPaymentRequisition.Enabled = false; }
            if (radOption.SelectedItem.ToString() == "Multiple") { ddlPaymentRequisition.Enabled = true; }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (ddlPaymentRequisition.SelectedIndex != 0)
            {
                dt = _cm.GetPaymentRequisitionMaster(Convert.ToInt32(ddlPaymentRequisition.SelectedValue));
                DataTable dtSession;

                if (Session["DT"] == null)
                {
                    Session["DT"] = dt;
                }
                else
                {
                    dtSession = (DataTable)Session["DT"];
                    bool grnExists = dtSession.AsEnumerable()
                       .Any(r => r.Field<string>("RequisitionNumber") == ddlPaymentRequisition.SelectedItem.ToString());

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
                    gvPaymentRequisitions.DataSource = dt;
                    gvPaymentRequisitions.DataBind();
                }
                else
                {
                    gvPaymentRequisitions.DataSource = null;
                    gvPaymentRequisitions.DataBind();
                }
            }
            ClearAmounts();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Session.Remove("DT");
            gvPaymentRequisitions.DataSource = null;
            gvPaymentRequisitions.DataBind();

            ClearAmounts();
        }

        protected void ddlPayee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPayee.SelectedIndex != 0)
            {

            }
        }

        protected void btnAddAll_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (ViewState["PaymentRequisition"] != null)
            {
                dt = (DataTable)ViewState["PaymentRequisition"];
                gvPaymentRequisitions.DataSource = dt;
                gvPaymentRequisitions.DataBind();
            }
            else
            {
                gvPaymentRequisitions.DataSource = null;
                gvPaymentRequisitions.DataBind();
            }
            ClearAmounts();
        }
        protected void ClearAmounts()
        {
            txtTotalAmount.Text = "0.0";
            txtAmountPaid.Text = "0.0";
        }
        private List<int> GetSelectedPRIds()
        {
            List<int> selectedPRIds = new List<int>();

            foreach (GridViewRow row in gvPaymentRequisitions.Rows)
            {
                CheckBox chk = row.FindControl("chkPR") as CheckBox;

                if (chk != null && chk.Checked)
                {
                    int prId = Convert.ToInt32(
                        gvPaymentRequisitions.DataKeys[row.RowIndex].Value
                    );

                    selectedPRIds.Add(prId);
                }
            }

            return selectedPRIds;
        }
        protected void chkPR_CheckedChanged(
    object sender,
    EventArgs e)
        {
            decimal totalAmount = 0;
            decimal amountPaid = 0;

            foreach (GridViewRow row in gvPaymentRequisitions.Rows)
            {
                CheckBox chk =
                    row.FindControl("chkPR") as CheckBox;

                if (chk == null || !chk.Checked)
                    continue;

                int index = row.RowIndex;

                totalAmount +=
                    Convert.ToDecimal(
                        gvPaymentRequisitions.DataKeys[index]
                            ["TotalAmount"]);

                amountPaid +=
                    Convert.ToDecimal(
                        gvPaymentRequisitions.DataKeys[index]
                            ["AmountPaid"]);
            }

            txtTotalAmount.Text =
                totalAmount.ToString("0.00");

            txtAmountPaid.Text =
                amountPaid.ToString("0.00");
        }
        //protected void chkPR_CheckedChanged(object sender, EventArgs e)
        //{
        //    GetSelectedPRIds();
        //    CalculatePaymentAmounts();
        //}
        private void CalculatePaymentAmounts()
        {
            decimal totalAmount = 0;
            decimal amountPaid = 0;
            decimal payAmount = 0;
            decimal difference = 0;

            foreach (GridViewRow row in gvPaymentRequisitions.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow)
                    continue;

                CheckBox chkPR = row.FindControl("chkPR") as CheckBox;

                if (chkPR == null || !chkPR.Checked)
                    continue;

                // Get the PR amount from the GridView
                decimal prAmount = 0;
                decimal prAmountPaid = 0;

                if (gvPaymentRequisitions.DataKeys[row.RowIndex]["TotalAmount"] != null)
                {
                    decimal.TryParse(
                        gvPaymentRequisitions.DataKeys[row.RowIndex]["TotalAmount"].ToString(),
                        out prAmount
                    );
                }

                if (gvPaymentRequisitions.DataKeys[row.RowIndex]["AmountPaid"] != null)
                {
                    decimal.TryParse(
                        gvPaymentRequisitions.DataKeys[row.RowIndex]["AmountPaid"].ToString(),
                        out prAmountPaid
                    );
                }

                totalAmount += prAmount;
                amountPaid += prAmountPaid;
            }

            // Outstanding amount for selected PRs

            decimal balanceAmount = totalAmount - amountPaid;

            // Default payment amount = outstanding amount
            //if (txtPayAmount.Text != "")
            //{
            //    payAmount = Convert.ToDecimal(txtPayAmount.Text);
            //}
            //else
            //{
            //    payAmount = balanceAmount;
            //}

            // Difference
            balanceAmount = balanceAmount - payAmount;

            // Display
            txtTotalAmount.Text = totalAmount.ToString("#,##0.00");
            txtAmountPaid.Text = amountPaid.ToString("#,##0.00");
            // txtPayAmount.Text = payAmount.ToString("#,##0.00");
            // txtBalanceAmount.Text = balanceAmount.ToString("#,##0.00");
        }
        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSelectAll = sender as CheckBox;

            if (chkSelectAll == null)
                return;

            foreach (GridViewRow row in gvPaymentRequisitions.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkPR = row.FindControl("chkPR") as CheckBox;

                    if (chkPR != null)
                    {
                        chkPR.Checked = chkSelectAll.Checked;
                    }
                }
            }
            CalculatePaymentAmounts();
        }

        protected void txtTotalAmount_TextChanged(object sender, EventArgs e)
        {
            CalculatePaymentAmounts();
        }

        protected void gvPaymentRequisitions_RowDataBound(
       object sender,
       GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            decimal totalAmount = 0;
            decimal amountPaid = 0;

            DataRowView drv =
                e.Row.DataItem as DataRowView;

            if (drv != null)
            {
                if (drv["TotalAmount"] != DBNull.Value)
                {
                    decimal.TryParse(
                        drv["TotalAmount"].ToString(),
                        out totalAmount);
                }

                if (drv["AmountPaid"] != DBNull.Value)
                {
                    decimal.TryParse(
                        drv["AmountPaid"].ToString(),
                        out amountPaid);
                }
            }

            decimal remainingAmount =
                totalAmount - amountPaid;

            if (remainingAmount < 0)
                remainingAmount = 0;

            TextBox txtDebitPayAmount =
                e.Row.FindControl("txtDebitPayAmount")
                as TextBox;

            TextBox txtCreditPayAmount =
                e.Row.FindControl("txtCreditPayAmount")
                as TextBox;

            if (txtDebitPayAmount != null)
            {
                txtDebitPayAmount.Text =
                    remainingAmount.ToString("0.00");
            }

            if (txtCreditPayAmount != null)
            {
                txtCreditPayAmount.Text = "0.00";
            }

            // Your COA and Bank binding
            DropDownList ddlDebitCOA =
                e.Row.FindControl("ddlDebitCOA")
                as DropDownList;

            DropDownList ddlCreditCOA =
                e.Row.FindControl("ddlCreditCOA")
                as DropDownList;

            DropDownList ddlBank =
                e.Row.FindControl("ddlBank")
                as DropDownList;

            BindCOA(ddlDebitCOA, "Accounts Payable");
            BindCOA(ddlCreditCOA, "Accounts Receivable");
            LoadBanks(ddlBank);
        }
    }
}
