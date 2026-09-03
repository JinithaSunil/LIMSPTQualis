<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="PartialDischarge.aspx.cs" Inherits="LIMS.PartialDischarge" %>

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
    </style>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>PARTIAL DISCHARGE</h2>
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
                                            <asp:Label ID="lblvoltage" runat="server" CssClass="form-control" /></td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:Label ID="jobOrderNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" type="date" Style="text-align: center;" /></td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:Label ID="worksheetno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" /></td>
                                        <td><b>Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                        </td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:Label ID="equipmentno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                            </div>
                            <br />
                            <h4>Characteristics of Data</h4>
                            <table>

                                <tr>
                                    <td style="width: 241.37px;"><b>Circuit :</b></td>
                                    <td>
                                        <asp:DropDownList ID="ddlcircuit" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                            <asp:ListItem Text="AC" Value="AC" />
                                            <asp:ListItem Text="DC" Value="DC" />
                                        </asp:DropDownList>
                                    <td><b>Band width (kHz)</b></td>
                                    <td>
                                        <asp:TextBox ID="txtbandwidth" runat="server" CssClass="form-control" Style="text-align: center" /></td>
                                </tr>
                                <tr>
                                    <td><b>Calibration (pc)</b></td>
                                    <td>
                                        <asp:TextBox ID="txtcalibration" runat="server" CssClass="form-control" Style="text-align: center" /></td>
                                    <td><b>Test Frequency (Hz)</b></td>
                                    <td>
                                        <asp:TextBox ID="txttestfrequency" runat="server" CssClass="form-control" Style="text-align: center" /></td>
                                </tr>
                                <tr>
                                    <td><b>Noise (pc)</b></td>
                                    <td>
                                        <asp:TextBox ID="txtnoise" runat="server" CssClass="form-control" Style="text-align: center" /></td>
                                    <td><b>Coupling capactor (pF)</b></td>
                                    <td>
                                        <asp:TextBox ID="txtcouplingcap" runat="server" CssClass="form-control" Style="text-align: center" /></td>
                                </tr>

                            </table>

                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Test Details</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <!-- Header Row -->
                                                <tr>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Date</th>
                                                    <th style="width: 15%; vertical-align: MIDDLE;" rowspan="2">Item Test</th>
                                                    <th style="vertical-align: MIDDLE;" rowspan="2">Core</th>
                                                    <th style="vertical-align: MIDDLE;" colspan="2">Voltage Test 50Hz</th>
                                                    <th style="vertical-align: MIDDLE;">Duration</th>
                                                    <th style="width: 15%; vertical-align: MIDDLE;">Maximum Partial Discharge Level at Uo/kV</th>
                                                    <th style="width: 7%; vertical-align: MIDDLE;" rowspan="2">Add Data</th>
                                                </tr>
                                                <tr>
                                                    <th>(........Uo)</th>
                                                    <th>(kV)</th>
                                                    <th>(s)</th>
                                                    <th>(pc)</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <!-- Input Fields Row -->
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="testdate" runat="server" CssClass="form-control" type="date" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlitemtest" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                            <asp:ListItem Text="Partial discharge after bending test 3 cycle diameter cylinder 120cm" Value="Partial discharge after bending test 3 cycle diameter cylinder 120cm" />
                                                            <asp:ListItem Text="Partial discharge after heating cycle 20 times" Value="Partial discharge after heating cycle 20 times" />
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
                                                        <asp:TextBox ID="txtvoltagetestU0" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtvoltagetestkV" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtduration" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtmaximumpd" runat="server" CssClass="form-control" Style="text-align: center;" />
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

                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Partial Discharge Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvPartialdischarge" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gvPartialdischarge_RowDataBound" OnRowCommand="gvPartialdischarge_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" ItemStyle-Width="85px" />
                                                <asp:BoundField DataField="ItemTest" HeaderText="Item test" ItemStyle-Width="17%" />
                                                <asp:BoundField DataField="CorePhase" HeaderText="Core" ItemStyle-Width="100px" />
                                                <asp:TemplateField HeaderText="Voltage test 50Hz" ItemStyle-Width="150px" HeaderStyle-Width="150px">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center;">
                                                            Voltage test<br />
                                                            50Hz
                                                        </div>
                                                        <div style="display: flex; justify-content: space-between; border-top: 1px solid #ddd; padding-top: 4px; margin-top: 4px;">
                                                            <div style="width: 50%; text-align: center; border-right: 1px solid #ddd; padding-right: 5px;">(.......Uo)</div>
                                                            <div style="width: 50%; text-align: center; padding-left: 5px;">(kV)</div>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div style="display: flex; justify-content: space-between;">
                                                            <div style="width: 50%; text-align: center; border-right: 1px solid #ddd; padding-right: 5px;"><%# Eval("volatagetest1") %></div>
                                                            <div style="width: 50%; text-align: center; padding-left: 5px;"><%# Eval("volatagetest2") %></div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="duration" HeaderText="Duration s" ItemStyle-Width="120px" />
                                                <asp:TemplateField HeaderText="Maximum PD Level" ItemStyle-Width="150px">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center;">
                                                            Maximum Partial discharge<br />
                                                            Level at Uo/kV
                                                        </div>
                                                        <div style="display: flex; justify-content: space-around;">
                                                            <div>(pc)</div>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div style="text-align: center;"><%# Eval("maxpd") %></div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="70px">
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
                                    <h4>Standard Value</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th style="width: 30%;">Maximum Partial Discharge(pc)</th>
                                                    <td>
                                                        <asp:TextBox ID="txtStandardmaxpd" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                            </thead>
                                        </table>
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
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>