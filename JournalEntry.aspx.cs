using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;
using static LIMS.DAL.Journal;
using static LIMS.DAL.GLModule;
using System.Diagnostics;
namespace LIMS
{
    public partial class JournalEntry : System.Web.UI.Page
    {
        DAL.GLModule _gl = new GLModule();
        DAL.APModule _ap = new DAL.APModule();
        DAL.ARModule _ar = new DAL.ARModule();
        DAL.Journal _jl = new DAL.Journal();
        DAL.BankingModule _bk = new DAL.BankingModule();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }
        private DataTable Lines
        {
            get
            {
                if (ViewState["Lines"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("JournalID");
                    dt.Columns.Add("AccountID");
                    dt.Columns.Add("Debit", typeof(decimal));
                    dt.Columns.Add("Credit", typeof(decimal));
                    dt.Columns.Add("LineDescription");
                    dt.Rows.Add("","Suspense", 0, 0,"");
                    dt.Rows.Add("", "Suspense", 0, 0,"");
                    ViewState["Lines"] = dt;
                }
                return (DataTable)ViewState["Lines"];
            }
            set { ViewState["Lines"] = value; }
        }

        private void BindGrid()
        {
            gvLines.DataSource = Lines;
            gvLines.DataBind();
        }

        protected void btnAddLine_Click(object sender, EventArgs e)
        {
            SaveGridToDataTable();
            Lines.Rows.Add("", "Suspense", 0, 0, "");
            BindGrid();
        }

        protected void gvLines_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                SaveGridToDataTable();
                Lines.Rows.RemoveAt(Convert.ToInt32(e.CommandArgument));
                BindGrid();
            }
        }

        private void SaveGridToDataTable()
        {
            int flag = 0;
            DataTable dt = Lines.Clone();
            foreach (System.Web.UI.WebControls.GridViewRow row in gvLines.Rows)
            {
                var ddl = (System.Web.UI.WebControls.DropDownList)row.FindControl("ddlAccount");
                var debit = (System.Web.UI.WebControls.TextBox)row.FindControl("txtDebit");
                var credit = (System.Web.UI.WebControls.TextBox)row.FindControl("txtCredit");
                var lineDescription = (System.Web.UI.WebControls.TextBox)row.FindControl("txtNarration");

                if (ddl.SelectedIndex == 0)
                {
                    ddl.BackColor = Color.LightPink;flag = 1;
                }
                if (debit.Text == "0" && credit.Text == "0") { debit.BorderColor = Color.Red;flag = 1; } 
                if (debit.Text == null && credit.Text == null) { credit.BorderColor = Color.Red;flag = 1;  }
                if (flag == 0)
                {
                    dt.Rows.Add("",ddl.SelectedValue,
                                       string.IsNullOrEmpty(debit.Text) ? 0 : Convert.ToDecimal(debit.Text),
                                       string.IsNullOrEmpty(credit.Text) ? 0 : Convert.ToDecimal(credit.Text),
                                       txtNarration.Text
                                   );
                }
               
            }
            Lines = dt;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveGridToDataTable();
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (DataRow row in Lines.Rows)
            {
                string account = row["AccountID"].ToString();
                decimal debit = row.Field<decimal>("Debit");
                decimal credit = row.Field<decimal>("Credit");
                totalDebit += debit;
                totalCredit += credit;
                // must have account selected
                if (string.IsNullOrWhiteSpace(account))
                {                    
                    lblMessage.Text = "Error: Account cannot be blank.";
                    return;
                }

                // at least one side must be > 0
                else if (debit == 0 && credit == 0)
                {
                    lblMessage.Text = "Error: Each line must have a Debit or Credit amount.";
                    return;
                }

                // prevent both debit & credit entered
                else if (debit > 0 && credit > 0)
                {
                    lblMessage.Text = "Error: A line cannot have both Debit and Credit.";
                    return;
                }
            }
            if (totalCredit != totalDebit)
            {
                lblMessage.Text = $"Journal not balanced! Debit={totalDebit}, Credit={totalCredit}";
                return;
            }
            else
            {

                // TODO: Save Journal Header & Lines to DB
                // Header: txtDate.Text, txtRef.Text, ddlCurrency.SelectedValue, txtNarration.Text
                // Lines: Lines DataTable

                //Journal Entry
                //--------------
                
                _jl.JournalDate = Convert.ToDateTime(txtDate.Text);
                _jl.ReferenceNo = txtReference.Text;
                _jl.Transaction = "Payment";
                _jl.Description = txtNarration.Text;
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

                int journalID = _jl.AddJournalHeader(journalHeader);

                DataTable journalLines = _jl.JournalLines();
                foreach (DataRow row in Lines.Rows)
                {
                    string JournalID = journalID.ToString();
                    string account = row["AccountID"].ToString();
                    decimal debit = row.Field<decimal>("Debit");
                    decimal credit = row.Field<decimal>("Credit");
                    string narration = row["LineDescription"].ToString();
                    journalLines.Rows.Add(JournalID,debit, credit, narration, account);                    
                }
                _jl.AddJournalLines(journalLines);
                _jl.AddGeneralLedger(journalID);
                //int JournalID = _jl.AddJournalWithGL(journalHeader, journalLines);
                Clear();
            }
        }

        protected void gvLines_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlAccount = (e.Row.FindControl("ddlAccount") as DropDownList);
                BindCOA(ddlAccount);    
            }
        }
        protected void BindCOA(DropDownList ddlCOA)
        {
            ddlCOA.DataSource = _gl.GetCOA();
            ddlCOA.DataTextField = "AccountName1";
            ddlCOA.DataValueField = "AccountID";
            ddlCOA.DataBind();
            ddlCOA.Items.Insert(0, "Select Account");
        }
        protected void Clear()
        {
            txtDate.Text = "";
            txtReference.Text = "";
            txtNarration.Text = "";
        }
    }
}
