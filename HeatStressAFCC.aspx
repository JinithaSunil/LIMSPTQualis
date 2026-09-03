<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="HeatStressAFCC.aspx.cs" Inherits="LIMS.TestRecordHeatStressAFCC" %>


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
                            <h2>HEAT STRESS AFCC</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td style="text-align: left"><b>Type</b></td>
                                        <td>
                                            <asp:Label ID="cableType" runat="server" CssClass="form-control" /></td>
                                        <td style="text-align: left"><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:Label ID="refSpec" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left"><b>Diameter</b></td>
                                        <td>
                                            <asp:Label ID="cableSize" runat="server" CssClass="form-control" /></td>
                                        <td style="text-align: left"><b>Test Method No.</b></td>
                                        <td>
                                            <asp:Label ID="testMethodNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left"><b>Voltage</b></td>
                                        <td>
                                            <asp:Label ID="lblvoltage" runat="server" CssClass="form-control" /></td>
                                        <td style="text-align: left"><b>Job Order No.</b></td>
                                        <td style="text-align: left">
                                            <asp:Label ID="jobOrderNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left"><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" type="date" Style="text-align: center;" /></td>
                                        <td style="text-align: left"><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:Label ID="worksheetno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left"><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" /></td>
                                        <td style="text-align: left"><b>Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left"><b>Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                        </td>
                                        <td style="text-align: left"><b>Equipment No.</b></td>
                                        <td>
                                            <asp:Label ID="equipmentno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                            </div>


                            <br />




                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">

                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <!-- Header Row -->
                                                <tr>
                                                    <th style="width: 5%; vertical-align: middle;">No.</th>
                                                    <th style="width: 30%; vertical-align: middle;">Description test</th>
                                                    <th style="width: 20%; vertical-align: middle;">unit</th>
                                                    <th style="width: 20; vertical-align: middle;">Specified</th>
                                                    <th style="width: 20%; vertical-align: middle;">Value test</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <!-- Input Fields Row -->
                                                <tr>
                                                    <td>1</td>
                                                    <td style="word-wrap: break-word; white-space: normal; text-align: left">
                                                        <asp:Label
                                                            ID="lbl1"
                                                            runat="server"
                                                            Text="Length of sample"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUOM1" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecified1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtValue1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>2</td>
                                                    <td style="word-wrap: break-word; white-space: normal; text-align: left">
                                                        <asp:Label
                                                            ID="lbl2"
                                                            runat="server"
                                                            Text="Actual Diameter of core"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUOM2" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecified2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtValue2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>3</td>
                                                    <td style="word-wrap: break-word; white-space: normal; text-align: left">
                                                        <asp:Label
                                                            ID="lbl3"
                                                            runat="server"
                                                            Text="Actual Cross sectional area"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUOM3" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecified3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtValue3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>4</td>
                                                    <td style="word-wrap: break-word; white-space: normal; text-align: left">
                                                        <asp:Label
                                                            ID="lbl4"
                                                            runat="server"
                                                            Text="Temperature of Tg –Test"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUOM4" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecified4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtValue4" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>5</td>
                                                    <td style="word-wrap: break-word; white-space: normal; text-align: left">
                                                        <asp:Label
                                                            ID="lbl5"
                                                            runat="server"
                                                            Text="Tension of 25% of Min. Ultimate Tensile Strength"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUOM5" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecified5" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtValue5" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>6</td>
                                                    <td style="word-wrap: break-word; white-space: normal; text-align: left">
                                                        <asp:Label
                                                            ID="lbl6"
                                                            runat="server"
                                                            Text="Heat/Stress duration for 25 % of RTS"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUOM6" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecified6" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtValue6" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>7</td>
                                                    <td style="word-wrap: break-word; white-space: normal; text-align: left">
                                                        <asp:Label
                                                            ID="lbl7"
                                                            runat="server"
                                                            Text="Visual after exposure"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUOM7" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecified7" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtValue7" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>8</td>
                                                    <td style="word-wrap: break-word; white-space: normal; text-align: left">
                                                        <asp:Label
                                                            ID="lbl8"
                                                            runat="server"
                                                            Text="Tensile min. 95 % of RTS"
                                                            CssClass="form-control"
                                                            Style="background-color: transparent; border: none; box-shadow: none; padding: 0; word-break: break-word;" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUOM8" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecified8" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtValue8" runat="server" CssClass="form-control" Style="text-align: center;" />
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
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>
