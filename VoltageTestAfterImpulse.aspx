<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="VoltageTestAfterImpulse.aspx.cs" Inherits="LIMS.VoltageTestAfterImpulse" %>


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

        .vertical-textbox {
            display: flex;
            align-items: center;
            justify-content: center;
            text-align: center;
            resize: none; /* optional: removes the resize handle */
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
                            <h2>VOLTAGE TEST AFTER IMPULSE</h2>
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
                                    <td><b>Voltage Test (kV)</b></td>
                                    <td>
                                        <asp:TextBox ID="txtVoltagetest" Width="100%" runat="server" CssClass="form-control vertical-textbox" /></td>
                                </tr>
                                <tr>
                                    <td><b>Duration (min)</b></td>
                                    <td>
                                        <asp:TextBox ID="txtDuration" Width="100%" runat="server" CssClass="form-control vertical-textbox" /></td>
                                </tr>
                                <tr>
                                    <td><b>Frequency (Hz)</b></td>
                                    <td>
                                        <asp:TextBox ID="txtFrequency" Width="100%" runat="server" CssClass="form-control vertical-textbox" /></td>
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
                                                    <th style="width: 25%; vertical-align: middle;" rowspan="2">Item Test</th>
                                                    <th style="vertical-align: middle;" rowspan="2">Phase</th>
                                                    <th style="vertical-align: middle;">Duration</th>
                                                    <th style="vertical-align: middle;">Current</th>
                                                    <th style="width: 19%; vertical-align: middle;" rowspan="2">Measured</th>
                                                    <th style="width: 7%; vertical-align: middle;" rowspan="2">Add Data</th>
                                                </tr>
                                                <tr>
                                                    <th>(minutes)</th>
                                                    <th>(mA)</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <!-- Input Fields Row -->
                                                <tr>
                                                    <td style="word-wrap: break-word; white-space: normal;">
                                                        <asp:Label
                                                            ID="lblItemTestVoltageApplied"
                                                            runat="server"
                                                            Text="Voltage to be applied to core to core and Earth connected to metal screen"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                            <asp:ListItem Text="Red" Value="Red" />
                                                            <asp:ListItem Text="Blue" Value="Blue" />
                                                            <asp:ListItem Text="Yellow" Value="Yellow" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtdurationmins" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtcurrent" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlmeasured" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                            <asp:ListItem Text="Pass" Value="Passed" Selected="True" />
                                                            <asp:ListItem Text="Fail" Value="Failed" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="text-align: center; vertical-align: middle;">
                                                        <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                                    </td>
                                                </tr>

                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>


                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Requirement</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <!-- Header Row -->
                                                <tr>
                                                    <th style="width: 25%; vertical-align: middle;">Item Test</th>
                                                    <th style="vertical-align: middle;">Phase Required</th>
                                                    <th style="vertical-align: middle;">Duration Required</th>
                                                    <th style="vertical-align: middle;">Current Required</th>
                                                    <th style="width: 19%; vertical-align: middle;">Measured Required</th>
                                                    <th style="width: 7%; vertical-align: middle;">Add Requirement</th>
                                                </tr>

                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td style="word-wrap: break-word; white-space: normal;">
                                                        <asp:Label
                                                            ID="lblrequirement"
                                                            runat="server"
                                                            Text="Requirement"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtphaserequired" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtdurationrequired" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtcurrentrequired" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlmeasuredrequired" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                            <asp:ListItem Text="No Breakdown of Insulation shall occur" Value="No Breakdown of Insulation shall occur" Selected="True" />
                                                            <asp:ListItem Text="-" Value="-" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="text-align: center; vertical-align: middle;">
                                                        <asp:Button ID="btnaddrequirement" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnaddrequirement_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Voltage test after Impulse Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvVoltageafterimpulse" runat="server" AutoGenerateColumns="false"
                                            OnRowCommand="gvVoltageafterimpulse_RowCommand"
                                            CssClass="table table-bordered table-striped">
                                            <Columns>
                                                <asp:BoundField DataField="ItemTest" HeaderText="Item Test" ItemStyle-Width="25%" />
                                                <asp:TemplateField HeaderText="Phase" ItemStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPhase" runat="server" Text='<%# Eval("CorePhase") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Duration" HeaderText="Duration (mins)" ItemStyle-Width="8%" />
                                                <asp:BoundField DataField="Current" HeaderText="Current (mA)" ItemStyle-Width="8%" />
                                                <asp:BoundField DataField="Measured" HeaderText="Measured" ItemStyle-Width="10%" />
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="5%">
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

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>
