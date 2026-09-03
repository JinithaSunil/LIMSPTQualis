using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class GeneralLedger : System.Web.UI.Page
	{
        DAL.GLModule _gl = new DAL.GLModule();
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                LoadAccounts();
            }
        }
        protected void gvGeneralLedger_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                decimal balance =
                    Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "RunningBalance"));

                e.Row.Cells[6].Text = FormatDrCr(balance);
            }
        }
        private string FormatDrCr(decimal amount)
        {
            if (amount > 0)
                return $"{Math.Abs(amount):N2} Dr";
            else if (amount < 0)
                return $"{Math.Abs(amount):N2} Cr";
            else
                return "0.00";
        }
        private void LoadAccounts()
        {
            ddlAccount.DataSource = _gl.GetBankAndCashAccounts();
            ddlAccount.DataTextField = "AccountName";
            ddlAccount.DataValueField = "AccountID";
            ddlAccount.DataBind();
            ddlAccount.Items.Insert(0, "Select Account");
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int accountId = Convert.ToInt32(ddlAccount.SelectedValue);
            DateTime fromDate = Convert.ToDateTime(txtFromDate.Text);
            DateTime toDate = Convert.ToDateTime(txtToDate.Text);

            decimal openingBalance = _gl.GetOpeningBalance(accountId, fromDate);

            lblOpeningBalance.Text = FormatDrCr(openingBalance);

            var ledger = _gl.GetGeneralLedger(accountId, fromDate, toDate, openingBalance);

            gvGeneralLedger.DataSource = ledger;
            gvGeneralLedger.DataBind();
        }

    }
}