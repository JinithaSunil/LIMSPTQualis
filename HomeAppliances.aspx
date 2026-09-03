<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="HomeAppliances.aspx.cs" Inherits="LIMS.HomeAppliances" MaintainScrollPositionOnPostback="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Results Form</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />

    <style type="text/css">
        body {
            font-family: "Calibri", "Segoe UI", sans-serif;
            /*font-size: 14px;*/
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
                <h2>Test Result Entry Form </h2>
                <asp:Label ID="lblstandardID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblReportID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblSampleID" runat="server" Visible="false"></asp:Label>
            </div>
            <div class="table-responsive">
                <table class="info-table">
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Work Order</td>
                        <td>
                            <asp:TextBox ID="txtworkorder" runat="server" CssClass="form-control" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Product</td>
                        <td>
                            <asp:TextBox ID="txtproduct" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Brand</td>
                        <td>
                            <asp:TextBox ID="txtbrand" runat="server" CssClass="form-control" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Model</td>
                        <td>
                            <asp:TextBox ID="txtmodel" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Standard</td>
                        <td>
                            <asp:TextBox ID="txtstandard" runat="server" CssClass="form-control" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Method No.</td>
                        <td>
                            <asp:TextBox ID="txttestMethodNo" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Temp</td>
                        <td>
                            <asp:TextBox ID="txttemp" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvTemp" runat="server"
                                ControlToValidate="txttemp" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Temperature is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Humidity (%)</td>
                        <td>
                            <asp:TextBox ID="txthumidity" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvHumidity" runat="server"
                                ControlToValidate="txthumidity" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Humidity is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Starting Date</td>
                        <td>
                            <asp:TextBox ID="txtstartingdate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvStartingDate" runat="server"
                                ControlToValidate="txtstartingdate" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Completion Date</td>
                        <td>
                            <asp:TextBox ID="txtcompletiondate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvCompletionDate" runat="server"
                                ControlToValidate="txtcompletiondate" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Completion Date is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
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

         <%--   <div class="table-responsive">
                <table style="width: 100%; border-collapse: collapse;" border="1">

                    <!-- Details of test items -->
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold; width: 30%;">Details of test items
                        </td>
                        <td style="width: 70%;">
                            <asp:TextBox ID="txtdetailsoftestitems" runat="server" CssClass="form-control" />
                        </td>
                    </tr>

                    <!-- Classification of installation and use -->
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Classification of installation and use
                        </td>
                        <td>
                            <asp:TextBox ID="txtclassification" runat="server" CssClass="form-control" />
                        </td>
                    </tr>

                    <!-- Supply Connection -->
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Supply Connection
                        </td>
                        <td>
                            <asp:TextBox ID="txtsupplyconnection" runat="server" CssClass="form-control" />
                        </td>
                    </tr>

                    <!-- Possible Verdict Section -->
                    <tr>
                        <td colspan="2" style="background-color: #d1d0d0; font-weight: bold;">Possible verdict of the trial case:
                        </td>
                    </tr>

                    <tr>
                        <td colspan="2" style="padding: 10px;">

                            <div style="margin-bottom: 10px;">
                                - <strong>test case does not apply to test object</strong> :
            <asp:TextBox ID="txtNotApplicable" runat="server" CssClass="form-control"
                Style="display: inline-block; width: 150px; margin-left: 10px;" />
                                T (Not applicable)
                            </div>

                            <div style="margin-bottom: 10px;">
                                - <strong>the test object has met the test requirements</strong> :
            <asp:TextBox ID="txtPass" runat="server" CssClass="form-control"
                Style="display: inline-block; width: 150px; margin-left: 10px;" />
                                L (Pass)
                            </div>

                            <div>
                                - <strong>the test object does not meet the requirements</strong> :
            <asp:TextBox ID="txtFail" runat="server" CssClass="form-control"
                Style="display: inline-block; width: 150px; margin-left: 10px;" />
                                TL (Failed)
                            </div>

                        </td>
                    </tr>

                </table>
            </div>--%>


            <!-- Add this in your form -->
            <%--<div>
    <asp:Label ID="lblImageUpload" runat="server" Text="Upload Lab Analysis Images:"></asp:Label>
    <asp:FileUpload ID="fuLabAnalysis" runat="server" AllowMultiple="true" />
    <asp:Button ID="btnUploadImages" runat="server" Text="Upload Images" OnClick="btnUploadImages_Click" />
</div>--%>

            <br />
            <br />
            <br />
            <div id="clauseContainer" runat="server" style="margin-bottom: 100px;"></div>
            <hr />
            <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <h2><u>TABLE: Power Input Deviation</u></h2>
                    <div class="table-responsive">
                        <table class="table table-bordered">
                            <thead>
                                <tr>
                                    <th>Input deviation from/on</th>
                                    <th>P Identifier (W)</th>
                                    <th>P Result (W)</th>
                                    <th>∆P (W ; %)</th>
                                    <th>Condition∆P (W ; %)</th>
                                    <th>Decision</th>

                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtInputDeviation" runat="server" CssClass="form-control" placeholder="Input deviation from/on" />
                                        <asp:RequiredFieldValidator ID="rfvInputDeviation" runat="server" ValidationGroup="PowerInputDetails"
                                            ControlToValidate="txtInputDeviation" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPIdentifier" runat="server" CssClass="form-control" placeholder="P Identifier (W)" />
                                        <asp:RequiredFieldValidator ID="rfvPIdentifier" runat="server" ValidationGroup="PowerInputDetails"
                                            ControlToValidate="txtPIdentifier" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPResult" runat="server" CssClass="form-control" placeholder="P Result (W)" />
                                        <asp:RequiredFieldValidator ID="rfvPResult" runat="server" ValidationGroup="PowerInputDetails"
                                            ControlToValidate="txtPResult" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDeltaP" runat="server" CssClass="form-control" placeholder="∆P (W ; %)" />
                                        <asp:RequiredFieldValidator ID="rfvDeltaP" runat="server" ValidationGroup="PowerInputDetails"
                                            ControlToValidate="txtDeltaP" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtConditionDeltaP" runat="server" CssClass="form-control" placeholder="Condition∆P (W ; %)" />
                                        <asp:RequiredFieldValidator ID="rfvConditionDeltaP" runat="server" ValidationGroup="PowerInputDetails"
                                            ControlToValidate="txtConditionDeltaP" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDecision" runat="server" CssClass="form-control" placeholder="Decision" />
                                        <asp:RequiredFieldValidator ID="rfvDecision" runat="server" ValidationGroup="PowerInputDetails"
                                            ControlToValidate="txtDecision" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="ADD"
                                            Style="background-color: #2196F3;" OnClick="btnAdd_Click"
                                            CausesValidation="true" ValidationGroup="PowerInputDetails" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="7">
                                        <strong>Additional information:</strong>
                                        <asp:TextBox ID="txtAdditionalInfo" runat="server" CssClass="form-control" placeholder="Additional information" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <hr />
                    <asp:GridView ID="gvTests" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvTests_RowDeleting">
                        <Columns>
                            <asp:TemplateField HeaderText="No.">
                                <ItemTemplate>
                                    <%# (gvTests.PageIndex * gvTests.PageSize) + Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="InputDeviation" HeaderText="Input deviation from/on" />
                            <asp:BoundField DataField="PIdentifier" HeaderText="P Identifier (W)" />
                            <asp:BoundField DataField="PResult" HeaderText="P Result (W)" />
                            <asp:BoundField DataField="DeltaP" HeaderText="∆P (W ; %)" />
                            <asp:BoundField DataField="ConditionDeltaP" HeaderText="Condition∆P (W ; %)" />
                            <asp:BoundField DataField="Decision" HeaderText="Decision" />
                            <asp:BoundField DataField="AdditionalInfo" HeaderText="Additional information" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDelete" runat="server" Style="color: red;" CommandName="Delete"
                                        OnClientClick="return confirm('Are you sure you want to delete this item?');"
                                        CssClass="text-danger" ToolTip="Delete">
                    <i class="fas fa-trash-alt"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>





                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <h2><u>TABLE: Heating Test</u></h2>
                    <div class="table-responsive">
                        <table class="table table-bordered">
                            <thead>
                                <tr>
                                    <th>Input deviation from/on</th>
                                    <th>P Identifier (W)</th>
                                    <th>P Result (W)</th>
                                    <th>∆P (W ; %)</th>
                                    <th>Condition∆P (W ; %)</th>
                                    <th>Decision</th>

                                    <th>Action</th>
                                </tr>
                                <%--<tr>
                                    <td colspan="5">Additional information</td>
                                </tr>--%>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtInputDeviation2" runat="server" CssClass="form-control" placeholder="Input deviation from/on" />
                                        <asp:RequiredFieldValidator ID="rfvInputDeviation2" runat="server" ValidationGroup="HeatingTestDetails"
                                            ControlToValidate="txtInputDeviation2" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPIdentifier2" runat="server" CssClass="form-control" placeholder="P Identifier (W)" />
                                        <asp:RequiredFieldValidator ID="rfvPIdentifier2" runat="server" ValidationGroup="HeatingTestDetails"
                                            ControlToValidate="txtPIdentifier2" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPResult2" runat="server" CssClass="form-control" placeholder="P Result (W)" />
                                        <asp:RequiredFieldValidator ID="rfvPResult2" runat="server" ValidationGroup="HeatingTestDetails"
                                            ControlToValidate="txtPResult2" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDeltaP2" runat="server" CssClass="form-control" placeholder="∆P (W ; %)" />
                                        <asp:RequiredFieldValidator ID="rfvDeltaP2" runat="server" ValidationGroup="HeatingTestDetails"
                                            ControlToValidate="txtDeltaP2" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtConditionDeltaP2" runat="server" CssClass="form-control" placeholder="Condition∆P (W ; %)" />
                                        <asp:RequiredFieldValidator ID="rfvConditionDeltaP2" runat="server" ValidationGroup="HeatingTestDetails"
                                            ControlToValidate="txtConditionDeltaP2" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDecision2" runat="server" CssClass="form-control" placeholder="Decision" />
                                        <asp:RequiredFieldValidator ID="rfvDecision2" runat="server" ValidationGroup="HeatingTestDetails"
                                            ControlToValidate="txtDecision2" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:Button ID="btnAdd2" runat="server" CssClass="btn btn-primary" Text="ADD"
                                            Style="background-color: #2196F3;" OnClick="btnAdd2_Click"
                                            CausesValidation="true" ValidationGroup="HeatingTestDetails" />
                                    </td>

                                </tr>
                                <tr>
                                    <td colspan="7">
                                        <strong>Additional information:</strong>
                                        <asp:TextBox ID="txtAdditionalInfo2" runat="server" CssClass="form-control" placeholder="Additional information" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <hr />
                    <asp:GridView ID="gvTests2" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvTests2_RowDeleting">
                        <Columns>
                            <asp:TemplateField HeaderText="No.">
                                <ItemTemplate>
                                    <%# (gvTests2.PageIndex * gvTests2.PageSize) + Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="InputDeviation" HeaderText="Input deviation from/on" />
                            <asp:BoundField DataField="PIdentifier" HeaderText="P Identifier (W)" />
                            <asp:BoundField DataField="PResult" HeaderText="P Result (W)" />
                            <asp:BoundField DataField="DeltaP" HeaderText="∆P (W ; %)" />
                            <asp:BoundField DataField="ConditionDeltaP" HeaderText="Condition∆P (W ; %)" />
                            <asp:BoundField DataField="Decision" HeaderText="Decision" />
                            <asp:BoundField DataField="AdditionalInfo" HeaderText="Additional information" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDelete2" runat="server" Style="color: red;" CommandName="Delete"
                                        OnClientClick="return confirm('Are you sure you want to delete this item?');"
                                        CssClass="text-danger" ToolTip="Delete">
                            <i class="fas fa-trash-alt"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
       <%--     <asp:UpdatePanel ID="UpdatePanel3" runat="server">
    <ContentTemplate>
        <h2><u>TABLE: Heating Test</u></h2>
        <div class="table-responsive">
            <table class="table table-bordered">
                <tbody>
                    <tr>
                        <td style="width: 10%;">
                            <asp:Label ID="lblTestNumber" runat="server" Text="11.8" Font-Bold="true" />
                        </td>
                        <td style="width: 40%;" colspan="2">
                            <strong>TABLE: Heating Test</strong>
                        </td>
                        <td style="width: 25%;"></td>
                        <td style="width: 25%;"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td colspan="2">
                            <asp:TextBox ID="txtTestVoltage" runat="server" CssClass="form-control" placeholder="Test voltage (V)" />
                            <asp:RequiredFieldValidator ID="rfvTestVoltage" runat="server" ValidationGroup="HeatingTestDetails"
                                ControlToValidate="txtTestVoltage" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtTestVoltageValue" runat="server" CssClass="form-control" placeholder="Value" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td colspan="2">
                            <asp:TextBox ID="txtAmbientTemp" runat="server" CssClass="form-control" placeholder="Ambient temperature (°C)" />
                            <asp:RequiredFieldValidator ID="rfvAmbientTemp" runat="server" ValidationGroup="HeatingTestDetails"
                                ControlToValidate="txtAmbientTemp" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtAmbientTempValue" runat="server" CssClass="form-control" placeholder="Value" />
                        </td>
                    </tr>
                    <tr style="background-color: #f0f0f0;">
                        <td colspan="2">
                            <strong>Thermocouple location</strong>
                        </td>
                        <td style="text-align: center;">
                            <strong>Max. measured temperature<br />rise, ΔT (K)</strong>
                        </td>
                        <td colspan="2" style="text-align: center;">
                            <strong>Maximum temperature rise<br />limit, ΔT (K)</strong>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:TextBox ID="txtThermocoupleLocation" runat="server" CssClass="form-control" placeholder="Thermocouple location" />
                            <asp:RequiredFieldValidator ID="rfvThermocoupleLocation" runat="server" ValidationGroup="HeatingTestDetails"
                                ControlToValidate="txtThermocoupleLocation" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:TextBox ID="txtMaxMeasuredTemp" runat="server" CssClass="form-control" placeholder="Max. measured" />
                            <asp:RequiredFieldValidator ID="rfvMaxMeasuredTemp" runat="server" ValidationGroup="HeatingTestDetails"
                                ControlToValidate="txtMaxMeasuredTemp" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtMaxTempLimit" runat="server" CssClass="form-control" placeholder="Max. limit" />
                            <asp:RequiredFieldValidator ID="rfvMaxTempLimit" runat="server" ValidationGroup="HeatingTestDetails"
                                ControlToValidate="txtMaxTempLimit" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" style="text-align: center;">
                            <asp:Button ID="btnAddHeating" runat="server" CssClass="btn btn-primary" Text="ADD ROW"
                                Style="background-color: #2196F3;" OnClick="btnAddHeating_Click"
                                CausesValidation="true" ValidationGroup="HeatingTestDetails" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <hr />
        <asp:GridView ID="gvHeatingTests" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvHeatingTests_RowDeleting" CssClass="table table-bordered">
            <Columns>
                <asp:BoundField DataField="ThermocoupleLocation" HeaderText="Thermocouple location" />
                <asp:BoundField DataField="MaxMeasuredTemp" HeaderText="Max. measured temperature rise, ΔT (K)" />
                <asp:BoundField DataField="MaxTempLimit" HeaderText="Maximum temperature rise limit, ΔT (K)" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkDeleteHeating" runat="server" Style="color: red;" CommandName="Delete"
                            OnClientClick="return confirm('Are you sure you want to delete this item?');"
                            CssClass="text-danger" ToolTip="Delete">
                            <i class="fas fa-trash-alt"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>--%>

            <div class="btn-container">
                <asp:Button ID="btnSave" runat="server" Text="Save Results" OnClick="btnSave_Click" CssClass="btn btn-save" CausesValidation="true" ValidationGroup="DefaultDataUpperDetails" />
                <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" OnClick="btnExport_Click" CssClass="btn btn-report" Visible="false" />
            </div>

        </div>

    </form>
</body>
</html>
