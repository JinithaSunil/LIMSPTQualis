<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="BusBarStraightness.aspx.cs" Inherits="LIMS.BusBarStraightness" %>


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
                            <h2>Bus Bar (Straightness) Test</h2>
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

                                <table class="table-responsive" style="width: 100%;">
                                    <thead>
                                        <tr>
                                            <th rowspan="2">No</th>
                                            <th rowspan="2">Description</th>
                                            <th rowspan="2">Unit</th>
                                            <th rowspan="2">Specified</th>
                                            <th colspan="8">Value test</th>
                                            <th rowspan="2">Median</th>
                                        </tr>
                                        <tr>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>7</th>
                                            <th>8</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><b>1</b></td>
                                            <td><b> Straightness :</b></td>
                                            <td> - </td>
                                            <td colspan="11"></td>
                                        </tr>
                                        
                                        <tr>
                                            <td>1.1</td>
                                            <td>Thickness and width</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="mm" Value="mm" Selected="True" />
                                                    <asp:ListItem Text="cm" Value="cm" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit1" runat="server"
                                                    ControlToValidate="ddlUnit1"
                                                    InitialValue=""
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified1_1" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified1_1" runat="server"
                                                    ControlToValidate="txtSpecified1_1"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <!-- 8 Value Test Columns with AutoPostBack and Validation -->
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_1_1" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_1_1" runat="server"
                                                    ControlToValidate="txtValueTest1_1_1"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_1_2" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_1_2" runat="server"
                                                    ControlToValidate="txtValueTest1_1_2"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_1_3" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_1_3" runat="server"
                                                    ControlToValidate="txtValueTest1_1_3"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_1_4" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_1_4" runat="server"
                                                    ControlToValidate="txtValueTest1_1_4"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_1_5" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_1_5" runat="server"
                                                    ControlToValidate="txtValueTest1_1_5"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_1_6" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_1_6" runat="server"
                                                    ControlToValidate="txtValueTest1_1_6"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_1_7" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_1_7" runat="server"
                                                    ControlToValidate="txtValueTest1_1_7"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_1_8" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_1_8" runat="server"
                                                    ControlToValidate="txtValueTest1_1_8"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblAverage1_1" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;" />
                                            </td>
                                        </tr>

                                        
                                        <tr>
                                            <td>1.2</td>
                                            <td>h2, every length of L2 from 400 mm</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit2" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="mm" Value="mm" Selected="True" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit2" runat="server"
                                                    ControlToValidate="ddlUnit2"
                                                    InitialValue=""
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified1_2" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified1_2" runat="server"
                                                    ControlToValidate="txtSpecified1_2"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <!-- 8 Value Test Columns with AutoPostBack and Validation -->
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_2_1" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_2_1" runat="server"
                                                    ControlToValidate="txtValueTest1_2_1"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_2_2" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_2_2" runat="server"
                                                    ControlToValidate="txtValueTest1_2_2"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_2_3" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_2_3" runat="server"
                                                    ControlToValidate="txtValueTest1_2_3"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_2_4" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_2_4" runat="server"
                                                    ControlToValidate="txtValueTest1_2_4"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_2_5" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_2_5" runat="server"
                                                    ControlToValidate="txtValueTest1_2_5"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_2_6" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_2_6" runat="server"
                                                    ControlToValidate="txtValueTest1_2_6"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_2_7" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_2_7" runat="server"
                                                    ControlToValidate="txtValueTest1_2_7"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_2_8" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_2_8" runat="server"
                                                    ControlToValidate="txtValueTest1_2_8"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblAverage1_2" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;" />
                                            </td>
                                        </tr>

                                        
                                        <tr>
                                            <td>1.3</td>
                                            <td>h1, for length upper 1 meter</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit3" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="mm" Value="mm" Selected="True" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit3" runat="server"
                                                    ControlToValidate="ddlUnit3"
                                                    InitialValue=""
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified1_3" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified1_3" runat="server"
                                                    ControlToValidate="txtSpecified1_3"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <!-- 8 Value Test Columns with AutoPostBack and Validation -->
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_3_1" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_3_1" runat="server"
                                                    ControlToValidate="txtValueTest1_3_1"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_3_2" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_3_2" runat="server"
                                                    ControlToValidate="txtValueTest1_3_2"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_3_3" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_3_3" runat="server"
                                                    ControlToValidate="txtValueTest1_3_3"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_3_4" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_3_4" runat="server"
                                                    ControlToValidate="txtValueTest1_3_4"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_3_5" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_3_5" runat="server"
                                                    ControlToValidate="txtValueTest1_3_5"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_3_6" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_3_6" runat="server"
                                                    ControlToValidate="txtValueTest1_3_6"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_3_7" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_3_7" runat="server"
                                                    ControlToValidate="txtValueTest1_3_7"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1_3_8" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;"
                                                    AutoPostBack="true" OnTextChanged="TextValue_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1_3_8" runat="server"
                                                    ControlToValidate="txtValueTest1_3_8"
                                                    ErrorMessage="Required"
                                                    ForeColor="Red"
                                                    Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblAverage1_3" runat="server" CssClass="form-control"
                                                    Style="text-align: center; width: 90%;" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>

                                <p>
                                    h1,h2 = deviasi strength
                                    <br />
                                    L1,L2 = Length of Busbar
                                    <br />
                                    k1  = multiply every at 1 meter from L1
                                </p>
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

