<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DashboardMgmt.aspx.cs" Inherits="LIMS.DashboardMgmt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-chart-matrix@2.0.1/dist/chartjs-chart-matrix.min.js"></script>
    <!--GANTT ALLOCATION CHART -->
    <script src="https://www.gstatic.com/charts/loader.js"></script>
    <style>
        #testTypeCounts {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 40px;
            margin-top: 10px;
        }

        .testTypeItem {
            display: flex;
            flex-direction: column;
            align-items: center;
        }

        .dot {
            width: 10px;
            height: 10px;
            border-radius: 50%;
            display: inline-block;
            margin-right: 6px;
        }

        .count {
            font-weight: bold;
        }

        .label {
            font-size: 14px;
        }
    </style>
    <style>
        #industryCounts {
            display: flex;
            justify-content: center;
            gap: 40px;
            margin-top: 10px;
        }

        .industryItem {
            text-align: center;
        }

        .dot {
            width: 10px;
            height: 10px;
            border-radius: 50%;
            display: inline-block;
            margin-right: 5px;
        }

        .count {
            font-weight: bold;
        }

        .label {
            font-size: 14px;
        }
    </style>
    <style>
        .hiddenCard {
            display: none;
        }

        .grid {
            border-collapse: collapse;
            width: 100%;
        }

            .grid th {
                background: #801628;
                color: white;
                padding: 8px;
            }

            .grid td {
                padding: 6px;
                border: 1px solid #ddd;
                text-align: center;
            }
    </style>
    <style>
        .vehicle-grid td {
            font-size: 10px;
            width: 60px;
            height: 45px;
            text-align: center;
            font-weight: bold;
            border: 1px solid #333;
            cursor: pointer;
        }

            .vehicle-grid td.booked {
                background: #2ecc71;
                color: #fff;
            }

            .vehicle-grid td.available {
                background: #eceff1;
                color: #fff;
            }
    </style>
    <!-- Chart.js -->
    <style>
        .chart-wrapper {
            height: 243px;
            padding: 15px;
        }

        .heatmap {
            border-collapse: separate;
            border-spacing: 10px;
        }

        .heat-legend {
            display: flex;
            gap: 12px;
            margin: 8px 0 12px 0;
            font-size: 12px;
            align-items: center;
        }

        .legend-item {
            display: flex;
            align-items: center;
            gap: 4px;
        }

        .legend-box {
            width: 14px;
            height: 14px;
            border-radius: 3px;
            border: 1px solid #ccc;
        }

        /* Same colors as heatmap */
        .heat-0 {
            background: #f5f5f5;
        }

        .heat-1 {
            background: #ffe082;
        }

        .heat-2 {
            background: #ffca28;
        }

        .heat-3 {
            background: #ff9800;
        }

        .heat-4 {
            background: #ef6c00;
        }

        .heat-5 {
            background: #d84315;
        }


        .month-nav {
            align-items: center;
            gap: 15px;
            margin-bottom: 10px;
        }

        .month-label {
            font-weight: 600;
            min-width: 120px;
            text-align: center;
        }

        .nav-btn {
            padding: 4px 10px;
        }
    </style>
    <style>
        .kpiGrid {
            display: grid;
            grid-template-columns: repeat(4,1fr);
            gap: 20px;
        }

        .kpiCard {
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
            text-align: center;
        }

            .kpiCard h2 {
                margin: 0;
                font-size: 30px;
            }

        .chartGrid {
            margin-top: 25px;
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
        }

        .chartCard {
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
        }

        .trendCard {
            margin-top: 20px;
            background: white;
            padding: 20px;
            border-radius: 8px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet" />
                <div class="header">

                    <table style="width: 100%">
                        <tr>
                            <td>
                                <h2>DASHBOARD</h2>
                            </td>
                            <td style="text-align: right"></td>
                        </tr>
                    </table>
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-md-12" style="text-align: center">
                            <div class="month-nav" style="text-align: center">
                                <asp:Button ID="btnPrev" runat="server" Text="◀"
                                    OnClick="btnPrev_Click" CssClass="nav-btn" />

                                <asp:Label ID="lblMonthYear" runat="server" CssClass="month-label" />

                                <asp:Button ID="btnNext" runat="server" Text="▶"
                                    OnClick="btnNext_Click" CssClass="nav-btn" />
                            </div>
                        </div>
                    </div>

                    <div id="divChart" runat="server" class="row card" visible="false">
                        <br />
                        <div class="col-md-6">
                            <div style="text-align: center">
                                <asp:Label ID="lblHeader" runat="server" Font-Size="16px" Font-Bold="true"></asp:Label>
                                <canvas id="targetChart"></canvas>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div style="text-align: center">
                                <asp:Label ID="lblHeader1" runat="server" Font-Size="16px" Font-Bold="true"></asp:Label>
                                <canvas id="targetChart1"></canvas>

                                <script>
                                    var revenueData = <%= chartJson %>;
                                    var bookingData = <%= chartJson1 %>;
                                    var trendData = <%= chartJson2 %>;

                                    function loadCharts() {

                                        if (!revenueData || !revenueData.labels) return;

                                        var ctx = document.getElementById('targetChart').getContext('2d');

                                        new Chart(ctx, {
                                            type: 'bar',

                                            data: {
                                                labels: revenueData.labels,

                                                datasets: [
                                                    {
                                                        label: 'Target',
                                                        data: revenueData.targets,
                                                        backgroundColor: '#BDE0FE'   // pastel blue
                                                    },
                                                    {
                                                        label: 'Achieved',
                                                        data: revenueData.achieved,
                                                        backgroundColor: '#CDEAC0'   // pastel green
                                                    }
                                                ]
                                            },

                                            options: {
                                                onClick: function () {
                                                    var SelectedMonth = <%= SelectedMonth %>;
                                                    var SelectedYear = <%= SelectedYear %>;
                                                    if (SelectedMonth && SelectedYear) {

                                                        var url = "RevenueReport.aspx?month="
                                                            + SelectedMonth + "&year=" + SelectedYear;
                                                        window.open(url, "_blank");   // opens in new tab

                                                    }
                                                },
                                                responsive: true,

                                                plugins: {
                                                    title: {
                                                        display: true,
                                                        text: 'Target vs Achievement'
                                                    }
                                                },

                                                scales: {
                                                    y: {
                                                        beginAtZero: true
                                                    }
                                                }
                                            }
                                        });

                                        if (!bookingData || !bookingData.labels) return;

                                        var ctx = document.getElementById('targetChart1').getContext('2d');

                                        new Chart(ctx, {
                                            type: 'bar',

                                            data: {
                                                labels: bookingData.labels,

                                                datasets: [
                                                    {
                                                        label: 'Target',
                                                        data: bookingData.targets,
                                                        backgroundColor: '#BDE0FE'   // pastel blue
                                                    },
                                                    {
                                                        label: 'Achieved',
                                                        data: bookingData.achieved,
                                                        backgroundColor: '#CDEAC0'   // pastel green
                                                    }
                                                ]
                                            },

                                            options: {
                                                responsive: true,

                                                plugins: {
                                                    title: {
                                                        display: true,
                                                        text: 'Target vs Achievement'
                                                    }
                                                },

                                                scales: {
                                                    y: {
                                                        beginAtZero: true
                                                    }
                                                }
                                            }
                                        });

                                        var ctx = document.getElementById('targetChart2').getContext('2d');

                                        new Chart(ctx, {
                                            type: 'line',
                                            data: {
                                                labels: trendData.labels,
                                                datasets: [
                                                    {
                                                        label: 'This Year',
                                                        data: trendData.thisYear,
                                                        borderColor: '#2C3E50',
                                                        backgroundColor: 'transparent',
                                                        tension: 0.4
                                                    },
                                                    {
                                                        label: 'Last Year',
                                                        data: trendData.lastYear,
                                                        borderColor: '#E74C3C',
                                                        backgroundColor: 'transparent',
                                                        tension: 0.4
                                                    }]
                                            },
                                            options: {
                                                responsive: true
                                            }
                                        });

                                    };
                                    document.addEventListener("DOMContentLoaded", function () {
                                        if (typeof loadCharts === "function") {
                                            loadCharts();
                                        }
                                    });
                                </script>


                            </div>
                        </div>
                    </div>
                    <div id="divTrend" runat="server" class="card" style="text-align: center !important" visible="false">
                        <div class="row" style="text-align: center">
                            <div class="col-md-2"></div>
                            <div class="col-md-8" style="text-align: center">
                                <asp:Label ID="lblHeader2" runat="server" Font-Size="16px" Font-Bold="true"></asp:Label>
                                <canvas id="targetChart2"></canvas>
                            </div>
                            <div class="col-md-2"></div>
                        </div>
                    </div>
                    <div id="divAllotment" class="row card" runat="server" visible="false">
                        <div class="col-md-12">
                            <asp:Label ID="lblHeader5" runat="server" Text="ALLOTMENT MAP" Font-Size="16px" Font-Bold="true"></asp:Label>

                            <div class="heat-legend">
                                <span class="legend-item">
                                    <span class="legend-box heat-0"></span>0
                                </span>
                                <span class="legend-item">
                                    <span class="legend-box heat-1"></span>1
                                </span>
                                <span class="legend-item">
                                    <span class="legend-box heat-2"></span>2
                                </span>
                                <span class="legend-item">
                                    <span class="legend-box heat-3"></span>3
                                </span>
                                <span class="legend-item">
                                    <span class="legend-box heat-4"></span>4
                                </span>
                                <span class="legend-item">
                                    <span class="legend-box heat-5"></span>5+
                                </span>
                            </div>

                            <% if (HasHeatMapData)
                                { %>
                            <table class="heatmap">
                                <thead>
                                    <tr>
                                        <th>Employee</th>
                                        <% for (int d = 1; d <= 31; d++)
                                            { %>
                                        <th><%= d %></th>
                                        <% } %>
                                    </tr>
                                </thead>
                                <tbody>
                                    <% foreach (var v in HeatMapData)
                                        { %>
                                    <tr>
                                        <td><%= v.EmployeeName %></td>
                                        <% for (int d = 1; d <= 31; d++)
                                            { %>
                                        <td class="heat <%= GetHeatClass(v.Days.ContainsKey(d) ? v.Days[d] : 0) %>"></td>
                                        <% } %>
                                    </tr>
                                    <% } %>
                                </tbody>
                            </table>

                            <% }
                                else
                                { %>

                            <div class="no-data">
                                No allocation data available for this month.
                            </div>

                            <% } %>
                        </div>
                    </div>
                    <div id="divAllotmentGantt" runat="server" class="row card" style="text-align: center; overflow-y: scroll">
                        <asp:Label ID="lblHeader9" runat="server"
                            Style="font-size: 18px; font-weight: bold;"></asp:Label>

                        <br />
                        <br />

                        <div id="timeline" style="height: 250px;"></div>
                        <script>
                            google.charts.load("current", { packages: ["timeline"] });
                            google.charts.setOnLoadCallback(loadEngineerChart);

                            function loadEngineerChart() {
                                var SelectedMonth = <%= SelectedMonth %>;
                                var SelectedYear = <%= SelectedYear %>;

                                window.fetch("GanttChartAllocation.aspx/GetEngineerAllocation", {
                                    method: "POST",
                                    headers: { "Content-Type": "application/json" },
                                    body: JSON.stringify({ month: SelectedMonth, year: SelectedYear })
                                })
                                    .then(res => res.json())
                                    .then(res => {

                                        let rows = res.d;

                                        drawTimeline(rows, SelectedMonth, SelectedYear);

                                    });
                            }


                            function drawTimeline(rows) {
                                var SelectedMonth = <%= SelectedMonth %>;
                                var SelectedYear = <%= SelectedYear %>;

                                var container = document.getElementById('timeline');
                                var chart = new google.visualization.Timeline(container);

                                var dataTable = new google.visualization.DataTable();

                                dataTable.addColumn({ type: 'string', id: 'Engineer' });
                                dataTable.addColumn({ type: 'string', id: 'Test' });
                                dataTable.addColumn({ type: 'date', id: 'Start' });
                                dataTable.addColumn({ type: 'date', id: 'End' });

                                let monthIndex = SelectedMonth - 1;

                                rows.forEach(x => {

                                    let startDate = new Date(SelectedYear, monthIndex, x.Start);

                                    let endMonth = monthIndex;

                                    // if task crosses month
                                    if (x.End < x.Start) {
                                        endMonth = monthIndex + 1;
                                    }

                                    let endDate = new Date(SelectedYear, endMonth, x.End);

                                    dataTable.addRow([
                                        x.Engineer,
                                        x.Test,
                                        startDate,
                                        endDate
                                    ]);

                                });

                                // ⭐ Pastel color palette
                                var options = {
                                    timeline: {
                                        colorByRowLabel: false,
                                        rowLabelStyle: { fontSize: 14 },
                                        barLabelStyle: { fontSize: 12 }
                                    },
                                    colors: [
                                        "#C6DEF1", // pastel blue
                                        "#F6C1C7", // pastel pink
                                        "#A8D5BA", // pastel green
                                        "#FFE0AC", // pastel orange
                                        "#E4C1F9", // pastel purple
                                        "#B5EAD7", // pastel mint
                                        "#FFDAC1", // pastel peach
                                        "#D4F1F4"  // pastel aqua
                                    ],
                                    height: rows.length * 45
                                };

                                chart.draw(dataTable, options);
                            }

                        </script>
                    </div>
                    <br />
                    <div id="divApproval" class="card" runat="server" visible="false" style="text-align: center">
                        <asp:Label ID="lblHeader6" runat="server" Text="WORK FLOW CHART" Font-Size="16px" Font-Bold="true"></asp:Label>

                        <div class="row text-center mt-3">

                            <div class="col-md-2">
                                <div class="kpi-card pastel-blue">
                                    <div class="kpi-title">Enquiries</div>
                                    <asp:Label ID="lblEnquiry" runat="server" CssClass="kpi-value" />
                                </div>
                            </div>

                            <div class="col-md-2">
                                <div class="kpi-card pastel-pink">
                                    <div class="kpi-title">Samples</div>
                                    <asp:Label ID="lblSample" runat="server" CssClass="kpi-value" />
                                </div>
                            </div>

                            <div class="col-md-2">
                                <div class="kpi-card pastel-yellow">
                                    <div class="kpi-title">Allocated</div>
                                    <asp:Label ID="lblAllocated" runat="server" CssClass="kpi-value" />
                                </div>
                            </div>

                            <div class="col-md-2">
                                <div class="kpi-card pastel-green">
                                    <div class="kpi-title">Tested</div>
                                    <asp:Label ID="lblTested" runat="server" CssClass="kpi-value" />
                                </div>
                            </div>

                            <div class="col-md-3">
                                <div class="kpi-card pastel-purple">
                                    <div class="kpi-title">Report Ready</div>
                                    <asp:Label ID="lblReportReady" runat="server" CssClass="kpi-value" />
                                </div>
                            </div>

                        </div>
                        <br />
                        <div class="row mt-12">

                            <div class="card col-md-6">
                                <canvas id="workflowChart" height="200" width="200"></canvas>
                                <script>
                                    function renderWorkflowChart(labels, samples, tested, reportReady) {

                                        const ctx = document.getElementById('workflowChart');

                                        new Chart(ctx, {
                                            type: 'line',
                                            data: {
                                                labels: labels,
                                                datasets: [
                                                    {
                                                        label: 'Samples',
                                                        data: samples,
                                                        borderColor: '#5DADE2',
                                                        backgroundColor: 'rgba(93,173,226,0.15)',
                                                        tension: 0.4,
                                                        borderWidth: 3,
                                                        fill: false
                                                    },
                                                    {
                                                        label: 'Tested',
                                                        data: tested,
                                                        borderColor: '#58D68D',
                                                        backgroundColor: 'rgba(88,214,141,0.15)',
                                                        tension: 0.4,
                                                        borderWidth: 3,
                                                        fill: false
                                                    },
                                                    {
                                                        label: 'Report Ready',
                                                        data: reportReady,
                                                        borderColor: '#AF7AC5',
                                                        backgroundColor: 'rgba(175,122,197,0.15)',
                                                        tension: 0.4,
                                                        borderWidth: 3,
                                                        fill: false
                                                    }
                                                ]
                                            },
                                            options: {
                                                responsive: true,
                                                plugins: {
                                                    legend: {
                                                        position: 'top'
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
                                    function loadChart(labels, enquiryData) {

                                        new Chart(document.getElementById("labChart"), {
                                            type: 'line',
                                            data: {
                                                labels: labels,
                                                datasets: [{
                                                    label: 'Enquiries',
                                                    data: enquiryData,
                                                    borderColor: 'rgba(54,162,235,1)',
                                                    backgroundColor: 'rgba(54,162,235,0.3)',
                                                    fill: true
                                                }]
                                            },
                                            options: {
                                                responsive: true,
                                                tension: 0.3
                                            }
                                        });
                                    }
                                    const ctx = document.getElementById('workflowChart');

                                    new Chart(ctx, {
                                        type: 'line',
                                        data: {
                                            labels: [
                                                '02 Feb - 08 Feb',
                                                '09 Feb - 15 Feb',
                                                '16 Feb - 22 Feb',
                                                '23 Feb - 01 Mar'
                                            ],
                                            datasets: [
                                                {
                                                    label: 'Samples',
                                                    data: [18, 12, 15, 10],
                                                    borderColor: '#5DADE2',
                                                    backgroundColor: 'rgba(93,173,226,0.15)',
                                                    fill: true,
                                                    tension: 0.4,
                                                    borderWidth: 3
                                                },
                                                {
                                                    label: 'Tested',
                                                    data: [10, 8, 9, 6],
                                                    borderColor: '#58D68D',
                                                    backgroundColor: 'rgba(88,214,141,0.15)',
                                                    fill: true,
                                                    tension: 0.4,
                                                    borderWidth: 3
                                                },
                                                {
                                                    label: 'Report Ready',
                                                    data: [5, 4, 6, 3],
                                                    borderColor: '#AF7AC5',
                                                    backgroundColor: 'rgba(175,122,197,0.15)',
                                                    fill: true,
                                                    tension: 0.4,
                                                    borderWidth: 3
                                                }
                                            ]
                                        },
                                        options: {
                                            responsive: true,
                                            plugins: {
                                                legend: {
                                                    position: 'top',
                                                    labels: {
                                                        font: { size: 13 }
                                                    }
                                                }
                                            },
                                            scales: {
                                                y: {
                                                    beginAtZero: true,
                                                    grid: {
                                                        color: 'rgba(0,0,0,0.05)'
                                                    }
                                                },
                                                x: {
                                                    grid: {
                                                        display: false
                                                    }
                                                }
                                            }
                                        }
                                    });
                                </script>
                            </div>
                            <div class="card col-md-6">
                                <canvas id="labChart" height="200" width="200"></canvas>
                            </div>
                            <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
                        </div>
                    </div>
                    <div id="divTestType" runat="server" class="row card" visible="false">
                        <div class="col-md-6">
                            <br />
                            <asp:Label ID="lblHeader4" runat="server" Text="TEST TYPE" Font-Size="16px" Font-Bold="true"></asp:Label>
                            <canvas id="testTypeChart" height="140"></canvas>
                            <div id="testTypeCounts" class="testType-counts"></div>

                        </div>
                        <script>
                            function loadTestTypeChart(month, year) {

                                $.ajax({
                                    type: "POST",
                                    url: "Default.aspx/GetTestTypeChart",
                                    data: JSON.stringify({ month: month, year: year }),
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",

                                    success: function (response) {

                                        var labels = [];
                                        var values = [];

                                        var countsHtml = "";

                                        var colors = [
                                            '#e89a9a',
                                            '#8eaadb',
                                            '#b7e4c7',
                                            '#D3D3D3',
                                            '#76A5AF'
                                        ];

                                        response.d.forEach(function (item, index) {
                                            labels.push(item.type);
                                            values.push(item.count);

                                            countsHtml +=
                                                '<div class="industryItem">' +
                                                '<div>' +
                                                '<span class="dot" style="background:' + colors[index % colors.length] + '"></span>' +
                                                '<span class="count">' + item.count + '</span>' +
                                                '</div>' +
                                                '<div class="label">' + item.type + '</div>' +
                                                '</div>';
                                        });
                                        $("#testTypeCounts").html(countsHtml);

                                        var total = values.reduce((a, b) => a + b, 0);

                                        new Chart(document.getElementById("testTypeChart"), {

                                            type: 'doughnut',

                                            data: {
                                                labels: labels,
                                                datasets: [{
                                                    data: values,
                                                    backgroundColor: [
                                                        '#e89a9a',
                                                        '#8eaadb',
                                                        '#b7e4c7',
                                                        '#D3D3D3'
                                                    ]
                                                }]
                                            },

                                            options: {

                                                cutout: '65%',

                                                plugins: {

                                                    legend: {
                                                        position: 'top'
                                                    },

                                                    tooltip: {
                                                        callbacks: {
                                                            label: function (context) {
                                                                return context.label + ": " + context.raw;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                });
                            }
                            $(document).ready(function () {
                                var SelectedMonth = <%= SelectedMonth %>;
                                var SelectedYear = <%= SelectedYear %>;
                                loadTestTypeChart(SelectedMonth, SelectedYear); // example month & year
                            });
                        </script>

                        <div class="col-md-6">
                            <br />
                            <asp:Label ID="lblHeader7" runat="server" Font-Size="16px" Font-Bold="true"></asp:Label>
                            <canvas id="industryChart" height="140"></canvas>
                            <div id="industryCounts" class="industry-counts"></div>
                            <script>

                                function loadIndustryChart(month, year) {
                                    $.ajax({
                                        type: "POST",
                                        url: "Default.aspx/GetIndustryChartData",
                                        data: JSON.stringify({ month: month, year: year }),
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: function (response) {

                                            var data = JSON.parse(response.d);
                                            var labels = [];
                                            var values = [];
                                            var countsHtml = "";

                                            var colors = [
                                                '#6C8EBF',
                                                '#E69138',
                                                '#93C47D',
                                                '#E06666',
                                                '#8E7CC3',
                                                '#76A5AF'
                                            ];


                                            data.forEach(function (item, index) {
                                                labels.push(item.Label);
                                                values.push(item.Value);
                                                countsHtml +=
                                                    '<div class="industryItem">' +
                                                    '<div>' +
                                                    '<span class="dot" style="background:' + colors[index] + '"></span>' +
                                                    '<span class="count">' + item.Value + '</span>' +
                                                    '</div>' +
                                                    '<div class="label">' + item.Label + '</div>' +
                                                    '</div>';
                                            });
                                            $("#industryCounts").html(countsHtml);

                                            var ctx = document.getElementById('industryChart').getContext('2d');

                                            new Chart(ctx, {
                                                type: 'doughnut',
                                                data: {
                                                    labels: labels,
                                                    datasets: [{
                                                        data: values,
                                                        backgroundColor: [
                                                            '#6C8EBF',
                                                            '#E69138',
                                                            '#93C47D',
                                                            '#E06666',
                                                            '#8E7CC3'
                                                        ]
                                                    }]
                                                },
                                                options: {
                                                    cutout: '65%',
                                                    plugins: {
                                                        legend: {
                                                            position: 'top'
                                                        }
                                                    }
                                                }
                                            });

                                        }
                                    });
                                }
                                $(document).ready(function () {
                                    var SelectedMonth = <%= SelectedMonth %>;
                                    var SelectedYear = <%= SelectedYear %>;
                                    loadIndustryChart(SelectedMonth, SelectedYear); // example month & year
                                });
                            </script>
                        </div>
                    </div>
                    <div id="divEquip" class="row card" runat="server" visible="false">
                        <div class="col-md-12">
                            <div style="width: 100%; height: auto; text-align: center">
                                <br />
                                <asp:Label ID="lblHeader3" runat="server" Font-Size="16px" Font-Bold="true"></asp:Label>
                                <br />
                                <br />
                                <div style="display: flex; align-items: center; gap: 40px; margin-bottom: 10px; font-size: 14px">

                                    <p>
                                        <b>Equipment Usage:</b>
                                        <span style="background: #ffffff; border: 1px solid #ccc; padding: 4px 12px; margin-left: 10px"></span>0
                                <span style="background: #b7e4c7; padding: 4px 12px; margin-left: 10px"></span>1-2
                                <span style="background: #52b788; padding: 4px 12px; margin-left: 10px"></span>3-4
                                <span style="background: #1b4332; padding: 4px 12px; margin-left: 10px"></span>5+
                                <asp:HyperLink ID="hypView" runat="server" Text="View All" NavigateUrl="~/EquipmentHeatMap.aspx" Target="_blank"></asp:HyperLink>
                                    </p>


                                </div>
                                <div id="heatmapContainer" style="width: 100%;">
                                    <canvas id="equipmentHeatChart"></canvas>
                                </div>
                                <script>
                                    var heatData = <%= chartJson3 %>;

                                    // Get unique equipment list
                                    const equipmentList = [...new Set(heatData.map(d => d.y))];
                                    var rows = equipmentList.length;
                                    var rowHeight = 24;

                                    document.getElementById("heatmapContainer").style.height =
                                        (rows * rowHeight + 60) + "px";
                                    // Get unique days
                                    const dayList = [...new Set(heatData.map(d => d.x))].sort((a, b) => a - b);

                                    const ctx = document.getElementById('equipmentHeatChart').getContext('2d');

                                    new Chart(ctx, {
                                        type: 'matrix',
                                        data: {
                                            datasets: [{
                                                label: 'Equipment Allocation',
                                                data: heatData,
                                                datalabels: {
                                                    display: true,
                                                    color: "#000"
                                                },
                                                backgroundColor(ctx) {

                                                    const value = ctx.dataset.data[ctx.dataIndex].v;

                                                    if (value == 0) return "#ffffff";
                                                    if (value <= 2) return "#b7e4c7";
                                                    if (value <= 4) return "#52b788";
                                                    return "#1b4332";
                                                },

                                                borderWidth: 1,
                                                width: () => 18,   // width of each day cell
                                                height: () => 18   // fixed cell height

                                                //width: ({ chart }) =>
                                                //    (chart.chartArea || {}).width / dayList.length - 2,

                                                //height: ({ chart }) =>
                                                //    (chart.chartArea || {}).height / equipmentList.length - 2
                                            }]
                                        },

                                        options: {
                                            responsive: true,
                                            maintainAspectRatio: false,
                                            layout: {
                                                padding: {
                                                    top: 0,
                                                    bottom: 0,
                                                    left: 0,
                                                    right: 0
                                                }
                                            },

                                            plugins: {
                                                legend: {
                                                    display: false
                                                },
                                                tooltip: {
                                                    callbacks: {
                                                        label(context) {
                                                            const d = context.raw;
                                                            return "Equipment: " + d.y +
                                                                " | Day: " + d.x +
                                                                " | Tests: " + d.v;
                                                        }
                                                    }
                                                }
                                            },

                                            scales: {
                                                x: {
                                                    offset: false,
                                                    grid: {
                                                        display: true
                                                    },
                                                    type: 'category',
                                                    position: 'top',
                                                    labels: dayList,

                                                    title: {
                                                        display: true,
                                                        text: 'Day of Month',
                                                        padding: {
                                                            bottom: 5
                                                        }
                                                    },

                                                    ticks: {
                                                        padding: 15
                                                    }
                                                },

                                                y: {
                                                    offset: false,
                                                    grid: {
                                                        display: false
                                                    },
                                                    type: 'category',
                                                    labels: equipmentList,

                                                    title: {
                                                        display: true,
                                                        text: 'Equipment'
                                                    },

                                                    ticks: {
                                                        autoSkip: false,
                                                        padding: 2
                                                    }
                                                }
                                            }
                                        }
                                    });

                                </script>
                            </div>
                            <br />
                        </div>
                    </div>
                    <div id="divFeedback" class="row card" runat="server" visible="false">
                        <div class="col-md-12" style="text-align: center">
                            <br />
                            <asp:Label ID="lblHeader8" runat="server" Font-Size="16px" Font-Bold="true"></asp:Label>

                            <div class="kpiGrid">

                                <div class="kpiCard">
                                    <h2 id="totalFeedback">0</h2>
                                    Total Feedback
                                </div>

                                <div class="kpiCard">
                                    <h2 id="satisfiedPercent">0%</h2>
                                    Satisfied
                                </div>

                                <div class="kpiCard">
                                    <h2 id="dissatisfiedPercent">0%</h2>
                                    Dissatisfied
                                </div>

                                <div class="kpiCard">
                                    <h2 id="avgScore">0</h2>
                                    Avg Score
                                </div>

                            </div>
                            <div class="chartGrid" style="text-align: center">
                                <div class="chartCard">
                                    <h4>Customer Satisfaction Distribution</h4>
                                    <canvas id="distributionChart"></canvas>
                                </div>

                                <div class="chartCard">
                                    <h4>Customer Satisfaction Index</h4>
                                    <div style="position: relative; height: 220px; width: 100%">
                                        <canvas id="gaugeChart"></canvas>

                                        <div id="gaugeValue"
                                            style="position: absolute; top: 60%; left: 50%; transform: translate(-50%,-50%); font-size: 26px; font-weight: bold;">
                                            0%
                                        </div>
                                    </div>
                                </div>
                                <script>    
                                    function loadDashboard(month, year) {
                                        fetch("Default.aspx/GetFeedbackData", {
                                            method: "POST",
                                            headers: { "Content-Type": "application/json" },
                                            body: JSON.stringify({ selectedMonth: month, selectedYear: year }),
                                        })
                                            .then(res => res.json())
                                            .then(res => {

                                                let data = res.d;
                                                console.log(data);

                                                document.getElementById("totalFeedback").innerText = data.Total;

                                                let sat = (data.Satisfied / data.Total * 100).toFixed(0);
                                                let dis = (data.Dissatisfied / data.Total * 100).toFixed(0);

                                                document.getElementById("satisfiedPercent").innerText = sat + "%";
                                                document.getElementById("dissatisfiedPercent").innerText = dis + "%";

                                                createDistributionChart(data.Satisfied, data.Dissatisfied);
                                                createTrendChart(data.Trend);
                                                createGaugeChart(data.GaugeScore);

                                            })

                                    }
                                    function createDistributionChart(satisfied, dissatisfied) {

                                        new Chart(document.getElementById("distributionChart"), {
                                            type: 'doughnut',
                                            data: {
                                                labels: ['Satisfied', 'Dissatisfied'],
                                                datasets: [{
                                                    data: [satisfied, dissatisfied],
                                                    backgroundColor: ['#28a745', '#dc3545']
                                                }]
                                            }
                                        })

                                    }
                                    var gaugeChart = null;
                                    function createGaugeChart(score) {

                                        if (score === undefined || score === null) score = 0;

                                        var canvas = document.getElementById("gaugeChart");

                                        if (!canvas) {
                                            console.log("Canvas not found");
                                            return;
                                        }

                                        var ctx = canvas.getContext("2d");

                                        if (gaugeChart) {
                                            gaugeChart.destroy();
                                        }

                                        gaugeChart = new Chart(ctx, {

                                            type: 'doughnut',

                                            data: {
                                                labels: ['Satisfied', 'Remaining'],   // REQUIRED in Chart.js v2
                                                datasets: [{
                                                    data: [score, 100 - score],
                                                    backgroundColor: ['#28a745', '#e9ecef'],
                                                    borderWidth: 0
                                                }]
                                            },

                                            options: {
                                                responsive: true,
                                                maintainAspectRatio: false,

                                                rotation: Math.PI,
                                                circumference: Math.PI,

                                                cutoutPercentage: 70,

                                                legend: { display: false },

                                                tooltips: { enabled: false }
                                            }

                                        });

                                        document.getElementById("gaugeValue").innerText = score + "%";
                                    }
                                    $(document).ready(function () {
                                        var SelectedMonth = <%= SelectedMonth %>;
                                        var SelectedYear = <%= SelectedYear %>;
                                        loadDashboard(SelectedMonth, SelectedYear); // example month & year                                        
                                    });
                                </script>
                            </div>
                            <div class="trendCard">
                                <center>
                                    <h4>Customer Satisfaction Trend</h4>
                                </center>
                                <canvas id="trendChart" height="100" width="auto"></canvas>
                                <script>
                                    function createTrendChart(trend) {

                                        let labels = [];
                                        let values = [];

                                        trend.forEach(x => {
                                            labels.push(x.MonthName);
                                            values.push(x.SatisfactionPercent);
                                        })

                                        new Chart(document.getElementById("trendChart"), {
                                            type: 'line',
                                            data: {
                                                labels: labels,
                                                datasets: [{
                                                    label: 'Customer Satisfaction %',
                                                    data: values,
                                                    borderColor: '#007bff',
                                                    fill: false,
                                                    tension: 0.4
                                                }]
                                            },
                                            options: {
                                                scales: {
                                                    y: { min: 0, max: 100 }
                                                }
                                            }
                                        })

                                    }
                                </script>
                            </div>

                        </div>
                    </div>
                    <div class="row">
                        <asp:GridView ID="gv" runat="server" Visible="false" CssClass="table" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="MonthNo" HeaderText="#" />
                                <asp:BoundField DataField="MonthName1" HeaderText="Month" />
                                <asp:BoundField DataField="TargetValue" HeaderText="Target" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
