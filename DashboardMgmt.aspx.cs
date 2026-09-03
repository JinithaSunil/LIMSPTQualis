using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Bibliography;

namespace LIMS
{
	public partial class DashboardMgmt : System.Web.UI.Page
	{
        public string chartJson = ""; public string chartJson1 = ""; public string chartJson2 = ""; public string chartJson3 = "";
        string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected List<TestEngineerHeat> HeatMapData;
        protected bool HasHeatMapData;
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            BindCharts();
        }
        protected void BindCharts()
        {
            string monthName = new DateTime(SelectedYear, SelectedMonth, 1).ToString("MMMM");
            //lblHeader6.Text = "PROJECT STATUS - " + monthName.ToUpper() + " - " + SelectedYear;
            lblHeader4.Text = "TEST TYPE - " + monthName.ToUpper() + " - " + SelectedYear;
            lblHeader7.Text = "INDUSTRY TYPE - " + monthName.ToUpper() + " - " + SelectedYear;
            lblHeader8.Text = "CUSTOMER SATIFACTION  - " + monthName.ToUpper() + " - " + SelectedYear;
            lblHeader9.Text = "PROJECT ALLOCATION  - " + monthName.ToUpper() + " - " + SelectedYear;
            string UserRole = "";

            if (Session["RoleID"] != null)
            {
                int RoleID = Convert.ToInt32(Session["RoleID"]);

                switch (RoleID)
                {
                    case 28: UserRole = "Director"; break;
                    case 7: UserRole = "Sales Manager"; break;
                    case 32: UserRole = "Division Head"; break;
                    case 2: UserRole = "Manager"; break;
                    case 3: UserRole = "Lab Analyst"; break;
                }
            }

            if (UserRole == "Director")
            {
                divChart.Visible = true;
                divTrend.Visible = true;
                divApproval.Visible = true;
                divAllotment.Visible = false;
                divAllotmentGantt.Visible = false;
                divFeedback.Visible = true;
                LoadDashboard();
            }
            if (UserRole == "Lab Manager")
            {
                divChart.Visible = true;
                divTrend.Visible = true;
                divEquip.Visible = true;
            }
            if (UserRole == "Team Manager")
            {
                divApproval.Visible = true;
                divAllotment.Visible = false;
                divAllotmentGantt.Visible = true;
                divTestType.Visible = true;
                divEquip.Visible = true;
            }
            if (UserRole == "Business Personnel")
            {
                divChart.Visible = true;
                divApproval.Visible = true;
            }
            if (UserRole == "Project Focal")
            {
                divAllotment.Visible = false;
                divAllotmentGantt.Visible = true;
                divApproval.Visible = true;
                divEquip.Visible = true;
            }
            if (UserRole == "QM")
            {
                divChart.Visible = true;
                divEquip.Visible = true;
                divFeedback.Visible = true;
            }
            // THEN LOAD DATA
            chartJson = GetRevenueChartData();
            chartJson1 = GetBookingChartData1();
            chartJson2 = GetTrendChart();

            UpdateMonthLabel();
            LoadTop20AllocationHeatMap(SelectedYear, SelectedMonth);

            CreateEquipmentHeatMap();
        }

        protected string GetRevenueChartData()
        {
            string sQuery = @"SELECT 
                                M.MonthName1,M.MonthNo,
                                M.TargetValue,
                                ISNULL(B.Achieved,0) AS Achieved
                            FROM SalesTargetMonthly M
                            LEFT JOIN
                            (
                                SELECT 
                                    MONTH(quotationdate) AS MonthNo,
                                    sum(GrandTotal) AS Achieved
                                FROM QuotationMaster
                                WHERE YEAR(quotationdate) =" + Convert.ToInt32(SelectedYear) +
                              @"  GROUP BY MONTH(quotationdate)
                            ) B ON M.MonthNo = B.MonthNo
                            ORDER BY M.MonthNo";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                sQuery, con);

            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                gv.DataSource = dt;
                gv.DataBind();
                lblHeader.Text = "REVENUE TARGET - " + SelectedYear;
            }
            else
            {
                gv.DataSource = null;
                gv.DataBind();
            }

