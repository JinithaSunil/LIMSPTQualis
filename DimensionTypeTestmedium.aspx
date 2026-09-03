<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="DimensionTypeTestmedium.aspx.cs" Inherits="LIMS.DimensionTypeTestmedium" %>


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
                            <h2>DIMENSION TYPE TEST MEDIUM-HIGH VOLTAGE</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="txtcableType" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvCableType" runat="server" ControlToValidate="txtcableType" ErrorMessage="Type is required" CssClass="text-danger" Display="Dynamic" />
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtrefSpec" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvRefSpec" runat="server" ControlToValidate="txtrefSpec" ErrorMessage="Ref.Spec. is required" CssClass="text-danger" Display="Dynamic" />
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="txtcableSize" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvCableSize" runat="server" ControlToValidate="txtcableSize" ErrorMessage="Size is required" CssClass="text-danger" Display="Dynamic" />
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txttestMethodNo" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvTestMethodNo" runat="server" ControlToValidate="txttestMethodNo" ErrorMessage="Test Method No. is required" CssClass="text-danger" Display="Dynamic" />
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvVoltage" runat="server" ControlToValidate="voltage" ErrorMessage="Voltage is required" CssClass="text-danger" Display="Dynamic" />
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtjobOrderNo" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvJobOrderNo" runat="server" ControlToValidate="txtjobOrderNo" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="txtdate" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtdate" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtWorksheetNo" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvWorksheetNo" runat="server" ControlToValidate="txtWorksheetNo" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
                                    </tr>
                                    <tr>
                                        <td><b>Room No. </b></td>
                                        <td>
                                            <asp:TextBox ID="txtRoomNo" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvRoomNo" runat="server" ControlToValidate="txtRoomNo" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
                                        <td><b>Room Humidity</b></td>
                                        <td>
                                            <asp:TextBox ID="txtRoomhumidity" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvroomHumidity" runat="server" ControlToValidate="txtRoomhumidity" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtRoomTemp" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvJRoomTemp" runat="server" ControlToValidate="txtRoomTemp" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtEquipment" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvEquipment" runat="server" ControlToValidate="txtEquipment" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
                                    </tr>
                                </table>
                            </div>

                            <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th colspan="2">Item Test</th>
                                            <th>Phase</th>
                                            <th>Unit</th>
                                            <th colspan="21">Measured of Sample</th>
                                            <th>Specified</th>
                                        </tr>
                                        <tr>
                                            <th colspan="4"></th>
                                            <th colspan="7">Sample-1</th>
                                            <th colspan="7">Sample-2</th>
                                            <th colspan="7">Sample-3</th>
                                            <th></th>
                                        </tr>
                                        <tr>
                                            <th colspan="4"></th>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Min-Max</th>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Min-Max</th>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Min-Max</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td rowspan="4">Double Armoring</td>
                                            <td>Thickness</td>

                                            <td><%--Phase--%></td>
                                            <td>
                                                <asp:TextBox ID="txt_unit_Thi_Double" runat="server" CssClass="form-control"> </asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_1_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_2_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_3_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_4_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_5_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_6_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_MinMax_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-2 Textboxes -->
                                            <td>
                                                <asp:TextBox ID="txt_S2_1_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_2_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_3_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_4_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_5_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_6_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_MinMax_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-3 Textboxes -->
                                            <td>
                                                <asp:TextBox ID="txt_S3_1_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_2_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_3_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_4_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_5_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_6_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_MinMax_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_speci_Thi_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>Width</td>
                                            <td><%--Phase2--%></td>
                                            <td>
                                                <asp:TextBox ID="txt_unit_Wid_Double" runat="server" CssClass="form-control"> </asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_1_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_2_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_3_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_4_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_5_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_6_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_MinMax_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-2 -->
                                            <td>
                                                <asp:TextBox ID="txt_S2_1_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_2_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_3_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_4_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_5_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_6_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_MinMax_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-3 -->
                                            <td>
                                                <asp:TextBox ID="txt_S3_1_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_2_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_3_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_4_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_5_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_6_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>

                                            <td>
                                                <asp:TextBox ID="txt_S3_MinMax_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_speci_Wid_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>Overlap</td>
                                            <td><%--Phase3--%></td>
                                            <td>
                                                <asp:TextBox ID="txt_unit_Overlap_Double" runat="server" CssClass="form-control"> </asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_1_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_2_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_3_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_4_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_5_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_6_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_MinMax_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>

                                            <!-- Sample-2 -->
                                            <td>
                                                <asp:TextBox ID="txt_S2_1_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_2_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_3_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_4_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_5_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_6_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_MinMax_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-3 -->
                                            <td>
                                                <asp:TextBox ID="txt_S3_1_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_2_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_3_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_4_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_5_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_6_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>

                                            <td>
                                                <asp:TextBox ID="txt_S3_MinMax_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_speci_Overlap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>Gap</td>
                                            <td><%--Phase4--%></td>
                                            <td>
                                                <asp:TextBox ID="txt_unit_Gap_Double" runat="server" CssClass="form-control"> </asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_1_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_2_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_3_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_4_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_5_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_6_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_MinMax_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>

                                            <!-- Sample-2 -->
                                            <td>
                                                <asp:TextBox ID="txt_S2_1_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_2_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_3_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_4_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_5_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_6_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_MinMax_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-3 -->
                                            <td>
                                                <asp:TextBox ID="txt_S3_1_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_2_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_3_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_4_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_5_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_6_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_MinMax_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_speci_Gap_Double" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>


                                        <!-- Outer Sheath -->
                                        <tr>
                                            <td rowspan="3">Outer Sheath</td>
                                            <td>Thickness</td>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="txt_unit_Thi_Outer" runat="server" CssClass="form-control"> </asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_1_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_2_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_3_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_4_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_5_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_6_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_MinMax_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-2 -->
                                            <td>
                                                <asp:TextBox ID="txt_S2_1_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_2_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_3_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_4_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_5_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_6_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_MinMax_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-3 -->
                                            <td>
                                                <asp:TextBox ID="txt_S3_1_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_2_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_3_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_4_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_5_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_6_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_MinMax_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_speci_Thi_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>Diameter</td>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="txt_unit_Diameter_Outer" runat="server" CssClass="form-control"> </asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_1_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_2_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_3_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_4_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_5_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_6_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_MinMax_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-2 -->
                                            <td>
                                                <asp:TextBox ID="txt_S2_1_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_2_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_3_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_4_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_5_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_6_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_MinMax_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-3 -->
                                            <td>
                                                <asp:TextBox ID="txt_S3_1_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_2_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_3_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_4_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_5_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_6_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_MinMax_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_speci_Diameter_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>Ovality</td>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="txt_unit_Ovality_Outer" runat="server" CssClass="form-control"> </asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_1_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_2_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_3_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_4_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_5_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_6_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S1_MinMax_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-2 -->
                                            <td>
                                                <asp:TextBox ID="txt_S2_1_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_2_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_3_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_4_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_5_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_6_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S2_MinMax_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <!-- Sample-3 -->
                                            <td>
                                                <asp:TextBox ID="txt_S3_1_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_2_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_3_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_4_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_5_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_6_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_S3_MinMax_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txt_speci_Ovality_Outer" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>

                            </div>

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn-primary" Text="Save Data" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
