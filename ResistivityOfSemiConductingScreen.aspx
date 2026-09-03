<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="ResistivityOfSemiConductingScreen.aspx.cs" Inherits="LIMS.ResistivityOfSemiConductingScreen" %>

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
                            <h2>RESISTIVITY OF SEMI-CONDUCTOR SCREEN</h2>
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

                            <!-- Input Form for Test Data - Horizontal Layout -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Test Details</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <!-- Header Row -->
                                                <tr>
                                                    <th>Test Category</th>
                                                    <th>Test Type</th>
                                                    <th>Core Phase</th>
                                                    <th>Diameter</th>
                                                    <th>Thickness</th>
                                                    <th>VolumeResistivity</th>
                                                    <th style="width: 7%;">Add Data</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <!-- Input Fields Row -->
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="ddlTestCategory" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                            <asp:ListItem Text="Insulation Screen" Value="Insulation Screen" />
                                                            <asp:ListItem Text="Conductor Screen" Value="Conductor Screen" />
                                                            <%-- <asp:ListItem Text="Partial discharge after heating cycle 20 times" Value="Partial discharge after heating cycle 20 times" />--%>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlTestType" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                            <asp:ListItem Text="Before ageing" Value="Before ageing" />
                                                            <asp:ListItem Text="After ageing" Value="After ageing" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                            <asp:ListItem Text="Red" Value="Red" />
                                                            <asp:ListItem Text="Blue" Value="Blue" />
                                                            <asp:ListItem Text="Yellow" Value="Yellow" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDiameter" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtThickness" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtVolumeResistivity" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <!-- Strippability Test Results Grid -->
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Resisitivity Semi-Conducting Screen Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvSemiConductingTest" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gvSemiConductingTest_RowDataBound" OnRowCommand="gvSemiConductingTest_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="ItemCategory" HeaderText="Item Category" />
                                                <asp:BoundField DataField="ItemTest" HeaderText="Item Test" />
                                                <asp:BoundField DataField="CorePhase" HeaderText="Core Phase" />
                                                <asp:BoundField DataField="Diameter" HeaderText="Diameter" />
                                                <asp:BoundField DataField="Thickness" HeaderText="Thickness" />
                                                <asp:BoundField DataField="VolumeResistivity" HeaderText="Volume Resistivity" />

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
                                    <h4>Standard before & after ageing</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="width: 30%;">Standard before & ageing for insulation screen</th>
                                                    <td>
                                                        <asp:TextBox ID="txtStandardmaxVolumeResitstivity" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>

                                                </tr>
                                                <th style="width: 30%;">Standard before & ageing for conductor screen</th>
                                                <td>
                                                    <asp:TextBox ID="txtStandardmaxVolumeResitstivity_conductor" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                </td>
                                            </thead>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnUpdate_Click" Visible="false" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
