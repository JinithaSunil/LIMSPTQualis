<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DashboardAccounting1.aspx.cs" Inherits="LIMS.DashboardAccounting1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --primary: #2C3E50; /* Deep Blue-Grey */
            --primary-light: #34495E;
            --success: #27AE60;
            --danger: #E74C3C;
            --warning: #F39C12;
            --info: #2980B9;
            --bg: #F4F6F9;
            --card-bg: #FFFFFF;
            --text-dark: #2C3E50;
            --text-light: #7F8C8D;
        }

        .kpi-card {
            background: #fff;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            transition: 0.3s ease;
        }

            .kpi-card:hover {
                transform: translateY(-3px);
            }

        .kpi-title {
            font-size: 13px;
            color: var(--text-light);
            font-weight: 500;
        }

        .kpi-value {
            font-size: 26px;
            font-weight: 700;
            color: var(--text-dark);
        }

        .summary-card {
            background: linear-gradient(135deg, #2C3E50, #34495E);
            color: white;
            border-radius: 12px;
            padding: 25px;
        }

        .chart-card {
            background: #fff;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        }

        .card {
            margin-bottom: 25px;
        }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script>
        function renderChart(months, income, expense) {

            const ctx = document.getElementById('incomeExpenseChart').getContext('2d');

            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: months,
                    datasets: [
                        {
                            label: 'Income',
                            data: income,
                            tension: 0.3
                        },
                        {
                            label: 'Expense',
                            data: expense,
                            tension: 0.3
                        }
                    ]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: { position: 'top' },
                        title: {
                            display: true,
                            text: 'Income vs Expense (Yearly)'
                        }
                    }
                }
            });
        }

    </script>

    <style>
        /* Layout */
        .container {
            display: flex;
            height: 100vh;
        }


        /* Main Content */
        .main {
            flex: 1;
            padding: 20px;
            overflow-y: auto;
        }

        /* KPI Cards */
        .cards {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 15px;
            margin-bottom: 20px;
        }

        .card {
            padding: 20px;
            border-radius: 0px;
            color: #fff;
            font-size: 14px;
        }

        .cash {
            background: #F6E6C9;
        }

        .bank {
            background: #DCEEFF;
        }

        .recv {
            background: #DFF5E4;
        }

        .pay {
            background: #FFE1E1;
        }

        .card h3 {
            font-size: 14px;
            font-weight: 600;
            color: #5D6D7E;
            margin-bottom: 10px;
        }

        .card p {
            font-size: 20px;
            font-weight: 700;
            color: #2C3E50;
        }

        /* Middle Section */
        .middle {
            display: grid;
            grid-template-columns: 2fr 1fr;
            gap: 20px;
            margin-bottom: 20px;
        }

        .box {
            background: #fff;
            border-radius: 0px;
            padding: 15px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.08);
        }

            .box h4 {
                margin: 0 0 10px;
                font-size: 15px;
                border-bottom: 1px solid #ddd;
                padding-bottom: 6px;
            }

        .chart-placeholder {
            height: 220px;
            background: repeating-linear-gradient( 45deg, #f1f1f1, #f1f1f1 10px, #e5e5e5 10px, #e5e5e5 20px );
            border-radius: 4px;
        }

        /* Quick Actions */
        .quick-actions button {
            width: 100%;
            padding: 12px;
            margin-bottom: 10px;
            border: none;
            color: #fff;
            font-size: 14px;
            border-radius: 4px;
            cursor: pointer;
        }

        .btn-green {
            background: #27ae60;
        }

        .btn-blue {
            background: #2980b9;
        }

        .btn-orange {
            background: #f39c12;
        }

        .btn-red {
            background: #e74c3c;
        }

        /* Bottom Section */
        .bottom {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 20px;
            color:dimgray;
        }

        .list-item {
            padding: 8px 0;
            border-bottom: 1px solid #eee;
            font-size: 14px;
        }

            .list-item:last-child {
                border-bottom: none;
            }

        .kpi-container {
            display: flex;
            gap: 20px;
            margin-bottom: 20px;
        }

        .kpi {
            flex: 1;
            padding: 15px;
            border-radius: 6px;
            background: #f4f6f8;
            text-align: center;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
        }

            .kpi span {
                font-size: 18px;
                font-weight: bold;
            }

        .income {
            border-left: 5px solid #E9F9EF;
            font-size:16px;
        }

        .expense {
            border-left: 5px solid #FDECEC;
        }

        .profit {
            border-left: 5px solid #EAF4FF;
        }

        .profit-status {
            margin-top: 10px;
            font-size: 18px;
            font-weight: bold;
        }
        /* TOP ROW */
.bg-cash        { background: #F6E6C9; }
.bg-bank        { background: #DCEEFF; }
.bg-receivable  { background: #DFF5E4; }
.bg-payable     { background: #FFE1E1; }

/* SECOND ROW */
.bg-ap          { background: #FFD6D6; }
.bg-ar          { background: #CFF3E3; }
.bg-approval    { background: #E6DBF5; }
.bg-completed   { background: #DDE7F8; }

/* PROFIT SECTION */
.bg-income      { background: #E9F9EF; }
.bg-expense     { background: #FDECEC; }
.bg-profit      { background: #EAF4FF; }
    </style>
    <style>
        .recent-activities {
            font-family: Segoe UI, Arial;
        }

        .activity-item {
            padding: 10px 0;
            border-bottom: 1px solid #eee;
        }

        .activity-time {
            font-size: 12px;
            color: #888;
            margin-bottom: 3px;
        }

        .activity-text {
            font-size: 14px;
            color: #333;
        }

            .activity-text strong {
                color: #8b3d73;
            }

            .activity-text .ref {
                display: block;
                font-size: 12px;
                color: #666;
            }

        .card-title {
            font-size: 14px;
            font-weight: 200;
            color: #5D6D7E;
            margin-bottom: 10px;
        }

        .card-value {
            font-size: 28px;
            font-weight: 700;
            color: #2C3E50;
        }
           /* Base Styles */
   .dashboard-container {
       padding: 20px;
       max-width: 1400px;
       margin: 0 auto;
       font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
   }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="dashboard-container">

        <!-- Main Dashboard Card -->
        <div class="card border-top-purple">
            <div class="chart-header">
                <h4 style="color:black"><i class="fa-solid fa-chart-line"></i>ACCOUNTS DASHBOARD</h4>
            </div>
            <hr />
            <div class="card-body">

                <!-- Main -->
                <div class="main">

                    <!-- KPI Cards -->
                    <div class="cards">
                        <div class="card cash bg-cash">
                            <h3 class="text">CASH BALANCE</h3>
                            <p><span id="spnCashBalance" runat="server"></span></p>
                        </div>
                        <div class="card bank">
                            <h3>Bank Balance</h3>
                            <p><span id="spnBankBal" runat="server"></span></p>
                        </div>
                        <div class="card recv">
                            <h3>Receivables</h3>
                            <p><span id="spnReceivables" runat="server"></span></p>
                        </div>
                        <div class="card pay">
                            <h3>Payables</h3>
                            <p><span id="spnPayables" runat="server"></span></p>
                        </div>
                    </div>
                    <div id="divAdmin" runat="server" class="row">
                        <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                            <div class="info-box-2 hover-zoom-effect" style="margin-top: 0px !important; background-color: #FFD6D6">

                                <div class="content">
                                    <div class="text">
                                        ACCTS. PAYABLE
                                    </div>
                                   <p> <asp:HyperLink ID="hypAP" runat="server" class="number" NavigateUrl="~/AccountsPayable.aspx"></asp:HyperLink></p>
                                </div>
                            </div>

                        </div>
                        <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                            <div class="info-box-2 hover-zoom-effect" style="background-color:#CFF3E3">

                                <div class="content">
                                    <div class="text">ACCTS. RECEIVABLE</div>
                                    <asp:HyperLink ID="hypAR" runat="server" class="number" NavigateUrl="~/AccountsReceivable.aspx"></asp:HyperLink>
                                </div>
                            </div>

                        </div>
                        <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                            <div class="info-box-2 hover-zoom-effect" style="background-color:#E6DBF5">

                                <div class="content">
                                    <div class="text">APPROVALS</div>
                                    <asp:HyperLink ID="hypAssigned" runat="server" class="number"></asp:HyperLink>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                            <div class="info-box-2 hover-zoom-effect" style="background-color: #DDE7F8">
                                <div class="icon">
                                    <i class="material-icons">equalizer</i>
                                </div>
                                <div class="content">
                                    <div class="text">COMPLETED</div>
                                    <asp:HyperLink ID="hypCompleted" runat="server" class="number"></asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- Middle -->
                    <div class="middle">
                        <div class="kpi-container">
                            <div class="kpi income bg-income" style="color: #5D6D7E;">
                                <h3>Total Income</h3>
                                <span id="lblIncome" runat="server">0</span>
                            </div>

                            <div class="kpi expense bg-expense" style="color: #5D6D7E;">
                                <h3>Total Expense</h3>
                                <span id="lblExpense" runat="server">0</span>
                            </div>

                            <div class="kpi profit bg-profit" style="color: #5D6D7E;">
                                <h3>Net Profit / Loss</h3>
                                <span id="lblProfit" runat="server">0</span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-9">
                            <canvas id="incomeExpenseChart" height="120"></canvas>

                            <div id="profitStatus" class="profit-status" runat="server"></div>

                        </div>
                        <div class="col-sm-3">
                            <div class="box quick-actions" >
                                <h3>Quick Actions</h3>
                                <button style="background-color:#DFF5E4;color:#5D6D7E" onclick="window.open('AccountsReceivable.aspx', '_blank');">+ New Receipt</button>
                                <button style="background-color:#DCEEFF;color:#5D6D7E" onclick="window.open('AccountsPayable.aspx', '_blank');">+ New Payment</button>
                                <button style="background-color:#FFE1E1;color:#5D6D7E" onclick="window.open('JournalEntry.aspx', '_blank');">
                                    + New Journal
                                </button>
                                <button style="background-color:#F6E6C9;color:#5D6D7E" onclick="window.open('Menus.aspx', '_blank');">View All</button>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <!-- Bottom -->
                        <div class="bottom">
                            <div class="box">
                                <h4>Tasks & Alerts</h4>
                                <div class="list-item">3 Pending Approvals</div>
                                <div class="list-item"><a href="InvoiceList.aspx?od=1" target="_blank"><span id="spnODInv" runat="server">0</span> Overdue Invoices</a></div>
                                <div class="list-item"><span id="spnCashBal" runat="server">0</span> Cash Balance</div>
                                <div class="list-item"><a href="ChequeRegister.aspx" target="_blank"><span id="spnChq" runat="server">0</span> Cheque(s) to Clear</a></div>
                            </div>

                            <div class="box">
                                <h4>Recent Activities</h4>
                                <asp:Repeater ID="rptActivity" runat="server">
                                    <ItemTemplate>
                                        <div class="recent-activities">
                                            <div class="activity-item">
                                                <div class="activity-time"><%#Eval("ActivityDate","{0:dd MMM yyyy . HH:mm tt}") %> </div>
                                                <div class="activity-text">
                                                    <strong><%#Eval("ModuleName") %></strong> — <%#Eval("ReferenceNo") %>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <!-- View All link -->
                                <a href="ActivityLog.aspx" class="view-all" target="_blank">View All
                                </a>
                            </div>

                            <div class="box">
                                <h4>Upcoming Reminders</h4>
                                <div class="list-item">Tax Filing Due Tomorrow</div>
                                <div class="list-item">Vendor Payment in 2 Days</div>
                                <div class="list-item">Audit Scheduled</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