            StringBuilder labels = new StringBuilder();
            StringBuilder targets = new StringBuilder();
            StringBuilder achieved = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                labels.Append("'" + row["MonthName1"] + "',");
                targets.Append(row["TargetValue"] + ",");
                achieved.Append(row["Achieved"] + ",");
            }

            return "{labels:[" + labels.ToString().TrimEnd(',') +
                   "],targets:[" + targets.ToString().TrimEnd(',') +
                   "],achieved:[" + achieved.ToString().TrimEnd(',') + "]}";
        }
        protected string GetBookingChartData1()
        {
            string sQuery = @"SELECT 
                                M.MonthName1,M.MonthNo,
                                M.BookingTarget,
                                ISNULL(B.Achieved,0) AS Achieved
                            FROM SalesTargetMonthly M
                            LEFT JOIN
                            (
                                SELECT 
                                    MONTH(quotationdate) AS MonthNo,
                                    count(*) AS Achieved
                                FROM QuotationMaster
                                WHERE YEAR(quotationdate) =" + Convert.ToInt32(SelectedYear) +
                              @"  GROUP BY MONTH(quotationdate)
                            ) B ON M.MonthNo = B.MonthNo
                            ORDER BY M.MonthNo";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                sQuery, con);

            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                lblHeader1.Text = "BOOKING TARGET - " + SelectedYear;
            }

            StringBuilder labels = new StringBuilder();
            StringBuilder targets = new StringBuilder();
            StringBuilder achieved = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                labels.Append("'" + row["MonthName1"] + "',");
                targets.Append(row["BookingTarget"] + ",");
                achieved.Append(row["Achieved"] + ",");
            }

            return "{labels:[" + labels.ToString().TrimEnd(',') +
                   "],targets:[" + targets.ToString().TrimEnd(',') +
                   "],achieved:[" + achieved.ToString().TrimEnd(',') + "]}";
        }
        protected string GetTrendChart()
        {
            DataTable dt = _cm.GetTrend();

            var labels = new List<string>();
            var thisYear = new List<int>();
            var lastYear = new List<int>();

            lblHeader2.Text = "REVENUE TREND - " + DateTime.Now.ToString("yyyy") + " v/s " + DateTime.Now.AddYears(-1).ToString("yyyy");


            foreach (DataRow dr in dt.Rows)
            {
                labels.Add(dr["MonthName"].ToString());
                thisYear.Add(Convert.ToInt32(dr["ThisYear"]));
                lastYear.Add(Convert.ToInt32(dr["LastYear"]));
            }

            var chartData = new
            {
                labels = labels,
                thisYear = thisYear,
                lastYear = lastYear
            };

            chartJson2 = new JavaScriptSerializer().Serialize(chartData);
            return chartJson2;
        }


        protected void txtYear_TextChanged(object sender, EventArgs e)
        {
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCharts();
        }


        /* Heat Map */
        protected int SelectedYear
        {
            get => ViewState["Year"] == null
                                    ? DateTime.Now.Year
                                    : (int)ViewState["Year"];
            set => ViewState["Year"] = value;
        }

        protected int SelectedMonth
        {
            get => ViewState["Month"] == null
                                    ? DateTime.Now.Month
                                    : (int)ViewState["Month"];
            set => ViewState["Month"] = value;
        }
        private void UpdateMonthLabel()
        {
            lblMonthYear.Text =
                new DateTime(SelectedYear, SelectedMonth, 1).ToString("MMMM yyyy");
        }
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            SelectedMonth--;
            if (SelectedMonth == 0)
            {
                SelectedMonth = 12;
                SelectedYear--;
            }

            BindCharts();

        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            SelectedMonth++;
            if (SelectedMonth == 13)
            {
                SelectedMonth = 1;
                SelectedYear++;
            }

            BindCharts();
        }
        public class TestEngineerHeat
        {
            public int EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public Dictionary<int, int> Days { get; set; }

            public TestEngineerHeat()
            {
                Days = new Dictionary<int, int>(); // 🔑 IMPORTANT
            }
        }
        protected void LoadTop20AllocationHeatMap(int year, int month)
        {
            string monthName = new DateTime(year, month, 1).ToString("MMMM");
            lblHeader5.Text = "ALLOTMENT MAP - " + monthName.ToUpper() + "-" + year;
            HeatMapData = new List<TestEngineerHeat>();
            var map = new Dictionary<int, TestEngineerHeat>();
            DataTable dt = _cm.GetAllocationHeat(year, month);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int vid = (int)Convert.ToInt32(dr["EmployeeID"]);
                    string name = dr["EmployeeName"].ToString();
                    int day = (int)dr["DayNo"];
                    int cnt = (int)dr["DailyAllocations"];

                    if (!map.ContainsKey(vid))
                    {
                        map[vid] = new TestEngineerHeat
                        {
                            EmployeeID = vid,
                            EmployeeName = name
                        };
                    }

                    map[vid].Days[day] = cnt;
                }
            }

            if (map.Count > 0)
            {
                HeatMapData = map.Values.ToList();
                HasHeatMapData = true;
            }
            else
            {
                HeatMapData.Clear(); // 👈 ensures empty
                HasHeatMapData = false;
            }
        }
        protected string GetHeatClass(int count)
        {
            if (count >= 5) return "heat-5";
            if (count == 4) return "heat-4";
            if (count == 3) return "heat-3";
            if (count == 2) return "heat-2";
            if (count == 1) return "heat-1";
            return "heat-0";
        }

        /* Project Status */
        private void LoadDashboard()
        {

            DataSet ds = _cm.GetDirLabStatus(SelectedYear.ToString(), SelectedMonth.ToString());

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
       
        protected void CreateEquipmentHeatMap()
        {
            int year = Convert.ToInt32(SelectedYear);
            int month = Convert.ToInt32(SelectedMonth);
            DataTable dt = _cm.GetEquipmentHeatMap(year, month);
            if (dt.Rows.Count > 0)
            {
                string monthName = new DateTime(SelectedYear, SelectedMonth, 1).ToString("MMMM");
                lblHeader3.Text = "MACHINE ALLOCATION - " + monthName.ToUpper() + "-" + SelectedYear;
                List<object> heat = new List<object>();

                foreach (DataRow dr in dt.Rows)
                {
                    string equipment = dr["EquipmentUsed"].ToString();

                    for (int day = 1; day <= 31; day++)
                    {
                        int value = 0;

                        if (dr.Table.Columns.Contains(day.ToString()) && dr[day.ToString()] != DBNull.Value)
                        {
                            value = Convert.ToInt32(dr[day.ToString()]);
                        }

                        heat.Add(new
                        {
                            x = day.ToString(),      // Day of month
                            y = equipment,           // Equipment name
                            v = value                // Usage count
                        });
                    }
                }

                chartJson3 = JsonConvert.SerializeObject(heat);
            }
        }
        [WebMethod]
        public static object GetTestTypeChart(int month, int year)
        {
            string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            List<object> data = new List<object>();
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //   string sQuery = @"
                //   SELECT  CASE 
                //               WHEN Testtype LIKE '%Dynamic Test%' OR Testtype = 'DynamicTest' THEN 'Dynamic Test'
                //               WHEN Testtype LIKE '%Static Test%' OR Testtype = 'StaticTest' THEN 'Static Test'
                //               WHEN Testtype LIKE '%Static&DynamicTest%' THEN 'Static&DynamicTest'
                //               ELSE Testtype
                //           END AS TesttypeGroup,
                //           COUNT(*) AS Count
                //   FROM    CustomerInquiry
                //   WHERE   InquiryStatusId NOT IN (4, 9, 39)
                //   AND MONTH(Created) = @Month AND YEAR(Created) = @Year
                //GROUP BY CASE 
                //               WHEN Testtype LIKE '%Dynamic Test%' OR Testtype = 'DynamicTest' THEN 'Dynamic Test'
                //               WHEN Testtype LIKE '%Static Test%' OR Testtype = 'StaticTest' THEN 'Static Test'
                //               WHEN Testtype LIKE '%Static&DynamicTest%' THEN 'Static&DynamicTest'
                //               ELSE Testtype
                //           END
                //   HAVING  CASE 
                //               WHEN Testtype LIKE '%Dynamic Test%' OR Testtype = 'DynamicTest' THEN 'Dynamic Test'
                //               WHEN Testtype LIKE '%Static Test%' OR Testtype = 'StaticTest' THEN 'Static Test'
                //               WHEN Testtype LIKE '%Static&DynamicTest%' THEN 'Static&DynamicTest'
                //               ELSE Testtype
                //           END IN ('Dynamic Test', 'Static Test', 'Static&DynamicTest')
                //   ORDER BY Count DESC";
                string sQuery = @"SELECT Testtype, COUNT(*) Total
                         FROM CustomerInquiry E
                         WHERE MONTH(E.Created)=@Month
                         AND YEAR(E.Created)=@Year
                         GROUP BY Testtype";
                SqlCommand cmd = new SqlCommand(sQuery, con);

                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    data.Add(new
                    {
                        type = dr["Testtype"].ToString(),
                        count = Convert.ToInt32(dr["Total"])
                    });

                }
            }
            return data;
        }

        /* Indusrty Chart */
        public class ChartItem
        {
            public string Label { get; set; }
            public int Value { get; set; }
        }

        [WebMethod]
        public static string GetIndustryChartData(int month, int year)
        {
            List<ChartItem> chartData = new List<ChartItem>();

            string conStr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @" SELECT t.TypeOfIndustry, COUNT(*) Total
                         FROM CustomerInquiry E
                         INNER JOIN CompanyDetails C ON E.CustomerMasterId = C.CustomerMasterId
						 INNER JOIN TypeOfIndustry t on t.TypeID = c.TypeOfIndustryID
                         WHERE MONTH(E.Created)=@Month
                         AND YEAR(E.Created)=@Year
                         GROUP BY t.TypeOfIndustry";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    chartData.Add(new ChartItem
                    {
                        Label = dr["TypeOfIndustry"].ToString(),
                        Value = Convert.ToInt32(dr["Total"])
                    });
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(chartData);
        }

        [WebMethod]
        public static object GetFeedbackData(int selectedMonth, int selectedYear)
        {
            string conn = System.Configuration.ConfigurationManager
            .ConnectionStrings["CM"].ConnectionString;

            int total = 0;
            int satisfied = 0;
            int dissatisfied = 0;
            double gaugeScore = 0;

            List<object> trend = new List<object>();

            using (SqlConnection con = new SqlConnection(conn))
            {

                SqlCommand cmd = new SqlCommand(@"
                                            SELECT 
                                        COUNT(*) Total,
                                        SUM(CASE WHEN SatisfactionStatus='Satisfied' THEN 1 ELSE 0 END) Satisfied,
                                        SUM(CASE WHEN SatisfactionStatus='Dissatisfied' THEN 1 ELSE 0 END) Dissatisfied
                                        FROM CustomerFeedback c
                                        INNER JOIN CustomerInquiry ci ON ci.InquiryId = c.InquiryID
                                        WHERE MONTH(ci.Created) = @Month
                                        AND YEAR(ci.Created) = @Year", con);
                cmd.Parameters.AddWithValue("@Month", selectedMonth);
                cmd.Parameters.AddWithValue("@Year", selectedYear);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if ((dr["Total"].ToString() != "") && (dr["Satisfied"].ToString() != "") && (dr["Dissatisfied"].ToString() != ""))
                    {
                        total = Convert.ToInt32(dr["Total"]);
                        satisfied = Convert.ToInt32(dr["Satisfied"]);
                        dissatisfied = Convert.ToInt32(dr["Dissatisfied"]);
                    }

                }

                dr.Close();

                SqlCommand trendCmd = new SqlCommand(@"SELECT 
                                        DATENAME(MONTH,ci.Created) MonthName,
                                        MONTH(ci.Created) MonthNo,
                                        CAST(
                                        SUM(CASE WHEN SatisfactionStatus='Satisfied' THEN 1 ELSE 0 END)*100.0
                                        / COUNT(*) AS DECIMAL(5,2)) SatisfactionPercent
                                        FROM CustomerFeedback c
                                        inner join CustomerInquiry ci on ci.InquiryId = c.InquiryID
                                        GROUP BY DATENAME(MONTH,ci.Created), MONTH(ci.Created)
                                        ORDER BY MonthNo", con);

                SqlDataReader tr = trendCmd.ExecuteReader();

                while (tr.Read())
                {
                    trend.Add(new
                    {
                        MonthName = tr["MonthName"].ToString(),
                        SatisfactionPercent = Convert.ToDecimal(tr["SatisfactionPercent"])

                    });
                }


                if (total > 0)
                {
                    gaugeScore = Math.Round((double)satisfied / total * 100, 2);
                }
            }

            return new
            {

                Total = total,
                Satisfied = satisfied,
                Dissatisfied = dissatisfied,
                GaugeScore = gaugeScore,
                Trend = trend

            };

        }

        [WebMethod]
        public static List<object> GetEngineerAllocation(int month, int year)
        {
            List<object> data = new List<object>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                string q = @"select EmployeeName,ProjectName +'-' + details as ProjectName,PlannedStartDateTime StartDate,PlannedEndDateTime EndDate from WorkPlanMaster wm
                                inner join WorkPlanDetails wd on wm.WorkPlanID=wd.WorkPlanID
                                where MONTH(PlannedStartDateTime)=@m and year(PlannedStartDateTime)=@y
                                ORDER BY EmployeeName, PlannedStartDateTime";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@m", month);
                cmd.Parameters.AddWithValue("@y", year);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    data.Add(new
                    {
                        Engineer = dr["EmployeeName"].ToString(),
                        Test = dr["ProjectName"].ToString(),
                        Start = Convert.ToDateTime(dr["StartDate"]).Day,
                        End = Convert.ToDateTime(dr["EndDate"]).Day
                    });
                }
            }

            return data;
        }
    }
}