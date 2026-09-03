<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="SpecialBendingTestLV.aspx.cs" Inherits="LIMS.SpecialBendingTestLV" %>

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
            border: 1px solid #ddd;
        }

        th {
            background-color: #f2f2f2;
            color: #333;
            padding: 10px;
            /*text-align: center;*/
            font-weight: bold;
        }

        td {
            padding: 8px;
            /*text-align: center;*/
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
                            <h2>Special Bending Test</h2>
                        </div>
                        <div class="body">
                            <!-- Info Table -->
                            <div class="table-responsive">
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

                                <!-- Test Data Table -->
                                <div class="table-responsive">
                                    <table class="table-bordered">
                                        <thead>
                                            <tr>
                                                <th>No</th>
                                                <th>Item</th>
                                                <th>Unit</th>
                                                <th>Specified Value</th>
                                                <th>Measured</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td>1</td>
                                                <td>Outer diameter of cable</td>
                                                <td>
                                                    <asp:DropDownList ID="ddlUnit1" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="cycle" Value="cycle" />
                                                        <asp:ListItem Text="°C" Value="°C" />
                                                        <asp:ListItem Text="mm" Value="mm" Selected="True" />
                                                        <asp:ListItem Text="m" Value="m" />
                                                        <asp:ListItem Text="-" Value="-" />
                                                        <asp:ListItem Text="h" Value="h" />
                                                        <asp:ListItem Text="kV" Value="kV" />
                                                        <asp:ListItem Text="mnt" Value="mnt" />
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedValue1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvSpecifiedValue1" runat="server"
                                                        ControlToValidate="txtSpecifiedValue1"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtMeasured1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvMeasured1" runat="server"
                                                        ControlToValidate="txtMeasured1"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>

                                            </tr>

                                            <tr>
                                                <td>2</td>
                                                <td>Diameter cylinder bending (7.D±5%) </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlUnit2" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="cycle" Value="cycle" />
                                                        <asp:ListItem Text="°C" Value="°C" />
                                                        <asp:ListItem Text="mm" Value="mm" Selected="True" />
                                                        <asp:ListItem Text="m" Value="m" />
                                                        <asp:ListItem Text="-" Value="-" />
                                                        <asp:ListItem Text="h" Value="h" />
                                                        <asp:ListItem Text="kV" Value="kV" />
                                                        <asp:ListItem Text="mnt" Value="mnt" />
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedValue2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvSpecifiedValue2" runat="server"
                                                        ControlToValidate="txtSpecifiedValue2"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtMeasured2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvMeasured2" runat="server"
                                                        ControlToValidate="txtMeasured2"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>3</td>
                                                <td>Cycle of operation</td>
                                                <td>
                                                    <asp:DropDownList ID="ddlUnit3" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="cycle" Value="cycle" Selected="True" />
                                                        <asp:ListItem Text="°C" Value="°C" />
                                                        <asp:ListItem Text="mm" Value="mm" />
                                                        <asp:ListItem Text="m" Value="m" />
                                                        <asp:ListItem Text="-" Value="-" />
                                                        <asp:ListItem Text="h" Value="h" />
                                                        <asp:ListItem Text="kV" Value="kV" />
                                                        <asp:ListItem Text="mnt" Value="mnt" />
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedValue3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvSpecifiedValue3" runat="server"
                                                        ControlToValidate="txtSpecifiedValue3"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtMeasured3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvMeasured3" runat="server"
                                                        ControlToValidate="txtMeasured3"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>

                                            </tr>

                                            <tr>
                                                <td>4</td>
                                                <td>Heating of sample in cylinder drum in air oven the maximum temperature conductor</td>
                                                <td>
                                                    <asp:DropDownList ID="ddlUnit4" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="cycle" Value="cycle" />
                                                        <asp:ListItem Text="°C" Value="°C" Selected="True" />
                                                        <asp:ListItem Text="mm" Value="mm" />
                                                        <asp:ListItem Text="m" Value="m" />
                                                        <asp:ListItem Text="-" Value="-" />
                                                        <asp:ListItem Text="h" Value="h" />
                                                        <asp:ListItem Text="kV" Value="kV" />
                                                        <asp:ListItem Text="mnt" Value="mnt" />
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedValue4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvSpecifiedValue4" runat="server"
                                                        ControlToValidate="txtSpecifiedValue4"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtMeasured4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvMeasured4" runat="server"
                                                        ControlToValidate="txtMeasured4"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>

                                            </tr>

                                            <tr>
                                                <td>5</td>
                                                <td>Duration of heating in air oven</td>
                                                <td>
                                                    <asp:DropDownList ID="ddlUnit5" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="cycle" Value="cycle" />
                                                        <asp:ListItem Text="°C" Value="°C" />
                                                        <asp:ListItem Text="mm" Value="mm" />
                                                        <asp:ListItem Text="m" Value="m" />
                                                        <asp:ListItem Text="-" Value="-" />
                                                        <asp:ListItem Text="h" Value="h" Selected="True" />
                                                        <asp:ListItem Text="kV" Value="kV" />
                                                        <asp:ListItem Text="mnt" Value="mnt" />
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedValue5" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvSpecifiedValue5" runat="server"
                                                        ControlToValidate="txtSpecifiedValue5"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtMeasured5" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="rfvMeasured5" runat="server"
                                                        ControlToValidate="txtMeasured5"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>

                                            </tr>

                                            <tr>
                                                <td>6</td>
                                                <td>Test voltage after heating in air oven</td>
                                                <td>
                                                    <asp:DropDownList ID="ddlUnit6" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="cycle" Value="cycle" />
                                                        <asp:ListItem Text="°C" Value="°C" />
                                                        <asp:ListItem Text="mm" Value="mm" />
                                                        <asp:ListItem Text="m" Value="m" />
                                                        <asp:ListItem Text="-" Value="-" />
                                                        <asp:ListItem Text="h" Value="h" />
                                                        <asp:ListItem Text="kV" Value="kV" Selected="True" />
                                                        <asp:ListItem Text="mnt" Value="mnt" />
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedValue6" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                        ControlToValidate="txtSpecifiedValue6"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtMeasured6" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                        ControlToValidate="txtMeasured6"
                                                        ErrorMessage="Required"
                                                        ForeColor="Red"
                                                        Display="Dynamic"
                                                        CssClass="validator-error">
                                                    </asp:RequiredFieldValidator>
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
