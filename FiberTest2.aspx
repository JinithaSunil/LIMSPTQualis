<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="FiberTest2.aspx.cs" Inherits="LIMS.FiberTest2" %>

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
            background-color: #068001;
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

        .invalid-input {
            border: 2px solid #ff9999 !important;
            background-color: #fff0f0 !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updt" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>STRIPPABILITY TEST FOR INSULATION SCREEN</h2>
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
                                            <asp:Label ID="voltage" runat="server" CssClass="form-control" /></td>
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

                            <!-- Input Form for Test Data - Horizontal Layout -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Test Details</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <!-- Header Row -->
                                                <tr>
                                                    <th>Date</th>
                                                    <th>Test Type</th>
                                                    <th>Core Phase</th>
                                                    <th>Specimen No.</th>
                                                    <th>Width Strip (mm)</th>
                                                    <th>Pulling Speed (mm/min)</th>
                                                    <th>Measured (N)</th>
                                                    <th style="width: 7%;">Add Data</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <!-- Input Fields Row -->
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="testdate" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlTestType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTestType_SelectedIndexChanged" Style="text-align: center;">
                                                            <asp:ListItem Text="Before ageing" Value="Before ageing" />
                                                            <asp:ListItem Text="After ageing complete cable 100°C/168 h" Value="After ageing complete cable 100°C/168 h" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCorePhase_SelectedIndexChanged" Style="text-align: center;">
                                                            <asp:ListItem Text="Red" Value="Red" />
                                                            <asp:ListItem Text="Blue" Value="Blue" />
                                                            <asp:ListItem Text="Yellow" Value="Yellow" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecimenNo" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtWidthStrip" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPullingSpeed" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtMeasured" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>

                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <!-- Strippability Test Results Grid -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Strippability Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvStrippabilityTest" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gvStrippabilityTest_RowDataBound" OnRowCommand="gvStrippabilityTest_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" />
                                                <asp:BoundField DataField="ItemTest" HeaderText="Item Test" />
                                                <asp:BoundField DataField="CorePhase" HeaderText="Core Phase" />
                                                <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No." />
                                                <asp:BoundField DataField="WidthStrip" HeaderText="Width strip insulation screen (mm)" />
                                                <asp:BoundField DataField="PullingSpeed" HeaderText="Pulling Speed (mm/min)" />
                                                <asp:BoundField DataField="Measured" HeaderText="Measured (N)" />
                                                <asp:TemplateField HeaderText="Strippability Min.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMinStrippability" runat="server" Text='<%# Eval("MinStrippability") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Strippability Max.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMaxStrippability" runat="server" Text='<%# Eval("MaxStrippability") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Actions">
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

                            <!-- Standard Row - Horizontal Layout -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Standard Values</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <!-- Header Row -->
                                                <tr>
                                                    <th>Width (mm)</th>
                                                    <th>Pulling Speed (mm/min)</th>
                                                    <th>Measured(N)</th>
                                                    <th>Min. Value(N)</th>
                                                    <th>Max. Value(N)</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <!-- Input Fields Row -->
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtStandardWidth" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtStandardSpeed" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtStandardmeasured" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtStandardMin" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtStandardMax" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <div>
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th colspan="4" class="text-center">Ageing specimen</th>
                                            <th colspan="4" class="text-center">Ageing complete cable</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td width="12.5%">Airflow:</td>
                                            <td width="12.5%">
                                                <asp:TextBox ID="txtSpecimenAirflow" runat="server" CssClass="form-control" />
                                            </td>
                                            <td colspan="2" width="25%">Standard: 8-20 air change per hour at temperature</td>
                                            <td width="12.5%">Airflow:</td>
                                            <td width="12.5%">
                                                <asp:TextBox ID="txtCableAirflow" runat="server" CssClass="form-control" />
                                            </td>
                                            <td colspan="2" width="25%">Standard: 8-20 air change per hour at temperature</td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="text-center">Date</td>
                                            <td colspan="2" class="text-center">Time</td>
                                            <td colspan="2" class="text-center">Date</td>
                                            <td colspan="2" class="text-center">Time</td>
                                        </tr>
                                        <tr>
                                            <td class="text-center">Start</td>
                                            <td class="text-center">Finish</td>
                                            <td class="text-center">Start</td>
                                            <td class="text-center">Finish</td>
                                            <td class="text-center">Start</td>
                                            <td class="text-center">Finish</td>
                                            <td class="text-center">Start</td>
                                            <td class="text-center">Finish</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenDateStart" runat="server" CssClass="form-control" TextMode="Date" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenDateFinish" runat="server" CssClass="form-control" TextMode="Date" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenTimeStart" runat="server" CssClass="form-control" TextMode="Time" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenTimeFinish" runat="server" CssClass="form-control" TextMode="Time" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCableDateStart" runat="server" CssClass="form-control" TextMode="Date" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCableDateFinish" runat="server" CssClass="form-control" TextMode="Date" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCableTimeStart" runat="server" CssClass="form-control" TextMode="Time" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCableTimeFinish" runat="server" CssClass="form-control" TextMode="Time" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>

                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th colspan="4" class="text-center">Conditioning in desiccators:</th>
                                            <th colspan="4" class="text-center">Conditioning</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td colspan="2" class="text-center">Date</td>
                                            <td colspan="2" class="text-center">Time</td>
                                            <td colspan="2" class="text-center">Date</td>
                                            <td colspan="2" class="text-center">Time</td>
                                        </tr>
                                        <tr>
                                            <td class="text-center">Start</td>
                                            <td class="text-center">Finish</td>
                                            <td class="text-center">Start</td>
                                            <td class="text-center">Finish</td>
                                            <td class="text-center">Start</td>
                                            <td class="text-center">Finish</td>
                                            <td class="text-center">Start</td>
                                            <td class="text-center">Finish</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDesiccatorsDateStart" runat="server" CssClass="form-control" TextMode="Date" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDesiccatorsDateFinish" runat="server" CssClass="form-control" TextMode="Date" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDesiccatorsTimeStart" runat="server" CssClass="form-control" TextMode="Time" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDesiccatorsTimeFinish" runat="server" CssClass="form-control" TextMode="Time" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtConditioningDateStart" runat="server" CssClass="form-control" TextMode="Date" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtConditioningDateFinish" runat="server" CssClass="form-control" TextMode="Date" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtConditioningTimeStart" runat="server" CssClass="form-control" TextMode="Time" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtConditioningTimeFinish" runat="server" CssClass="form-control" TextMode="Time" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%--<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updt" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>STRIPPABILITY TEST FOR INSULATION SCREEN</h2>
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
                                            <asp:Label ID="voltage" runat="server" CssClass="form-control" /></td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:Label ID="jobOrderNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:Label ID="Date" runat="server" CssClass="form-control" /></td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:Label ID="worksheetno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:Label ID="roomno" runat="server" CssClass="form-control" /></td>
                                        <td><b>Room Humidity</b></td>
                                        <td>
                                            <asp:Label ID="roomhumidity" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp.</b></td>
                                        <td>
                                            <asp:Label ID="roomtemp" runat="server" CssClass="form-control" /></td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:Label ID="equipmentno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                            </div>

                            <!-- Input Form for Test Data -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Test Data Input</h4>
                                    <div class="form-group">
                                        <label>Test Type</label>
                                        <asp:DropDownList ID="ddlTestType" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="Before ageing" Value="Before ageing" />
                                            <asp:ListItem Text="After ageing complete cable 100°C/168 h" Value="After ageing complete cable 100°C/168 h" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label>Core Phase</label>
                                        <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="Red" Value="Red" />
                                            <asp:ListItem Text="Blue" Value="Blue" />
                                            <asp:ListItem Text="Yellow" Value="Yellow" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label>Specimen No.</label>
                                        <asp:TextBox ID="txtSpecimenNo" runat="server" CssClass="form-control" placeholder="Enter specimen number" />
                                    </div>
                                    <div class="form-group">
                                        <label>Width strip insulation screen (mm)</label>
                                        <asp:TextBox ID="txtWidthStrip" runat="server" CssClass="form-control" placeholder="Enter width strip" />
                                    </div>
                                    <div class="form-group">
                                        <label>Pulling Speed (mm/min)</label>
                                        <asp:TextBox ID="txtPullingSpeed" runat="server" CssClass="form-control" placeholder="Enter pulling speed" />
                                    </div>
                                    <div class="form-group">
                                        <label>Measured (N)</label>
                                        <asp:TextBox ID="txtMeasured" runat="server" CssClass="form-control" placeholder="Enter measured value" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Button ID="btnAddToGrid" runat="server" Text="Add to Grid" CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                    </div>
                                </div>
                            </div>

                            <!-- Strippability Test Results Grid -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Strippability Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvStrippabilityTest" runat="server" AutoGenerateColumns="false" 
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gvStrippabilityTest_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" />
                                                <asp:BoundField DataField="ItemTest" HeaderText="Item Test" />
                                                <asp:BoundField DataField="CorePhase" HeaderText="Core Phase" />
                                                <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No." />
                                                <asp:BoundField DataField="WidthStrip" HeaderText="Width strip insulation screen (mm)" />
                                                <asp:BoundField DataField="PullingSpeed" HeaderText="Pulling Speed (mm/min)" />
                                                <asp:BoundField DataField="Measured" HeaderText="Measured (N)" />
                                                <asp:TemplateField HeaderText="Strippability Min.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMinStrippability" runat="server" Text='<%# Eval("MinStrippability") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Strippability Max.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMaxStrippability" runat="server" Text='<%# Eval("MaxStrippability") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                            <!-- Standard Row -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <div class="form-group">
                                        <label>Standard Values</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">Width: </span>
                                            <asp:TextBox ID="txtStandardWidth" runat="server" CssClass="form-control" placeholder="e.g. 10±1" />
                                            <span class="input-group-addon">Pulling Speed: </span>
                                            <asp:TextBox ID="txtStandardSpeed" runat="server" CssClass="form-control" placeholder="e.g. 250±50" />
                                            <span class="input-group-addon">Min: </span>
                                            <asp:TextBox ID="txtStandardMin" runat="server" CssClass="form-control" placeholder="e.g. 4" />
                                            <span class="input-group-addon">Max: </span>
                                            <asp:TextBox ID="txtStandardMax" runat="server" CssClass="form-control" placeholder="e.g. 45" />
                                        </div>
                                    </div>
                                    <asp:Button ID="btnSaveStandard" runat="server" Text="Save Standard Values" CssClass="btn btn-info" OnClick="btnSaveStandard_Click" />
                                </div>
                            </div>

                            <!-- Final Submission -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Test Results" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>--%>
