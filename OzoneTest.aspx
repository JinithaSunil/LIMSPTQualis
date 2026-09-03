<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="OzoneTest.aspx.cs" Inherits="LIMS.OzoneTest" %>



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
            padding: 5px;
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

        .table-bordered th,
        .table-bordered td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: center;
        }

        .table-bordered th {
            background-color: #f2f2f2;
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
                            <h2>OZONE TEST</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Drum No.</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvVoltage" runat="server"
                                                ControlToValidate="voltage"
                                                ErrorMessage="drum number is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" ReadOnly="true" />

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
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true" />

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
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                    </tr>
                                </table>

                            </div>
                            <br />
                            <div class="table-responsive">
                                <table class="table table-bordered text-center">
                                    <thead>
                                        <tr>
                                            <th colspan="7" style="text-align: left">
                                                <strong>Material:</strong>
                                                <asp:TextBox runat="server" ID="txtmaterial" CssClass="form-control"></asp:TextBox>
                                            </th>
                                        </tr>
                                        <tr>
                                            <th rowspan="2">Item Test</th>
                                            <th rowspan="2">Unit</th>
                                            <th rowspan="2">Specified</th>
                                            <th colspan="3">Measured</th>
                                        </tr>
                                        <tr>
                                            <th>Sample 1</th>
                                            <th>Sample 2</th>
                                            <th>Sample 3</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>Thickness</td>
                                            <td>
                                                <asp:DropDownList ID="ddlthickness1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm" Selected="True">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlthickness1" ControlToValidate="ddlthickness1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvThicknessSpecified" ControlToValidate="txtthicknessSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvThicknessInput1" ControlToValidate="txtthicknessInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvThicknessInput2" ControlToValidate="txtthicknessInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvThicknessInput3" ControlToValidate="txtthicknessInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>

                                        </tr>

                                        <tr>
                                            <td>Width</td>
                                            <td>
                                                <asp:DropDownList ID="ddlwidth1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm" Selected="True">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlwidth1" ControlToValidate="ddlwidth1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtwidthSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWidthSpecified" ControlToValidate="txtwidthSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtwidthInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWidthInput1" ControlToValidate="txtwidthInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtwidthInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWidthInput2" ControlToValidate="txtwidthInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtwidthInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWidthInput3" ControlToValidate="txtwidthInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>


                                        <tr>
                                            <td>Diameter</td>
                                            <td>
                                                <asp:DropDownList ID="ddldiameter1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm" Selected="True">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddldiameter1" ControlToValidate="ddldiameter1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDiameterSpecified" ControlToValidate="txtdiameterSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDiameterInput1" ControlToValidate="txtdiameterInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDiameterInput2" ControlToValidate="txtdiameterInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDiameterInput3" ControlToValidate="txtdiameterInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>Length of Sample</td>
                                            <td>
                                                <asp:DropDownList ID="ddllengthsample1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm" Selected="True">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddllengthsample1" ControlToValidate="ddllengthsample1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtlengthsampleSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLengthSampleSpecified" ControlToValidate="txtlengthsampleSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtlengthsampleInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLengthSampleInput1" ControlToValidate="txtlengthsampleInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtlengthsampleInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLengthSampleInput2" ControlToValidate="txtlengthsampleInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtlengthsampleInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLengthSampleInput3" ControlToValidate="txtlengthsampleInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>Gauge Length / Diameter Mandrell</td>
                                            <td>
                                                <asp:DropDownList ID="ddlgaugelength1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm" Selected="True">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlgaugelength1" ControlToValidate="ddlgaugelength1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtgaugelengthSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvGaugeLengthSpecified" ControlToValidate="txtgaugelengthSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtgaugelengthInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvGaugeLengthInput1" ControlToValidate="txtgaugelengthInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtgaugelengthInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvGaugeLengthInput2" ControlToValidate="txtgaugelengthInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtgaugelengthInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvGaugeLengthInput3" ControlToValidate="txtgaugelengthInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>Temperature</td>
                                            <td>
                                                <asp:DropDownList ID="ddltemperature1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C" Selected="true">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddltemperature1" ControlToValidate="ddltemperature1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txttemperatureSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatureSpecified" ControlToValidate="txttemperatureSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txttemperatureInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatureInput1" ControlToValidate="txttemperatureInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txttemperatureInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatureInput2" ControlToValidate="txttemperatureInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txttemperatureInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatureInput3" ControlToValidate="txttemperatureInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>Volume Oven</td>
                                            <td>
                                                <asp:DropDownList ID="ddlvolumeoven1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C" Selected="true">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlvolumeoven1" ControlToValidate="ddlvolumeoven1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtvolumeovenSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvVolumeOvenSpecified" ControlToValidate="txtvolumeovenSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtvolumeovenInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvVolumeOvenInput1" ControlToValidate="txtvolumeovenInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtvolumeovenInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvVolumeOvenInput2" ControlToValidate="txtvolumeovenInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtvolumeovenInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvVolumeOvenInput3" ControlToValidate="txtvolumeovenInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td rowspan="2">Ozone Concentration</td>
                                            <td>
                                                <asp:DropDownList ID="ddlozoneconcentration1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%" Selected="true">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlozoneconcentration1" ControlToValidate="ddlozoneconcentration1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtozoneconcentrationSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvOzoneConcentrationSpecified" ControlToValidate="txtozoneconcentrationSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtozoneconcentrationInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvOzoneConcentrationInput1" ControlToValidate="txtozoneconcentrationInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtozoneconcentrationInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvOzoneConcentrationInput2" ControlToValidate="txtozoneconcentrationInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtozoneconcentrationInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvOzoneConcentrationInput3" ControlToValidate="txtozoneconcentrationInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>

                                        </tr>

                                        <tr>

                                            <td>
                                                <asp:DropDownList ID="ddlozoneconcentration2" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm" Selected="true">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="ddlozoneconcentration1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtozoneconcentrationSpecified2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtozoneconcentrationSpecified2" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtozoneconcentrationInput2_1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtozoneconcentrationInput2_1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtozoneconcentrationInput2_2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtozoneconcentrationInput2_2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtozoneconcentrationInput2_3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtozoneconcentrationInput2_3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>

                                        </tr>
                                        <tr>
                                            <td>Duration</td>
                                            <td>
                                                <asp:DropDownList ID="ddlduration1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C" Selected="true">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlduration1" ControlToValidate="ddlduration1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdurationSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDurationSpecified" ControlToValidate="txtdurationSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdurationInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDurationInput1" ControlToValidate="txtdurationInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdurationInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDurationInput2" ControlToValidate="txtdurationInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdurationInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDurationInput3" ControlToValidate="txtdurationInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>Elongate</td>
                                            <td>
                                                <asp:DropDownList ID="ddlelongate1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour" Selected="true">hour</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlelongate1" ControlToValidate="ddlelongate1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtelongateSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvElongateSpecified" ControlToValidate="txtelongateSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtelongateInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvElongateInput1" ControlToValidate="txtelongateInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtelongateInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvElongateInput2" ControlToValidate="txtelongateInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtelongateInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvElongateInput3" ControlToValidate="txtelongateInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>Result</td>
                                            <td>
                                                <asp:DropDownList ID="ddlresult1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="cm³">cm³</asp:ListItem>
                                                    <asp:ListItem Value="%">%</asp:ListItem>
                                                    <asp:ListItem Value="pphm">pphm</asp:ListItem>
                                                    <asp:ListItem Value="hour">hour</asp:ListItem>
                                                    <asp:ListItem Value="-" Selected="true">-</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlresult1" ControlToValidate="ddlresult1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtresultSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvResultSpecified" ControlToValidate="txtresultSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtresultInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvResultInput1" ControlToValidate="txtresultInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtresultInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvResultInput2" ControlToValidate="txtresultInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtresultInput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvResultInput3" ControlToValidate="txtresultInput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                    </tbody>


                                </table>
                            </div>

                            <div>
                            </div>
                        </div>
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>
