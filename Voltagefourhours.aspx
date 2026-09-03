<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="Voltagefourhours.aspx.cs" Inherits="LIMS.Voltagefourhours" %>

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
                            <h2>VOLTAGE 4 HOURS</h2>
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
                                    <td><b>Duration (Hours)</b></td>
                                    <td>
                                        <asp:TextBox ID="txtDuration" Width="100%" runat="server" CssClass="form-control vertical-textbox" /></td>
                                </tr>
                                <tr>
                                    <td><b>Frequency (Hz)</b></td>
                                    <td>
                                        <asp:TextBox ID="txtFrequency" Width="100%" runat="server" CssClass="form-control vertical-textbox" /></td>
                                </tr>
                            </table>


                            <div class="row mt-4">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead class="thead-light">
                                                <tr>
                                                    <th style="vertical-align: middle; width: 15%;">Item test</th>
                                                    <th colspan="8" style="text-align: center; background-color: #f2f2f2;">Voltage to applied to core to core<br>
                                                        and<br>
                                                        Earth connected to metal screen
                                                    </th>
                                                    <th style="vertical-align: middle; width: 15%;">Requirement</th>
                                                </tr>

                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td><b>Duration (mins)</b></td>
                                                    <td>
                                                        <asp:TextBox ID="txtDur30" runat="server"  CssClass="form-control vertical-textbox"  Text="30" Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDur60" runat="server"  CssClass="form-control vertical-textbox"  Text="60" Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDur90" runat="server"  CssClass="form-control vertical-textbox"  Text="90" Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDur120" runat="server"  CssClass="form-control vertical-textbox"  Text="120" Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDur150" runat="server"  CssClass="form-control vertical-textbox"  Text="150" Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDur180" runat="server"  CssClass="form-control vertical-textbox"  Text="180" Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDur210" runat="server"  CssClass="form-control vertical-textbox"  Text="210" Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDur240" runat="server"  CssClass="form-control vertical-textbox"  Text="240" Width="100%" />
                                                    </td>
                                                    <td rowspan="2" style="vertical-align: middle;">
                                                        <asp:TextBox ID="txtrequirementDurcurrent" runat="server"  CssClass="form-control vertical-textbox"  Text="-" Height="100px" Width="100%" />
                                                    </td>

                                                </tr>
                                                <tr>
                                                    <td><b>Current (mA)</b></td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurr30" runat="server"  CssClass="form-control vertical-textbox"   Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurr60" runat="server"  CssClass="form-control vertical-textbox"   Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurr90" runat="server"  CssClass="form-control vertical-textbox"   Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurr120" runat="server"  CssClass="form-control vertical-textbox"   Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurr150" runat="server"  CssClass="form-control vertical-textbox"   Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurr180" runat="server"  CssClass="form-control vertical-textbox"   Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurr210" runat="server"  CssClass="form-control vertical-textbox"   Width="100%" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurr240" runat="server"  CssClass="form-control vertical-textbox"   Width="100%" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td><b>Measured</b></td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlMeasured30" runat="server" CssClass="form-control vertical-textbox" Width="100%">
                                                            <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                            <asp:ListItem Text="FAIL" Value="FAIL" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlMeasured60" runat="server" CssClass="form-control vertical-textbox" Width="100%">
                                                            <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                            <asp:ListItem Text="FAIL" Value="FAIL" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlMeasured90" runat="server" CssClass="form-control vertical-textbox" Width="100%">
                                                            <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                            <asp:ListItem Text="FAIL" Value="FAIL" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlMeasured120" runat="server" CssClass="form-control vertical-textbox" Width="100%">
                                                            <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                            <asp:ListItem Text="FAIL" Value="FAIL" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlMeasured150" runat="server" CssClass="form-control vertical-textbox" Width="100%">
                                                            <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                            <asp:ListItem Text="FAIL" Value="FAIL" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlMeasured180" runat="server" CssClass="form-control vertical-textbox" Width="100%">
                                                            <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                            <asp:ListItem Text="FAIL" Value="FAIL" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlMeasured210" runat="server" CssClass="form-control vertical-textbox" Width="100%">
                                                            <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                            <asp:ListItem Text="FAIL" Value="FAIL" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlMeasured240" runat="server" CssClass="form-control vertical-textbox" Width="100%">
                                                            <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                            <asp:ListItem Text="FAIL" Value="FAIL" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="vertical-align: middle;">
                                                        <asp:TextBox ID="txtRequirement" runat="server" CssClass="form-control vertical-textbox"  Height="60px" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false"  OnClick="btnUpdate_Click"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>
