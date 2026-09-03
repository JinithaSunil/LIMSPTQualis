<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="TensileStrengthandElongation.aspx.cs" Inherits="LIMS.TensileStrengthandElongation" %>

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
            overflow-x: auto !important;
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
            vertical-align: middle;
        }

        td {
            padding: 5px;
            text-align: center;
            vertical-align: middle;
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

        input[type="text"], input[type="number"], input[type="date"], select {
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

        .btn-add {
            background-color: #b34788;
            color: white;
            border: none;
            padding: 8px 12px;
            border-radius: 3px;
            cursor: pointer;
            font-size: 12px;
        }

            .btn-add:hover {
                background-color: #b34788;
            }

        .btn-danger {
            background-color: #dc3545;
            color: white;
            border: none;
            padding: 5px 10px;
            border-radius: 3px;
            cursor: pointer;
            font-size: 12px;
        }

            .btn-danger:hover {
                background-color: #c82333;
            }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }

        .tests-section th, .test-row td {
            vertical-align: middle;
        }

        .invalid-input {
            border: 2px solid #ff9999 !important;
            background-color: #fff0f0 !important;
        }

        .table.table-bordered {
            table-layout: fixed;
            width: 200%;
        }

            .table.table-bordered th,
            .table.table-bordered td {
                overflow: hidden;
                text-overflow: ellipsis;
                white-space: nowrap;
            }

        #gvTestResults {
            width: 200% !important;
            table-layout: auto !important;
            font-size: 12px;
        }

            #gvTestResults th,
            #gvTestResults td {
                white-space: nowrap;
                padding: 8px 6px;
                text-align: center;
                vertical-align: middle;
                min-width: 80px;
                border: 1px solid #ddd;
            }

        .table-bordered th,
        .table-bordered td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: center;
            vertical-align: middle;
            overflow: auto;
        }

        .table-bordered th {
            background-color: #f2f2f2;
        }

        .w-100 {
            width: 100%;
        }

        .fas {
            font-family: "Font Awesome 5 Free";
        }

        .me-1 {
            margin-right: 4px;
        }

        .mt-4 {
            margin-top: 20px;
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
                            <h2>TENSILE STRENGTH & ELONGATION</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>The Test</b></td>
                                        <td>
                                            <asp:TextBox ID="test" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rvftest" runat="server"
                                                ControlToValidate="test"
                                                ErrorMessage="The Test is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Drum No.</b></td>
                                        <td>
                                            <asp:TextBox ID="drumno" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rvfdrumno" runat="server"
                                                ControlToValidate="drumno"
                                                ErrorMessage="Drum No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" />
                                            <%--   <asp:RequiredFieldValidator ID="rfvCableType" runat="server"
                                                ControlToValidate="cableType"
                                                ErrorMessage="Type is required"
                                                Display="Dynamic" ForeColor="Red" />--%>
                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" />
                                            <%--  <asp:RequiredFieldValidator ID="rfvRefSpec" runat="server"
                                                ControlToValidate="refSpec"
                                                ErrorMessage="Ref. Spec. is required"
                                                Display="Dynamic" ForeColor="Red" />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" />
                                            <%--      <asp:RequiredFieldValidator ID="rfvCableSize" runat="server"
                                                ControlToValidate="cableSize"
                                                ErrorMessage="Size is required"
                                                Display="Dynamic" ForeColor="Red" />--%>
                                        </td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" />
                                            <%--<asp:RequiredFieldValidator ID="rfvTestMethodNo" runat="server"
                                                ControlToValidate="testMethodNo"
                                                ErrorMessage="Test Method No. is required"
                                                Display="Dynamic" ForeColor="Red" />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" />
                                            <%--   <asp:RequiredFieldValidator ID="rfvVoltage" runat="server"
                                                ControlToValidate="voltage"
                                                ErrorMessage="Voltage is required"
                                                Display="Dynamic" ForeColor="Red" />--%>
                                        </td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" />
                                            <%-- <asp:RequiredFieldValidator ID="rfvJobOrderNo" runat="server"
                                                ControlToValidate="jobOrderNo"
                                                ErrorMessage="Job Order No. is required"
                                                Display="Dynamic" ForeColor="Red" />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" TextMode="Date" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvDate" runat="server"
                                                ControlToValidate="Date"
                                                ErrorMessage="Date is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" />
                                            <%--  <asp:RequiredFieldValidator ID="rfvWorksheetNo" runat="server"
                                                ControlToValidate="worksheetno"
                                                ErrorMessage="Worksheet No. is required"
                                                Display="Dynamic" ForeColor="Red" />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <%--      <asp:RequiredFieldValidator ID="rfvRoomNo" runat="server"
                                                ControlToValidate="roomno"
                                                ErrorMessage="Room No. is required"
                                                Display="Dynamic" ForeColor="Red" />--%>
                                        </td>
                                        <td><b>Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomHumidity" runat="server"
                                                ControlToValidate="roomhumidity"
                                                ErrorMessage="Room Humidity is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomTemp" runat="server"
                                                ControlToValidate="roomtemp"
                                                ErrorMessage="Room Temperature is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvEquipmentNo" runat="server"
                                                ControlToValidate="equipmentno"
                                                ErrorMessage="Equipment No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Length</b></td>
                                        <td>
                                            <asp:TextBox ID="length" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rvflength" runat="server"
                                                ControlToValidate="length"
                                                ErrorMessage="Length is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <br />
                            <div class="table-responsive">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th rowspan="3" style="vertical-align: middle; width: 135px!important">Date</th>
                                            <th rowspan="3" style="vertical-align: middle; width: 180px!important">Item Test</th>
                                            <th rowspan="3" style="vertical-align: middle; width: 150px">Core Phase</th>
                                            <th rowspan="3" style="vertical-align: middle; width: 150px">Type of sample Test</th>
                                            <th rowspan="3" style="vertical-align: middle; width: 80px">Specimen<br />
                                                number</th>
                                            <th rowspan="2" style="vertical-align: middle; width: 200px">Thickness/Weight<br />
                                                (comma separated inputs)</th>
                                            <th rowspan="2" style="vertical-align: middle; width: 100px">(Avg.,Min.)</th>
                                            <th rowspan="2" style="vertical-align: middle; width: 200px">Width/Diameter/Length/Density<br />
                                                (comma separated inputs)</th>
                                            <th rowspan="2" style="vertical-align: middle; width: 100px">(Avg.,Min.)</th>
                                            <th rowspan="2" style="vertical-align: middle; width: 80px">Area</th>
                                            <th rowspan="2" style="vertical-align: middle; width: 80px">BF</th>
                                            <th rowspan="2" style="vertical-align: middle; width: 80px">TS</th>
                                            <th colspan="3" style="vertical-align: middle; width: 240px">Elongation</th>
                                            <th colspan="2" style="vertical-align: middle; width: 160px">Median</th>
                                            <th colspan="2" style="vertical-align: middle; width: 160px">Variation</th>
                                            <th rowspan="3" style="vertical-align: middle; width: 80px">Add Data</th>
                                        </tr>
                                        <tr>
                                            <th style="vertical-align: middle;">LO</th>
                                            <th style="vertical-align: middle;">L1</th>
                                            <th style="vertical-align: middle;">EL.</th>
                                            <th style="vertical-align: middle;">TS</th>
                                            <th style="vertical-align: middle;">EL.</th>
                                            <th style="vertical-align: middle;">TS</th>
                                            <th style="vertical-align: middle;">EL.</th>
                                        </tr>
                                        <tr>
                                            <th colspan="2" style="vertical-align: middle;">mm/gr</th>
                                            <th colspan="2" style="vertical-align: middle;">mm/(gr/cm<sup>3</sup>)</th>
                                            <th style="vertical-align: middle;">mm<sup>2</sup></th>
                                            <th style="vertical-align: middle;">N</th>
                                            <th style="vertical-align: middle;">N/mm<sup>2</sup></th>
                                            <th style="vertical-align: middle;">mm</th>
                                            <th style="vertical-align: middle;">mm</th>
                                            <th style="vertical-align: middle;">%</th>
                                            <th style="vertical-align: middle;">N/mm<sup>2</sup></th>
                                            <th style="vertical-align: middle;">%</th>
                                            <th style="vertical-align: middle;">%</th>
                                            <th style="vertical-align: middle;">%</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtItemTest" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="0" Text="Select Phase" />
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtsampletest" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenNumber" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtThicknessWeight" runat="server" CssClass="form-control" placeholder="e.g., 1.2, 1.3, 1.4" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtThicknessAvgMin" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtWidthDiameter" runat="server" CssClass="form-control" placeholder="e.g., 10, 10.1, 10.2" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtWidthAvgMin" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtArea" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBF" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTS" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtLO" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtL1" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtEL" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMedianTS" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMedianEL" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtVariationTS" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtVariationEL" runat="server" CssClass="form-control" />
                                            </td>
                                            <td rowspan="2" style="vertical-align:middle">
                                                <asp:Button ID="btnAdd" runat="server" CssClass="btn-add w-100" Text=" + " OnClick="btnAdd_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5" class="text-center fw-bold" style="background-color: #f2f2f2;">Specified</td>
                                            <td colspan="2">
                                                <asp:TextBox ID="txtSpecThickness" runat="server" CssClass="form-control" ReadOnly="true" Text="-" /></td>
                                            <td colspan="2">
                                                <asp:TextBox ID="txtSpecWidth" runat="server" CssClass="form-control" ReadOnly="true" Text="-" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecArea" runat="server" CssClass="form-control" ReadOnly="true" Text="-" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecBF" runat="server" CssClass="form-control" Text="Min:" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecTS" runat="server" CssClass="form-control" ReadOnly="true" Text="-" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecLO" runat="server" CssClass="form-control" ReadOnly="true" Text="-" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecL1" runat="server" CssClass="form-control" ReadOnly="true" Text="-" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecEL" runat="server" CssClass="form-control" Text="Min:" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecMedianTS" runat="server" CssClass="form-control" Text="Min:" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecMedianEL" runat="server" CssClass="form-control" Text="Min:" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecVarTS" runat="server" CssClass="form-control" Text="Max:" /></td>
                                            <td>
                                                <asp:TextBox ID="txtSpecVarEL" runat="server" CssClass="form-control" Text="Max:" /></td>
                                            <td></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>

                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Test Results</h4>
                                    <div class="table-responsive" style="max-width: 100%; overflow-x: auto;">
                                        <asp:GridView ID="gvTestResults" runat="server" AutoGenerateColumns="false"
                                            OnRowCommand="gvTestResults_RowCommand" OnRowDataBound="gvTestResults_RowDataBound"
                                            CssClass=" #gvTestResults">
                                            <Columns>
                                                <asp:BoundField DataField="TestDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                                                <asp:BoundField DataField="ItemTest" HeaderText="Item Test" />
                                                <asp:BoundField DataField="CorePhase" HeaderText="Core Phase" />
                                                <asp:BoundField DataField="SampleType" HeaderText="Type of Sample Test" />
                                                <asp:BoundField DataField="SpecimenNumber" HeaderText="Specimen Number" />
                                                <asp:BoundField DataField="ThicknessWeight" HeaderText="Thickness/Weight (mm/gr)" />
                                                <asp:BoundField DataField="ThicknessAvgMin" HeaderText="(Avg.,Min.) (mm/gr)" />
                                                <asp:BoundField DataField="WidthDiameter" HeaderText="Width/Diameter/Length/Density (mm/(gr/cm²))" />
                                                <asp:BoundField DataField="WidthAvgMin" HeaderText="(Avg.,Min.) (mm/(gr/cm²))" />
                                                <asp:BoundField DataField="Area" HeaderText="Area (mm²)" />
                                                <asp:BoundField DataField="BF" HeaderText="BF (N)" />
                                                <asp:BoundField DataField="TS" HeaderText="TS (N/mm²)" />
                                                <asp:BoundField DataField="LO" HeaderText="Elongation LO (mm)" />
                                                <asp:BoundField DataField="L1" HeaderText="Elongation L1 (mm)" />
                                                <asp:BoundField DataField="EL" HeaderText="Elongation EL. (%)" />
                                                <asp:BoundField DataField="MedianTS" HeaderText="Median TS (N/mm²)" />
                                                <asp:BoundField DataField="MedianEL" HeaderText="Median EL. (%)" />
                                                <asp:BoundField DataField="VariationTS" HeaderText="Variation TS (%)" />
                                                <asp:BoundField DataField="VariationEL" HeaderText="Variation EL. (%)" />
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
                                            <th colspan="4" class="text-center">Conditioning in desiccators</th>
                                            <th colspan="4" class="text-center">Conditioning in desiccators</th>
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
                                <asp:Button ID="btnSaveData" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveData_Click" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnUpdate_Click" Visible="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function calculateAvgMin(inputId, outputId) {
            var input = document.getElementById(inputId);
            var output = document.getElementById(outputId);

            if (input && output) {
                var values = input.value.split(',');
                var numbers = [];

                for (var i = 0; i < values.length; i++) {
                    var trimmed = values[i].trim();
                    if (trimmed !== '' && !isNaN(trimmed)) {
                        numbers.push(parseFloat(trimmed));
                    }
                }

                if (numbers.length > 0) {
                    var sum = numbers.reduce(function (a, b) { return a + b; }, 0);
                    var avg = sum / numbers.length;
                    var min = Math.min.apply(Math, numbers);

                    output.value = 'Avg: ' + avg.toFixed(2) + ', Min: ' + min.toFixed(2);
                } else {
                    output.value = '';
                }
            }
        }

        function setupAutoCalculation() {
            // Get the TextBox client IDs (these will be generated by ASP.NET)
            var thicknessInput = '<%= txtThicknessWeight.ClientID %>';
            var thicknessOutput = '<%= txtThicknessAvgMin.ClientID %>';
            var widthInput = '<%= txtWidthDiameter.ClientID %>';
            var widthOutput = '<%= txtWidthAvgMin.ClientID %>';

            // Add event listeners
            var thicknessElement = document.getElementById(thicknessInput);
            if (thicknessElement) {
                thicknessElement.addEventListener('input', function () {
                    calculateAvgMin(thicknessInput, thicknessOutput);
                });
                thicknessElement.addEventListener('blur', function () {
                    calculateAvgMin(thicknessInput, thicknessOutput);
                });
            }

            var widthElement = document.getElementById(widthInput);
            if (widthElement) {
                widthElement.addEventListener('input', function () {
                    calculateAvgMin(widthInput, widthOutput);
                });
                widthElement.addEventListener('blur', function () {
                    calculateAvgMin(widthInput, widthOutput);
                });
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            setupAutoCalculation();
        });

        // Reinitialize after UpdatePanel postback
        function pageLoad() {
            setupAutoCalculation();
        }

        // Optional: Validate form before submission
        function validateForm() {
            var itemTest = document.getElementById('<%= txtItemTest.ClientID %>');
            var corePhase = document.getElementById('<%= ddlCorePhase.ClientID %>');
            var sampleType = document.getElementById('<%= txtsampletest.ClientID %>');

            if (!itemTest || itemTest.value.trim() === '') {
                alert('Please enter Item Test');
                if (itemTest) itemTest.focus();
                return false;
            }

            if (!corePhase || corePhase.selectedIndex === 0) {
                alert('Please select Core Phase');
                if (corePhase) corePhase.focus();
                return false;
            }

            if (!sampleType || sampleType.value.trim() === '') {
                alert('Please enter Sample Type');
                if (sampleType) sampleType.focus();
                return false;
            }

            return true;
        }

        function confirmDelete() {
            return confirm('Are you sure you want to delete this record?');
        }
    </script>
</asp:Content>

