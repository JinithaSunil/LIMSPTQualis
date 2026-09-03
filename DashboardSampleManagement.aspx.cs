using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using DocumentFormat.OpenXml.Bibliography;

namespace LIMS
{
    public partial class DashboardSampleManagement : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Usertype"] != null)
                {
                    BindYear();
                    BindMonths();
                    LoadAll();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
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
            BindTimeline();
            BindVerificationStatus();
            BindWeeklySamples();
        }
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAll();
        }
        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAll();
        }
        private void BindTimeline()
        {
            List<string> stageOrder = new List<string>
                            {
                                "Enquiry",
                                "TAF",
                                "SampleReceived",
                                "Allocated",
                                "Tested",
                                "ReportGenerated"
                            };
            DataTable dt = _cm.GetTimeLine(2026, 2, 5);
            if (dt.Rows.Count > 0)
            {
                DataTable summary = new DataTable();
                summary.Columns.Add("StageName");
                summary.Columns.Add("PendingCount", typeof(int));
                summary.Columns.Add("DelayedCount", typeof(int));

                var groups = dt.AsEnumerable()
                .GroupBy(r => r["CurrentStatus"].ToString())
                .ToDictionary(g => g.Key, g => g);


                foreach (var stage in stageOrder)
                {
                    DataRow row = summary.NewRow();
                    row["StageName"] = stage;

                    if (groups.ContainsKey(stage))
                    {
                        var g = groups[stage];
                        row["PendingCount"] = g.Count();
                        row["DelayedCount"] = g.Sum(x => Convert.ToInt32(x["DelayedCount"]));
                    }
                    else
                    {
                        // If no records exist for that stage
                        row["PendingCount"] = 0;
                        row["DelayedCount"] = 0;
                    }

                    summary.Rows.Add(row);
                }

                rptCards.DataSource = summary;
                rptCards.DataBind();
            }
            else
            {
                rptCards.DataSource = null;
                rptCards.DataBind();
            }
        }

        private void BindVerificationStatus()
        {
            int pending = 0;
            int verified = 0;
            string DepartmentID = Session["DepartmentID"].ToString();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_GetSampleVerificationSummary", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Month", SqlDbType.Int).Value = ddlMonth.SelectedValue;
                cmd.Parameters.Add("@Year", SqlDbType.Int).Value = ddlYear.SelectedValue;
                cmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DepartmentID;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if ((dr["PendingVerification"] != null)&& (dr["PendingVerification"].ToString() !=""))
                    {
                        pending = Convert.ToInt32(dr["PendingVerification"]);
                    }
                    if ((dr["Verified"] != null) && (dr["Verified"].ToString() != ""))
                    {
                        verified = Convert.ToInt32(dr["Verified"]);
                    }
                }
            }

            string script = $@"
<script>
Chart.register(ChartDataLabels);

var ctx = document.getElementById('verificationChart').getContext('2d');

new Chart(ctx, {{
    type: 'doughnut',
    data: {{
        labels: ['Pending', 'Verified'],
        datasets: [{{
            data: [{pending}, {verified}],
            backgroundColor: ['#FADBD8', '#D5F5E3']
        }}]
    }},
    options: {{
        responsive: true,
        plugins: {{
            legend: {{
                position: 'bottom'
            }},
            datalabels: {{
                color: '#333',
                font: {{
                    weight: 'bold',
                    size: 14
                }},
                formatter: function(value, context) {{
                    let total = context.chart.data.datasets[0].data.reduce((a, b) => a + b, 0);
                    let percentage = ((value / total) * 100).toFixed(1) + '%';
                    return value + '\n' + percentage;
                }}
            }}
        }}
    }}
}});
</script>";

            ClientScript.RegisterStartupScript(this.GetType(), "doughnutChart", script);

            ClientScript.RegisterStartupScript(this.GetType(), "chart", script);
        }

        private void BindWeeklySamples()
        {
            List<string> labels = new List<string>();
            List<int> counts = new List<int>();
            string DepartmentID = Session["DepartmentID"].ToString();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_GetWeeklySampleCount", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Month", SqlDbType.Int).Value = ddlMonth.SelectedValue;
                cmd.Parameters.Add("@Year", SqlDbType.Int).Value = ddlYear.SelectedValue;
                cmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DepartmentID;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    DateTime weekStart = Convert.ToDateTime(dr["WeekStart"]);
                    labels.Add(weekStart.ToString("dd MMM"));
                    counts.Add(Convert.ToInt32(dr["SampleCount"]));
                }
            }

            string labelString = string.Join("','", labels);
            string dataString = string.Join(",", counts);

            string script = $@"
<script>
window.addEventListener('load', function() {{

    if (typeof ChartDataLabels !== 'undefined') {{
        Chart.register(ChartDataLabels);
    }}

    var ctx = document.getElementById('weeklyChart').getContext('2d');

    new Chart(ctx, {{
        type: 'bar',
        data: {{
            labels: ['{labelString}'],
            datasets: [{{
                label: 'Samples',
                data: [{dataString}],
                backgroundColor: '#D6EAF8',
                borderRadius: 8
            }}]
        }},
        options: {{
            responsive: true,
            plugins: {{
                legend: {{
                    display: false
                }},
                datalabels: {{
                    anchor: 'end',
                    align: 'top',
                    color: '#333',
                    font: {{
                        weight: 'bold',
                        size: 14
                    }},
                    formatter: function(value) {{
                        return value;   // 👈 Only number
                    }}
                }}
            }},
            scales: {{
                y: {{
                    beginAtZero: true
                }}
            }}
        }}
    }});

}});
</script>";

            ClientScript.RegisterStartupScript(this.GetType(), "weeklyChart", script);

            ClientScript.RegisterStartupScript(this.GetType(), "weeklyChart", script);
        }
    }
}