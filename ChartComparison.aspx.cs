using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ChartComparison : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Populate();
            }
        }
        protected void Populate()
        {
            DataTable dtChartData = new DataTable();
            dtChartData = _cm.getChartData();
            gvChart.DataSource = dtChartData;
            gvChart.DataBind();

            Chart1.DataSource = dtChartData;
            Chart1.DataSource = dtChartData;
        }
    }
}