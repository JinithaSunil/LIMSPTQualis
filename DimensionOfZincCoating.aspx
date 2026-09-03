<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="DimensionOfZincCoating.aspx.cs" Inherits="LIMS.Zinccoating1" %>


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
                            <h2>DIMENSION OF ZINC COATING TEST</h2>
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
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" ReadOnly="true" />

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
                                            <th rowspan="2">No</th>
                                            <th rowspan="2">Item Type</th>
                                            <th rowspan="2">Unit</th>
                                            <th>Specified</th>
                                            <th rowspan="2">1</th>
                                            <th rowspan="2">2</th>
                                            <th rowspan="2">3</th>
                                            <th rowspan="2">Avg</th>
                                        </tr>

                                    </thead>

                                    <tbody>
                                        <!-- Row 1: Diameter of steel wire -->
                                        <tr>
                                            <td>1</td>
                                            <td>Diameter of steel wire</td>
                                            <td>
                                                <asp:DropDownList ID="ddlDiameter_steelwire1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="">Select</asp:ListItem>
                                                    <asp:ListItem Value="gr/m2">gr/m²</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit1" runat="server" ControlToValidate="ddlDiameter_steelwire1"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="diameterSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDSpecified" runat="server" ControlToValidate="diameterSpecified" ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="diameterInput1" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvD1" runat="server" ControlToValidate="diameterInput1" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="diameterInput2" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvD2" runat="server" ControlToValidate="diameterInput2" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="diameterInput3" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvD3" runat="server" ControlToValidate="diameterInput3" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="diameterInputAvg" runat="server" CssClass="form-control" ReadOnly="true" />
                                                <%--<asp:RequiredFieldValidator ID="rfvD4" runat="server" ControlToValidate="diameterInput4" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>

                                        <!-- Row 2: Weight / Thickness before immersed -->
                                        <tr>
                                            <td>2</td>
                                            <td>Weight / Thickness before immersed</td>
                                            <td>
                                                <asp:DropDownList ID="ddlBeforeImmersed" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="">--Select--</asp:ListItem>
                                                    <asp:ListItem Value="gr/m2">gr/m²</asp:ListItem>
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit2" runat="server" ControlToValidate="ddlBeforeImmersed"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="beforeImmersedSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBSpecified" runat="server" ControlToValidate="beforeImmersedSpecified" ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="beforeImmersedInput1" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvB1" runat="server" ControlToValidate="beforeImmersedInput1" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="beforeImmersedInput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvB2" runat="server" ControlToValidate="beforeImmersedInput2" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="beforeImmersedInput3" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvB3" runat="server" ControlToValidate="beforeImmersedInput3" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="beforeImmersedInputAvg" runat="server" CssClass="form-control" ReadOnly="true" />
                                                <%--     <asp:RequiredFieldValidator ID="rfvB4" runat="server" ControlToValidate="beforeImmersedInput4" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>


                                        </tr>

                                        <!-- Row 3: Weight / Thickness after immersed -->
                                        <tr>
                                            <td>3</td>
                                            <td>Weight / Thickness after immersed</td>
                                            <td>
                                                <asp:DropDownList ID="ddlAfterImmersed" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="">--Select--</asp:ListItem>
                                                    <asp:ListItem Value="gr/m2">gr/m²</asp:ListItem>
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit3" runat="server" ControlToValidate="ddlAfterImmersed"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="afterImmersedSpecified" runat="server" CssClass="form-control" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvASpecified" runat="server" ControlToValidate="afterImmersedSpecified" ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="afterImmersedInput1" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvA1" runat="server" ControlToValidate="afterImmersedInput1" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="afterImmersedInput2" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvA2" runat="server" ControlToValidate="afterImmersedInput2" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="afterImmersedInput3" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvA3" runat="server" ControlToValidate="afterImmersedInput3" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="afterImmersedInputAvg" runat="server" CssClass="form-control" ReadOnly="true" />
                                                <%--  <asp:RequiredFieldValidator ID="rfvA" runat="server" ControlToValidate="afterImmersedInput4" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>


                                        </tr>

                                        <!-- Row 4: Weight / Thickness of zinc coating -->
                                        <tr>
                                            <td>4</td>
                                            <td>Weight / Thickness of zinc coating</td>
                                            <td>
                                                <asp:DropDownList ID="ddlZincCoating" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="">--Select--</asp:ListItem>
                                                    <asp:ListItem Value="gr/m2">gr/m²</asp:ListItem>
                                                    <asp:ListItem Value="mm">mm</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit4" runat="server" ControlToValidate="ddlZincCoating"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="zincCoatingSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvZSpecified" runat="server" ControlToValidate="zincCoatingSpecified" ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="zincCoatingInput1" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvZ1" runat="server" ControlToValidate="zincCoatingInput1" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="zincCoatingInput2" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvZ2" runat="server" ControlToValidate="zincCoatingInput2" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="zincCoatingInput3" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvZ3" runat="server" ControlToValidate="zincCoatingInput3" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="zincCoatingInputAvg" runat="server" CssClass="form-control" readpnly="true" />
                                                <%--<asp:RequiredFieldValidator ID="rfvZ4" runat="server" ControlToValidate="zincCoatingInput4" ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />--%>
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
