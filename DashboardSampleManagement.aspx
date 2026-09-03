<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DashboardSampleManagement.aspx.cs" Inherits="LIMS.DashboardSampleManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0"></script>
<script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2.2.0"></script>
    <script>
        window.onload = function () {
            let line = document.querySelector('.timeline-container::before');
        }
    </script>
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
            border-radius: 14px;
            padding: 18px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            transition: transform 0.2s ease;
        }

            .kpi-card:hover {
                transform: translateY(-4px);
            }


            .kpi-card h2 {
                font-weight: 700;
                margin: 0;
            }

            .kpi-card small {
                opacity: 0.8;
            }
        /* Individual pastel colors */

        .pastel-blue {
            background-color: #e3f2fd;
        }

        .pastel-pink {
            background-color: #fce4ec;
        }

        .pastel-yellow {
            background-color: #fff8e1;
        }

        .pastel-green {
            background-color: #e8f5e9;
        }

        .pastel-purple {
            background-color: #f3e5f5;
        }

        /* Value styling */
        .kpi-value {
            font-size: 28px;
            font-weight: 600;
        }

        .kpi-title {
            font-size: 15px;
            color: #555;
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
            padding: 2%;
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
        /* 🌈 Pastel Colors Per Stage */
        .card[data-stage="Enquiry"] {
            background: #F3E8FF; /* pastel purple */
        }

        .card[data-stage="TAF"] {
            background: #FFE8D6; /* pastel peach */
        }

        .card[data-stage="SampleReceived"] {
            background: #DFF6F0; /* pastel teal */
        }

        .card[data-stage="Allocated"] {
            background: #E3F2FD; /* pastel blue */
        }

        .card[data-stage="Tested"] {
            background: #FFF9C4; /* pastel yellow */
        }

        .card[data-stage="ReportGenerated"] {
            background: #E8F5E9; /* pastel green */
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="dashboard-container">

        <!-- Main Dashboard Card -->
        <div class="card border-top-purple">
            <div class="chart-header">
                <h3><i class="fa-solid fa-flask"></i>SAMPLE MANAGEMENT DASHBOARD</h3>
            </div>

            <div class="card-body">
                <div class="row">
                    <div class="col-md-3">
                        <asp:DropDownList ID="ddlYear"
                            runat="server" CssClass="form-control" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlYear_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <asp:DropDownList
                            ID="ddlMonth"
                            runat="server"
                            CssClass="form-control"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <asp:Repeater ID="rptCards" runat="server">
                        <ItemTemplate>
                            <div class="col-md-3">
                                <div class="card" data-stage='<%# Eval("StageName").ToString().Trim() %>'>
                                    <div class="card-icon">
                                        <%--<i class='<%# Eval("IconClass") %>'></i>--%>
                                    </div>
                                    <div class="card-content">
                                        <div class="card-title"><%# Eval("StageName") %></div>
                                        <div class="card-count"><%# Eval("PendingCount") %></div>
                                        <div class="card-delay">
                                            Delayed: <%# Eval("DelayedCount") %>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-4">
                        <div style="text-align: center; font-weight: bold">
                            Sample Verification Status   
                        </div>
                        <br />
                        <canvas id="verificationChart"></canvas>
                    </div>
                    <div class="col-md-6">
                        <div style="text-align: center; font-weight: bold">
                            Weekly Sample Count
                        </div>
                        <br />
                        <canvas id="weeklyChart"></canvas>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>
