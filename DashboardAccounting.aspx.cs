using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class DashboardAccounting : System.Web.UI.Page
    {
        DAL.APModule _ap = new DAL.APModule();
        DAL.ARModule _ar = new DAL.ARModule();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dtPendingPayable = APModule.getPendingAPVendors();
                hypAP.Text = dtPendingPayable.Rows.Count.ToString();

                DataTable dtPendingReceipts = ARModule.getPendingARCustomers();
                hypAR.Text = dtPendingReceipts.Rows.Count.ToString();

            }
        }
    }
}