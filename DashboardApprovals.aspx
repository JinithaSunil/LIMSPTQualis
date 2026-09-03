<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DashboardApprovals.aspx.cs" Inherits="LIMS.DashboardApprovals" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>

    <style>
        .online, .idle {
            height: 12px;
            width: 12px;
            border-radius: 50%;
            display: inline-block;
            margin-right: 6px;
        }

        /* Green bulb */
        .online {
            background-color: #28a745;
            box-shadow: 0 0 6px #28a745;
        }

        /* Yellow bulb */
        .idle {
            background-color: #ffc107;
            box-shadow: 0 0 6px #ffc107;
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

    <!-- Process flow -->
    <style>
        .process {
            display: flex;
            font-family: Arial;
        }

        .step {
            position: relative;
            padding: 20px 45px;
            background: #1f6f8b;
            color: #fff;
            margin-right: 10px;
            text-align: center;
        }

            .step::after {
                content: "";
                position: absolute;
                top: 0;
                right: -30px;
                width: 0;
                height: 0;
                border-top: 35px solid transparent;
                border-bottom: 35px solid transparent;
                border-left: 30px solid #1f6f8b;
            }

            .step:last-child::after {
                display: none;
            }

            .step span {
                font-size: 22px;
                font-weight: bold;
            }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="dashboard-container">

        <!-- Main Dashboard Card -->
        <div class="card border-top-purple">
            <div class="chart-header">
                <h3><i class="fa-solid fa-image"></i>DASHBOARD</h3>
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
                                <asp:ListItem Selected="True">DAY</asp:ListItem>
                                <asp:ListItem>WEEK</asp:ListItem>
                                <asp:ListItem>YEAR</asp:ListItem>
                                <asp:ListItem>ALL</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>

                <!-- Stats Cards - PR Approval -->
                <div class="row">
                    <div id="divPR" runat="server" class="col-xl-3 col-lg-3 col-md-6 col-sm-12 mb-4">
                        <div class="card stats-card shadow-sm h-100">
                            <div class="card-body bg-purple-light">
                                <div class="d-flex justify-content-between align-items-center">
                                    <table style="text-align: center; width: 100%">
                                        <tr>
                                            <td colspan="2">
                                                <h4>PR</h4>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-purple">
                                                <h4>
                                                    <asp:Label ID="lblPendingPR" runat="server" Text="0"></asp:Label></h4>
                                            </td>
                                            <td class="text-purple">
                                                <h4>
                                                    <asp:Label ID="lblAllPR" runat="server" Text="0"></asp:Label></h4>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Pending</td>
                                            <td>Completed</td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 mb-4">
                        <div class="card stats-card shadow-sm h-100">
                            <div class="card-body bg-purple-light">
                                <div class="d-flex justify-content-between align-items-center">
                                    <table style="text-align: center; width: 100%">
                                        <tr>
                                            <td colspan="2">
                                                <h4>PR</h4>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-purple">
                                                <h4>
                                                    <asp:Label ID="Label3" runat="server" Text="0"></asp:Label></h4>
                                            </td>
                                            <td class="text-purple">
                                                <h4>
                                                    <asp:Label ID="Label4" runat="server" Text="0"></asp:Label></h4>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Pending</td>
                                            <td>Completed</td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <!-- Stats Cards - Quotation -->
                <div class="row">
                    <asp:PlaceHolder ID="phProcess" runat="server"></asp:PlaceHolder>
                </div>
                <hr />
                <div class="row" style="text-align: center">
                    <h5>ONLINE USERS
                        <asp:Label ID="lblCount" runat="server" CssClass="user-count"></asp:Label>
                    </h5>
                </div>
                <div class="row">
                    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" CssClass="table">
                        <Columns>
                            <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" />
                            <asp:BoundField DataField="LoginTime" HeaderText="Login Time" DataFormatString="{0:dd-MMM-yyyy HH:mm}" />
                            <asp:BoundField DataField="Duration" HeaderText="Active Duration" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <span class='<%# Eval("StatusClass") %>'></span>
                                    <%# Eval("StatusText") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

    </div>

</asp:Content>
