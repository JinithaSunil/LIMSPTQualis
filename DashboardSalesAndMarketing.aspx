<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DashboardSalesAndMarketing.aspx.cs" Inherits="LIMS.DashboardSalesAndMarketing" %>

<%@ Register Assembly="System.Web.DataVisualization"
    Namespace="System.Web.UI.DataVisualization.Charting"
    TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        #deptPieChart + div ul {
            text-align: left !important;
        }

        canvas {
            max-height: 320px;
        }

        .dashboard-card {
            background: #ffffff;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            margin-bottom: 20px;
        }

        .kpi-card {
            background: linear-gradient(135deg,#1abc9c,#16a085);
            color: white;
            border-radius: 12px;
            padding: 25px;
        }

            .kpi-card h2 {
                font-weight: 700;
                margin: 0;
            }

            .kpi-card small {
                opacity: 0.8;
            }

        .card-title {
            font-weight: 600;
            margin-bottom: 15px;
        }

        .rep-item {
            display: flex;
            justify-content: space-between;
            padding: 8px 0;
            border-bottom: 1px solid #eee;
        }

            .rep-item:last-child {
                border-bottom: none;
            }

        .percent-high {
            color: #2ecc71;
            font-weight: 600;
        }

        .percent-medium {
            color: #f39c12;
            font-weight: 600;
        }

        .activity-danger {
            color: #e74c3c;
            font-weight: bold;
            font-size: 24px;
        }
        
        /* Base Styles */
        .dashboard-container {
            padding: 20px;
            max-width: 1400px;
            margin: 0 auto;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }
         /* Card Borders */
 .border-top-purple {
     border-top: 4px solid var(--purple);
 }

    </style>
     <style>
     /* Root Variables */
     :root {
         --purple: #b34788;
         --green: #3c763d;
         --red: #a94442;
         --blue: #31708f;
         --light-bg: #f8f9fa;
         --primary-color: #2c3e50;
         --secondary-color: #34495e;
         --accent-color: #3498db;
         --border-color: #e0e0e0;
         --text-color: #333;
         --text-light: #7f8c8d;
     }

     /* Base Styles */
     .dashboard-container {
         padding: 20px;
         max-width: 1400px;
         margin: 0 auto;
         font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
     }

     /* Card Styling */
     .card {
         border: none;
         border-radius: 8px;
         box-shadow: 0 4px 12px rgba(0,0,0,0.08);
         margin-bottom: 25px;
         overflow: hidden;
         transition: transform 0.3s, box-shadow 0.3s;
         background: white;
     }

         .card:hover {
             transform: translateY(-3px);
             box-shadow: 0 8px 16px rgba(0,0,0,0.12);
         }

     .card-header {
         padding: 20px 25px;
         background-color: white;
         border-bottom: none;
     }

         .card-header h2, .card-header h3, .card-header h4 {
             margin: 0;
             color: #333;
             font-weight: 600;
         }

     .card-body {
         padding: 25px;
     }

     /* Card Borders */
     .border-top-purple {
         border-top: 4px solid var(--purple);
     }

     .border-top-green {
         border-top: 4px solid var(--green);
     }

     .border-top-red {
         border-top: 4px solid var(--red);
     }

     .border-top-blue {
         border-top: 4px solid var(--blue);
     }

     /* Stats Cards */
     .stats-card .card-body {
         min-height: 120px;
         display: flex;
         flex-direction: column;
         justify-content: center;
         padding: 20px;
     }

     .stats-card h3 {
         font-size: 28px;
         font-weight: 700;
         margin-bottom: 5px;
     }

     .stats-card p {
         margin-bottom: 0;
         font-size: 16px;
         color: #6c757d;
     }

     /* Footer Styling */
     .card-footer {
         padding: 0;
         background-color: transparent;
         border-top: 1px solid rgba(0,0,0,0.08);
     }

         .card-footer .btn {
             padding: 12px 15px;
             text-decoration: none;
             font-weight: 500;
             border-radius: 0;
             display: inline-block;
             color: #fff;
             text-align: center;
             border: none;
             transition: background-color 0.3s;
         }
             /* New style for when only View button is visible */
             .card-footer .btn.view-only {
                 width: 100%;
                 border-radius: 0 0 8px 8px;
             }
     /* Period Selector */
     .period-selector {
         margin-bottom: 25px;
     }

     .rbl input[type="radio"] {
         margin-right: 5px;
         margin-left: 0px;
     }

     .rbl label {
         margin-right: 15px;
         font-weight: 500;
         cursor: pointer;
     }

     /* Icon Styling */
     .icon-box {
         display: flex;
         align-items: center;
         justify-content: center;
         width: 56px;
         height: 56px;
         border-radius: 50%;
         font-size: 1.5rem;
     }

     /* Background Colors */
     .bg-purple-light {
         background-color: rgba(179, 71, 136, 0.1);
     }

     .bg-green-light {
         background-color: rgba(60, 118, 61, 0.1);
     }

     .bg-red-light {
         background-color: rgba(169, 68, 66, 0.1);
     }

     .bg-blue-light {
         background-color: rgba(49, 112, 143, 0.1);
     }

     .bg-c-purple {
         background-color: var(--purple);
         color: white;
     }

     .bg-c-green {
         background-color: var(--green);
         color: white;
     }

     .bg-c-red {
         background-color: var(--red);
         color: white;
     }

     .bg-c-blue {
         background-color: var(--blue);
         color: white;
     }

     /* Text Colors */
     .text-purple {
         color: var(--purple);
     }

     .text-green {
         color: var(--green);
     }

     .text-red {
         color: var(--red);
     }

     .text-blue {
         color: var(--blue);
     }

     /* Chart Section Styling */
     .chart-section {
         background: white;
         border-radius: 8px;
         box-shadow: 0 4px 12px rgba(0,0,0,0.08);
         margin-bottom: 25px;
         overflow: hidden;
     }

     .chart-hover {
         transform: translateY(-3px);
         box-shadow: 0 8px 16px rgba(0,0,0,0.12);
     }

     .chart-header {
         padding: 20px 25px;
         background: white;
         border-bottom: 1px solid var(--border-color);
         display: flex;
         justify-content: space-between;
         align-items: center;
         flex-wrap: wrap;
         gap: 15px;
     }

         .chart-header h3 {
             margin: 0;
             color: var(--text-color);
             font-weight: 600;
             display: flex;
             align-items: center;
             gap: 10px;
         }

     .chart-controls {
         display: flex;
         gap: 10px;
         align-items: center;
         flex-wrap: wrap;
     }

     .refresh-btn {
         padding: 8px 12px;
         background: var(--purple);
         color: white;
         border: none;
         border-radius: 4px;
         cursor: pointer;
         transition: background-color 0.3s;
         display: flex;
         align-items: center;
         gap: 5px;
     }

         .refresh-btn:hover {
             background: #2980b9;
         }

     .chart-container {
         padding: 20px;
         position: relative;
     }

     /* Summary Section */
     .summary-section {
         display: grid;
         grid-template-columns: 1fr 1fr;
         gap: 20px;
         margin-top: 20px;
     }

     .summary-box {
         background: var(--light-bg);
         border-radius: 6px;
         padding: 20px;
         border: 1px solid var(--border-color);
     }

         .summary-box h4 {
             margin-top: 0;
             margin-bottom: 15px;
             color: var(--primary-color);
             font-size: 1.1rem;
         }

     .department-list {
         list-style: none;
         padding: 0;
         margin: 0;
     }

         .department-list li {
             padding: 10px 0;
             display: flex;
             align-items: center;
             gap: 10px;
             border-bottom: 1px dashed var(--border-color);
         }

             .department-list li:last-child {
                 border-bottom: none;
             }

         .department-list i {
             color: var(--accent-color);
             width: 20px;
         }

     .status-table {
         width: 100%;
         border-collapse: collapse;
     }

         .status-table th, .status-table td {
             padding: 10px 8px;
             text-align: left;
             border-bottom: 1px solid var(--border-color);
         }

         .status-table th {
             font-weight: 600;
             color: var(--primary-color);
             background: rgba(52, 152, 219, 0.1);
         }

         .status-table tr:hover {
             background: rgba(52, 152, 219, 0.05);
         }

     /* Footer */
     .chart-footer {
         padding: 15px 25px;
         background: var(--light-bg);
         border-top: 1px solid var(--border-color);
         display: flex;
         justify-content: space-between;
         align-items: center;
         font-size: 11px;
     }

     .last-updated {
         color: var(--text-light);
     }

     .total-badge {
         background: var(--purple);
         color: white;
         padding: 4px 12px;
         border-radius: 12px;
         font-weight: 500;
     }

     /* Notification */
     .notification {
         position: fixed;
         top: 20px;
         right: 20px;
         background: var(--green);
         color: white;
         padding: 12px 20px;
         border-radius: 4px;
         transform: translateX(100%);
         transition: transform 0.3s ease;
         z-index: 1000;
         box-shadow: 0 4px 12px rgba(0,0,0,0.15);
     }

         .notification.show {
             transform: translateX(0);
         }

     /* Helper Classes */
     .d-flex {
         display: flex;
     }

     .justify-content-between {
         justify-content: space-between;
     }

     .align-items-center {
         align-items: center;
     }

     .flex-grow-1 {
         flex-grow: 1;
     }

     .border-start {
         border-left: 1px solid rgba(0,0,0,0.08);
     }

     .mb-4 {
         margin-bottom: 1.5rem;
     }

     .text-white {
         color: #fff;
     }

     /* Responsive Design */
     @media (max-width: 768px) {
         .chart-header {
             flex-direction: column;
             align-items: flex-start;
         }

         .chart-controls {
             width: 100%;
             justify-content: flex-start;
         }

         .summary-section {
             grid-template-columns: 1fr;
         }

         .chart-footer {
             flex-direction: column;
             gap: 10px;
             text-align: center;
         }
     }
 </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div class="dashboard-container" style="font-size:12px">

        <!-- Main Dashboard Card -->
        <div class="card border-top-purple">
            <div class="chart-header">
                <h3><i class="fa-solid fa-chart-column"></i>SALES DASHBOARD</h3>
               
            </div>

            <div class="card-body">
            <asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="true"
                OnSelectedIndexChanged="ddlYear_SelectedIndexChanged">
            </asp:DropDownList>

            <br />
            <br />

            <div class="row">


                <!-- Activities -->
                <div class="col-md-12">
                    <div class="dashboard-card text-center">
                        <canvas id="quotationChart" height="120"></canvas>
                    </div>
                </div>


            </div>
</div></div>
             <div class="card">
      

      <div class="card-body">
                <div class="col-md-4">
                    <div class="dashboard-card">
                        <div class="card-title" style="text-align:center">Department Wise PO Revenue</div>
                        <canvas id="deptPieChart" height="200"></canvas>
                    </div>
                </div>

                <!-- Sales Comparison -->
                <div class="col-md-8">
                    <div class="dashboard-card">
                        <div class="card-title" style="text-align:center">Enquiry v/s Quotation v/s Invoicing</div>
                        <canvas id="salesChart" height="200"></canvas>
                    </div>
                </div>


            </div></div>

            <div class="row">

                <!-- Open Requests -->
                <div class="col-md-6">
                    <div class="dashboard-card">
                        <div class="card-title" style="text-align:center">Open Requests</div>
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Request#</th>
                                    <th>Department</th>
                                    <th>Company</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptPendingRequests" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("RequestNo") %></td>
                                            <td><%# Eval("DepartmentName") %></td>
                                            <td><%# Eval("CompanyName") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>


                <!-- Won Sales by Reason -->
                <div class="col-md-6">
                    <div class="dashboard-card">
                        <div class="card-title" style="text-align:center">Open Quotations</div>
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Quotation#</th>
                                    <th>Company</th>
                                    <th>Value</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptPendingQuotations" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("QuotationNumber") %></td>
                                            <td><%# Eval("CompanyName") %></td>
                                            <td><%# Eval("GrandTotal") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>

            </div>
            <div class="row">
                <!-- Top Sales Reps -->
                <div class="col-md-4">
                    <div class="dashboard-card">
                        <div class="card-title" style="text-align:center">Top Sales Reps</div>

                        <asp:Repeater ID="rptTopReps" runat="server">
                            <ItemTemplate>
                                <div class="rep-item">
                                    <span><%# Eval("EmployeeName") %></span>
                                    <span><%# Eval("TotalsalesAmount") %></span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </div>
            <script>
                // Forecast Bar Chart
                new Chart(document.getElementById("forecastChart"), {
                    type: 'bar',
                    data: {
                        labels: ["Jan", "Feb", "Mar"],
                        datasets: [{
                            label: "Sales",
                            data: [800000, 1200000, 950000],
                            backgroundColor: "#1abc9c"
                        }]
                    }
                });

                // Donut - Reason
                new Chart(document.getElementById("reasonChart"), {
                    type: 'doughnut',
                    data: {
                        labels: ["Price", "Solution", "Service"],
                        datasets: [{
                            data: [2400000, 734000, 143000],
                            backgroundColor: ["#16a085", "#1abc9c", "#f39c12"]
                        }]
                    }
                });

                // Donut - Source
                new Chart(document.getElementById("sourceChart"), {
                    type: 'doughnut',
                    data: {
                        labels: ["Website", "Prospecting", "Events"],
                        datasets: [{
                            data: [2400000, 734000, 143000],
                            backgroundColor: ["#2c3e50", "#1abc9c", "#e67e22"]
                        }]
                    }
                });

    </script>

            <asp:GridView ID="gvDivisionSales" runat="server" AutoGenerateColumns="false"
                CssClass="table table-bordered" Visible="false">
                <Columns>
                    <asp:BoundField DataField="DepartmentName" HeaderText="Division" />
                    <asp:BoundField DataField="TotalSales" HeaderText="Total Sales" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="TotalTarget" HeaderText="Total Target" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="AchievementPercent" HeaderText="Achievement %" DataFormatString="{0:N2}%" />
                </Columns>
            </asp:GridView>

            <br />
            <asp:HiddenField ID="hfInquiry" runat="server" />
            <asp:HiddenField ID="hfQuotation" runat="server" />
            <asp:HiddenField ID="hfPO" runat="server" />
            <br />


            <script>
                document.addEventListener("DOMContentLoaded", function () {

                    var rawData = <%= string.IsNullOrEmpty(ChartDataJson) ? "[]" : ChartDataJson %>;

                    console.log(rawData);

                    if (!rawData || rawData.length === 0) {
                        alert("No Data Found");
                        return;
                    }

                    // Labels (Quarters)
                    var labels = rawData.map(x => x.Quarter);

                    // Get department names dynamically (exclude Quarter column)
                    var departments = Object.keys(rawData[0]).filter(key => key !== "Quarter");

                    var datasets = departments.map(function (dept) {
                        return {
                            label: dept,
                            data: rawData.map(row => row[dept] || 0),
                            barThickness: 25,
                            maxBarThickness: 30,
                            categoryPercentage: 0.5,
                            barPercentage: 0.6
                        };
                    });

                    var ctx = document.getElementById('quotationChart').getContext('2d');

                    new Chart(ctx, {
                        type: 'bar',
                        data: {
                            labels: labels,
                            datasets: datasets
                        },
                        options: {
                            indexAxis: 'y',   // horizontal
                            responsive: true,
                            plugins: {
                                title: {
                                    display: true,
                                    text: 'Quarterly Department-wise Quotation'
                                },
                                legend: {
                                    position: 'bottom'
                                }
                            },
                            scales: {
                                x: { stacked: true },
                                y: { stacked: true }
                            }
                        }
                    });

                });
            </script>
            <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>


            <script>
                function loadChart1(labels, noQuotation, quotation, invoice) {
                    new Chart(document.getElementById("salesChart"), {
                        type: 'bar',
                        data: {
                            labels: labels,
                            datasets: [
                                {
                                    label: 'Enquiry Only',
                                    data: noQuotation,
                                    backgroundColor: '#FFF3B0'
                                },
                                {
                                    label: 'Quotation Generated',
                                    data: quotation,
                                    backgroundColor: '#FFD6A5'
                                },
                                {
                                    label: 'Invoice Generated',
                                    data: invoice,
                                    backgroundColor: '#B7E4C7'
                                }
                            ]
                        },
                        options: {
                            indexAxis: 'y',  // Makes it horizontal
                            responsive: true,
                            plugins: {
                                legend: {
                                    position: 'bottom'
                                }
                            },
                            scales: {
                                x: {
                                    stacked: true,
                                    title: {
                                        display: true,
                                        text: 'Number of Inquiries'
                                    }
                                },
                                y: {
                                    stacked: true,
                                    title: {
                                        display: true,
                                        text: 'Departments'
                                    }
                                }
                            }
                        }
                    });
                }
            </script>
            <canvas id="salesFunnel" height="50" width="auto" style="visibility: hidden"></canvas>
            <canvas id="divisionChart" height="120"></canvas>
            <script>
                function loadChart(labels, data) {

                    const ctx = document.getElementById('divisionChart').getContext('2d');

                    // Generate different colors dynamically
                    const backgroundColors = [];
                    const borderColors = [];

                    for (let i = 0; i < labels.length; i++) {
                        const r = Math.floor(Math.random() * 200);
                        const g = Math.floor(Math.random() * 200);
                        const b = Math.floor(Math.random() * 200);

                        backgroundColors.push(`rgba(${r}, ${g}, ${b}, 0.6)`);
                        borderColors.push(`rgba(${r}, ${g}, ${b}, 1)`);
                    }

                    new Chart(ctx, {
                        type: 'bar',
                        data: {
                            labels: labels,
                            datasets: [{
                                label: 'Total Sales',
                                data: data,
                                backgroundColor: backgroundColors,
                                borderColor: borderColors,
                                borderWidth: 1
                            }]
                        },
                        options: {
                            responsive: true,
                            plugins: {
                                tooltip: {
                                    enabled: true
                                },
                                legend: {
                                    display: false
                                }
                            },
                            scales: {
                                y: {
                                    beginAtZero: true
                                }
                            }
                        }
                    });
                }
            </script>
            <div id="salesPyramid" style="visibility: hidden"></div>
            <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
            <script>
                document.addEventListener("DOMContentLoaded", function () {

                    var options = {
                        series: [{
                            name: "Count",
                            data: [
                                parseInt(document.getElementById('<%= hfInquiry.ClientID %>').value),
                                parseInt(document.getElementById('<%= hfQuotation.ClientID %>').value),
                                parseInt(document.getElementById('<%= hfPO.ClientID %>').value)
                            ]
                        }],
                        chart: {
                            type: 'bar',
                            height: 500
                        },
                        plotOptions: {
                            bar: {
                                horizontal: true,   // must be TRUE
                                isFunnel: true      // enables funnel/pyramid mode
                            }
                        },
                        dataLabels: {
                            enabled: true
                        },
                        xaxis: {
                            categories: [
                                "Customer Inquiry",
                                "Quotation",
                                "PO Received"
                            ]
                        },
                        colors: ['#36A2EB', '#FFCE56', '#4BC0C0'],
                        legend: {
                            show: false
                        }
                    };

                    var chart = new ApexCharts(document.querySelector("#salesPyramid"), options);
                    chart.render();

                });
            </script>

            <!-- Load Dept wise PIE-->
            <script>
                new Chart(document.getElementById("deptPieChart"), {
                    type: 'pie',
                    data: {
                        labels: [<%= DeptLabels %>],
                        datasets: [{
                            data: [<%= DeptValues %>],
                            backgroundColor: [
                                "#A8DADC",  // Pastel Teal
                                "#BDE0FE",  // Pastel Blue
                                "#FFC8DD",  // Pastel Pink
                                "#CDB4DB",  // Pastel Purple
                                "#FFDDD2",  // Pastel Peach
                                "#D8F3DC",  // Pastel Mint
                                "#F1FAEE",  // Soft Cream
                                "#E9EDC9"   // Pastel Olive
                            ]
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: {
                                position: 'bottom'
                            }
                        }
                    }
                });
            </script>

        </div>
</asp:Content>
