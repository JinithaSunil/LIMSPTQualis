using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS.UserControls
{    
    public partial class JournalEntry : System.Web.UI.UserControl
    {
        DAL.CM _cm = new DAL.CM(); 
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void LoadJournal(int detailId)
        {
            DataTable dt = _cm.LoadJournal(detailId);

            gvJournal.DataSource = dt;
            gvJournal.DataBind();

            CalculateTotals();
        }
        private void CalculateTotals()
        {
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (GridViewRow row in gvJournal.Rows)
            {
                TextBox txtDebit = row.FindControl("txtDebit") as TextBox;
                TextBox txtCredit = row.FindControl("txtCredit") as TextBox;

                decimal debit = 0;
                decimal credit = 0;

                if (txtDebit != null)
                    decimal.TryParse(txtDebit.Text, out debit);

                if (txtCredit != null)
                    decimal.TryParse(txtCredit.Text, out credit);

                totalDebit += debit;
                totalCredit += credit;
            }

            lblDebit.Text = totalDebit.ToString("N2");
            lblCredit.Text = totalCredit.ToString("N2");

            decimal difference = totalDebit - totalCredit;

            lblDifference.Text = difference.ToString("N2");

            if (difference == 0)
            {
                lblDifference.CssClass = "text-success fw-bold";
            }
            else
            {
                lblDifference.CssClass = "text-danger fw-bold";
            }
        }

        public DataTable GetJournalLines()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("LineNo");
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("Remarks");
            dt.Columns.Add("AccountID");

            foreach (GridViewRow row in gvJournal.Rows)
            {
                DropDownList ddlAccount = (DropDownList)row.FindControl("ddlAccount");
                DropDownList ddlType = (DropDownList)row.FindControl("ddlType");
                TextBox txtAmount = (TextBox)row.FindControl("txtAmount");
                TextBox txtRemarks = (TextBox)row.FindControl("txtRemarks");

                decimal amount = 0;
                decimal.TryParse(txtAmount.Text, out amount);

                decimal debit = 0;
                decimal credit = 0;

                if (ddlType.SelectedValue == "D")
                    debit = amount;
                else
                    credit = amount;

                dt.Rows.Add(
                    "",
                    debit,
                    credit,
                    txtRemarks.Text,
                    ddlAccount.SelectedValue
                );
            }

            return dt;
        }
    }
}