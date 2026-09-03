<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DashboardPurchasing.aspx.cs" Inherits="LIMS.DashboardPurchasing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>


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
    <div class="dashboard-container">

        <!-- Main Dashboard Card -->
        <div class="card border-top-purple">
            <div class="chart-header">
                <h3><i class="fa-solid fa-image"></i>PURCHASE DASHBOARD</h3>
                <div class="chart-controls">
                    <button class="refresh-btn" id="refreshChart">
                        <i class="fas fa-sync-alt"></i>Refresh
                    </button>
                </div>
            </div>

            <div class="card-body">
                <!-- Period Selector -->
                <div class="row mb-4">
                    <div class="col-12">
                        <div class="form-group period-selector">
                            <asp:RadioButtonList ID="radPeriod" runat="server" OnSelectedIndexChanged="radPeriod_SelectedIndexChanged"
                                AutoPostBack="true" RepeatDirection="Horizontal" CssClass="rbl" Font-Bold="true">
                                <asp:ListItem >DAY</asp:ListItem>
                                <asp:ListItem>WEEK</asp:ListItem>
                                <asp:ListItem>YEAR</asp:ListItem>
                                <asp:ListItem Selected="True">ALL</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>

                <!-- Stats Cards -->
                <div class="row">
                    <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 mb-4">
                        <div class="card stats-card shadow-sm h-100">
                            <div class="card-body bg-purple-light">
                                <div class="d-flex justify-content-between align-items-center">
                                    <div>
                                        <h3 class="text-purple">
                                            <asp:Label ID="lblPending" runat="server" Text="0"></asp:Label>
                                        </h3>
                                        <p class="text-muted mb-0" style="font-weight: bold">Pending PR</p>
                                    </div>
                                    <div class="icon-box bg-purple-light">
                                        <i class="fa fa-clock-o text-purple"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer p-0">
                                <div class="d-flex">
                                    <asp:LinkButton ID="lnkPendingPR" runat="server" CssClass="btn bg-c-purple flex-grow-1"
                                        CommandName="PendingPR" OnCommand="LinkButton_Command">
                                <i class="fa fa-list"></i> All Dept.
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lnkViewItem" runat="server" CssClass="btn bg-c-purple border-start"
                                        CommandName="ViewItems" OnCommand="LinkButton_Command">
                                <i class="fa fa-list"></i> Purchase
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 mb-4">
                        <div class="card stats-card shadow-sm h-100">
                            <div class="card-body bg-green-light">
                                <div class="d-flex justify-content-between align-items-center">
                                    <div>
                                        <h3 class="text-green">
                                            <asp:Label ID="lblEnquirySent" runat="server" Text="0"></asp:Label>
                                        </h3>
                                        <p class="text-muted mb-0" style="font-weight: bold">Enquiry Sent</p>
                                    </div>
                                    <div class="icon-box bg-green-light">
                                        <i class="fa fa-share-square-o text-green"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer p-0">
                                <div class="d-flex">
                                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn bg-c-green flex-grow-1"
                                        CommandName="PendingPR" OnCommand="LinkButton_Command">
                                <i class="fa fa-plus-square"></i> Add New
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn bg-c-green border-start"
                                        CommandName="ViewItems" OnCommand="LinkButton_Command">
                                <i class="fa fa-list"></i> View
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 mb-4">
                        <div class="card stats-card shadow-sm h-100">
                            <div class="card-body bg-red-light">
                                <div class="d-flex justify-content-between align-items-center">
                                    <div>
                                        <h3 class="text-red">
                                            <asp:Label ID="lblQuoteReceived" runat="server" Text="0"></asp:Label>
                                        </h3>
                                        <p class="text-muted mb-0" style="font-weight: bold">Quote Received</p>
                                    </div>
                                    <div class="icon-box bg-red-light">
                                        <i class="fa fa-file-text-o text-red"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer p-0">
                                <div class="d-flex">
                                    <asp:LinkButton ID="LinkButton3" runat="server" CssClass="btn bg-c-red flex-grow-1"
                                        CommandName="PendingPR" OnCommand="LinkButton_Command">
                                <i class="fa fa-plus-square"></i> Add New
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="LinkButton4" runat="server" CssClass="btn bg-c-red border-start"
                                        CommandName="ViewItems" OnCommand="LinkButton_Command">
                                <i class="fa fa-list"></i> View
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 mb-4">
                        <div class="card stats-card shadow-sm h-100">
                            <div class="card-body bg-blue-light">
                                <div class="d-flex justify-content-between align-items-center">
                                    <div>
                                        <h3 class="text-blue">
                                            <asp:Label ID="lblPOPending" runat="server" Text="0"></asp:Label>
                                        </h3>
                                        <p class="text-muted mb-0" style="font-weight: bold">PO Pending</p>
                                    </div>
                                    <div class="icon-box bg-blue-light">
                                        <i class="fa fa-file-powerpoint-o text-blue"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer p-0">
                                <div class="d-flex">
                                    <asp:LinkButton ID="LinkButton5" runat="server" CssClass="btn bg-c-blue flex-grow-1"
                                        CommandName="PendingPR" OnCommand="LinkButton_Command">
                                <i class="fa fa-plus-square"></i> Add New
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="LinkButton6" runat="server" CssClass="btn bg-c-blue border-start"
                                        CommandName="ViewItems" OnCommand="LinkButton_Command">
                                <i class="fa fa-list"></i> View
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Chart Section -->
        <div class="chart-section card border-top-purple">
            <div class="chart-header">
                <h3><i class="fa-solid fa-sliders"></i>PENDING PURCHASE REQUEST APPROVALS</h3>
                <div class="chart-controls">
                    <div>Total pending requests: <span class="total-badge" id="totalBadge">0</span></div>

                </div>
            </div>

            <div class="chart-container">
                <div id="approvalChart" style="width: 100%; height: 400px;"></div>

            </div>

            <div class="chart-footer">
                <div class="last-updated" style="margin-left: auto;">Last updated: <span id="updateTime">Just now</span></div>
            </div>

        </div>

        <!--Top Suupplier chart-->

        <div class="card border-top-purple">
            <div class="chart-header bg-gradient-purple text-white">
                <h3 class="mb-0">
                    <i class="fas fa-trophy me-2"></i>TOP SUPPLIERS OF 
            <asp:Label ID="lblMonth" runat="server" CssClass="fw-bold"></asp:Label>
                </h3>
            </div>
            <div class="card-body">
                <!-- Key Metrics Row -->
                <div class="row mb-4">
                    <div class="col-md-4">
                        <div class="card metric-card bg-purple-light">
                            <div class="card-body text-center py-2">
                                <small class="text-muted"><b>TOTAL SUPPLIERS</b></small>
                                <h4 class="mb-0">
                                    <asp:Label ID="lblTotalSuppliers" runat="server"></asp:Label></h4>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="card metric-card bg-purple-light">
                            <div class="card-body text-center py-2">
                                <small class="text-muted"><b>TOP SUPPLIER SHARE</b></small>
                                <h4 class="mb-0">
                                    <asp:Label ID="lblTopShare" runat="server"></asp:Label></h4>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="card metric-card bg-purple-light">
                            <div class="card-body text-center py-2">
                                <small class="text-muted"><b>AVG ORDERS</b></small>
                                <h4 class="mb-0">
                                    <asp:Label ID="lblAvgOrders" runat="server"></asp:Label></h4>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Charts Row -->
                <div class="row">
                    <!-- By Value -->
                    <div class="col-lg-6 col-md-12 mb-4">
                        <div class="card h-100 shadow-sm">
                            <div class="card-header bg-blue-light">
                                <h5 class="mb-0">TOP 5 BY VALUE</h5>
                            </div>
                            <div class="card-body">
                                <div id="piechart1_3d" style="width: 100%; height: 300px;"></div>
                                <div class="mt-3">
                                    <asp:Repeater ID="rptValueRanking" runat="server">
                                        <ItemTemplate>
                                            <div class="d-flex justify-content-between align-items-center py-2 border-bottom">
                                                <span class="text-truncate" style="max-width: 70%">
                                                    <i class="fas fa-<%# Container.ItemIndex == 0 ? "crown text-warning" : "circle" %> me-2"></i>
                                                    <%# Eval("VendorName") %>
                                                </span>
                                                <span class="badge bg-primary rounded-pill">IDR <%# Eval("Total", "{0:N2}") %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- By Count -->
                    <div class="col-lg-6 col-md-12">
                        <div class="card h-100 shadow-sm">
                            <div class="card-header bg-blue-light">
                                <h5 class="mb-0">TOP 5 BY COUNT</h5>
                            </div>
                            <div class="card-body">
                                <div id="piechart2_3d" style="width: 100%; height: 300px;"></div>
                                <div class="mt-3">
                                    <asp:Repeater ID="rptCountRanking" runat="server">
                                        <ItemTemplate>
                                            <div class="d-flex justify-content-between align-items-center py-2 border-bottom">
                                                <span class="text-truncate" style="max-width: 70%">
                                                    <i class="fas fa-<%# Container.ItemIndex == 0 ? "crown text-warning" : "circle" %> me-2"></i>
                                                    <%# Eval("VendorName") %>
                                                </span>
                                                <span class="badge bg-primary rounded-pill"><%# Eval("Total") %> orders</span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="chart-footer">
                <div class="last-updated" style="margin-left: auto;">Last updated: <span id="updateTime1">Just now</span></div>
            </div>
        </div>
    </div>



    <script type="text/javascript">
        google.charts.load('current', { 'packages': ['corechart'] });
        google.charts.setOnLoadCallback(drawChart);

        function drawChart() {
            // Get chart data from server-side code
            var chartData = <%= GetChartDataJson() %>;
            var data = google.visualization.arrayToDataTable(chartData);

            var options = {
                title: '',
                isStacked: true,
                colors: ['#4e79a7', '#f28e2b', '#e15759', '#76b7b2'],
                hAxis: {
                    title: 'Number of Requests',
                    minValue: 0,
                    format: '0',
                    viewWindow: {
                        min: 0
                    },
                    textStyle: { fontSize: 12 }
                },
                vAxis: {
                    title: 'Departments',
                    textStyle: { fontSize: 12 }
                },
                bar: { groupWidth: '75%' },
                legend: {
                    position: 'bottom',
                    textStyle: { fontSize: 12 }
                },
                chartArea: { width: '85%', height: '70%', top: 20 },
                animation: {
                    duration: 1000,
                    easing: 'out',
                    startup: true
                },
                backgroundColor: 'transparent'
            };

            var chart = new google.visualization.BarChart(document.getElementById('approvalChart'));
            chart.draw(data, options);

            // Update summary data after chart is drawn
            updateSummaryData();

            // Setup refresh functionality
            setupRefresh(chart, data, options);

            // Initialize and update time
            updateTime();
            updateTime1();
        }

        function setupRefresh(chart, data, options) {
            // Refresh functionality
            document.getElementById('refreshChart').addEventListener('click', function () {
                showNotification('Refreshing chart...');
                // Reload the page to get fresh data
                setTimeout(function () {
                    location.reload();
                }, 500);
            });
        }

        function showNotification(message) {
            // Create notification element if it doesn't exist
            var notification = document.getElementById('notification');
            if (!notification) {
                notification = document.createElement('div');
                notification.id = 'notification';
                notification.className = 'notification';
                document.body.appendChild(notification);
            }

            notification.textContent = message;
            notification.classList.add('show');
            setTimeout(function () {
                notification.classList.remove('show');
            }, 3000);
        }

        // Update time display
        function updateTime() {
            const now = new Date();
            const timeString = now.toLocaleTimeString() + ' ' + now.toLocaleDateString();

            const el = document.getElementById('updateTime');
            if (el) {
                el.textContent = timeString;
            }

            // Update time every minute
            setTimeout(updateTime, 60000);
        }
        function updateTime1() {
            const now = new Date();
            const timeString = now.toLocaleTimeString() + ' ' + now.toLocaleDateString();

            const el = document.getElementById('updateTime1');
            if (el) {
                el.textContent = timeString;
            }

            // Update time every minute
            setTimeout(updateTime, 60000);
        }

        // Calculate and display summary data
        function updateSummaryData() {
            try {
                var chartData = <%= GetChartDataJson() %>;

                if (!chartData || chartData.length <= 1) {
                    console.warn('No chart data available');
                    return;
                }

                var totals = {
                    Tier1: 0,
                    Tier2: 0,
                    Tier3: 0,
                    Tier4: 0
                };

                // Skip header row and calculate totals
                for (var i = 1; i < chartData.length; i++) {
                    if (chartData[i] && chartData[i].length >= 5) {
                        totals.Tier1 += parseInt(chartData[i][1]) || 0;
                        totals.Tier2 += parseInt(chartData[i][2]) || 0;
                        totals.Tier3 += parseInt(chartData[i][3]) || 0;
                        totals.Tier4 += parseInt(chartData[i][4]) || 0;
                    }
                }

                // Calculate grand total
                var grandTotal = totals.Tier1 + totals.Tier2 + totals.Tier3 + totals.Tier4;

                // Update total badge
                var totalBadgeEl = document.getElementById('totalBadge');
                if (totalBadgeEl) {
                    totalBadgeEl.textContent = grandTotal;
                }

                // Update summary table if elements exist
                if (document.getElementById('Tier1Count')) {
                    document.getElementById('Tier1Count').textContent = totals.Tier1;
                }
                if (document.getElementById('Tier2Count')) {
                    document.getElementById('Tier2Count').textContent = totals.Tier2;
                }
                if (document.getElementById('Tier3Count')) {
                    document.getElementById('Tier3Count').textContent = totals.Tier3;
                }
                if (document.getElementById('Tier4Count')) {
                    document.getElementById('Tier4Count').textContent = totals.Tier4;
                }

                // Update department list dynamically if element exists
                var departmentList = document.getElementById('departmentList');
                if (departmentList) {
                    departmentList.innerHTML = '';
                    for (var i = 1; i < chartData.length; i++) {
                        if (chartData[i] && chartData[i][0]) {
                            var li = document.createElement('li');
                            li.innerHTML = '<i class="fas fa-building"></i> ' + chartData[i][0] + ' Department';
                            departmentList.appendChild(li);
                        }
                    }
                }

                console.log('Summary updated - Total requests:', grandTotal);

            } catch (error) {
                console.error('Error updating summary data:', error);
            }
        }

        // Initialize everything when page loads
        document.addEventListener('DOMContentLoaded', function () {
            // Set initial time if chart hasn't loaded yet
            updateTime();
            updateTime1();
        });
    </script>
</asp:Content>
