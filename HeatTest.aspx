<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="HeatTest.aspx.cs" Inherits="LIMS.HeatTest" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


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
            padding: 8px;
            text-align: center;
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

        input[type="text"], input[type="number"], input[type="date"] {
            width: 90%;
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
                            <h2>HEAT SHOCK TEST </h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" ReadOnly />
                                            <asp:RequiredFieldValidator ID="rfvCableType" runat="server"
                                                ControlToValidate="cableType"
                                                ErrorMessage="Type is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" ReadOnly />
                                            <asp:RequiredFieldValidator ID="rfvRefSpec" runat="server"
                                                ControlToValidate="refSpec"
                                                ErrorMessage="Ref. Spec. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" ReadOnly />
                                            <asp:RequiredFieldValidator ID="rfvCableSize" runat="server"
                                                ControlToValidate="cableSize"
                                                ErrorMessage="Size is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" ReadOnly />
                                            <asp:RequiredFieldValidator ID="rfvTestMethodNo" runat="server"
                                                ControlToValidate="testMethodNo"
                                                ErrorMessage="Test Method No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Drum No.</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" ReadOnly />
                                            <asp:RequiredFieldValidator ID="rfvVoltage" runat="server"
                                                ControlToValidate="voltage"
                                                ErrorMessage="drum number is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" ReadOnly />
                                            <asp:RequiredFieldValidator ID="rfvJobOrderNo" runat="server"
                                                ControlToValidate="jobOrderNo"
                                                ErrorMessage="Job Order No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" TextMode="Date"/>
                                            <asp:RequiredFieldValidator ID="rfvDate" runat="server"
                                                ControlToValidate="Date"
                                                ErrorMessage="Date is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" ReadOnly />
                                            <asp:RequiredFieldValidator ID="rfvWorksheetNo" runat="server"
                                                ControlToValidate="worksheetno"
                                                ErrorMessage="Worksheet No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" ReadOnly />
                                            <asp:RequiredFieldValidator ID="rfvRoomNo" runat="server"
                                                ControlToValidate="roomno"
                                                ErrorMessage="Room No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvRoomHumidity" runat="server"
                                                ControlToValidate="roomhumidity"
                                                ErrorMessage="Room Humidity is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control"  />
                                            <asp:RequiredFieldValidator ID="rfvRoomTemp" runat="server"
                                                ControlToValidate="roomtemp"
                                                ErrorMessage="Room Temperature is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" ReadOnly />
                                            <asp:RequiredFieldValidator ID="rfvEquipmentNo" runat="server"
                                                ControlToValidate="equipmentno"
                                                ErrorMessage="Equipment No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <br />

                            <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th colspan="1"><b style="font-size: 14px; text-align: left;">Insulation / Sheath : </b></th>
                                            <th colspan="9">
                                                <asp:TextBox runat="server" ID="txtitemtype" CssClass="form-control" Style="width: 25%"></asp:TextBox>
                                            </th>
                                        </tr>

                                        <tr>
                                            <th rowspan="2">Core</th>
                                            <th rowspan="2">Specimen No.</th>
                                            <th rowspan="2">Diameter<br>
                                                (mm)</th>
                                            <th rowspan="2">Thickness<br>
                                                (mm)</th>

                                            <th colspan="2">Bending</th>

                                            <th rowspan="2">Temp.<br>
                                                (°C)</th>
                                            <th rowspan="2">Time<br>
                                                (hour)</th>

                                            <th rowspan="2">Result (%)</th>
                                            <th rowspan="2">Add Data</th>
                                        </tr>
                                        <tr>
                                            <th>Diameter of mandrel</th>
                                            <th>Total spiral</th>
                                        </tr>
                                    </thead>


                                    <tbody>
                                        <tr>
                                            <td style="width: 11%">

                                                <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCorePhase_SelectedIndexChanged" Style="text-align: center;">
                                                    <%--<asp:ListItem Text="Tape Red" Value="Red" />
                                                    <asp:ListItem Text="Tape Yellow" Value="Yellow" />
                                                    <asp:ListItem Text="Tape Blue" Value="Blue" />--%>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenNo" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtDiameter" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtThickness" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtBeforeheating" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtafterheating" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtTemperature" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtresult" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td style="text-align: center;">
                                                <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>

                            <!-- Strippability Test Results Grid -->

                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Shrinkage Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvShrinkageTest" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gvShrinkageTest_RowDataBound" OnRowCommand="gvShrinkageTest_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="CorePhase" HeaderText="Core" />
                                                <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No." />

                                                <asp:BoundField DataField="Diameter" HeaderText="Diameter (mm)" />
                                                <asp:BoundField DataField="Thickness" HeaderText="Thickness (mm)" />

                                                <asp:BoundField DataField="Beforeheating" HeaderText="Diameter of mandrel" />
                                                <asp:BoundField DataField="afterheating" HeaderText="Total spiral" />

                                                <asp:BoundField DataField="Temperature" HeaderText="Temperature (°C)" />
                                                <asp:BoundField DataField="Duration" HeaderText="Duration (Hours)" />
                                                <asp:BoundField DataField="result" HeaderText="Result (%)" />
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


                            <!-- Standard Row - Horizontal Layout -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Standard Values</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>Specified</th>
                                                    <th>Temperature (°C)</th>
                                                    <th>Duration (Hours)</th>
                                                    <th>Diameter of mandrel</th>
                                                    <th>Total spiral</th>
                                                    <th>Result (%)</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                                <tr>
                                                    <td>Standard</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificTemperature" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificDuration" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificBeforeHeating" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificAfterHeating" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificResult" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                            </tbody>

                                        </table>
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
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>
