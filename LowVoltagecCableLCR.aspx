<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="LowVoltagecCableLCR.aspx.cs" Inherits="LIMS.LowVoltagecCbleLCR" %>

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
                            <h2>Low Voltage Cable LCR</h2>
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
                            <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th colspan="4">ITEM TEST</th>
                                            <th>Inductance</th>
                                            <th>Maximum inductance</th>
                                            <th>Resistance</th>
                                            <th>L/R Ratio</th>
                                            <th colspan="2">Mutual capacitance</th>
                                        </tr>

                                        <tr>
                                            <th colspan="4">Unit</th>
                                            <th>
                                                <asp:DropDownList runat="server" ID="ddlunitInd" CssClass="form-control">
                                                    <asp:ListItem Text="µH" Value="µH" />
                                                    <asp:ListItem Text="mH/km" Value="mH/km" />
                                                    <asp:ListItem Text="Ω" Value="Ω" />
                                                    <asp:ListItem Text="µ H/ Ω" Value="µ H/ Ω" />
                                                    <asp:ListItem Text="nF/km" Value="nF/km" />
                                                </asp:DropDownList></th>
                                            <th>
                                                <asp:DropDownList runat="server" ID="ddlunitMax" CssClass="form-control">
                                                    <asp:ListItem Text="mH/km" Value="mH/km" />
                                                    <asp:ListItem Text="µH" Value="µH" />
                                                    <asp:ListItem Text="Ω" Value="Ω" />
                                                    <asp:ListItem Text="µ H/ Ω" Value="µ H/ Ω" />
                                                    <asp:ListItem Text="nF/km" Value="nF/km" />
                                                </asp:DropDownList></th>
                                            <th>
                                                <asp:DropDownList runat="server" ID="ddlunitResi" CssClass="form-control">

                                                    <asp:ListItem Text="Ω" Value="Ω" />
                                                    <asp:ListItem Text="µH" Value="µH" />
                                                    <asp:ListItem Text="mH/km" Value="mH/km" />
                                                    <asp:ListItem Text="µ H/ Ω" Value="µ H/ Ω" />
                                                    <asp:ListItem Text="nF/km" Value="nF/km" />
                                                </asp:DropDownList></th>
                                            <th>
                                                <asp:DropDownList runat="server" ID="ddlunitLR" CssClass="form-control">
                                                    <asp:ListItem Text="µ H/ Ω" Value="µ H/ Ω" />
                                                    <asp:ListItem Text="µH" Value="µH" />
                                                    <asp:ListItem Text="mH/km" Value="mH/km" />
                                                    <asp:ListItem Text="Ω" Value="Ω" />
                                                    <asp:ListItem Text="nF/km" Value="nF/km" />
                                                </asp:DropDownList></th>
                                            <th colspan="2">
                                                <asp:DropDownList runat="server" ID="ddlunitMutual" CssClass="form-control">
                                                    <asp:ListItem Text="nF/km" Value="nF/km" />
                                                    <asp:ListItem Text="µH" Value="µH" />
                                                    <asp:ListItem Text="mH/km" Value="mH/km" />
                                                    <asp:ListItem Text="Ω" Value="Ω" />
                                                    <asp:ListItem Text="µ H/ Ω" Value="µ H/ Ω" />
                                                </asp:DropDownList></th>
                                        </tr>

                                        <tr>
                                            <th colspan="4">Specified</th>
                                            <th>
                                                <asp:TextBox runat="server" ID="txtspecifiedinductance" CssClass="form-control" /></th>
                                            <th>
                                                <asp:TextBox runat="server" ID="txtspecifiedMaximumind" CssClass="form-control" /></th>
                                            <th>
                                                <asp:TextBox runat="server" ID="txtspecifiedResistance" CssClass="form-control" /></th>
                                            <th>
                                                <asp:TextBox runat="server" ID="txtspecifiedLR" CssClass="form-control" /></th>
                                            <th colspan="2">
                                                <asp:TextBox runat="server" ID="txtspecifiedMutual" CssClass="form-control" /></th>
                                        </tr>
                                        <tr>
                                            <th>Date</th>
                                            <th>Drum No.</th>
                                            <th>length</th>
                                            <th>Core No/Pair</th>
                                            <th colspan="5">Test Result</th>
                                            <th>Add Date</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <td rowspan="2">
                                            <asp:TextBox runat="server" ID="txtDate" CssClass="form-control" TextMode="Date" /></td>
                                        <td rowspan="2">
                                            <asp:TextBox runat="server" ID="txtdrumnNo" CssClass="form-control" /></td>
                                        <td rowspan="2">
                                            <asp:TextBox runat="server" ID="txtlength" CssClass="form-control" /></td>
                                        <td>
                                            <asp:DropDownList runat="server" ID="ddlCorePhase" CssClass="form-control" />
                                            <br />
                                            <asp:DropDownList runat="server" ID="ddlCore1" CssClass="form-control" /></td>
                                        <td rowspan="2">
                                            <asp:TextBox runat="server" ID="txtInput1" CssClass="form-control" /></td>
                                        <td rowspan="2">
                                            <asp:TextBox runat="server" ID="txtInput2" CssClass="form-control" /></td>
                                        <td rowspan="2">
                                            <asp:TextBox runat="server" ID="txtInput3" CssClass="form-control" />
                                        </t>
                                       <td rowspan="2">
                                           <asp:TextBox runat="server" ID="txtInput4" CssClass="form-control" /></td>
                                        <td rowspan="2">
                                            <asp:TextBox runat="server" ID="txtInput5" CssClass="form-control" /></td>
                                        <td style="text-align: center;">
                                            <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                        </td>
                                    </tbody>
                                </table>

                            </div>


                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Low Voltage Cable LCR</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvPressureTest" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped"
                                            OnRowDataBound="gvPressureTest_RowDataBound"
                                            OnRowCreated="gvPressureTest_RowCreated"
                                            OnRowCommand="gvPressureTest_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" />
                                                <asp:BoundField DataField="DrumNo" HeaderText="Drum No." />
                                                <asp:BoundField DataField="Length" HeaderText="Length" />
                                                <asp:TemplateField HeaderText="Core No/Pair">
                                                    <ItemTemplate>
                                                        <%# Eval("CorePhase") + "," + Eval("Core1") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:BoundField DataField="Input1" HeaderText="1" />
                                                <asp:BoundField DataField="Input2" HeaderText="2" />
                                                <asp:BoundField DataField="Input3" HeaderText="3" />
                                                <asp:BoundField DataField="Input4" HeaderText="4" />
                                                <asp:BoundField DataField="Input5" HeaderText="5" />
                                                <asp:TemplateField HeaderText="Actions">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete"
                                                            CommandName="DeleteRow" CommandArgument='<%# Container.DataItemIndex %>'
                                                            OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                            CssClass="btn btn-danger btn-sm" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                    </div>
                                </div>
                            </div>

                            <div>
                            </div>
                        </div>
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnUpdate_Click" Visible="false" />
                    </div>
                    %>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>
