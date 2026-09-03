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

namespace LIMS
{
    public partial class DashboardLab : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindYear();
                BindMonths();
                LoadAll();
            }
        }
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadAll();
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
        private void BindMonths()
        {
            ddlMonth.Items.Clear();

            for (int i = 1; i <= 12; i++)
            {
                string monthName =
                    System.Globalization.CultureInfo
                    .CurrentCulture.DateTimeFormat
                    .GetMonthName(i);

                ddlMonth.Items.Add(
                    new ListItem(monthName, i.ToString())
                );
            }

            // Select Current Month
            ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
        }
        protected void LoadAll()
        {
            LoadDashboard();
            LoadWorkflowChart();
        }
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAll();
        }
        private void LoadDashboard()
        {
            
                DataSet ds = _cm.GetLabStatus(ddlYear.SelectedValue,ddlMonth.SelectedValue);

                if (ds.Tables.Count >= 3)
                {
                    // ---------------- KPI ----------------
                    DataTable dtKPI = ds.Tables[0];

                    if (dtKPI.Rows.Count > 0)
                    {
                        lblEnquiry.Text = dtKPI.Rows[0]["Enquiries"].ToString();
                        lblSample.Text = dtKPI.Rows[0]["Samples"].ToString();
                        lblAllocated.Text = dtKPI.Rows[0]["Allocated"].ToString();
                        lblTested.Text = dtKPI.Rows[0]["Tested"].ToString();
                        lblReportReady.Text = dtKPI.Rows[0]["ReportReady"].ToString();
                    }

                    // ---------------- Department Grid ----------------
                    DataTable dtDepartment = ds.Tables[1];
                    gvDepartment.DataSource = dtDepartment;
                    gvDepartment.DataBind();

                    // ---------------- Weekly Chart ----------------
                    DataTable dtWeekly = ds.Tables[2];

                    string labels = "";
                    string enquiryData = "";

                    foreach (DataRow row in dtWeekly.Rows)
                    {
                        DateTime weekStart = Convert.ToDateTime(row["WeekStart"]);

                        labels += "'"
                            + weekStart.ToString("dd MMM")
                            + " - "
                            + weekStart.AddDays(6).ToString("dd MMM")
                            + "',";

                        enquiryData += row["Enquiries"] + ",";
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "chart",
                        $"loadChart([{labels}], [{enquiryData}]);", true);
                }
            

        }
        private void LoadWorkflowChart()
        {
            DataTable dt = _cm.GetWeeklyWorkFlow(ddlYear.SelectedValue,ddlMonth.SelectedValue);
            var labels = new List<string>();
            var samples = new List<int>();
            var tested = new List<int>();
            var reportReady = new List<int>();

            foreach (DataRow row in dt.Rows)
            {
                labels.Add(row["WeekLabel"].ToString());
                samples.Add(Convert.ToInt32(row["Samples"]));
                tested.Add(Convert.ToInt32(row["Tested"]));
                reportReady.Add(Convert.ToInt32(row["ReportReady"]));
            }

            JavaScriptSerializer js = new JavaScriptSerializer();

            string script = $@"
        renderWorkflowChart(
            {js.Serialize(labels)},
            {js.Serialize(samples)},
            {js.Serialize(tested)},
            {js.Serialize(reportReady)}
        );";

            ClientScript.RegisterStartupScript(this.GetType(),
                "workflowChartScript", script, true);
        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAll();
        }
    }
}