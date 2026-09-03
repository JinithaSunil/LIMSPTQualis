<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="PartialUBendingTest.aspx.cs" Inherits="LIMS.PartialUBendingTest" %>

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
            text-align: left;
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
                            <h2>Partial U Bending HV Test</h2>
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

                            <hr />

                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtItemTest" runat="server" CssClass="form-control" placeholder="Test ITem" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                ControlToValidate="txtItemTest" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:DropDownList ID="ddlUnit1" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="m" Value="m" />
                                                <asp:ListItem Text="mm" Value="mm" Selected="True" />
                                                <asp:ListItem Text="-" Value="-" />
                                                <asp:ListItem Text="max" Value="max" />
                                                <asp:ListItem Text="min" Value="min" />
                                                <asp:ListItem Text="pcs" Value="pcs" />
                                                <asp:ListItem Text="mnt" Value="mnt" />
                                                <asp:ListItem Text="I/mnt" Value="I/mnt" />
                                                <asp:ListItem Text="°C/mnt" Value="°C/mnt" />
                                                <asp:ListItem Text="°C" Value="°C" />
                                                <asp:ListItem Text="gr" Value="gr" />
                                                <asp:ListItem Text="%" Value="%" />
                                                <asp:ListItem Text="A" Value="A" />
                                                <asp:ListItem Text="V" Value="V" />
                                                <asp:ListItem Text="kV" Value="kV" />
                                                <asp:ListItem Text="Nom" Value="Nom" />
                                                <asp:ListItem Text="liter" Value="liter" />
                                                <asp:ListItem Text="......kV/......h" Value="......kV/......h" />
                                            </asp:DropDownList>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtSpecifiedValue1" runat="server" CssClass="form-control" placeholder="Specified Value" />
                                            <asp:RequiredFieldValidator ID="rfvSpecifiedValue1" runat="server"
                                                ControlToValidate="txtSpecifiedValue1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtMeasured1" runat="server" CssClass="form-control" placeholder="Measured Value" />
                                            <asp:RequiredFieldValidator ID="rfvMeasured1" runat="server"
                                                ControlToValidate="txtMeasured1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary" Text="ADD" OnClick="btnAdd_Click" CausesValidation="true" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <hr />

                            <asp:GridView ID="gvTests" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvTests_RowDeleting">
                                <Columns>
                                    <asp:TemplateField HeaderText="No.">
                                        <ItemTemplate>
                                            <%# (gvTests.PageIndex * gvTests.PageSize) + Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ItemTest" HeaderText="Test Item" />
                                    <asp:BoundField DataField="Unit" HeaderText="Unit" />
                                    <asp:BoundField DataField="SpecifiedValue" HeaderText="Specified Value" />
                                    <asp:BoundField DataField="MeasuredValue" HeaderText="Measured Value" />
                                    <asp:CommandField ShowDeleteButton="True" />
                                </Columns>
                            </asp:GridView>

                            <%--<asp:Button ID="btnSave" runat="server" CssClass="btn btn-success" Text="Save All" OnClick="btnSave_Click" CausesValidation="false" />--%>

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" CausesValidation="false" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" CausesValidation="false" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" CausesValidation="false" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>



