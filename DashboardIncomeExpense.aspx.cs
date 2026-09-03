using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class DashboardIncomeExpense : System.Web.UI.Page
	{
        string cs = System.Configuration.ConfigurationManager
                        .ConnectionStrings["cs"].ConnectionString;

        public int InvPending, InvPaid, InvOverdue;
        public int PayDue, PayPaid, PayOverdue;
        protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                LoadInvoiceStatus();
                LoadPaymentStatus();
            }
        }
        void LoadInvoiceStatus()
        {
            using (SqlDataAdapter da = new SqlDataAdapter(
                "SELECT Status, COUNT(*) Total FROM InvoiceMaster GROUP BY Status", cs))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                InvPending = GetCount(dt, "Pending");
                InvPaid = GetCount(dt, "Paid");
                InvOverdue = GetCount(dt, "Overdue");
            }
        }

        void LoadPaymentStatus()
        {
            using (SqlDataAdapter da = new SqlDataAdapter(
                "SELECT Status, COUNT(*) Total FROM PaymentMaster GROUP BY Status", cs))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                PayDue = GetCount(dt, "Due");
                PayPaid = GetCount(dt, "Paid");
                PayOverdue = GetCount(dt, "Overdue");

            }
        }

        int GetCount(DataTable dt, string status)
        {
            DataRow[] rows = dt.Select($"Status='{status}'");
            return rows.Length > 0 ? Convert.ToInt32(rows[0]["Total"]) : 0;
        }
    }
}
