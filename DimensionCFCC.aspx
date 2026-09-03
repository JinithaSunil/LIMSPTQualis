<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="DimensionCFCC.aspx.cs" Inherits="LIMS.DimensionCFCC" %>

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
            width: 50px;
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
                            <h2>PENGUJIAN DIMENSION CFCC</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true" />
                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true" />
                                        </td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" Style="text-align: center;"  ReadOnly="true"  />
                                        </td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" Style="text-align: center;"  ReadOnly="true"  />
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
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" Style="text-align: center;"  ReadOnly="true" />
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
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" Style="text-align: center;"  ReadOnly="true" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="table-responsive">
                                <table class="table table-bordered text-center">
                                    <thead>
                                        <tr>
                                            <th rowspan="2">No</th>
                                            <th rowspan="2">Description</th>
                                            <th rowspan="2">Unit</th>
                                            <th rowspan="2">Specified</th>
                                            <th colspan="7">Value test</th>
                                        </tr>
                                        <tr>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>

                                        <!-- Section 1 -->
                                        <tr>
                                            <td rowspan="2">1</td>
                                            <td rowspan="2">Thickness of galvanic</td>
                                            <td rowspan="2">
                                                <asp:DropDownList ID="ddlUnit1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" Selected="True" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="s" Value="sec" />
                                                    <asp:ListItem Text="cm3" Value="cm3" />
                                                </asp:DropDownList>
                                            </td>
                                            <td rowspan="2">
                                                <asp:TextBox ID="txtSpecified1" runat="server" CssClass="form-control" VALUE="Min.:" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified1" runat="server" ControlToValidate="txtSpecified1"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td rowspan="2">
                                                <asp:TextBox ID="txtValueTest1" runat="server" CssClass="form-control" OnTextChanged="Input_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1" runat="server" ControlToValidate="txtValueTest1"
                                                    ErrorMessage="Value Test 1 is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td rowspan="2">
                                                <asp:TextBox ID="txtValueTest2" runat="server" CssClass="form-control" OnTextChanged="Input_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest2" runat="server" ControlToValidate="txtValueTest2"
                                                    ErrorMessage="Value Test 2 is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td rowspan="2">
                                                <asp:TextBox ID="txtValueTest3" runat="server" CssClass="form-control" OnTextChanged="Input_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest3" runat="server" ControlToValidate="txtValueTest3"
                                                    ErrorMessage="Value Test 3 is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td rowspan="2">
                                                <asp:TextBox ID="txtValueTest4" runat="server" CssClass="form-control" OnTextChanged="Input_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest4" runat="server" ControlToValidate="txtValueTest4"
                                                    ErrorMessage="Value Test 4 is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td rowspan="2">
                                                <asp:TextBox ID="txtValueTest5" runat="server" CssClass="form-control" OnTextChanged="Input_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest5" runat="server" ControlToValidate="txtValueTest5"
                                                    ErrorMessage="Value Test 5 is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td rowspan="2">
                                                <asp:TextBox ID="txtValueTest6" runat="server" CssClass="form-control" OnTextChanged="Input_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest6" runat="server" ControlToValidate="txtValueTest6"
                                                    ErrorMessage="Value Test 6 is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueavgmin" runat="server" CssClass="form-control" ReadOnly="true" VALUE="Min: " />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtValueavgmax" runat="server" CssClass="form-control" ReadOnly="true" VALUE="Max: " />

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
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" CausesValidation="false" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>
