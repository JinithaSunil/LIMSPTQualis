<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DashboardIncomeExpense.aspx.cs" Inherits="LIMS.DashboardIncomeExpense" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Invoice & Payment Dashboard</title>

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

<style>
    .kpi-card {
        color: #fff;
        padding: 20px;
        border-radius: 8px;
        text-align: center;
        font-weight: bold;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ERP.Dashboard" %>

<div class="container-fluid p-4">

    <!-- KPI CARDS -->
    <div class="row g-3">
        <div class="col-md-2">
            <div class="kpi-card bg-primary">
                Invoices Pending<br />
                <asp:Label ID="lblInvPending" runat="server" Font-Size="XX-Large" />
            </div>
        </div>
        <div class="col-md-2">
            <div class="kpi-card bg-success">
                Invoices Paid<br />
                <asp:Label ID="lblInvPaid" runat="server" Font-Size="XX-Large" />
            </div>
        </div>
        <div class="col-md-2">
            <div class="kpi-card bg-danger">
                Invoices Overdue<br />
                <asp:Label ID="lblInvOverdue" runat="server" Font-Size="XX-Large" />
            </div>
        </div>
        <div class="col-md-2">
            <div class="kpi-card bg-warning text-dark">
                Payments Due<br />
                <asp:Label ID="lblPayDue" runat="server" Font-Size="XX-Large" />
            </div>
        </div>
        <div class="col-md-2">
            <div class="kpi-card bg-success">
                Payments Paid<br />
                <asp:Label ID="lblPayPaid" runat="server" Font-Size="XX-Large" />
            </div>
        </div>
        <div class="col-md-2">
            <div class="kpi-card bg-danger">
                Payments Overdue<br />
                <asp:Label ID="lblPayOverdue" runat="server" Font-Size="XX-Large" />
            </div>
        </div>
    </div>

    <!-- CHARTS -->
    <div class="row mt-4">
        <div class="col-md-6">
            <h5>Invoice Status</h5>
            <canvas id="invoiceChart"></canvas>
        </div>
        <div class="col-md-6">
            <h5>Payment Status</h5>
            <canvas id="paymentChart"></canvas>
        </div>
    </div>
    <asp:HiddenField ID="hfInvPending" runat="server" />
<asp:HiddenField ID="hfInvPaid" runat="server" />
<asp:HiddenField ID="hfInvOverdue" runat="server" />

<asp:HiddenField ID="hfPayDue" runat="server" />
<asp:HiddenField ID="hfPayPaid" runat="server" />
<asp:HiddenField ID="hfPayOverdue" runat="server" />

</div>

<script>
    new Chart(document.getElementById('invoiceChart'), {
        type: 'pie',
        data: {
            labels: ['Pending', 'Paid', 'Overdue'],
            datasets: [{
                data: [
                    <%=InvPending%>,
                    <%=InvPaid%>,
                    <%=InvOverdue%>
                ]
            }]
        }
    });

    new Chart(document.getElementById('paymentChart'), {
        type: 'pie',
        data: {
            labels: ['Due', 'Paid', 'Overdue'],
            datasets: [{
                data: [
                    <%=PayDue%>,
                    <%=PayPaid%>,
                    <%=PayOverdue%>
                ]
            }]
        }
    });
</script>


</asp:Content>
