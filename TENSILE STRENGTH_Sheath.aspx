<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="TENSILE STRENGTH_Sheath.aspx.cs" Inherits="LIMSStaging.TENSILE_STRENGTH_Sheath" %>


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
                                <table>
                                    <thead>
                                        <tr>
                                            <th rowspan="2">Date</th>
                                            <th rowspan="2">Item Test</th>
                                            <th rowspan="2">Core Phase</th>
                                            <th rowspan="2">Type of sample test</th>
                                            <th rowspan="2">Specimen No.</th>
                                            <th colspan="7">Thickness</th>
                                            <th colspan="7">Width</th>
                                            <th rowspan="2">Area</th>
                                            <th rowspan="2">BF</th>
                                            <th rowspan="2">TS</th>
                                            <th colspan="2">Elongation</th>
                                            <th colspan="2">Median</th>
                                            <th colspan="2">Variation</th>
                                        </tr>
                                        <tr>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Min</th>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Min</th>
                                            <th>LD</th>
                                            <th>LL</th>
                                            <th>EL</th>
                                            <th>TS</th>
                                            <th>EL</th>
                                            <th>TS</th>

                                        </tr>
                                    </thead>
                                    <tbody>
                                        <!-- First Section: Before aging -->
                                        <tr>
                                            <td rowspan="5">03/01/2024</td>
                                            <td rowspan="5">Before aging</td>
                                            <td rowspan="5">Sheath</td>
                                            <td rowspan="5">Dumbel</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_1_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_2_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_3_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_4_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_5_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_6_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_Min_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_1_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_2_sampl1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_3_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_4_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_5_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_6_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_Min_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Area_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_BF_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBATS_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LD_ELS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LL_ELS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_MEDS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_MEDS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_VARS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_VARS1" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>2</td>

                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_1_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_2_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_3_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_4_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_5_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_6_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_Min_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_1_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_3_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_4_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_5_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_6_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_Min_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Area_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_BF_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBATS_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LD_ELS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LL_ELS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_MEDS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_MEDS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_VARS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_VARS2" runat="server" CssClass="form-control"></asp:TextBox></td>


                                        </tr>





                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_1_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_2_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_3_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_4_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_5_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_6_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_Min_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_1_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_3_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_4_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_5_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_6_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_Min_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Area_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_BF_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBATS_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LD_ELS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LL_ELS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_MEDS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_MEDS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_VARS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_VARS3" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>4</td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_1_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_2_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_3_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_4_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_5_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_6_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_Min_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_1_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_3_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_4_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_5_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_6_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_Min_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Area_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_BF_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBATS_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LD_ELS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LL_ELS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_MEDS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_MEDS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_VARS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_VARS4" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>5</td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_1_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_2_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_3_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_4_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_5_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_6_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Thick_Min_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_1_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_3_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_4_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_5_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_6_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Width_Min_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_Area_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_BF_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBATS_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LD_ELS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_LL_ELS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_MEDS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_MEDS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_EL_VARS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBA_TS_VARS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr class="specified-row">
                                            <td colspan="5">Specified</td>
                                            <td colspan="7">-</td>
                                            <td colspan="7">-</td>
                                            <td>-</td>
                                            <td>Min.<br>
                                                12.5</td>
                                            <td>-</td>
                                            <td>-</td>
                                            <td>Min.<br>
                                                200</td>
                                            <td>Min.<br>
                                                -</td>
                                            <td>Min.<br>
                                                200</td>
                                            <td>3.0</td>
                                            <td>-1.8</td>
                                        </tr>

                                        <!-- Second Section: After aging 10 days -->
                                        <tr>
                                            <td rowspan="5">03/01/2024</td>
                                            <td rowspan="5">After aging 10D (+/- 2h)</td>
                                            <td rowspan="5">Sheath</td>
                                            <td rowspan="5">Dumbel</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_1_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_2_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_3_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_4_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_5_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_6_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_Min_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_1_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_2_sampl1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_3_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_4_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_5_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_6_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_Min_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Area_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_BF_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFATS_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LD_ELS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LL_ELS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_MEDS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_MEDS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_VARS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_VARS1" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_1_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_2_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_3_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_4_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_5_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_6_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_Min_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_1_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_3_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_4_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_5_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_6_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_Min_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Area_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_BF_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFATS_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LD_ELS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LL_ELS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_MEDS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_MEDS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_VARS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_VARS2" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_1_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_2_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_3_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_4_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_5_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_6_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_Min_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_1_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox6" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_3_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_4_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_5_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_6_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_Min_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Area_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_BF_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFATS_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LD_ELS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LL_ELS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_MEDS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_MEDS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_VARS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_VARS3" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>4</td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_1_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_2_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_3_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_4_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_5_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_6_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_Min_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_1_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox7" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_3_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_4_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_5_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_6_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_Min_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Area_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_BF_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFATS_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LD_ELS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LL_ELS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_MEDS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_MEDS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_VARS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_VARS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>5</td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_1_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_2_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_3_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_4_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_5_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_6_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Thick_Min_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_1_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox8" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_3_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_4_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_5_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_6_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Width_Min_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_Area_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_BF_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFATS_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LD_ELS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_LL_ELS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_MEDS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_MEDS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_EL_VARS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFA_TS_VARS5" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr class="specified-row">
                                            <td colspan="5">Specified</td>
                                            <td colspan="7">-</td>
                                            <td colspan="7">-</td>
                                            <td>-</td>
                                            <td>Min.<br>
                                                -</td>
                                            <td>-</td>
                                            <td>-</td>
                                            <td>Min.<br>
                                                200</td>
                                            <td>Min.<br>
                                                -</td>
                                            <td>Min.<br>
                                                200</td>
                                            <td>Max: 3</td>
                                            <td>Max: 3</td>
                                        </tr>

                                        <!-- Third Section: After aging cable 30D -->
                                        <tr>
                                            <td rowspan="5">03/01/2024</td>
                                            <td rowspan="5">After aging complete Cable 30D</td>
                                            <td rowspan="5">Sheath</td>
                                            <td rowspan="5">Dumbel</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_1_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_2_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_3_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_4_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_5_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_6_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_Min_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_1_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox9" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_3_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_4_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_5_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_6_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_Min_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Area_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_BF_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFCTS_S1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LD_ELS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LL_ELS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_MEDS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_MEDS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_VARS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_VARS1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_1_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_2_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_3_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_4_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_5_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_6_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_Min_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_1_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox10" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_3_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_4_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_5_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_6_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_Min_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Area_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_BF_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFCTS_S2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LD_ELS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LL_ELS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_MEDS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_MEDS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_VARS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_VARS2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_1_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_2_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_3_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_4_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_5_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_6_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_Min_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_1_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox11" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_3_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_4_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_5_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_6_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_Min_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Area_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_BF_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFCTS_S3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LD_ELS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LL_ELS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_MEDS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_MEDS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_VARS3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_VARS3" runat="server" CssClass="form-control"></asp:TextBox></td>

                                        </tr>
                                        <tr>
                                            <td>4</td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_1_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_2_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_3_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_4_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_5_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_6_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_Min_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_1_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox12" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_3_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_4_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_5_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_6_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_Min_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Area_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_BF_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFCTS_S4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LD_ELS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LL_ELS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_MEDS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_MEDS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_VARS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_VARS4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>5</td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_1_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_2_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_3_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_4_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_5_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_6_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Thick_Min_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_1_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox13" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_3_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_4_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_5_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_6_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Width_Min_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_Area_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_BF_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFCTS_S5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LD_ELS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_LL_ELS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_MEDS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_MEDS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_EL_VARS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtAFC_TS_VARS5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr class="specified-row">
                                            <td colspan="5">Specified</td>
                                            <td colspan="7">-</td>
                                            <td colspan="7">-</td>
                                            <td>-</td>
                                            <td>Min.<br>
                                                -</td>
                                            <td>-</td>
                                            <td>-</td>
                                            <td>Min.<br>
                                                200</td>
                                            <td>Min.<br>
                                                -</td>
                                            <td>Min.<br>
                                                200</td>
                                            <td>Max: 3</td>
                                            <td>Max: 3</td>
                                        </tr>

                                        <br />
                                        <!-- Aging Specimens Section -->
                                        <%--  <tr>
                                            <td colspan="14" class="section-header">Aging specimen</td>
                                            <td colspan="14" class="section-header">Aging specimen</td>
                                        </tr>
                                        <tr>
                                            <td>Airflow = 18.0</td>
                                            <td colspan="8">Standard: 8-20 air changes per hour at temp.</td>
                                            <td>Airflow: 12.5</td>
                                            <td colspan="8">Standard: 8-20 air changes per hour at temp.</td>
                                            <td colspan="3">PT. Qualix Indonesia</td>
                                            <td colspan="5" class="section-header">Witnessed by</td>
                                        </tr>
                                        <tr>
                                            <td>Date</td>
                                            <td colspan="2">Time</td>
                                            <td>Date</td>
                                            <td colspan="2">Time</td>
                                            <td>Date</td>
                                            <td colspan="2">Time</td>
                                            <td>Date</td>
                                            <td colspan="2">Time</td>
                                            <td>Tested by:</td>
                                            <td colspan="3" rowspan="2" class="signature-cell">
                                                <img src="/api/placeholder/100/50" alt="signature" />
                                            </td>
                                            <td>Name</td>
                                            <td>Company</td>
                                            <td>Signature</td>
                                        </tr>
                                        <tr>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td rowspan="2">Checked by:</td>
                                            <td rowspan="2"></td>
                                            <td rowspan="2"></td>
                                            <td rowspan="2"></td>
                                        </tr>
                                        <tr>
                                            <td>23/12/2024</td>
                                            <td>02/01/2025</td>
                                            <td>09:50</td>
                                            <td>09:50</td>
                                            <td>23/12/2024</td>
                                            <td>30/12/2024</td>
                                            <td>10:00</td>
                                            <td>10:00</td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" class="section-header">Conditioning</td>
                                            <td colspan="4" class="section-header">Conditioning</td>
                                            <td colspan="4" class="section-header">Conditioning</td>
                                            <td colspan="4" class="section-header">Conditioning</td>
                                            <td colspan="8"></td>
                                        </tr>
                                        <tr>
                                            <td>Date</td>
                                            <td colspan="3">Time</td>
                                            <td>Date</td>
                                            <td colspan="3">Time</td>
                                            <td>Date</td>
                                            <td colspan="3">Time</td>
                                            <td>Date</td>
                                            <td colspan="3">Time</td>
                                            <td colspan="8"></td>
                                        </tr>
                                        <tr>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td>Start</td>
                                            <td>Finish</td>
                                            <td colspan="8"></td>
                                        </tr>
                                        <tr>
                                            <td>02/01/2025</td>
                                            <td>03/01/2025</td>
                                            <td>08:30</td>
                                            <td>08:30</td>
                                            <td>30/12/2024</td>
                                            <td>31/12/2024</td>
                                            <td>10:00</td>
                                            <td>10:00</td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td colspan="8"></td>
                                        </tr>--%>
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

