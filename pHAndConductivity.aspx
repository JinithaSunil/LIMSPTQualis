<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="pHAndConductivity.aspx.cs" Inherits="LIMS.pHAndConductivity" %>



<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
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
                            <h2>pH & CONDUCTIVITY</h2>
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

                                            <th rowspan="2">Item Test</th>
                                            <th rowspan="2">Unit</th>
                                            <th rowspan="2">Specified</th>
                                            <th colspan="2">Measured</th>
                                        </tr>
                                        <tr>

                                            <th>Sample 1</th>
                                            <th>Sample 2</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>

                                            <td>Sample Weight</td>
                                            <td>
                                                <asp:DropDownList ID="ddlweight_sample1" runat="server" CssClass="form-control">

                                                    <asp:ListItem Value="gr" Selected="True">gr</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                    <asp:ListItem Value="ml">ml</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="min">min</asp:ListItem>
                                                    <asp:ListItem Value="μS/mm">μS/mm</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleUnit1" ControlToValidate="ddlweight_sample1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtweight_sampleSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleSpecified" ControlToValidate="txtweight_sampleSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtweight_sampleinput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleInput1" ControlToValidate="txtweight_sampleinput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtweight_sampleinput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleInput2" ControlToValidate="txtweight_sampleinput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>

                                            <td>Air destilasi</td>
                                            <td>
                                                <asp:DropDownList ID="ddlair2" runat="server" CssClass="form-control">

                                                    <asp:ListItem Value="ml" Selected="True">ml</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="min">min</asp:ListItem>
                                                    <asp:ListItem Value="μS/mm">μS/mm</asp:ListItem>

                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvair2" ControlToValidate="ddlair2" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAirSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvAirSpecified" ControlToValidate="txtAirSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAirInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvAirInput1" ControlToValidate="txtAirInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAirInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvAirInput2" ControlToValidate="txtAirInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>

                                        </tr>

                                        <tr>
                                            <td>Burn Temperature</td>
                                            <td>
                                                <asp:DropDownList ID="ddlBurnTemperatureUnit" runat="server" CssClass="form-control">

                                                    <asp:ListItem Value="°C" Selected="True">°C</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                    <asp:ListItem Value="ml">ml</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="min">min</asp:ListItem>
                                                    <asp:ListItem Value="μS/mm">μS/mm</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvBurnTemperatureUnit" ControlToValidate="ddlBurnTemperatureUnit" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBurnTemperatureSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBurnTemperatureSpecified" ControlToValidate="txtBurnTemperatureSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBurnTemperatureInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBurnTemperatureInput1" ControlToValidate="txtBurnTemperatureInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBurnTemperatureInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBurnTemperatureInput2" ControlToValidate="txtBurnTemperatureInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>


                                        <tr>
                                            <td>Burn Time</td>
                                            <td>
                                                <asp:DropDownList ID="ddlBurnTimeUnit" runat="server" CssClass="form-control">

                                                    <asp:ListItem Value="min" Selected="True">min</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                    <asp:ListItem Value="ml">ml</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="μS/mm">μS/mm</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvBurnTimeUnit" ControlToValidate="ddlBurnTimeUnit" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBurnTimeSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBurnTimeSpecified" ControlToValidate="txtBurnTimeSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBurnTimeInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBurnTimeInput1" ControlToValidate="txtBurnTimeInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBurnTimeInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBurnTimeInput2" ControlToValidate="txtBurnTimeInput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>

                                            <td>pH</td>
                                            <td>
                                                <asp:DropDownList ID="ddlpH1" runat="server" CssClass="form-control">

                                                    <asp:ListItem Value="-" Selected="True">-</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                    <asp:ListItem Value="ml">ml</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="min">min</asp:ListItem>
                                                    <asp:ListItem Value="μS/mm">μS/mm</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="ddlpH1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="pHSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="pHSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="pHinput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="pHinput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="pHinput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="pHinput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>

                                        </tr>
                                        <tr>
                                            <td>Conductivity</td>
                                            <td>
                                                <asp:DropDownList ID="ddlConductivityUnit" runat="server" CssClass="form-control">

                                                    <asp:ListItem Value="μS/mm" Selected="True">μS/mm</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                    <asp:ListItem Value="ml">ml</asp:ListItem>
                                                    <asp:ListItem Value="°C">°C</asp:ListItem>
                                                    <asp:ListItem Value="min">min</asp:ListItem>

                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvConductivityUnit" ControlToValidate="ddlConductivityUnit" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtConductivitySpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvConductivitySpecified" ControlToValidate="txtConductivitySpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtConductivityInput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvConductivityInput1" ControlToValidate="txtConductivityInput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtConductivityInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvConductivityInput2" ControlToValidate="txtConductivityInput2" runat="server"
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
