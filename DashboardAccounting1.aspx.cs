using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;

namespace LIMS
{
	public partial class DashboardAccounting1 : System.Web.UI.Page
	{
		DAL.CM _cm = new DAL.CM();
		DAL.GLModule _gl = new DAL.GLModule();
        DAL.APModule _ap = new DAL.APModule();
        DAL.ARModule _ar = new DAL.ARModule();
        protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindDashboard();
                DataTable dtPendingPayable = APModule.getPendingAPVendors();
                hypAP.Text = dtPendingPayable.Rows.Count.ToString();

                DataTable dtPendingReceipts = ARModule.getPendingARCustomers();
                hypAR.Text = dtPendingReceipts.Rows.Count.ToString();
                LoadIncomeExpense(2026);
            }
		}
		protected void BindDashboard()
		{
			DataTable dt = _cm.getInvoiceOverDue();
			if (dt.Rows.Count > 0) { spnODInv.InnerHtml = dt.Rows.Count.ToString(); }
			dt = _cm.getCashBalance();
			if(dt.Rows.Count > 0) { spnCashBalance.InnerHtml = _cm.ToIDR(Convert.ToDecimal(dt.Rows[0]["CashBalance"].ToString())); spnCashBal.InnerHtml = _cm.ToIDR(Convert.ToDecimal(dt.Rows[0]["CashBalance"].ToString())); }
            dt = _cm.getBankBalance();
            if (dt.Rows.Count > 0) { spnBankBal.InnerHtml = _cm.ToIDR(Convert.ToDecimal(dt.Rows[0]["BankBalance"].ToString())); }
            dt = _cm.getReceivables();
            if (dt.Rows.Count > 0) { spnReceivables.InnerHtml = _cm.ToIDR(Convert.ToDecimal(dt.Rows[0]["Receivables"].ToString())); }
            dt = _cm.getPayables();
            if (dt.Rows.Count > 0) { spnPayables.InnerHtml = _cm.ToIDR(Convert.ToDecimal(dt.Rows[0]["Payables"].ToString())); }
            dt = _cm.getPendingChequeClearance();
			if(dt.Rows.Count > 0) { spnChq.InnerHtml = dt.Rows.Count.ToString(); }
			DataTable[] dt1 = new DataTable[4];
            dt = _gl.getAccountActivityLog();
			if(dt.Rows.Count > 0)
			{
                rptActivity.DataSource = dt;
				rptActivity.DataBind();
            }
        }

        private void LoadIncomeExpense(int year)
        {
            DataTable dt = _gl.GetIncomeExpenseYearly(year);

            List<string> months = new List<string>();
            List<decimal> income = new List<decimal>();
            List<decimal> expense = new List<decimal>();

            decimal totalIncome = 0;
            decimal totalExpense = 0;

            foreach (DataRow row in dt.Rows)
            {
                months.Add(row["MonthName"].ToString());
                income.Add(Convert.ToDecimal(row["Income"]));
                expense.Add(Convert.ToDecimal(row["Expense"]));

                totalIncome += Convert.ToDecimal(row["Income"]);
                totalExpense += Convert.ToDecimal(row["Expense"]);
            }

            decimal profit = totalIncome - totalExpense;

            lblIncome.InnerText = _cm.ToIDR(totalIncome);
            lblExpense.InnerText = _cm.ToIDR(totalExpense);
            lblProfit.InnerText = _cm.ToIDR(profit);

            string status = profit >= 0 ? "PROFIT" : "LOSS";
            profitStatus.InnerText = "Status : " + status;
            profitStatus.Style["color"] = profit >= 0 ? "green" : "red";

            ScriptManager.RegisterStartupScript(
                this, GetType(), "chart",
                $"renderChart({ToJs(months)}, {ToJs(income)}, {ToJs(expense)});",
                true);
        }


        private string ToJs<T>(List<T> list)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(list);
        }
    }
}