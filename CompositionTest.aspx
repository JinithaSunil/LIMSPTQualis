<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="CompositionTest.aspx.cs" Inherits="LIMS.CompositionTest" %>


<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
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
            padding: 5px;
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

        .table-bordered th,
        .table-bordered td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: center;
        }

        .table-bordered th {
            background-color: #f2f2f2;
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
                            <h2>COMPOSITION TEST</h2>
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
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" ReadOnly="true" />

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
                            <br />
                            <h3>Aluminium / Steel / Copper</h3>
                            <div class="table-responsive">
                                <table class="table table-bordered text-center">
                                    <thead>
                                        <tr>
                                            <th rowspan="2">No</th>
                                            <th rowspan="2">Item Test</th>
                                            <th colspan="1">Test Result</th>
                                            <th colspan="1">Standard</th>
                                            <th rowspan="2">Remark</th>
                                        </tr>
                                        <tr>
                                            <th>%</th>
                                            <th>%</th>
                                        </tr>
                                    </thead>


                                    <tbody>
                                        <tr>
                                            <td>1</td>
                                            <td>Al</td>
                                            <td>
                                                <asp:TextBox ID="txtAlTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvAlTestResult" runat="server"
                                                    ControlToValidate="txtAlTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAlStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvAlStandard" runat="server"
                                                    ControlToValidate="txtAlStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAlRemark" runat="server" CssClass="form-control" />
                                                <%-- <asp:RequiredFieldValidator ID="rfvAlRemark" runat="server"
                                                    ControlToValidate="txtAlRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>Si</td>
                                            <td>
                                                <asp:TextBox ID="txtSiTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSiTestResult" runat="server"
                                                    ControlToValidate="txtSiTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSiStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSiStandard" runat="server"
                                                    ControlToValidate="txtSiStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSiRemark" runat="server" CssClass="form-control" />
                                                <%--<asp:RequiredFieldValidator ID="rfvSiRemark" runat="server"
                                                    ControlToValidate="txtSiRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>Fe</td>
                                            <td>
                                                <asp:TextBox ID="txtFeTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvFeTestResult" runat="server"
                                                    ControlToValidate="txtFeTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtFeStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvFeStandard" runat="server"
                                                    ControlToValidate="txtFeStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtFeRemark" runat="server" CssClass="form-control" />
                                                <%--                                                <asp:RequiredFieldValidator ID="rfvFeRemark" runat="server"
                                                    ControlToValidate="txtFeRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>4</td>
                                            <td>Cu</td>
                                            <td>
                                                <asp:TextBox ID="txtCuTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCuTestResult" runat="server"
                                                    ControlToValidate="txtCuTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCuStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCuStandard" runat="server"
                                                    ControlToValidate="txtCuStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCuRemark" runat="server" CssClass="form-control" />
                                                <%--<asp:RequiredFieldValidator ID="rfvCuRemark" runat="server"
                                                    ControlToValidate="txtCuRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>5</td>
                                            <td>Mn</td>
                                            <td>
                                                <asp:TextBox ID="txtMnTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvMnTestResult" runat="server"
                                                    ControlToValidate="txtMnTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMnStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvMnStandard" runat="server"
                                                    ControlToValidate="txtMnStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMnRemark" runat="server" CssClass="form-control" />
                                                <%--     <asp:RequiredFieldValidator ID="rfvMnRemark" runat="server"
                                                    ControlToValidate="txtMnRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>6</td>
                                            <td>Ti</td>
                                            <td>
                                                <asp:TextBox ID="txtTiTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTiTestResult" runat="server"
                                                    ControlToValidate="txtTiTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTiStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTiStandard" runat="server"
                                                    ControlToValidate="txtTiStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTiRemark" runat="server" CssClass="form-control" />
                                                <%--            <asp:RequiredFieldValidator ID="rfvTiRemark" runat="server"
                                                    ControlToValidate="txtTiRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>7</td>
                                            <td>V</td>
                                            <td>
                                                <asp:TextBox ID="txtVTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvVTestResult" runat="server"
                                                    ControlToValidate="txtVTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtVStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvVStandard" runat="server"
                                                    ControlToValidate="txtVStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtVRemark" runat="server" CssClass="form-control" />
                                                <%--           <asp:RequiredFieldValidator ID="rfvVRemark" runat="server"
                                                    ControlToValidate="txtVRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>8</td>
                                            <td>Cr</td>
                                            <td>
                                                <asp:TextBox ID="txtCrTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCrTestResult" runat="server"
                                                    ControlToValidate="txtCrTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCrStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCrStandard" runat="server"
                                                    ControlToValidate="txtCrStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCrRemark" runat="server" CssClass="form-control" />
                                                <%--             <asp:RequiredFieldValidator ID="rfvCrRemark" runat="server"
                                                    ControlToValidate="txtCrRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>9</td>
                                            <td>B</td>
                                            <td>
                                                <asp:TextBox ID="txtBTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBTestResult" runat="server"
                                                    ControlToValidate="txtBTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBStandard" runat="server"
                                                    ControlToValidate="txtBStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBRemark" runat="server" CssClass="form-control" />
                                                <%--     <asp:RequiredFieldValidator ID="rfvBRemark" runat="server"
                                                    ControlToValidate="txtBRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>10</td>
                                            <td>Zn</td>
                                            <td>
                                                <asp:TextBox ID="txtZnTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvZnTestResult" runat="server"
                                                    ControlToValidate="txtZnTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtZnStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvZnStandard" runat="server"
                                                    ControlToValidate="txtZnStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtZnRemark" runat="server" CssClass="form-control" />
                                                <%--              <asp:RequiredFieldValidator ID="rfvZnRemark" runat="server"
                                                    ControlToValidate="txtZnRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>11</td>
                                            <td>Ga</td>
                                            <td>
                                                <asp:TextBox ID="txtGaTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvGaTestResult" runat="server"
                                                    ControlToValidate="txtGaTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtGaStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvGaStandard" runat="server"
                                                    ControlToValidate="txtGaStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtGaRemark" runat="server" CssClass="form-control" />
                                                <%--  <asp:RequiredFieldValidator ID="rfvGaRemark" runat="server"
                                                    ControlToValidate="txtGaRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>12</td>
                                            <td>C</td>
                                            <td>
                                                <asp:TextBox ID="txtCTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCTestResult" runat="server"
                                                    ControlToValidate="txtCTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCStandard" runat="server"
                                                    ControlToValidate="txtCStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCRemark" runat="server" CssClass="form-control" />
                                                <%--                          <asp:RequiredFieldValidator ID="rfvCRemark" runat="server"
                                                    ControlToValidate="txtCRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>13</td>
                                            <td>S</td>
                                            <td>
                                                <asp:TextBox ID="txtSTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSTestResult" runat="server"
                                                    ControlToValidate="txtSTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSStandard" runat="server"
                                                    ControlToValidate="txtSStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSRemark" runat="server" CssClass="form-control" />
                                                <%--<asp:RequiredFieldValidator ID="rfvSRemark" runat="server"
                                                    ControlToValidate="txtSRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>14</td>
                                            <td>P</td>
                                            <td>
                                                <asp:TextBox ID="txtPTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvPTestResult" runat="server"
                                                    ControlToValidate="txtPTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvPStandard" runat="server"
                                                    ControlToValidate="txtPStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPRemark" runat="server" CssClass="form-control" />
                                                <%--<asp:RequiredFieldValidator ID="rfvPRemark" runat="server"
                                                    ControlToValidate="txtPRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>15</td>
                                            <td>Mg</td>
                                            <td>
                                                <asp:TextBox ID="txtMgTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvMgTestResult" runat="server"
                                                    ControlToValidate="txtMgTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMgStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvMgStandard" runat="server"
                                                    ControlToValidate="txtMgStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMgRemark" runat="server" CssClass="form-control" />
                                                <%-- <asp:RequiredFieldValidator ID="rfvMgRemark" runat="server"
                                                    ControlToValidate="txtMgRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>16</td>
                                            <td>Na</td>
                                            <td>
                                                <asp:TextBox ID="txtNaTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvNaTestResult" runat="server"
                                                    ControlToValidate="txtNaTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtNaStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvNaStandard" runat="server"
                                                    ControlToValidate="txtNaStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtNaRemark" runat="server" CssClass="form-control" />
                                                <%--<asp:RequiredFieldValidator ID="rfvNaRemark" runat="server"
                                                    ControlToValidate="txtNaRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>17</td>
                                            <td>Zr</td>
                                            <td>
                                                <asp:TextBox ID="txtZrTestResult" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvZrTestResult" runat="server"
                                                    ControlToValidate="txtZrTestResult" ErrorMessage="Test Result is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtZrStandard" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvZrStandard" runat="server"
                                                    ControlToValidate="txtZrStandard" ErrorMessage="Standard is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtZrRemark" runat="server" CssClass="form-control" />
                                                <%--  <asp:RequiredFieldValidator ID="rfvZrRemark" runat="server"
                                                    ControlToValidate="txtZrRemark" ErrorMessage="Remark is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="text-align: right;"><strong>Spec. Max :</strong></td>
                                            <td>
                                                <asp:TextBox ID="txtTestResultSpecMax" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTestResultSpecMax" runat="server"
                                                    ControlToValidate="txtTestResultSpecMax" ErrorMessage="Test Result Spec is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtStandardSpecMax" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvStandardSpecMax" runat="server"
                                                    ControlToValidate="txtStandardSpecMax" ErrorMessage="Standard Spec is Required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtRemarkSpecMax" runat="server" CssClass="form-control" />
                                                <%--<asp:RequiredFieldValidator ID="rfvRemarkSpecMax" runat="server"
                                                    ControlToValidate="txtRemarkSpecMax" ErrorMessage="Remark Spec is required" ForeColor="Red" Display="Dynamic" />--%>
                                            </td>
                                        </tr>
                                    </tbody>

                                </table>
                            </div>

                            <div>
                            </div>
                        </div>
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>
