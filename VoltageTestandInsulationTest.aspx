<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="VoltageTestandInsulationTest.aspx.cs" Inherits="LIMS.VoltageTestandInsulationTest" %>


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
                            <h2>VOLTAGE TEST & INSULATION TEST</h2>
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


                            <!--Voltage Test  on complete cable -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <h4>1.Voltage Test  on complete cable</h4>
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
                                                        <asp:TextBox ID="testdate1" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCore1" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtLength1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtImmerse1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTemp1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtVoltage1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTime1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtresults1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspecified1" runat="server" CssClass="form-control" Value="No Breakdown" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGrid1" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid1_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <!-- Voltage Test on complete cable GridView -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Voltage Test on complete cable Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv1" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gv1_RowDataBound" OnRowCommand="gv1_RowCommand">
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

                            <!-- Voltage Test on insulation -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <h4>2. Voltage Test on insulation</h4>
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
                                                        <asp:TextBox ID="testdate2" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCore2" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtLength2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtImmerse2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTemp2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtVoltage2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTime2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtresults2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspecified2" runat="server" CssClass="form-control" Value="No Breakdown" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGrid2" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid2_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>



                            <!-- Voltage Test on insulation GridView -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Voltage Test on insulation Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv2" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gv2_RowDataBound" OnRowCommand="gv2_RowCommand">
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

                            <!-- Insulation Resistance at ambient temperature -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <h4>3. Insulation Resistance at ambient temperature</h4>
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
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Measurement<br />
                                                        MΩ</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Insulation Resistance<br />
                                                        MΩ. km</th>
                                                    <th style="width: 11%; vertical-align: MIDDLE;" rowspan="2">Specified</th>
                                                    <th style="width: 7%; vertical-align: MIDDLE;" rowspan="2">Add Data</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="testdate3" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCore3" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtLength3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtImmerse3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTemp3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtMeasurement3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInsulationResistance3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspecified3" runat="server" CssClass="form-control" Value="Min.:-" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGrid3" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid3_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <!-- Insulation Resistance at ambient temperature GridView -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Insulation Resistance at ambient temperature Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv3" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gv3_RowDataBound" OnRowCommand="gv3_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" ItemStyle-Width="85px" />
                                                <asp:BoundField DataField="Core" HeaderText="Core" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Length" HeaderText="Length (m)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Immerse" HeaderText="Immerse (hour)" ItemStyle-Width="100px" />
                                                <asp:BoundField DataField="Temperature" HeaderText="Temp. (°C)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Measurement" HeaderText="Measurement (MΩ)" ItemStyle-Width="100px" />
                                                <asp:BoundField DataField="InsulationResistance" HeaderText="Insulation Resistance (MΩ. km)" ItemStyle-Width="150px" />
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

                            <!-- Insulation Resistance at max. conductor temperature -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <h4>4. Insulation Resistance at max. conductor temperature</h4>
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
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Factor correction k<sub>i</sub><br />
                                                        -</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Measurement<br />
                                                        MΩ</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Insulation Resistance<br />
                                                        MΩ. km</th>
                                                    <th style="width: 11%; vertical-align: MIDDLE;" rowspan="2">Specified</th>
                                                    <th style="width: 7%; vertical-align: MIDDLE;" rowspan="2">Add Data</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="testdate4" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCore4" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtLength4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtImmerse4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTemp4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtFactor4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtMeasurement4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInsulationResistance4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspecified4" runat="server" CssClass="form-control" Value="Min.:-" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGrid4" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid4_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <!-- Insulation Resistance at max. conductor temperature GridView -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Insulation Resistance at max. conductor temperature Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv4" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gv4_RowDataBound" OnRowCommand="gv4_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" ItemStyle-Width="85px" />
                                                <asp:BoundField DataField="Core" HeaderText="Core" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Length" HeaderText="Length (m)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Immerse" HeaderText="Immerse (hour)" ItemStyle-Width="100px" />
                                                <asp:BoundField DataField="Temperature" HeaderText="Temp. (°C)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Factor" HeaderText="Factor correction k<sub>i</sub>" HtmlEncode="false" ItemStyle-Width="120px" />
                                                <asp:BoundField DataField="Measurement" HeaderText="Measurement (MΩ)" ItemStyle-Width="100px" />
                                                <asp:BoundField DataField="InsulationResistance" HeaderText="Insulation Resistance (MΩ. km)" ItemStyle-Width="150px" />
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

                            <!-- Inner & Outer diameter of insulation -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <h4>5. Inner & Outer diameter of insulation</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Date</th>
                                                    <th style="width: 10%; vertical-align: MIDDLE;" rowspan="2">Core</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Length<br />
                                                        m</th>
                                                    <th style="vertical-align: MIDDLE;" colspan="3">Inner diameter of insulation</th>
                                                    <th style="vertical-align: MIDDLE;" colspan="3">Outer diameter of insulation</th>
                                                    <th style="width: 7%; vertical-align: MIDDLE;" rowspan="2">Add Data</th>
                                                </tr>
                                                <tr>
                                                    <th style="vertical-align: MIDDLE;">1<br />
                                                        mm</th>
                                                    <th style="vertical-align: MIDDLE;">2<br />
                                                        mm</th>
                                                    <th style="vertical-align: MIDDLE;">Avg<br />
                                                        mm</th>
                                                    <th style="vertical-align: MIDDLE;">1<br />
                                                        mm</th>
                                                    <th style="vertical-align: MIDDLE;">2<br />
                                                        mm</th>
                                                    <th style="vertical-align: MIDDLE;">Avg<br />
                                                        mm</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="testdate5" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCore5" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtLength5" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInner1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInner2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInnerAvg" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtOuter1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtOuter2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtOuterAvg" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGrid5" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid5_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <!-- Inner & Outer diameter of insulation Specified -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="vertical-align: MIDDLE; width: 141px" colspan="2">Specified</th>
                                                    <td style="vertical-align: MIDDLE; width: 146px;">
                                                        <asp:TextBox ID="txtspecifiedlength5" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspecifiedInnerdiameter5" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspecifiedOuterdiameter5" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center; width: 95px;"></td>
                                            </thead>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <!-- Inner & Outer diameter of insulation GridView -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Inner & Outer diameter of insulation Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv5" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gv5_RowDataBound" OnRowCommand="gv5_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" ItemStyle-Width="85px" />
                                                <asp:BoundField DataField="Core" HeaderText="Core" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Length" HeaderText="Length (m)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Inner1" HeaderText="Inner 1 (mm)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Inner2" HeaderText="Inner 2 (mm)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="InnerAvg" HeaderText="Inner Avg (mm)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Outer1" HeaderText="Outer 1 (mm)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Outer2" HeaderText="Outer 2 (mm)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="OuterAvg" HeaderText="Outer Avg (mm)" ItemStyle-Width="80px" />
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

                            <!--Volume Resistivity, KI at ambient temperature-->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <h4>6. Volume Resistivity, KI at ambient temperature</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Date</th>
                                                    <th style="width: 10%; vertical-align: MIDDLE;" rowspan="2">Core</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Temp.<br />
                                                        °C</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Insulation resistance<br />
                                                        MΩ.L</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Volume resistivity at 20°C<br />
                                                        Ω.cm</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Insulation resistant constant KI at 20°C<br />
                                                        M.O.Km</th>
                                                    <th style="width: 7%; vertical-align: MIDDLE;" rowspan="2">Add Data</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="testdate6" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCore6" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTemp6" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInsulationResistance" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtVolumeResistivity" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInsulationConstant" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGrid6" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid6_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <!-- Volume Resistivity, KI at ambient temperature Specified -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="vertical-align: MIDDLE; width: 141px" colspan="2">Specified</th>
                                                    <td style="vertical-align: MIDDLE; width: 232px;">
                                                        <asp:TextBox ID="txtspecifiedtemp6" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="vertical-align: MIDDLE; width: 230px;">
                                                        <asp:TextBox ID="txtspecifiedInsulationresistance6" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="vertical-align: MIDDLE; width: 232px;">
                                                        <asp:TextBox ID="txtspecifiedVolumeresistivity6" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="vertical-align: MIDDLE; width: 273px;">
                                                        <asp:TextBox ID="txtspecifiedInsulationresistantconstantKIat20C6" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center; width: 95px;"></td>
                                            </thead>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <!-- Volume Resistivity, KI at ambient temperature GridView -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Volume Resistivity, KI at ambient temperature Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv6" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gv6_RowDataBound" OnRowCommand="gv6_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" ItemStyle-Width="85px" />
                                                <asp:BoundField DataField="Core" HeaderText="Core" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Temperature" HeaderText="Temp. (°C)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="InsulationResistance" HeaderText="Insulation resistance (MΩ.L)" ItemStyle-Width="120px" />
                                                <asp:BoundField DataField="VolumeResistivity" HeaderText="Volume resistivity at 20°C (Ω.cm)" ItemStyle-Width="150px" />
                                                <asp:BoundField DataField="InsulationConstant" HeaderText="Insulation resistant constant KI at 20°C (M.O.Km)" ItemStyle-Width="200px" />         
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

                            <!--Volume Resistivity, KI at max. conductor temperature -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <h4>7. Volume Resistivity, KI at max. conductor temperature</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Date</th>
                                                    <th style="width: 10%; vertical-align: MIDDLE;" rowspan="2">Core</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Temp.<br />
                                                        °C</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Insulation resistance<br />
                                                        MΩ.L</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Volume resistivity at °C<br />
                                                        Ω.cm</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Insulation resistant constant KI at °C<br />
                                                        M.O.km</th>
                                                  
                                                    <th style="width: 7%; vertical-align: MIDDLE;" rowspan="2">Add Data</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="testdate7" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCore7" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTemp7" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInsulationResistance7" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtVolumeResistivity7" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInsulationConstant7" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGrid7" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid7_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <!-- Volume Resistivity, KI at max. conductor temperature Specified -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 0px!important;">
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="vertical-align: MIDDLE; width: 141px" colspan="2">Specified</th>
                                                    <td style="vertical-align: MIDDLE; width: 232px;">
                                                        <asp:TextBox ID="txtspecifiedtemp7" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="vertical-align: MIDDLE; width: 235px;">
                                                        <asp:TextBox ID="txtspecifiedInsulationresistance7" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="vertical-align: MIDDLE; width: 235px;">
                                                        <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="vertical-align: MIDDLE; width: 262px;">
                                                        <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center; width: 95px;"></td>
                                            </thead>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <!-- Volume Resistivity, KI at max. conductor temperature GridView -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Volume Resistivity, KI at max. conductor temperature Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv7" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gv7_RowDataBound" OnRowCommand="gv7_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" ItemStyle-Width="85px" />
                                                <asp:BoundField DataField="Core" HeaderText="Core" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="Temperature" HeaderText="Temp. (°C)" ItemStyle-Width="80px" />
                                                <asp:BoundField DataField="InsulationResistance" HeaderText="Insulation resistance (MΩ.L)" ItemStyle-Width="120px" />
                                                <asp:BoundField DataField="VolumeResistivity" HeaderText="Volume resistivity at °C (Ω.cm)" ItemStyle-Width="150px" />
                                                <asp:BoundField DataField="InsulationConstant" HeaderText="Insulation resistant constant KI at °C (M.O.km)" ItemStyle-Width="200px" />
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
