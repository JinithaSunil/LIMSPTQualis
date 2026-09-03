using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace LIMS
{
    public partial class DashboardSalesAndMarketing : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected string ChartDataJson = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindYear();
               // LoadDivisionSales();
               LoadMonthlyChart();
                LoadSalesFunnel();
                LoadQuarterly();
                LoadDepartmentSales();
                LoadTopReps();
                LoadPendingRequests();
                LoadPendingQuotations();
                LoadDepartmentPie();
            }
        }

        private void BindYear()
        {
            ddlYear.Items.Clear();
            for (int i = 2025; i <= DateTime.Now.Year; i++)
            {
                ddlYear.Items.Add(i.ToString());
            }
            ddlYear.SelectedValue = DateTime.Now.Year.ToString();
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMonthlyChart();
            LoadSalesFunnel();
            LoadQuarterly();
            LoadDepartmentSales();
            LoadTopReps();
            LoadPendingRequests();
            LoadPendingQuotations();
            LoadDepartmentPie();
        }

        private void LoadDivisionSales()
        {
            DataTable dt = _cm.getSalesAchievementYearly(ddlYear.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                gvDivisionSales.DataSource = dt;
                gvDivisionSales.DataBind();

                BindChart(dt);
            }


        }

        private void BindChart(DataTable dt)
        {
            string[] labels = new string[dt.Rows.Count];
            decimal[] data = new decimal[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                labels[i] = dt.Rows[i]["DepartmentName"].ToString();
                data[i] = Convert.ToDecimal(dt.Rows[i]["TotalSales"]);
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            string chartLabels = js.Serialize(labels);
            string chartData = js.Serialize(data);

            ClientScript.RegisterStartupScript(this.GetType(), "chart",
                $"loadChart({chartLabels}, {chartData});", true);
        }

        private void LoadMonthlyChart()
        {
            DataTable dt = new DataTable();

            dt = _cm.getSalesAchievementMonthly(ddlYear.SelectedValue);
            var divisions = dt.AsEnumerable()
                .Select(r => r["DepartmentName"].ToString())
                .Distinct()
                .ToList();

            string[] months = { "January","February","March","April","May","June",
                        "July","August","September","October","November","December" };

            var datasets = new List<object>();

            for (int m = 1; m <= 12; m++)
            {
                var monthData = new List<decimal>();

                foreach (string div in divisions)
                {
                    var row = dt.AsEnumerable().FirstOrDefault(r =>
                        r["DepartmentName"].ToString() == div &&
                        Convert.ToInt32(r["SaleMonth"]) == m);

                    monthData.Add(row != null ? Convert.ToDecimal(row["TotalSales"]) : 0);
                }

                datasets.Add(new
                {
                    label = months[m - 1],
                    data = monthData
                });
            }

            JavaScriptSerializer js = new JavaScriptSerializer();

            string labelsJson = js.Serialize(divisions);
            string datasetJson = js.Serialize(datasets);

            ClientScript.RegisterStartupScript(this.GetType(), "chart",
                $"loadStackedChart({labelsJson}, {datasetJson});", true);
        }

        private void LoadSalesFunnel()
        {
            DataTable dt = _cm.GetSalesFunnel(ddlYear.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                hfInquiry.Value = dt.Rows[0]["TotalInquiry"].ToString();
                hfQuotation.Value = dt.Rows[0]["TotalQuotation"].ToString();
                hfPO.Value = dt.Rows[0]["TotalPO"].ToString();
            }
        }
        private void LoadQuarterly()
        {
            DataTable dt = _cm.GetQuarterlySales(ddlYear.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                ChartDataJson = JsonConvert.SerializeObject(dt);
            }
        }
        private void LoadDepartmentSales()
        {
            DataTable dt = _cm.GetSalesStatusByDepartment(ddlYear.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                var labels = dt.AsEnumerable().Select(r => r["DepartmentName"].ToString()).ToList();
                var noQuotation = dt.AsEnumerable().Select(r => Convert.ToInt32(r["NoQuotation"])).ToList();
                var quotation = dt.AsEnumerable().Select(r => Convert.ToInt32(r["QuotationReached"])).ToList();
                var invoice = dt.AsEnumerable().Select(r => Convert.ToInt32(r["InvoiceGenerated"])).ToList();
                string script = $@"
<script>
loadChart1({JsonConvert.SerializeObject(labels)},
           {JsonConvert.SerializeObject(noQuotation)},
           {JsonConvert.SerializeObject(quotation)},
           {JsonConvert.SerializeObject(invoice)});
</script>";

                Page.Controls.Add(new LiteralControl(script));
            }
        }
        private void LoadTopReps()
        {
            DataTable dt = _cm.GetTopSalesReps(ddlYear.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                rptTopReps.DataSource = dt;
                rptTopReps.DataBind();
            }
        }
        private void LoadPendingRequests()
        {
            DataTable dt = _cm.GetPendingRequests(ddlYear.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                rptPendingRequests.DataSource = dt;
                rptPendingRequests.DataBind();
            }
        }
        private void LoadPendingQuotations()
        {
            DataTable dt = _cm.GetPendingQuotations(ddlYear.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                rptPendingQuotations.DataSource = dt;
                rptPendingQuotations.DataBind();
            }
        }
        protected string DeptLabels = "";
        protected string DeptValues = "";

        private void LoadDepartmentPie()
        {
            DataTable dt = _cm.GetSalesPie(ddlYear.SelectedValue);
            if (dt.Rows.Count > 0)
            {

                List<string> labels = new List<string>();
                List<string> values = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    labels.Add("'" + row["DeptCode"].ToString() + "'");
                    values.Add(row["TotalRevenue"].ToString());
                }

                DeptLabels = string.Join(",", labels);
                DeptValues = string.Join(",", values);
            }
        }
    }
}