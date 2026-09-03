<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="TanDeltaTest.aspx.cs" Inherits="LIMS.TanDeltaTest" %>

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
            padding: 8px;
            /*text-align: center;*/
        }

        .info-table td:first-child {
            /*background-color: #f2f2f2;*/
            width: 25%;
            font-weight: bold;
            text-align: left;
        }

        .test-table th {
            background-color: #e0e0e0;
            vertical-align: middle;
        }

        input[type="text"], input[type="number"], input[type="date"],
        .form-control {
            width: 100%;
            padding: 6px;
            border: 1px solid #ddd;
            border-radius: 3px;
            margin: 0 auto;
            display: block;
            text-align: center;
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

        .gridview {
            width: 100%;
            margin-top: 20px;
            border-collapse: collapse;
        }

            .gridview th {
                background-color: #b34788;
                color: white;
                padding: 10px;
                text-align: center;
            }

            .gridview td {
                padding: 8px;
                text-align: center;
                border: 1px solid #ddd;
            }

            .gridview tr:nth-child(even) {
                background-color: #f2f2f2;
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
                            <h2>Tan δ Test</h2>
                        </div>
                        <div class="body">
                            <!-- Table for basic sample data -->
                            <div class="table-responsive">
                                <table class="info-table">
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
                                <p><b>1. Tan δ measurement as a function of the voltage</b></p>
                                <table class="test-table">
                                    <tr>
                                        <td colspan="5"><b>Characteristic of data</b></td>
                                    </tr>
                                    <tr>
                                        <td><b>Temperature test object :</b></td>
                                        <td>
                                            <asp:TextBox ID="txtTemperatureTestObject1" runat="server" CssClass="form-control" placeholder="Temperature Test Object" />
                                            <asp:RequiredFieldValidator
                                                ID="rfvTemperatureTestObject1"
                                                runat="server"
                                                ControlToValidate="txtTemperatureTestObject1"
                                                InitialValue=""
                                                ErrorMessage="Temperature Test Object is required"
                                                ForeColor="Red"
                                                Display="Dynamic" />
                                        </td>
                                        <td>°C </td>
                                    </tr>
                                    <tr>
                                        <td><b>Length of test object :</b></td>
                                        <td>
                                            <asp:TextBox ID="txtLengthOfTestObject1" runat="server" CssClass="form-control" placeholder="Length of test object" />
                                            <asp:RequiredFieldValidator
                                                ID="rfvLengthOfTestObject1"
                                                runat="server"
                                                ControlToValidate="txtLengthOfTestObject1"
                                                InitialValue=""
                                                ErrorMessage="Length of Test Object is required"
                                                ForeColor="Red"
                                                Display="Dynamic" />
                                        </td>
                                        <td>m </td>
                                    </tr>
                                    <tr>
                                        <td><b>Standard capacitor :</b></td>
                                        <td>
                                            <asp:TextBox ID="txtStandardCapacitor1" runat="server" CssClass="form-control" placeholder="Standard capacitor" />
                                            <asp:RequiredFieldValidator
                                                ID="rfvStandardCapacitor1"
                                                runat="server"
                                                ControlToValidate="txtStandardCapacitor1"
                                                InitialValue=""
                                                ErrorMessage="Standard Capacitor is required"
                                                ForeColor="Red"
                                                Display="Dynamic" />
                                        </td>
                                        <td>pF </td>
                                    </tr>
                                    <tr>
                                        <th>Core</th>
                                        <th>Voltage test<br />
                                            50 Hz<br />
                                            kV</th>
                                        <th>Capacitance</th>
                                        <th>Tan δ</th>
                                        <th>Action</th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="ddlGetCore" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        <td>
                                            <asp:TextBox ID="txtVoltageTest50HzkV" runat="server" CssClass="form-control" placeholder="kV" /></td>
                                        <td>
                                            <table style="width: 100%; border: none;">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtCapacitanceMeasurednF" runat="server" CssClass="form-control" placeholder="Measured nF" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtCapacitanceResultµF" runat="server" CssClass="form-control" placeholder="Result µF" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <table style="width: 100%; border: none;">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtTanδMeasured" runat="server" CssClass="form-control" placeholder="Measured" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtTanδResult" runat="server" CssClass="form-control" placeholder="Result" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtTanδStandard" runat="server" CssClass="form-control" placeholder="Standard" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="ADD" OnClick="btnAdd_Click" CausesValidation="true" /></td>
                                    </tr>
                                </table>
                            </div>
                            <hr />

                            <asp:GridView ID="gvTests" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvTests_RowDeleting" CssClass="gridview">
                                <Columns>
                                    <asp:TemplateField HeaderText="No.">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Core" HeaderText="Core" />
                                    <asp:BoundField DataField="VoltageTest" HeaderText="Voltage Test (kV)" />
                                    <asp:BoundField DataField="CapacitanceMeasured" HeaderText="Capacitance Measured (nF)" />
                                    <asp:BoundField DataField="CapacitanceResult" HeaderText="Capacitance Result (µF)" />
                                    <asp:BoundField DataField="TanDeltaMeasured" HeaderText="Tan δ Measured" />
                                    <asp:BoundField DataField="TanDeltaResult" HeaderText="Tan δ Result (.......X 10⁻⁴)" />
                                    <asp:BoundField DataField="TanDeltaStandard" HeaderText="Tan δ Standard (.......X 10⁻⁴)" />
                                    <asp:CommandField ShowDeleteButton="True" DeleteText="Remove" />
                                </Columns>
                            </asp:GridView>


                            <hr />
                            <div class="table-responsive" style="margin-top: 100px;">
                                <p><b>2. Tan δ measurement as a function of the voltage - Second Test</b></p>
                                <table class="test-table">
                                    <tr>
                                        <td colspan="5"><b>Characteristic of data</b></td>
                                    </tr>
                                    <tr>
                                        <td><b>Temperature test object :</b></td>
                                        <td>
                                            <asp:TextBox ID="txtTemperatureTestObject2" runat="server" CssClass="form-control" placeholder="Temperature Test Object" />
                                            <asp:RequiredFieldValidator
                                                ID="rfvTemperatureTestObject2"
                                                runat="server"
                                                ControlToValidate="txtTemperatureTestObject2"
                                                InitialValue=""
                                                ErrorMessage="Temperature Test Object (2) is required"
                                                ForeColor="Red"
                                                Display="Dynamic" />
                                        </td>
                                        <td>°C </td>
                                    </tr>
                                    <tr>
                                        <td><b>Length of test object :</b></td>
                                        <td>
                                            <asp:TextBox ID="txtLengthOfTestObject2" runat="server" CssClass="form-control" placeholder="Length of test object" />
                                            <asp:RequiredFieldValidator
                                                ID="rfvLengthOfTestObject2"
                                                runat="server"
                                                ControlToValidate="txtLengthOfTestObject2"
                                                InitialValue=""
                                                ErrorMessage="Length of Test Object (2) is required"
                                                ForeColor="Red"
                                                Display="Dynamic" />
                                        </td>
                                        <td>m </td>
                                    </tr>
                                    <tr>
                                        <td><b>Standard capacitor :</b></td>
                                        <td>
                                            <asp:TextBox ID="txtStandardCapacitor2" runat="server" CssClass="form-control" placeholder="Standard capacitor" />
                                            <asp:RequiredFieldValidator
                                                ID="rfvStandardCapacitor2"
                                                runat="server"
                                                ControlToValidate="txtStandardCapacitor2"
                                                InitialValue=""
                                                ErrorMessage="Standard Capacitor (2) is required"
                                                ForeColor="Red"
                                                Display="Dynamic" />
                                        </td>
                                        <td>pF </td>
                                    </tr>
                                    <tr>
                                        <th>Core</th>
                                        <th>Voltage test<br />
                                            50 Hz<br />
                                            kV</th>
                                        <th>Capacitance</th>
                                        <th>Tan δ</th>
                                        <th>Action</th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="ddlTempType" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="a. Ambient temperature" Value="AmbientTemperature" />
                                                <asp:ListItem Text="b. Rated temperature (90°C)" Value="RatedTemperature(90°C)" />
                                            </asp:DropDownList>
                                            <br />
                                            <asp:RequiredFieldValidator ID="rfvUnit1_4" runat="server"
                                                ControlToValidate="ddlTempType" InitialValue=""
                                                ErrorMessage="Required" ForeColor="Red" Display="Dynamic" />

                                            <asp:DropDownList ID="ddlGetCore2" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        <td>
                                            <asp:TextBox ID="txtVoltageTest50HzkV2" runat="server" CssClass="form-control" placeholder="kV" /></td>
                                        <td>
                                            <table style="width: 100%; border: none;">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtCapacitanceMeasurednF2" runat="server" CssClass="form-control" placeholder="Measured nF" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtCapacitanceResultµF2" runat="server" CssClass="form-control" placeholder="Result µF" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <table style="width: 100%; border: none;">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtTanδMeasured2" runat="server" CssClass="form-control" placeholder="Measured" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtTanδResult2" runat="server" CssClass="form-control" placeholder="Result" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtTanδStandard2" runat="server" CssClass="form-control" placeholder="Standard" /></td>
                                                </tr>
                                            </table>
                                        </td>

                                        <td>
                                            <asp:Button ID="btnAdd2" runat="server" CssClass="btn btn-primary" Text="ADD" OnClick="btnAdd2_Click" CausesValidation="true" />
                                        </td>

                                    </tr>
                                </table>
                            </div>
                            <hr />

                            <asp:GridView ID="gvTests2" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvTests2_RowDeleting" CssClass="gridview">
                                <Columns>
                                    <asp:TemplateField HeaderText="No.">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TemperatureType" HeaderText="Temperature Type" />
                                    <asp:BoundField DataField="Core" HeaderText="Core" />
                                    <asp:BoundField DataField="VoltageTest" HeaderText="Voltage Test (kV)" />
                                    <asp:BoundField DataField="CapacitanceMeasured" HeaderText="Capacitance Measured (nF)" />
                                    <asp:BoundField DataField="CapacitanceResult" HeaderText="Capacitance Result (µF)" />
                                    <asp:BoundField DataField="TanDeltaMeasured" HeaderText="Tan δ Measured" />
                                    <asp:BoundField DataField="TanDeltaResult" HeaderText="Tan δ Result (.......X 10⁻⁴)" />
                                    <asp:BoundField DataField="TanDeltaStandard" HeaderText="Tan δ Standard (.......X 10⁻⁴)" />
                                    <asp:CommandField ShowDeleteButton="True" DeleteText="Remove" />
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
