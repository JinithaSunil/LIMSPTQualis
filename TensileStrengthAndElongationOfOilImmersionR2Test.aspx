<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="TensileStrengthAndElongationOfOilImmersionR2Test.aspx.cs" Inherits="LIMS.TensileStrengthAndElongationOfOilImmersionR2Test" %>

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
            width: 100%;
            overflow-x: auto;
            margin-bottom: 20px;
        }

        table {
            min-width: 600px;
            width: 100%;
            border-collapse: collapse;
            background-color: white;
        }

        table, th, td {
            border: 1px solid #000;
        }

        th {
            background-color: #D3D3D3;
            color: #000;
            padding: 10px;
            text-align: center;
            font-weight: bold;
        }

        td {
            padding: 8px;
            text-align: center;
            color: black;
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

        input[type="text"], input[type="number"], input[type="date"],
        .form-control {
            width: 100%;
            max-width: 280px;
            padding: 6px;
            border: 1px solid #ddd;
            border-radius: 3px;
            margin: 0 auto;
            display: block;
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

        /* Validation styles */
        .validator-error {
            color: red;
            font-size: smaller;
            display: block;
            margin-top: 5px;
        }

        /* Responsive adjustments */
        @media screen and (max-width: 768px) {
            .body {
                padding: 10px;
            }

            th, td {
                padding: 6px;
                font-size: 14px;
            }

            input[type="text"], input[type="number"], input[type="date"],
            .form-control {
                max-width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>TENSILE STRENGTH & ELONGATION OF OIL IMMERSION</h2>
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
                                        <td><b class="required-field">Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="Date"
                                                ErrorMessage="Date is required" CssClass="validator-error" Display="Dynamic"></asp:RequiredFieldValidator>
                                        </td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:Label ID="worksheetno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b class="required-field">Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomNo" runat="server" ControlToValidate="roomno"
                                                ErrorMessage="Room No. is required" CssClass="validator-error" Display="Dynamic"></asp:RequiredFieldValidator>
                                        </td>
                                        <td><b class="required-field">Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomHumidity" runat="server" ControlToValidate="roomhumidity"
                                                ErrorMessage="Room Humidity is required" CssClass="validator-error" Display="Dynamic"></asp:RequiredFieldValidator>
                                            <asp:RangeValidator ID="rvRoomHumidity" runat="server" ControlToValidate="roomhumidity"
                                                Type="Double" MinimumValue="0" MaximumValue="100" ErrorMessage="Must be between 0-100%"
                                                CssClass="validator-error" Display="Dynamic"></asp:RangeValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b class="required-field">Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomTemp" runat="server" ControlToValidate="roomtemp"
                                                ErrorMessage="Room Temperature is required" CssClass="validator-error" Display="Dynamic"></asp:RequiredFieldValidator>
                                            <asp:RangeValidator ID="rvRoomTemp" runat="server" ControlToValidate="roomtemp"
                                                Type="Double" MinimumValue="-50" MaximumValue="100" ErrorMessage="Must be between -50°C to 100°C"
                                                CssClass="validator-error" Display="Dynamic"></asp:RangeValidator>
                                        </td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:Label ID="equipmentno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                            </div>

                            <div class="table-responsive">
                                <table class="test-table">
                                    <thead>
                                        <tr>
                                            <th rowspan="2" colspan="2">Item</th>
                                            <th rowspan="2">Unit</th>
                                            <th rowspan="2">Equipment No.</th>
                                            <th colspan="6">Measured value</th>
                                            <th rowspan="2">Specified</th>
                                        </tr>
                                        <tr>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>Median</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <!-- Core Specimen Section -->
                                        <tr>
                                            <td><b>Core</b></td>
                                            <td colspan="11">
                                                <asp:TextBox ID="txtCore" runat="server" CssClass="form-control" Style="margin-left: 20px;" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                    ControlToValidate="txtCore"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td><b>Specimen type </b></td>
                                            <td colspan="11">
                                                <asp:TextBox ID="txtSpecimenType" runat="server" CssClass="form-control" Style="margin-left: 20px;" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                    ControlToValidate="txtSpecimenType"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <%--TS & EL Before Immersed In Oil--%>

                                        <tr>
                                            <td rowspan="9" class="section-header">Core<br />
                                                Specimen type<br />
                                                TS & EL<br />
                                                Before<br />
                                                Immersed In<br />
                                                Oil<br />
                                                Date:
                                                <asp:TextBox ID="txtBDate" runat="server" CssClass="form-control" Style="margin-left: 20px; width: 120px;" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                                    ControlToValidate="txtBDate"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td class="text-left">Room temp.</td>
                                            <td>°C</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo1" runat="server" CssClass="input-medium" />
                                            </td>
                                            <td colspan="6">
                                                <asp:TextBox ID="txtRoomTemp1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvRoomTemp1" runat="server"
                                                    ControlToValidate="txtRoomTemp1" ErrorMessage="*" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified1" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">Thickness</td>
                                            <td>mm</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo2" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue1_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue1_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue1_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue1_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue1_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian1" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified2" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">Width/Dia</td>
                                            <td>mm</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo3" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue2_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue2_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue2_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue2_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue2_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian2" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified3" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">A</td>
                                            <td>mm²</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo4" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue3_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue3_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue3_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue3_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue3_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian3" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified4" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">BF</td>
                                            <td>N</td>
                                            <td rowspan="5">
                                                <asp:TextBox ID="txtEquipNo5" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue4_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue4_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue4_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue4_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue4_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian4" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified5" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">TS</td>
                                            <td>N/mm²</td>

                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue5_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue5_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue5_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue5_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue5_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian5" runat="server" CssClass="label-result" />
                                            </td>
                                            <td class="text-right">Min.:
                                                <asp:TextBox ID="txtSpecified6" runat="server" CssClass="input-small" /></td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">LO</td>
                                            <td>mm</td>

                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue6_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue6_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue6_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue6_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue6_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian6" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified7" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">L1</td>
                                            <td>mm</td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue7_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue7_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue7_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue7_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue7_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian7" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified8" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">EL</td>
                                            <td>%</td>

                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue8_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue8_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue8_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue8_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue8_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian8" runat="server" CssClass="label-result" />
                                            </td>
                                            <td class="text-right">Min.:<asp:TextBox ID="txtSpecified9" runat="server" CssClass="input-small" /></td>
                                        </tr>

                                        <!-- Immersed In Oil Section -->

                                        <tr>
                                            <td rowspan="5" class="section-header">Immersed In<br />
                                                Oil
                                            </td>
                                            <td class="text-left">Temp.</td>
                                            <td>°C</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo6" runat="server" CssClass="input-medium" /></td>
                                            <td colspan="6">
                                                <asp:TextBox ID="txtTemp1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtSpecified10" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td rowspan="4">Time</td>
                                            <td>H</td>
                                            <td rowspan="4">
                                                <asp:TextBox ID="txtEquipNo7" runat="server" CssClass="input-medium" />
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtH1" runat="server" CssClass="input-small" TextMode="Time"/>
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtH2" runat="server" CssClass="input-small" TextMode="Time"/>
                                            </td>


                                            <td>
                                                <asp:TextBox ID="txtSpecified11" runat="server" CssClass="input-small" /></td>
                                        </tr>

                                        <tr>
                                            <td rowspan="2">Date</td>
                                            <td colspan="3">Start</td>
                                            <td colspan="3">End</td>
                                            <td></td>
                                        </tr>

                                        <tr>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtStartDate1" runat="server" CssClass="input-small" TextMode="Date"/>
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtEndDate1" runat="server" CssClass="input-small" TextMode="Date"/>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified12" runat="server" CssClass="input-small" /></td>
                                        </tr>

                                        <tr>
                                            <td>H</td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtH3" runat="server" CssClass="input-small" TextMode="Time"/>
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtH4" runat="server" CssClass="input-small" TextMode="Time"/>
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtSpecified13" runat="server" CssClass="input-small" /></td>
                                        </tr>

                                        <!------------------------------------- Conditioning Section ---------------------------------------->


                                        <tr>
                                            <td rowspan="5" class="section-header">Conditioning
                                            </td>
                                            <td class="text-left">Room Temp.</td>
                                            <td>°C</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo8" runat="server" CssClass="input-medium" /></td>
                                            <td colspan="6">
                                                <asp:TextBox ID="txtConditioningRoomTemp" runat="server" CssClass="input-small" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified14" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td rowspan="4">Time</td>
                                            <td>H</td>
                                            <td rowspan="4">
                                                <asp:TextBox ID="txtEquipNo9" runat="server" CssClass="input-medium" /></td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtConditioningTime1" runat="server" CssClass="input-small" TextMode="Time"/>
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtConditioningTime2" runat="server" CssClass="input-small" TextMode="Time"/>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified15" runat="server" CssClass="input-small" /></td>
                                        </tr>

                                        <tr>

                                            <td rowspan="2">Date</td>
                                            <td colspan="3">Start</td>
                                            <td colspan="3">End</td>
                                            <td></td>
                                        </tr>

                                        <tr>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtConditioningDate1" runat="server" CssClass="input-small" TextMode="Date" />
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtConditioningDate2" runat="server" CssClass="input-small" TextMode="Date"/>
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtSpecified16" runat="server" CssClass="input-small" /></td>
                                        </tr>

                                        <tr>

                                            <td>H</td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtConditioningTime3" runat="server" CssClass="input-small" TextMode="Time"/>
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtConditioningTime4" runat="server" CssClass="input-small" TextMode="Time"/>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified17" runat="server" CssClass="input-small" /></td>
                                        </tr>

                                        <!------------------------------------------------------ TS & EL After Section ----------------------------------------->
                                        <tr>
                                            <td rowspan="8" class="section-header">TS & EL<br />
                                                After<br />
                                                Immersed In<br />
                                                Oil<br />
                                                <br />
                                                Date:
                                                <asp:TextBox ID="txtADate" runat="server" CssClass="form-control" Style="margin-left: 20px; width: 120px;" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                                                    ControlToValidate="txtADate"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td class="text-left">Thickness</td>
                                            <td>mm</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo10" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue9_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue9_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue9_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue9_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue9_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian9" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified18" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <!--------------------------------------------------- Continue with all other After rows... --------------------------------------->
                                        <tr>
                                            <td class="text-left">Width/Dia</td>
                                            <td>mm</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo11" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue10_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue10_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue10_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue10_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue10_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian10" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified19" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">A</td>
                                            <td>mm²</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo12" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue11_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue11_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue11_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue11_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue11_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian11" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified20" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">BF</td>
                                            <td>N</td>
                                            <td rowspan="5">
                                                <asp:TextBox ID="txtEquipNo13" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue12_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue12_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue12_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue12_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue12_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian12" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified21" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">TS</td>
                                            <td>N/mm²</td>

                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue13_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue13_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue13_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue13_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue13_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian13" runat="server" CssClass="label-result" />
                                            </td>
                                            <td class="text-right">Min.:
                                                <asp:TextBox ID="txtSpecified22" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">LO</td>
                                            <td>mm</td>

                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue14_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue14_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue14_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue14_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue14_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian14" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified23" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">L1</td>
                                            <td>mm</td>

                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue15_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue15_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue15_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue15_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue15_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian15" runat="server" CssClass="label-result" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified24" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">EL</td>
                                            <td>%</td>

                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue16_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue16_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue16_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue16_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue16_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian16" runat="server" CssClass="label-result" />
                                            </td>
                                            <td class="text-right">Min.:
                                                <asp:TextBox ID="txtSpecified25" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>
                                        <!----------------------------------------------------------- Variation Section ----------------------------------------------------------->
                                        <tr>
                                            <td rowspan="2" class="section-header">Variation
                                            </td>
                                            <td class="text-left">TS</td>
                                            <td>%</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo14" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue17_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue17_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue17_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue17_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue17_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian17" runat="server" CssClass="label-result" />
                                            </td>
                                            <td class="text-right">Max.:
                                                <asp:TextBox ID="txtSpecified26" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="text-left">EL</td>
                                            <td>%</td>
                                            <td>
                                                <asp:TextBox ID="txtEquipNo15" runat="server" CssClass="input-medium" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue18_1" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue18_2" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue18_3" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue18_4" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredValue18_5" runat="server" CssClass="input-small"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblMedian18" runat="server" CssClass="label-result" />
                                            </td>
                                            <td class="text-right">Max.:
                                                <asp:TextBox ID="txtSpecified27" runat="server" CssClass="input-small" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>

                            </div>


                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" CausesValidation="true" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" CausesValidation="false" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" CausesValidation="true" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
