<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="Impulsetest2.aspx.cs" Inherits="LIMS.Impulsetest2" %>

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
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>IMPULSE TEST</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:Label ID="cableType" runat="server" CssClass="form-control vertical-textbox" /></td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:Label ID="refSpec" runat="server" CssClass="form-control vertical-textbox" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:Label ID="cableSize" runat="server" CssClass="form-control vertical-textbox" /></td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:Label ID="testMethodNo" runat="server" CssClass="form-control vertical-textbox" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:Label ID="lblvoltage" runat="server" CssClass="form-control vertical-textbox" /></td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:Label ID="jobOrderNo" runat="server" CssClass="form-control vertical-textbox" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control vertical-textbox" type="date" Style="text-align: center;" /></td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:Label ID="worksheetno" runat="server" CssClass="form-control vertical-textbox" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" /></td>
                                        <td><b>Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                        </td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:Label ID="equipmentno" runat="server" CssClass="form-control vertical-textbox" /></td>
                                    </tr>
                                </table>
                            </div>

                            <br />
                            <br />
                            <%-- Modified Core Phase Selection (Radio Buttons) --%>
                            <div class="row phase-selection">
                                <div class="col-md-12">
                                    <h4>Core Phase</h4>
                                    <div class="inline-radios">
                                        <asp:RadioButtonList ID="rblCorePhase" runat="server" RepeatDirection="Horizontal"
                                            CssClass="compact-radiolist" AutoPostBack="true"
                                            OnSelectedIndexChanged="rblCorePhase_SelectedIndexChanged">
                                            <asp:ListItem Text="Red" Value="Red" Selected="True" />
                                            <asp:ListItem Text="Blue" Value="Blue" />
                                            <asp:ListItem Text="Yellow" Value="Yellow" />
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>

                            <%-- Currently Working On Label --%>
                            <div class="row current-phase">
                                <div class="col-md-12">
                                    <h5>
                                        <asp:Label ID="lblSelectedPhase" runat="server" Text="Red Phase"></asp:Label>
                                        <asp:Label ID="lblTestMode" runat="server" Text=" - 10 Positive Impulse Test followed by a Voltage test"></asp:Label>
                                    </h5>
                                </div>
                            </div>


                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Panel ID="pnlTestData" runat="server" Visible="false">
                                        <div class="table-responsive">
                                            <table class="table table-bordered">
                                                <thead>
                                                    <tr>
                                                        <th rowspan="2">No</th>
                                                        <th rowspan="2">Item Test</th>
                                                        <th rowspan="2">Unit</th>
                                                        <th colspan="2">Specified</th>
                                                        <th colspan="2">Measured</th>
                                                        <th rowspan="2">Remark</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <!-- Test 1 -->
                                                    <tr>
                                                        <td rowspan="6">1</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark1_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark1_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark1_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark1_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark1_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult1" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark1_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <!-- Test 2 -->
                                                    <tr>
                                                        <td rowspan="6">2</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark2_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark2_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark2_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark2_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark2_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult2" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark2_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>

                                                    <!-- Test 3 -->
                                                    <tr>
                                                        <td rowspan="6">3</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark3_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark3_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark3_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark3_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark3_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult3" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark3_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>

                                                    <!-- Test 4 -->
                                                    <tr>
                                                        <td rowspan="6">4</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark4_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark4_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark4_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark4_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark4_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult4" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark4_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>

                                                    <!-- Test 5 -->
                                                    <tr>
                                                        <td rowspan="6">5</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark5_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark5_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark5_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark5_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark5_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult5" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark5_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>

                                                    <!-- Test 6 -->
                                                    <tr>
                                                        <td rowspan="6">6</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark6_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark6_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark6_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark6_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark6_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult6" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark6_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>

                                                    <!-- Test 7 -->
                                                    <tr>
                                                        <td rowspan="6">7</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark7_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark7_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark7_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark7_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark7_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult7" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult7" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark7_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <!-- Test 8 -->
                                                    <tr>
                                                        <td rowspan="6">8</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark8_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark8_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark8_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark8_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark8_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult8" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult8" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark8_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>

                                                    <!-- Test 9 -->
                                                    <tr>
                                                        <td rowspan="6">9</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark9_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark9_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark9_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark9_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark9_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult9" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult9" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark9_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>


                                                    <!-- Test 10 -->
                                                    <tr>
                                                        <td rowspan="6">10</td>
                                                        <td>Impulse test at Withstand Voltage</td>
                                                        <td>kV</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecImpulseVoltage10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredImpulseVoltage10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark10_1" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Peak Voltage</td>
                                                        <td>Up(kV)</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecPeakVoltage10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredPeakVoltage10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark10_2" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Front wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecFrontTime10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredFrontTime10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark10_3" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tail wave time</td>
                                                        <td>μs</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTailTime10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredTailTime10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark10_4" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Conductor temperature</td>
                                                        <td><sup>o</sup>C</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecconductortemp10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtMeasuredconductortemp10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark10_5" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Test Result</td>
                                                        <td>-</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtSpecTestResult10" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                        <td colspan="2">
                                                            <asp:DropDownList ID="ddlTestResult10" runat="server" CssClass="form-control vertical-textbox">
                                                                <asp:ListItem Text="PASS" Value="PASS" Selected="True" />
                                                                <asp:ListItem Text="FAIL" Value="FAIL" />
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtRemark10_6" runat="server" CssClass="form-control vertical-textbox" Style="text-align: center;" />
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                            <%-- Currently Working On Label --%>
                            <div class="row current-phase">
                                <div class="col-md-12">
                                    <h5>
                                        <asp:Label ID="lblfooterSelectedPhase" runat="server" Text="Red Phase"></asp:Label>
                                        <asp:Label ID="lblfooterTestMode" runat="server" Text=" - 10 Positive Impulse Test followed by a Voltage test"></asp:Label>
                                    </h5>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Panel ID="pnlNavigation" runat="server" Visible="false" CssClass="form-actions" Style="margin-top: 20px;">
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="btn btn-secondary" Text="Previous" OnClick="btnPrevious_Click" Visible="false" />
                                    <asp:Button ID="btnNext" runat="server" CssClass="btn btn-primary" Text="Next" OnClick="btnNext_Click" />
                                    <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" Visible="false" />
                                    <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false"  OnClick="btnUpdate_Click"/>
                                    <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-warning" Text="Clear" OnClick="btnClearData_Click" />
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>
