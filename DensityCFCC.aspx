<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="DensityCFCC.aspx.cs" Inherits="LIMS.DensityCFCC" %>

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
                            <h2>PENGUJIAN DENSITY CFCC</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true"/>
                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true"/>
                                        </td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Grade</b></td>
                                        <td>
                                            <asp:TextBox ID="grade" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true"/>
                                        </td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true"/>
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
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomNo" runat="server"
                                                ControlToValidate="roomno"
                                                ErrorMessage="Room No. is required"
                                                Display="Dynamic" ForeColor="Red" />
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
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true"/>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="table-responsive">
                                <table class="table table-bordered text-center">
                                    <thead>
                                        <tr>
                                            <th>No</th>
                                            <th>Description</th>
                                            <th>Unit</th>
                                            <th>Specified</th>
                                            <th>Value test</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <!-- Row 1 -->
                                        <tr>
                                            <td>1</td>
                                            <td>Mass</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="gr" Value="gr" Selected="True" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="s" Value="sec" />
                                                    <asp:ListItem Text="cm3" Value="cm3" />
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified1" runat="server" CssClass="form-control numeric-only" />
                                                <asp:RegularExpressionValidator ID="revSpecified1" runat="server"
                                                    ControlToValidate="txtSpecified1"
                                                    ValidationExpression="^\d+(\.\d+)?$"
                                                    ErrorMessage="Enter a valid number"
                                                    Display="Dynamic"
                                                    ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1" runat="server" CssClass="form-control numeric-only" />
                                                <asp:RegularExpressionValidator ID="revValueTest1" runat="server"
                                                    ControlToValidate="txtValueTest1"
                                                    ValidationExpression="^\d+(\.\d+)?$"
                                                    ErrorMessage="Enter a valid number"
                                                    Display="Dynamic"
                                                    ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <!-- Row 2 -->
                                        <tr>
                                            <td>2</td>
                                            <td>Volume</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit2" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="s" Value="sec" />
                                                    <asp:ListItem Text="cm3" Value="cm3" Selected="True" />
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified2" runat="server" CssClass="form-control numeric-only" />
                                                <asp:RegularExpressionValidator ID="revSpecified2" runat="server"
                                                    ControlToValidate="txtSpecified2"
                                                    ValidationExpression="^\d+(\.\d+)?$"
                                                    ErrorMessage="Enter a valid number"
                                                    Display="Dynamic"
                                                    ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest2" runat="server" CssClass="form-control numeric-only" />
                                                <asp:RegularExpressionValidator ID="revValueTest2" runat="server"
                                                    ControlToValidate="txtValueTest2"
                                                    ValidationExpression="^\d+(\.\d+)?$"
                                                    ErrorMessage="Enter a valid number"
                                                    Display="Dynamic"
                                                    ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <!-- Row 3 -->
                                        <tr>
                                            <td>3</td>
                                            <td>Density</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit3" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="s" Value="sec" />
                                                    <asp:ListItem Text="cm3" Value="cm3" />
                                                    <asp:ListItem Text="gr/cm3" Value="gr/cm3" />
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified3" runat="server" CssClass="form-control numeric-only" />
                                                <asp:RegularExpressionValidator ID="revSpecified3" runat="server"
                                                    ControlToValidate="txtSpecified3"
                                                    ValidationExpression="^\d+(\.\d+)?$"
                                                    ErrorMessage="Enter a valid number"
                                                    Display="Dynamic"
                                                    ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest3" runat="server" CssClass="form-control numeric-only" />
                                                <asp:RegularExpressionValidator ID="revValueTest3" runat="server"
                                                    ControlToValidate="txtValueTest3"
                                                    ValidationExpression="^\d+(\.\d+)?$"
                                                    ErrorMessage="Enter a valid number"
                                                    Display="Dynamic"
                                                    ForeColor="Red" />
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
    <script type="text/javascript">
        function allowOnlyNumbers() {
            document.querySelectorAll('input[type="text"]').forEach(function (input) {
                // Only restrict those with numeric validation class
                if (input.classList.contains('numeric-only')) {
                    input.addEventListener('input', function () {
                        this.value = this.value.replace(/[^0-9.]/g, '');
                    });
                }
            });
        }

        window.onload = function () {
            allowOnlyNumbers();
        };
    </script>

</asp:Content>
