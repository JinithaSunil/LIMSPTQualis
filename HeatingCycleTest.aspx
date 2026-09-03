<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="HeatingCycleTest.aspx.cs" Inherits="LIMS.HeatingCycleTest" %>


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
                            <h2>HEATING CYCLE Test</h2>
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
                                        <td colspan="6" style="text-align: center;">Heating cycle applied to conductor
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align: center;">Start </td>
                                        <td style="text-align: center;">Reached
                                            <br />
                                            95⁰-100⁰</td>
                                        <td style="text-align: center;">Actual
                                            <br />
                                            Temp.</td>
                                        <td style="text-align: center;">Cooling</td>
                                        <td style="text-align: center;"></td>
                                    </tr>

                                    <tr>
                                        <td style="text-align: center;">Date</td>
                                        <td style="text-align: center;">Time</td>
                                        <td style="text-align: center;">Time</td>
                                        <td style="text-align: center;">⁰C</td>
                                        <td style="text-align: center;">Time</td>
                                    </tr>

                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" placeholder="Test ITem" TextMode="Date" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                ControlToValidate="txtStartDate" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>


                                        <td>
                                            <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control" TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                ControlToValidate="txtStartTime" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtReachedTime" runat="server" CssClass="form-control" TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                                ControlToValidate="txtReachedTime" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtActualTemp" runat="server" CssClass="form-control" TextMode="Number" />
                                            <asp:RequiredFieldValidator ID="rfvMeasured1" runat="server"
                                                ControlToValidate="txtActualTemp" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtCooling" runat="server" CssClass="form-control" TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                                                ControlToValidate="txtCooling" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
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
                                    <asp:TemplateField HeaderText="Cycle">
                                        <ItemTemplate>
                                            <%# (gvTests.PageIndex * gvTests.PageSize) + Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" />
                                    <asp:BoundField DataField="StartTime" HeaderText="Start Time" />
                                    <asp:BoundField DataField="ReachedTime" HeaderText="Reached Time" />
                                    <asp:BoundField DataField="ActualTemp" HeaderText="Actual Temp (°C)" />
                                    <asp:BoundField DataField="CoolingTime" HeaderText="Cooling Time" />
                                    <asp:CommandField ShowDeleteButton="True" />
                                </Columns>
                            </asp:GridView>


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


