<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="LIMS.Dashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Management Dashboard</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</head>

<body>
<form id="form1" runat="server">
<div class="container-fluid p-4">

    <!-- KPI CARDS -->
    <div class="row text-center">
        <asp:Repeater ID="rptCards" runat="server">
            <ItemTemplate>
                <div class="col-md-2 mb-3">
                    <div class="card shadow">
                        <div class="card-body">
                            <h6><%# Eval("Title") %></h6>
                            <h2><%# Eval("Count") %></h2>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <!-- CHARTS -->
    <div class="row mt-4">
        <div class="col-md-4">
            <canvas id="prChart"></canvas>
        </div>
        <div class="col-md-4">
            <canvas id="poChart"></canvas>
        </div>
        <div class="col-md-4">
            <canvas id="calibrationChart"></canvas>
        </div>
    </div>

</div>

<script>
    // PR Chart
    new Chart(document.getElementById('prChart'), {
        type: 'pie',
        data: {
            labels: ['Pending', 'Approved', 'Rejected'],
            datasets: [{
                data: [<%=PRPending%>, <%=PRApproved%>, <%=PRRejected%>]
            }]
        }
    });

    // PO Chart
    new Chart(document.getElementById('poChart'), {
        type: 'bar',
        data: {
            labels: ['Open', 'Closed', 'Cancelled'],
            datasets: [{
                data: [<%=POOpen%>, <%=POClosed%>, <%=POCancelled%>]
            }]
        }
    });

    // Calibration Process Chart
    new Chart(document.getElementById('calibrationChart'), {
        type: 'bar',
        data: {
            labels: ['Received', 'Assigned', 'Calibrated', 'Returned'],
            datasets: [{
                data: [
                    <%=ToolsReceived%>,
                    <%=ToolsAssigned%>,
                    <%=ToolsCalibrated%>,
                    <%=ToolsReturned%>
                ]
            }]
        }
    });
</script>

</form>
</body>
</html>

