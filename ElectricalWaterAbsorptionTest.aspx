<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="ElectricalWaterAbsorptionTest.aspx.cs" Inherits="LIMS.ElectricalWaterAbsorptionTest" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }

        .card {
            background: white;
            border-radius: 3px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            margin-bottom: 30px;
        }

        .violet {
            border: 0;
            height: 4px;
            background-color: #b34788;
            margin: 0;
        }

        .header {
            padding: 15px 20px;
            border-bottom: 1px solid #e0e0e0;
        }

            .header h2 {
                margin: 0;
                color: #333;
                font-size: 18px;
                font-weight: bold;
            }

        .body {
            padding: 20px;
        }

        .table-responsive {
            overflow-x: auto;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            background-color: white;
        }

        table, th, td {
            border: 1px solid #ddd;
        }

        th {
            background-color: #f2f2f2;
            color: #333;
            padding: 10px;
            text-align: center;
            font-weight: bold;
        }

        td {
            padding: 8px;
            text-align: center;
        }

        .info-table td:first-child {
            background-color: #f2f2f2;
            width: 25%;
            font-weight: bold;
        }

        .test-table th {
            background-color: #e0e0e0;
            vertical-align: middle;
        }

        input[type="text"], input[type="number"], input[type="date"] {
            width: 100%;
            padding: 6px;
            border: 1px solid #ddd;
            border-radius: 3px;
        }

        .btn-primary {
            background-color: #b34788;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 3px;
            cursor: pointer;
            font-weight: bold;
        }

            .btn-primary:hover {
                background-color: #933375;
            }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }

        /* Make header cells and input aligned */
        .tests-section th, .test-row td {
            vertical-align: middle;
        }
    </style>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>ELECTRICAL WATER ABSORPTION TEST</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:Label ID="cableType" runat="server" CssClass="form-control" /></td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:Label ID="refSpec" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:Label ID="cableSize" runat="server" CssClass="form-control" /></td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:Label ID="testMethodNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:Label ID="lblvoltage" runat="server" CssClass="form-control" /></td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:Label ID="jobOrderNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" type="date" Style="text-align: center;" /></td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:Label ID="worksheetno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" /></td>
                                        <td><b>Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                        </td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:Label ID="equipmentno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                            </div>


                            <!--Voltage Test (AC) on insulation -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <h4>1. Voltage Test (AC) on insulation</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Date</th>
                                                    <th style="width: 10%; vertical-align: MIDDLE;" rowspan="2">Core</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Length<br />
                                                        m</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Immerse<br />
                                                        hour</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Temp.<br />
                                                        °C</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Voltage<br />
                                                        kV</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Time<br />
                                                        minute</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Result</th>
                                                    <th style="width: 11%; vertical-align: MIDDLE;" rowspan="2">Specified</th>
                                                    <th style="width: 7%; vertical-align: MIDDLE;" rowspan="2">Add Data</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="testdateAC" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCoreAC" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtLengthAC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtImmerseAC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTempAC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtVoltageAC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTimeAC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtresultsAC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspecifiedAC" runat="server" CssClass="form-control" Value="No Breakdown" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGridAC" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGridAC_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <!-- GridView for AC Test Results -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>AC Voltage Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvVoltageTestAC" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gvVoltageTestAC_RowDataBound" OnRowCommand="gvVoltageTestAC_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" ItemStyle-Width="85px" />
                                                <asp:BoundField DataField="Core" HeaderText="Core" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Length" HeaderText="Length (m)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Immerse" HeaderText="Immerse (hour)" ItemStyle-Width="100px" />
                                                <asp:BoundField DataField="Temperature" HeaderText="Temp. (°C)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Voltage" HeaderText="Voltage (kV)" ItemStyle-Width="90px" />
                                                <asp:BoundField DataField="Time" HeaderText="Time (minute)" ItemStyle-Width="100px" />
                                                <asp:BoundField DataField="Result" HeaderText="Result" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Specified" HeaderText="Specified" ItemStyle-Width="100px" />
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="70px">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete"
                                                            CommandName="DeleteRow" CommandArgument='<%# Container.DataItemIndex %>'
                                                            OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                            CssClass="btn btn-danger btn-sm" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                            <!-- Voltage Test (DC) on insulation -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <h4>2. Voltage Test (DC) on insulation</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Date</th>
                                                    <th style="width: 10%; vertical-align: MIDDLE;" rowspan="2">Core</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Length<br />
                                                        m</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Immerse<br />
                                                        hour</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Temp.<br />
                                                        °C</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Voltage<br />
                                                        kV</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Time<br />
                                                        hour</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Result</th>
                                                    <th style="width: 11%; vertical-align: MIDDLE;" rowspan="2">Specified</th>
                                                    <th style="width: 7%; vertical-align: MIDDLE;" rowspan="2">Add Data</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="testdateDC" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCoreDC" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtLengthDC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtImmerseDC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTempDC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtVoltageDC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTimeDC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtresultsDC" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspecifiedDC" runat="server" CssClass="form-control" Value="No Breakdown" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGridDC" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGridDC_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>



                            <!-- GridView for DC Test Results -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>DC Voltage Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvVoltageTestDC" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gvVoltageTestDC_RowDataBound" OnRowCommand="gvVoltageTestDC_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" ItemStyle-Width="85px" />
                                                <asp:BoundField DataField="Core" HeaderText="Core" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Length" HeaderText="Length (m)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Immerse" HeaderText="Immerse (hour)" ItemStyle-Width="100px" />
                                                <asp:BoundField DataField="Temperature" HeaderText="Temp. (°C)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Voltage" HeaderText="Voltage (kV)" ItemStyle-Width="90px" />
                                                <asp:BoundField DataField="Time" HeaderText="Time (hour)" ItemStyle-Width="100px" />
                                                <asp:BoundField DataField="Result" HeaderText="Result" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Specified" HeaderText="Specified" ItemStyle-Width="100px" />
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="70px">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete"
                                                            CommandName="DeleteRow" CommandArgument='<%# Container.DataItemIndex %>'
                                                            OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                            CssClass="btn btn-danger btn-sm" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                             <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnUpdate_Click" Visible="false" />

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>

