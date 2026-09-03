<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScrewCurrentConductingPartsAndItsRelationship.aspx.cs" Inherits="LIMS.ScrewCurrentConductingPartsAndItsRelationship" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Results Form</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />

    <style type="text/css">
        body {
            font-family: Arial, "Segoe UI", sans-serif;
            font-size: 14px;
        }

        .form-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }

        .clause-table {
            border-collapse: collapse;
            width: 100%;
            margin-bottom: 20px;
            border: 2px solid #333;
        }

        .header-row {
            color: black;
            font-weight: bold;
        }

        .sub-header-row {
            background-color: #d1d0d0;
            font-weight: bold;
        }

        .clause-table td, .clause-table th {
            border: 1px solid black;
            padding: 8px;
            text-align: left;
            vertical-align: top;
        }

        .clause-number {
            width: 10%;
            font-weight: bold;
        }

        .requirement {
            width: 45%;
        }

        .result {
            width: 25%;
        }

        .remark {
            width: 20%;
        }

        /*.checkbox-group label {
            display: block;
            margin-bottom: 5px;
        }*/

        .checkbox-group {
            display: flex;
            flex-direction: column;
            gap: 10px; /* spacing between rows */
        }

        .checkbox-row {
            display: flex;
            gap: 20px; /* spacing between items in a row */
        }

        .checkbox-item {
            display: flex;
            align-items: center;
            white-space: nowrap;
        }

        .checkbox-item {
            display: flex;
            align-items: center;
            white-space: nowrap;
            font-size: 18px; /* Increase font size */
        }

            .checkbox-item input[type="checkbox"] {
                width: 20px; /* Increase checkbox width */
                height: 20px; /* Increase checkbox height */
                margin-right: 8px; /* Space between checkbox and label */
            }

        .btn-container {
            text-align: center;
            margin-top: 20px;
        }

        .btn {
            padding: 10px 20px;
            margin: 5px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-save {
            /*background-color: #4CAF50;*/
            background-color: #b34788 !important;
            color: white;
        }



        .btn-report {
            background-color: #b34788 !important;
            color: black;
        }

        .btn:hover {
            opacity: 0.8;
        }



        .table-responsive {
            width: 100%;
            overflow-x: auto;
            margin-bottom: 20px;
        }

        table {
            width: 100%;
            min-width: 800px; /* More room for fields */
            border-collapse: collapse;
            background-color: #fff;
        }

        table, th, td {
            border: 2px solid #333;
            text-align: left;
        }

        td {
            padding: 8px 10px;
            text-align: left;
            vertical-align: middle;
            text-align: left;
        }

        .info-table td:first-child,
        .info-table td:nth-child(3) {
            width: 20%;
        }

        input[type="text"], input[type="number"], input[type="date"],
        .form-control {
            width: 100%;
            max-width: 100%;
            padding: 6px 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
            display: block;
            box-sizing: border-box;
        }

        .center-text {
            text-align: center;
        }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="form-container">

            <div class="header">
                <h2>Test Result Entry Form :
                    <asp:Label ID="lblReportName" runat="server"></asp:Label>
                </h2>
                <asp:Label ID="lblstandardID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblReportID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblSampleID" runat="server" Visible="false"></asp:Label>
            </div>
            <div class="table-responsive">
                <table class="info-table">

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test category</td>
                        <td colspan="3">
                            <asp:TextBox ID="txtTestCategory" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"
                                ControlToValidate="txtTestCategory" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>


                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Type</td>
                        <td>
                            <asp:TextBox ID="txtType" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                                ControlToValidate="txtType" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>

                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Method</td>
                        <td>
                            <asp:TextBox ID="txtstandard" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                ControlToValidate="txtstandard" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Ranking</td>
                        <td>
                            <asp:TextBox ID="txtRanking" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server"
                                ControlToValidate="txtRanking" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Job Order No.</td>
                        <td>
                            <asp:TextBox ID="txtworkorder" runat="server" CssClass="form-control" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Date</td>
                        <td>
                            <asp:TextBox ID="txtstartingdate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvStartingDate" runat="server"
                                ControlToValidate="txtstartingdate" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>

                        <td style="background-color: #d1d0d0; font-weight: bold;">Request No.</td>
                        <td>
                            <asp:TextBox ID="txtRequestNo" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvCompletionDate" runat="server"
                                ControlToValidate="txtRequestNo" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Completion Date is required"
                                Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Room temp.(°C)</td>
                        <td>
                            <asp:TextBox ID="txttemp" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvTemp" runat="server"
                                ControlToValidate="txttemp" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Temperature is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Humidity (%)</td>
                        <td>
                            <asp:TextBox ID="txthumidity" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvHumidity" runat="server"
                                ControlToValidate="txthumidity" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Humidity is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Sample Code</td>
                        <td>
                            <asp:TextBox ID="txtsampleCode" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server"
                                ControlToValidate="txtsampleCode" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />

                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Equipment No.</td>
                        <td>
                            <asp:TextBox ID="txtTestEquipmentNo" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server"
                                ControlToValidate="txtTestEquipmentNo" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Equipment</td>
                        <td colspan="3">
                            <asp:CheckBoxList ID="chkEquipmentList" runat="server" RepeatColumns="1" RepeatDirection="Horizontal">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <br />
            <br />

            <div id="clauseContainer" runat="server" style="margin-bottom: 100px;"></div>

            <div class="btn-container">
                <asp:Button ID="btnSave" runat="server" Text="Save Results" OnClick="btnSave_Click" CssClass="btn btn-save" CausesValidation="true" ValidationGroup="DefaultDataUpperDetails" />
            </div>

        </div>

    </form>
</body>
</html>

