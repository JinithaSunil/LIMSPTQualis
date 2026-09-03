<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MorrisDoughnutChart.aspx.cs" Inherits="LIMS.MorrisDoughnutChart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
<div class="box box-danger">
    <div class="box-header with-border">
        <h3 class="box-title">
            Donut Chart</h3>      
    </div>
    <div class="box-body chart-responsive">
        <div class="chart" id="sales-chart" style="height: 300px; position: relative;">
        </div>
    </div>
    <!-- /.box-body -->
</div>
<link type="text/css" rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/morris.js/0.5.1/morris.css" />
<script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js"></script>
<script type="text/javascript" src="//cdnjs.cloudflare.com/ajax/libs/raphael/2.1.0/raphael-min.js"></script>
<script type="text/javascript" src="//cdnjs.cloudflare.com/ajax/libs/morris.js/0.5.1/morris.min.js"></script>
<script type="text/javascript">
    $(function () {
        "use strict";
        //DONUT CHART
        var donut = new Morris.Donut({
            element: 'sales-chart',
            resize: true,
            colors: ["#3c8dbc", "#f56954", "#00a65a"],
            data: [
                { label: "Download Sales", value: 12 },
                { label: "In-Store Sales", value: 30 },
                { label: "Mail-Order Sales", value: 20 }
            ],
            hideHover: 'auto'
        });
    })
</script>
</asp:Content>
