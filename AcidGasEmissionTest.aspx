<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="AcidGasEmissionTest.aspx.cs" Inherits="LIMS.AcidGasEmissionTest" %>


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
            text-align: left;
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
                            <h2>ACID GAS EMISSION TEST</h2>
                        </div>
                        <div class="body">
                            <!-- Info Table -->
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
                                <table class="table-responsive" style="width: 100%;">
                                    <thead>
                                        <tr>
                                            <th rowspan="2" style="width: 5%">No</th>
                                            <th rowspan="2" colspan="2" style="width: 25%; text-align: left;">Item</th>
                                            <th rowspan="2" style="width: 10%">Unit</th>
                                            <th rowspan="2" style="width: 10%">Specified</th>
                                            <th colspan="6" style="width: 50%">Measured</th>
                                        </tr>
                                        <tr>
                                            <th>Blank</th>
                                            <th>Sample 1</th>
                                            <th>Sample 2</th>
                                            <%--<th>4</th>
                                            <th>5</th>
                                            <th>6</th>--%>
                                        </tr>
                                    </thead>
                                    <tbody>

                                        <!-- Parameter 1: Sample Weight -->
                                        <tr>
                                            <td>1</td>
                                            <td rowspan="12">HCL Content</td>
                                            <td>Sample Weight</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" Selected="True" />
                                                    <asp:ListItem Text="ml" Value="ml" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit1" runat="server" ControlToValidate="ddlUnit1"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified1" runat="server" ControlToValidate="txtSpecified1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_1" runat="server" ControlToValidate="txtValueTest1_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_2" runat="server" ControlToValidate="txtValueTest1_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_3" runat="server" ControlToValidate="txtValueTest1_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest1_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_4" runat="server" ControlToValidate="txtValueTest1_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_5" runat="server" ControlToValidate="txtValueTest1_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_6" runat="server" ControlToValidate="txtValueTest1_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <!-- Parameter 2: Sodium Hydroxide  -->
                                        <tr>
                                            <td>2</td>
                                            <td>Sodium Hydroxide </td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit2" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" Selected="True" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit2" runat="server" ControlToValidate="ddlUnit2"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified2" runat="server" ControlToValidate="txtSpecified2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest2_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest2_1" runat="server" ControlToValidate="txtValueTest2_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest2_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest2_2" runat="server" ControlToValidate="txtValueTest2_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest2_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest2_3" runat="server" ControlToValidate="txtValueTest2_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest2_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest2_4" runat="server" ControlToValidate="txtValueTest2_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest2_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest2_5" runat="server" ControlToValidate="txtValueTest2_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest2_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest2_6" runat="server" ControlToValidate="txtValueTest2_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <!-- Parameter 3: Nitric Acid -->
                                        <tr>
                                            <td>3</td>
                                            <td>Nitric Acid</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit3" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" Selected="True" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit3" runat="server" ControlToValidate="ddlUnit3"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified3" runat="server" ControlToValidate="txtSpecified3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest3_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest3_1" runat="server" ControlToValidate="txtValueTest3_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest3_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest3_2" runat="server" ControlToValidate="txtValueTest3_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest3_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest3_3" runat="server" ControlToValidate="txtValueTest3_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest3_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest3_4" runat="server" ControlToValidate="txtValueTest3_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest3_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest3_5" runat="server" ControlToValidate="txtValueTest3_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest3_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest3_6" runat="server" ControlToValidate="txtValueTest3_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <!-- Parameter 4: Silver Nitrate -->
                                        <tr>
                                            <td>4</td>
                                            <td>Silver Nitrate</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit4" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" Selected="True" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit4" runat="server" ControlToValidate="ddlUnit4"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified4" runat="server" ControlToValidate="txtSpecified4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest4_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest4_1" runat="server" ControlToValidate="txtValueTest4_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest4_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest4_2" runat="server" ControlToValidate="txtValueTest4_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest4_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest4_3" runat="server" ControlToValidate="txtValueTest4_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest4_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest4_4" runat="server" ControlToValidate="txtValueTest4_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest4_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest4_5" runat="server" ControlToValidate="txtValueTest4_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest4_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest4_6" runat="server" ControlToValidate="txtValueTest4_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <!-- Parameter 5: Nitrobenzene (toluene) -->
                                        <tr>
                                            <td>5</td>
                                            <td>Nitrobenzene (Toluene)</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit5" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" Selected="True" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit5" runat="server" ControlToValidate="ddlUnit5"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified5" runat="server" ControlToValidate="txtSpecified5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest5_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest5_1" runat="server" ControlToValidate="txtValueTest5_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest5_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest5_2" runat="server" ControlToValidate="txtValueTest5_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest5_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest5_3" runat="server" ControlToValidate="txtValueTest5_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest5_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest5_4" runat="server" ControlToValidate="txtValueTest5_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest5_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest5_5" runat="server" ControlToValidate="txtValueTest5_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest5_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest5_6" runat="server" ControlToValidate="txtValueTest5_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <!-- Parameter 6: Ferric Ammonium  Sulphate -->
                                        <tr>
                                            <td>6</td>
                                            <td>Ferric Ammonium  Sulphate</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit6" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" Selected="True"/>
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit6" runat="server" ControlToValidate="ddlUnit6"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified6" runat="server" ControlToValidate="txtSpecified6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest6_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest6_1" runat="server" ControlToValidate="txtValueTest6_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest6_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest6_2" runat="server" ControlToValidate="txtValueTest6_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest6_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest6_3" runat="server" ControlToValidate="txtValueTest6_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%-- <td>
                                                <asp:TextBox ID="txtValueTest6_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest6_4" runat="server" ControlToValidate="txtValueTest6_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest6_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest6_5" runat="server" ControlToValidate="txtValueTest6_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest6_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest6_6" runat="server" ControlToValidate="txtValueTest6_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <!-- Parameter 7: Burn Temperature -->
                                        <tr>
                                            <td>7</td>
                                            <td>Burn Temperature</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit7" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="mm" Value="mm"/>
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" Selected="True"/>
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit7" runat="server" ControlToValidate="ddlUnit7"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified7" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified7" runat="server" ControlToValidate="txtSpecified7"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest7_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest7_1" runat="server" ControlToValidate="txtValueTest7_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest7_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest7_2" runat="server" ControlToValidate="txtValueTest7_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest7_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest7_3" runat="server" ControlToValidate="txtValueTest7_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest7_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest7_4" runat="server" ControlToValidate="txtValueTest7_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest7_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest7_5" runat="server" ControlToValidate="txtValueTest7_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest7_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest7_6" runat="server" ControlToValidate="txtValueTest7_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <!-- Parameter 8: Time for Reaching Burn Temperature -->
                                        <tr>
                                            <td>8</td>
                                            <td>Time for Reaching Burn Temperature</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit8" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="second" Value="second"/>
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" Selected="True"/>
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit8" runat="server" ControlToValidate="ddlUnit8"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified8" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified8" runat="server" ControlToValidate="txtSpecified8"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest8_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest8_1" runat="server" ControlToValidate="txtValueTest8_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest8_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest8_2" runat="server" ControlToValidate="txtValueTest8_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest8_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest8_3" runat="server" ControlToValidate="txtValueTest8_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest8_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest8_4" runat="server" ControlToValidate="txtValueTest8_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest8_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest8_5" runat="server" ControlToValidate="txtValueTest8_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest8_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest8_6" runat="server" ControlToValidate="txtValueTest8_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <!-- Parameter 9: Burn Time -->
                                        <tr>
                                            <td>9</td>
                                            <td>Burn Time</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit9" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="-" Value="-"/>
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" Selected="True"/>
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit9" runat="server" ControlToValidate="ddlUnit9"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified9" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified9" runat="server" ControlToValidate="txtSpecified9"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest9_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest9_1" runat="server" ControlToValidate="txtValueTest9_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest9_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest9_2" runat="server" ControlToValidate="txtValueTest9_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest9_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest9_3" runat="server" ControlToValidate="txtValueTest9_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest9_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest9_4" runat="server" ControlToValidate="txtValueTest9_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest9_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest9_5" runat="server" ControlToValidate="txtValueTest9_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest9_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest9_6" runat="server" ControlToValidate="txtValueTest9_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <tr>
                                            <td>10</td>
                                            <td>Ammonium Thiocyanate for the Tritation</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit10" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="%" Value="%"/>
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" Selected="True"/>
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit10" runat="server" ControlToValidate="ddlUnit10"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified10" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified10" runat="server" ControlToValidate="txtSpecified10"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest10_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest10_1" runat="server" ControlToValidate="txtValueTest10_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest10_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest10_2" runat="server" ControlToValidate="txtValueTest10_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest10_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest10_3" runat="server" ControlToValidate="txtValueTest10_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest10_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest10_4" runat="server" ControlToValidate="txtValueTest10_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest10_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest10_5" runat="server" ControlToValidate="txtValueTest10_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest10_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest10_6" runat="server" ControlToValidate="txtValueTest10_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <tr>
                                            <td rowspan="2">11</td>
                                            <td rowspan="2">(HCI) Content</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit11" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="%" Value="%" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" Selected="True"/>
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit11" runat="server" ControlToValidate="ddlUnit11"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified11" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified11" runat="server" ControlToValidate="txtSpecified11"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest11_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtValueTest11_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest11_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtValueTest11_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest11_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtValueTest11_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest11_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtValueTest11_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest11_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtValueTest11_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest11_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtValueTest11_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                        <tr>

                                            <%--<td>(HCI) Content</td>--%>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit12" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="%" Value="%" Selected="True" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="Ω.m" Value="Ω.m" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="hour" Value="hour" />
                                                    <asp:ListItem Text="second" Value="second" />
                                                    <asp:ListItem Text="l/min" Value="l/min" />
                                                    <asp:ListItem Text="min" Value="min" />
                                                    <asp:ListItem Text="g" Value="g" />
                                                    <asp:ListItem Text="ml" Value="ml" />
                                                    <asp:ListItem Text="mg/g" Value="mg/g" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit12" runat="server" ControlToValidate="ddlUnit12"
                                                    InitialValue="" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified12" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified12" runat="server" ControlToValidate="txtSpecified12"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest12_1" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest12_1" runat="server" ControlToValidate="txtValueTest12_1"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest12_2" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest12_2" runat="server" ControlToValidate="txtValueTest12_2"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest12_3" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest12_3" runat="server" ControlToValidate="txtValueTest12_3"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtValueTest12_4" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest12_4" runat="server" ControlToValidate="txtValueTest12_4"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest12_5" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest12_5" runat="server" ControlToValidate="txtValueTest12_5"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest12_6" runat="server" CssClass="form-control" Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest12_6" runat="server" ControlToValidate="txtValueTest12_6"
                                                    ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />
                                            </td>--%>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>

                            <div class="form-actions">
                                <div class="form-actions">
                                    <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" CausesValidation="true" />
                                    <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" CausesValidation="false" />
                                    <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" CausesValidation="true" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


