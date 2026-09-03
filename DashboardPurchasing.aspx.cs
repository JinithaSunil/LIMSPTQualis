using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class DashboardPurchasing : System.Web.UI.Page
	{
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                  
                    BindDashboardData("ALL");
                    lblMonth.Text = DateTime.Now.ToString("MMMM yy");
                    BindMetrics();
                    BindCharts();
                    BindRankings();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        [WebMethod]
        public static List<object> GetChartData1()
        {
            string query = @"select EmployeeDepartmentName, isnull(Tier1, 0)Tier1,isnull(Tier2,0)Tier2,isnull(Tier3,0)Tier3,isnull(Tier4,0)Tier4 from
                             (SELECT * FROM (
	                           SELECT case 
	                                  when pr.currenttier='1' then 'Tier1'
	                                  when pr.currenttier='2' then 'Tier2'
	                                  when pr.currenttier='3' then 'Tier3'
									  when pr.currenttier='4' then 'Tier4'
	                          end CurrentOwner,isnull(count(pd.PRID),0) as Total,EmployeeDepartmentName											
	                          from purchaserequisitiondetails pd
											inner join purchaserequisition pr on pd.prid= pr.prid
											inner join purchaserequisitionstatus ps on ps.StatusID = pd.StatusID
											inner join EmployeeDepartment ed on ed.EmployeeDepartmentID = pr.DepartmentID
											group by EmployeeDepartmentName,pr.CurrentTier
	) d PIVOT (
	 SUM(Total) 
	    FOR CurrentOwner in (Tier1,Tier2,Tier3,Tier4)) x)pvt";
            string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            List<object> chartData = new List<object>();
            chartData.Add(new object[]
                            {"EmployeeDepartmentName","Tier1","Tier2","Tier3","Tier4"});
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            chartData.Add(new object[]
                            {
                                sdr["EmployeeDepartmentName"],sdr["Tier1"], sdr["Tier2"],sdr["Tier3"],sdr["Tier4"]
                            });
                        }
                    }
                    con.Close();
                    return chartData;
                }
            }
        }
        protected void BindDashboardData(string Period)
        {
            try
            {
                DataTable[] dt = new DataTable[5];
                dt = _cm.getDashboardPurchaseCount(Period);

                // Debug: Log the counts for each DataTable
                for (int i = 0; i < dt.Length; i++)
                {
                    if (dt[i] != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"DataTable[{i}] Row Count: {dt[i].Rows.Count}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"DataTable[{i}] is null");
                    }
                }

                // Enquiry Sent (dt[0])
                if (dt[0] != null && dt[0].Rows.Count > 0)
                {
                    lblEnquirySent.Text = dt[0].Rows.Count.ToString();
                }
                else
                {
                    lblEnquirySent.Text = "0";
                }

                // Quote Received (dt[1])
                if (dt[1] != null && dt[1].Rows.Count > 0)
                {
                    lblQuoteReceived.Text = dt[1].Rows.Count.ToString();
                }
                else
                {
                    lblQuoteReceived.Text = "0";
                }

                // PO Pending (dt[2])
                if (dt[2] != null && dt[2].Rows.Count > 0)
                {
                    lblPOPending.Text = dt[2].Rows.Count.ToString();
                }
                else
                {
                    lblPOPending.Text = "0";
                }

                // Pending Approval (dt[4]) - Changed to show count instead of percentage
                if (dt[3] != null && dt[3].Rows.Count > 0)
                {
                    lblPending.Text = dt[3].Rows.Count.ToString();
                }
                else
                {
                    lblPending.Text = "0";
                }

                // Set button visibility after binding data
                SetButtonVisibilityBasedOnRole();
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error in BindDashboardData: {ex.Message}");
                // Set default values in case of error
                lblEnquirySent.Text = "0";
                lblQuoteReceived.Text = "0";
                lblPOPending.Text = "0";
                lblPending.Text = "0";
            }
        }
        protected void SetButtonVisibilityBasedOnRole()
        {
            // Check if user is Purchase Manager 
            bool isPurchaseManager = CheckIfPurchaseManager(); 

            if (isPurchaseManager)
            {
                // Hide "Add New" buttons for Purchase Manager
                lnkPendingPR.Visible = false;
                LinkButton1.Visible = false;
                LinkButton3.Visible = false;
                LinkButton5.Visible = false;

                // Add view-only class to View buttons
                lnkViewItem.CssClass = "btn bg-c-purple view-only";
                LinkButton2.CssClass = "btn bg-c-green view-only";
                LinkButton4.CssClass = "btn bg-c-red view-only";
                LinkButton6.CssClass = "btn bg-c-blue view-only";
            }
            else
            {
                // Show all buttons for other roles
                lnkPendingPR.Visible = true;
                LinkButton1.Visible = true;
                LinkButton3.Visible = true;
                LinkButton5.Visible = true;
                lnkViewItem.Visible = true;
                LinkButton2.Visible = true;
                LinkButton4.Visible = true;
                LinkButton6.Visible = true;
            }
        }

        // Check if current user is Purchase Manager based on UserID
        private bool CheckIfPurchaseManager()
        {
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();
                return userID == "4007"; // Purchase Manager UserID
            }
            return false;
        }

        protected void LinkButton_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "ViewItems":
                    LinkButton btn = (LinkButton)sender;
                    bool isPurchaseManager = CheckIfPurchaseManager();

                    if (btn.ID == "lnkViewItem") // Pending Approval View
                    {
                        Response.Redirect(isPurchaseManager ? "PRListPending.aspx" : "PurchaseRequisitionList.aspx");
                    }
                    else if (btn.ID == "LinkButton4") // Quote Received View
                    {
                        Response.Redirect(isPurchaseManager ? "VendorQuotationManagement.aspx" : "VendorQuotationManagement.aspx");
                    }
                    else if (btn.ID == "LinkButton2") // Enquiry sent View
                    {
                        Response.Redirect(isPurchaseManager ? "EnquiryList.aspx" : "EnquiryList.aspx");
                    }
                    else // PO Pending View
                    {
                        Response.Redirect(isPurchaseManager ? "POMasterList.aspx" : "PurchaseOrderList.aspx");
                    }
                    break;
                case "PendingPR":
                    LinkButton btn1 = (LinkButton)sender;
                    if (btn1.ID == "lnkPendingPR") // ADD Pending Approval 
                    {
                        Response.Redirect("PRListPending.aspx");
                    }
                    else if (btn1.ID == "LinkButton1") // ADD Enquiry sent  
                    {
                        Response.Redirect("FloatEnquiry.aspx");
                    }
                    else if (btn1.ID == "LinkButton3") // ADD Quote Received
                    {
                        Response.Redirect("FloatEnquiry.aspx");
                    }
                    else // ADD PO Pending
                    {
                        Response.Redirect("PurchaseOrderList.aspx");
                    }
                    break;
            }
        }
        protected void radPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (radPeriod.SelectedValue)
            {
                case "DAY": BindDashboardData("DAY"); break;
                case "WEEK": BindDashboardData("WEEK"); break;
                case "MONTH": BindDashboardData("MONTH"); break;
                case "YEAR": BindDashboardData("YEAR"); break;
                case "ALL": BindDashboardData("ALL"); break;
            }
        }

        private void BindMetrics()
        {
            try
            {
                // Get total suppliers from VendorDetails table (as per your requirement)
                lblTotalSuppliers.Text = _cm.GetAllSuppliers().Rows.Count.ToString();

                // Get top supplier share dynamically
                decimal topShare = GetTopSupplierShare();
                lblTopShare.Text = topShare.ToString("0.0") + "%";

                // Get average orders dynamically
                decimal avgOrders = GetAverageOrders("Count");
                lblAvgOrders.Text = avgOrders.ToString("0.0");
            }
            catch (Exception ex)
            {
                // Handle errors gracefully
                lblTotalSuppliers.Text = "0";
                lblTopShare.Text = "0.0%";
                lblAvgOrders.Text = "0.0";

                // Log the error
                System.Diagnostics.Debug.WriteLine("Error binding metrics: " + ex.Message);
            }
        }
        public decimal GetTopSupplierShare()
        {
            try
            {
                // Get all suppliers data for current month/period (same as your charts use)
                DataTable dtAll = _cm.getTopVendors(5, "Value"); 

                if (dtAll.Rows.Count == 0) return 0;

                // Calculate total value from all suppliers
                decimal totalValue = 0;
                foreach (DataRow row in dtAll.Rows)
                {
                    totalValue += Convert.ToDecimal(row["Total"]);
                }

                if (totalValue == 0) return 0;

                // The first row should be the top supplier (since your query orders by Total DESC)
                decimal topSupplierValue = Convert.ToDecimal(dtAll.Rows[0]["Total"]);

                // Calculate percentage: (Top Supplier Value / Total Value) * 100
                decimal percentage = (topSupplierValue / totalValue) * 100;
                return percentage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error calculating top supplier share: " + ex.Message);
                return 0;
            }
        }
        public decimal GetAverageOrders(string type)
        {
            try
            {
                DataTable dtCount = _cm.getTopVendors(5, type);
                if (dtCount.Rows.Count == 0) return 0;

                decimal totalOrders = 0;
                foreach (DataRow row in dtCount.Rows)
                {
                    totalOrders += Convert.ToDecimal(row["Total"]);
                }

                return totalOrders / dtCount.Rows.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error calculating average orders: " + ex.Message);
                return 0;
            }
        }
        private void BindCharts()
        {
            // Value Chart
            string valueChartScript = BuildChartScript("Value", "piechart1_3d");
            ClientScript.RegisterStartupScript(this.GetType(), "ValueChartScript", valueChartScript, false);

            // Count Chart
            string countChartScript = BuildChartScript("Count", "piechart2_3d");
            ClientScript.RegisterStartupScript(this.GetType(), "CountChartScript", countChartScript, false);
        }
        private string BuildChartScript(string type, string divId)
        {
            DataTable dt = _cm.getTopVendors(5, type);
            StringBuilder script = new StringBuilder();

            script.Append(@"
        <script type='text/javascript'>
            google.charts.load('current', {'packages':['corechart']});
            google.charts.setOnLoadCallback(drawChart);
            
            function drawChart() {
                var data = new google.visualization.DataTable();
                data.addColumn('string', 'Vendor');
                data.addColumn('number', '" + (type == "Value" ? "Amount" : "Count") + @"');
                data.addRows([");

            foreach (DataRow row in dt.Rows)
            {
                // Use "Total" column for both Value and Count (since that's what your DB returns)
                script.AppendFormat("['{0}', {1}],",
                    row["VendorName"].ToString().Replace("'", "\\'"),
                    row["Total"]); // Always use "Total" column
            }

            if (dt.Rows.Count > 0) script.Length--;

            script.AppendFormat(@"]);
                var options = {{
                    title: '',
                    is3D: true,
                    pieSliceText: 'value-and-percentage',
                    legend: {{position: 'right'}},
                    chartArea: {{width: '90%', height: '80%'}}
                }};
                var chart = new google.visualization.PieChart(document.getElementById('{1}'));
                chart.draw(data, options);
            }}
        </script>", type, divId);

            return script.ToString();
        }
        private void BindRankings()
        {
            // For Value Rankings (uses "Total" column)
            DataTable dtValue = _cm.getTopVendors(5, "Value");
            rptValueRanking.DataSource = dtValue;
            rptValueRanking.DataBind();

            // For Count Rankings (also uses "Total" column but displays as count)
            DataTable dtCount = _cm.getTopVendors(5, "Count");
            rptCountRanking.DataSource = dtCount;
            rptCountRanking.DataBind();
        }
        protected string GetChartDataJson()
        {
            var chartData = GetChartData1();
            return Newtonsoft.Json.JsonConvert.SerializeObject(chartData);
        }

    }

}